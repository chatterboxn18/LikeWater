using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Application = UnityEngine.Application;

namespace DungeonQuest
{
	public class DQResourceManager : MonoBehaviour
	{
		public static bool IsReady => _isReady;
		private static bool _isReady;
		private int _upgradesPerRow = 6;
		private int _upgradesPerColumn = 5;
		public static Dictionary<int, List<Sprite>> Upgrades = new Dictionary<int, List<Sprite>>();
		
		public static List<List<Sprite>> Sprites = new List<List<Sprite>>();

		private int _skinWidth = 450; 
		private int _skinHeight = 450;

		//Debug 
		[SerializeField] private List<UpgradeCard> _upgradeList = new List<UpgradeCard>();
		public List<List<UpgradeCard>> FullList = new List<List<UpgradeCard>>();

		public static List<UpgradeCard> UpgradeList = new List<UpgradeCard>();
		
		public enum BoostType
		{
			Attack,
			Speed,
			Magic
		}
		
		[Serializable]
		public class UpgradeCard
		{
			public DQCharacterData.RedVelvet Character;
			public int SpriteIndex;
			public BoostType Type;
			public float BoostPercent;
			public float BoostAmount;
			public int BasePrice;
			public float PriceInflation;
			//TEMP DATA
			public int Level;
			public int Price;
		}

		public class Enemy
		{
			public string Name;
			public Stats EnemyStat = new Stats();
			public struct Stats
			{
				public int HP;
				public int Defense;
				public float Dodge;
				public int SpDefense;

			}
			public Art[] EnemyArt;
			public struct Art
			{
				public Sprite MainSprite;
				public Sprite[] Extensions;
			}
			
		} 
		
		public static Dictionary<DQCharacterData.RedVelvet, Color32> Colors = new Dictionary<DQCharacterData.RedVelvet, Color32>
		{
			{DQCharacterData.RedVelvet.Irene, new Color32(255, 209, 220,255)},
			{DQCharacterData.RedVelvet.Seulgi, new Color32(253, 253, 150, 255)},
			{DQCharacterData.RedVelvet.Wendy, new Color32(174, 198, 207, 255)},
			{DQCharacterData.RedVelvet.Joy, new Color32(119, 221, 119, 255)},
			{DQCharacterData.RedVelvet.Yeri, new Color32(179, 158, 181, 255)}
		};

		private IEnumerator Start()
		{
			UpgradeList = _upgradeList;
			
			DQCharacterData.current.Setup(
				(DQCharacterData) SerializationManager.Load(Application.persistentDataPath + "/saves/" + DQConfig.SaveName + ".rv"));

			yield return DownloadJsonFiles();
			yield return GetUpgrades();
			yield return GetSkins();
			
			_isReady = true;
		}
		
