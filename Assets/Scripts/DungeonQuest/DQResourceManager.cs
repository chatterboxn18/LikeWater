using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace DungeonQuest
{
	public class DQResourceManager : MonoBehaviour
	{
		public static bool IsReady => _isReady;
		private static bool _isReady;
		private int _upgradesPerRow = 6;
		private int _upgradesPerColumn = 5;
		public static Dictionary<int, List<Sprite>> Upgrades = new Dictionary<int, List<Sprite>>();
		
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
			DQCharacterData.current.Setup(
				(DQCharacterData) SerializationManager.Load(Application.persistentDataPath + "/saves/" + DQConfig.SaveName + ".rv"));
			yield return DownloadFile("DungeonQuest/redvelvet-upgrades.png", (success, path) =>
			{
				if (success)
				{
					
				}
			});
			yield return GetUpgrades();
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

		private IEnumerator GetUpgrades()
		{
			DownloadHandler request = null;
			var succeed = false;
			yield return GetTexture("DungeonQuest/redvelvet-upgrades.png", (success, handler) =>
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