		private IEnumerator DownloadFile(string fileName, Action<bool, string> onComplete)
		{
			
			var request = UnityWebRequest.Get(Path.Combine(DQConfig.ServerName,fileName));
			request.useHttpContinue = false;
			yield return request.SendWebRequest();
			if (!string.IsNullOrEmpty(request.error))
			{
				Debug.LogError("The path " + request.url + " has error: "+ request.error);
				onComplete(false, fileName);
				yield break;
			}

			try
			{
				if (fileName.Contains("/") || fileName.Contains("\\"))
				{
					fileName = fileName.Replace("\\", "/");
					var files = fileName.Split('/');
					for (int i = 0; i < files.Length - 1; i++)
					{
						Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, files[i]));
					}
				}
				if (File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + fileName))
				{
					File.Delete(Application.persistentDataPath + Path.DirectorySeparatorChar + fileName);
				}
				File.WriteAllBytes(Application.persistentDataPath + Path.DirectorySeparatorChar + fileName,
					request.downloadHandler.data);
			}
			catch (Exception e)
			{
				Debug.LogError("And you deleted the old one o.o");
				Debug.LogError(e);
			}

			onComplete(true, Application.persistentDataPath + Path.DirectorySeparatorChar + fileName);

		}

		private IEnumerator DownloadJsonFiles()
		{
			//get list from config in the future
			var list = new List<string>() {"enemies.json"};
			foreach (var item in list)
			{
				var finalPath = "";
				var succeed = false;
				var serverPath =Path.Combine(DQConfig.FolderName, item);
				yield return DownloadFile(serverPath, (success, path) =>
				{
					succeed = success; 
					finalPath = path;
				});
				if (!succeed) Debug.LogError("Request return null for json: " + item); yield break;
			}
		}
		
		private IEnumerator GetUpgrades()
		{
			var relativePath = Path.Combine(DQConfig.FolderName, "redvelvet-upgrades.png");
			if (!File.Exists(Path.Combine(Application.persistentDataPath, relativePath)))
			{
				var succeeded = false;
				yield return DownloadFile("DungeonQuest/redvelvet-upgrades.png", (success, path) =>
				{
					succeeded = success;
				});
				if (!succeeded) Debug.LogError("Download of " + "redvelvet-upgrades.png" + " failed.");
			}
			
			
			DownloadHandler request = null;
			var succeed = false;
			yield return GetTexture(relativePath, (success, handler) =>
			{
				succeed = success; 
				request = handler;
			});

			if (request == null) yield break;

			var texture = ((DownloadHandlerTexture) request).texture;

			var spriteCount = Mathf.RoundToInt((float) texture.height / _upgradesPerColumn);
			var spriteWidth = Mathf.RoundToInt((float) texture.width / _upgradesPerRow);
			for (var i = _upgradesPerColumn-1; i > -1; i--)
			{
				var spriteList = new List<Sprite>();
				for (var j = 0; j < _upgradesPerRow; j++)
				{
					var sprite = Sprite.Create(texture,
						new Rect(j * spriteWidth, i * spriteCount, spriteWidth, spriteCount),
						new Vector2(0.5f, 0.5f));
					spriteList.Add(sprite);
				}

				Upgrades[_upgradesPerColumn -1 - i] = spriteList;
			}
		}

		private IEnumerator GetSkins()
		{
			DownloadHandler request = null;
			var succeed = false;
			yield return GetTexture("DungeonQuest/peekaboo-spritesheet.png", (success, handler) =>
			{
				succeed = success; 
				request = handler;
			});

			if (request == null) yield break;

			var texture = ((DownloadHandlerTexture) request).texture;

			var rows = Mathf.RoundToInt((float) texture.height / _skinHeight);
			for (var i = 0; i <10; i++) // always ten as for now
			{
				var spriteList = new List<Sprite>();
				for (var j = 0; j < rows; j++)
				{
					var sprite = Sprite.Create(texture,
						new Rect(i* _skinWidth, j * _skinHeight, _skinWidth, _skinHeight),
						new Vector2(0.5f, 0.5f));
					spriteList.Add(sprite);
				}

				Sprites.Add(spriteList);
			}
		}
		
		private IEnumerator GetFile(string path, Action<bool, DownloadHandler> onComplete)
		{
			var uriBuilder = new UriBuilder(Application.persistentDataPath + Path.DirectorySeparatorChar + path);
			var uri = uriBuilder.Uri;
			var request = UnityWebRequest.Get(uri);
			yield return request.SendWebRequest();
			if (!string.IsNullOrEmpty(request.error))
			{
				Debug.Log("Always error");
				var streamBuilder = new UriBuilder(Path.Combine(Application.streamingAssetsPath,path));
				var stream = streamBuilder.Uri;
				var streaming = UnityWebRequest.Get(stream);
				yield return streaming.SendWebRequest();
				onComplete(string.IsNullOrEmpty(streaming.error), streaming.downloadHandler);
				yield break;
			}
			onComplete(string.IsNullOrEmpty(request.error), request.downloadHandler);
		}
		
		private IEnumerator GetTexture(string path, Action<bool, DownloadHandler> onComplete)
		{
			var uriBuilder = new UriBuilder(Path.Combine(Application.persistentDataPath,path));
			var uri = uriBuilder.Uri;
			var request = UnityWebRequestTexture.GetTexture(uri);
			yield return request.SendWebRequest();
			if (!string.IsNullOrEmpty(request.error))
			{
				var streamBuilder = new UriBuilder(Path.Combine(Application.streamingAssetsPath,path));
				var stream = streamBuilder.Uri;
				var streaming = UnityWebRequestTexture.GetTexture(stream);
				yield return streaming.SendWebRequest();
				onComplete(string.IsNullOrEmpty(streaming.error), streaming.downloadHandler);
				yield break;
			}
			onComplete(string.IsNullOrEmpty(request.error), request.downloadHandler);
		}
		
		public static Sprite LoadSprite(string path)
		{
			byte[] bytes = File.ReadAllBytes(path);
			Texture2D texture = new Texture2D(1, 1);
			texture.LoadImage(bytes);
			Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
				new Vector2(0.5f, 0.5f));
			return sprite;
		}
		
	}

}

