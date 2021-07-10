using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;

namespace LikeWater
{
	public class LWResourceManager : MonoBehaviour
	{
		public static List<Music> MusicList => _musicList;

		private static List<VideoItem> _videoList = new List<VideoItem>();
		private static List<Music> _musicList = new List<Music>();

		public static bool IsLoaded => _isLoaded;

		private static bool _isLoaded;

		public static Dictionary<int, List<Sprite>> Sprites = new Dictionary<int, List<Sprite>>();
		private int _spriteWidth = 400;
		private int _spriteHeight = 600;

		private static List<Sprite> _drinkIcons = new List<Sprite>();
		public static List<Sprite> DrinkIcons => _drinkIcons;

		private static List<Flower> _flowers = new List<Flower>();

		public static List<Flower> Flowers => _flowers;
		
		private static List<NewsItem> _news = new List<NewsItem>();

		public static List<NewsItem> News => _news;
		
		private static StreamItems _streamItem = new StreamItems()
		{
			AppLinks =  new List<LinkItem>(), 
			Videos = new List<VideoItem>()
		};

		public static StreamItems StreamItem => _streamItem;

		public class Flower
		{
			public string Name;
			public string Type;
			public int Index;
			public int Cost;
			public int Earns;
		}

		public class NewsItem
		{
			public string Title;
			public string Url;
			public bool hasThumbnail;
			public string Thumbnail;
		}

		public class StreamItems
		{
			public List<LinkItem> AppLinks;
			public List<VideoItem> Videos;
		}
		
		public class VideoItem
		{
			public string ImageUrl;
			public string Title;
			public string Date;
			public string Url;
		}

		public class LinkItem
		{
			public string Name;
			public Sprite Icon;
			public string Link;
		}

		public class Music
		{
			public string Title;
			public List<LinkItem> Links;
			public List<VideoItem> Videos;
		}

		private IEnumerator Start()
		{

			LWData.current.Setup(
				(LWData) SerializationManager.Load(Application.persistentDataPath + "/saves/likewater.rv"));
			PrepareData();
			yield return DownloadFiles();
			yield return GetVideos();
			yield return GetFlowers();
			yield return GetSprites();
			yield return GetDrinkIcons();
			yield return GetNews();
			yield return GetStreams();
			_isLoaded = true;
		}

		private void PrepareData()
		{
			var date = DateTime.Today;
			var key = date.Month + "/" + date.Year;
			var dict = LWData.current.FlowerDictionary;

			if (!dict.ContainsKey(key))
			{
				dict.Add(key, new List<LWData.FlowerMonth>());
				var days = DateTime.DaysInMonth(date.Year, date.Month);
				for (var day = 0; day < days; day++)
				{
					var flower = new LWData.FlowerMonth {Date = date.Month + "/" + (day + 1) + "/" + date.Year};
					dict[key].Add(flower);
				}

				LWData.current.FlowerDictionary = dict;
			}

			if (string.IsNullOrEmpty(LWData.current.MainFlower))
				LWData.current.MainFlower = date.ToShortDateString();

		}

		private IEnumerator DownloadFiles()
		{
			var configPath = "";
			var noInternet = false;
			yield return DownloadFile(LWConfig.ConfigFile, (success, path) =>
			{
				if (success)
				{
					configPath = path;
				}
			});

			if (string.IsNullOrEmpty(configPath))
			{
				noInternet = true;
				if (File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + LWConfig.ConfigFile))
				{
					configPath = Application.persistentDataPath + Path.DirectorySeparatorChar + LWConfig.ConfigFile;
				}
				else if (File.Exists(
					Application.streamingAssetsPath + Path.DirectorySeparatorChar + LWConfig.ConfigFile))
				{
					configPath = Application.streamingAssetsPath + Path.DirectorySeparatorChar + LWConfig.ConfigFile;
				}
				else
				{
					Debug.LogError("Critical Failure - No config");
					yield break;
				}
			}

			var uriBuilder = new UriBuilder(configPath);
			var uri = uriBuilder.Uri;
			var request = UnityWebRequest.Get(uri);
			yield return request.SendWebRequest();
			var data = JSON.Parse(request.downloadHandler.text);
			var config = data["config"];
			var configDate = config["last-update"];

			if (PlayerPrefs.HasKey(LWConfig.LastModifiedKey))
			{
				if (configDate == PlayerPrefs.GetString(LWConfig.LastModifiedKey))
				{
					//Don't need to download new files
					yield break;
				}
			}

			var configFiles = config["files"].AsArray;
			var hasError = false;
			foreach (var file in configFiles)
			{
				yield return DownloadFile(file.Value,
					(complete, path) =>
					{
						if (!complete)
						{
							Debug.LogError("The path: " + path + " failed to download.");
							hasError = true;
						}
					});
			}
			
			// Don't want to update the config unless all the files downloaded
			if (!hasError)
			{
				Debug.Log("Updated the files");
				PlayerPrefs.SetString(LWConfig.LastModifiedKey, configDate);
			}
			
		}
		
		private IEnumerator DownloadFile(string fileName, Action<bool, string> onComplete)
		{
			
			var request = UnityWebRequest.Get(LWConfig.ServerPath+ fileName);
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
		
		private IEnumerator GetFlowers()
		{
			DownloadHandler request = null;
			var succeed = false;
			yield return GetFile("likewater-pots.json", (success, handler) =>
			{
				succeed = success; 
				request = handler;
			});

			if (request == null) yield break;
			var data = request.text;
			var flowerItems = JSON.Parse(data);
			foreach (var flower in flowerItems["Flowers"].AsArray)
			{
				var value = flower.Value;
				var item = new Flower();
				item.Name = value["Name"];
				item.Type = value["Type"];
				if (!int.TryParse(value["Index"], out item.Index))
					Debug.LogError("Failed to parse: " + value["Index"]);
				if (!int.TryParse(value["Cost"], out item.Cost))
					Debug.LogError("Failed to parse: " + value["Cost"]);
				if (!int.TryParse(value["Earns"], out item.Earns))
					Debug.LogError("Failed to parse: " + value["Earns"]);
				_flowers.Add(item);

			}
		}

		private IEnumerator GetVideos()
		{
			DownloadHandler request = null;
			var succeed = false;
			yield return GetFile("likewater-music.json", (success, handler) =>
			{
				succeed = success; 
				request = handler;
			});

			if (request == null) yield break;

			var data = request.text;
			var videoItems = JSON.Parse(data);
			foreach (var videoItem in videoItems["Music"].AsArray)
			{
				var music = new Music();
				var value = videoItem.Value;

				var links = value["Links"].AsArray;
				var videos = value["Videos"].AsArray;
				music.Title = value["Title"];
				music.Links = new List<LinkItem>();
				foreach (var linkItem in links)
				{
					var linkValue = linkItem.Value;
					var item = new LinkItem();
					item.Link = linkValue["link"];
					yield return GetImage(LWConfig.ServerPath + linkValue["icon"], (sprite) => { item.Icon = sprite; });
					music.Links.Add(item);

				}

				music.Videos = new List<VideoItem>();
				Debug.Log(value);
				foreach (var vidItem in videos)
				{
					var videoValue = vidItem.Value;
					var video = new VideoItem();
					video.Title = videoValue["title"];
					video.Date = videoValue["date"];
					video.Url = videoValue["video-url"];
					video.ImageUrl = GetImageLink(videoValue["image-url"]);
					//yield return GetImage(videoValue["image-url"], sprite => { video.ImageUrl = sprite; });
					music.Videos.Add(video);
				}

				_musicList.Add(music);
			}
		}

		private IEnumerator GetNews()
		{
			DownloadHandler request = null;
			var succeed = false;
			yield return GetFile("likewater-news.json", (success, handler) =>
			{
				succeed = success; 
				request = handler;
			});

			if (request == null) yield break;

			var data = request.text;
			var newItems = JSON.Parse(data);
			foreach (var newItem in newItems["News"].AsArray)
			{
				var news = new NewsItem();
				var value = newItem.Value;
				news.Title = value["Title"];
				news.Url = value["Link"];
				var thumb = !string.IsNullOrEmpty(value["Thumbnail"]);
				news.hasThumbnail = thumb;
				if (thumb)
					news.Thumbnail = GetImageLink(value["Thumbnail"]);
				_news.Add(news);
			}
		}
		
		private IEnumerator GetStreams()
		{
			DownloadHandler request = null;
			var succeed = false;
			yield return GetFile("likewater-stream-android.json", (success, handler) =>
			{
				succeed = success; 
				request = handler;
			});

			if (request == null || !succeed) yield break;

			var data = request.text;
			var streams = JSON.Parse(data);
			var items = streams["Stream"];
			
			foreach (var appLink in items["Links"].AsArray)
			{
				var link = new LinkItem();
				var value = appLink.Value;
				link.Name= value["title"];
				link.Link = value["link"];
				_streamItem.AppLinks.Add(link);
			}
			
			foreach (var appLink in items["Videos"].AsArray)
			{
				var media = new VideoItem();
				var value = appLink.Value;
				media.Title= value["title"];
				media.Date = value["date"];
				media.Url = value["video-url"];
				media.ImageUrl = GetImageLink(value["image-url"]);
				_streamItem.Videos.Add(media);
			}
		}
		
		private IEnumerator GetDrinkIcons()
		{
			DownloadHandler request = null;
			var succeed = false;
			yield return GetTexture("likewater-drinks.png", (success, handler) =>
			{
				succeed = success; 
				request = handler;
			});

			if (request == null) yield break;

			var texture = ((DownloadHandlerTexture) request).texture;

			var width = 300;
			var height = 300;
			var spriteCount = Mathf.RoundToInt((float) texture.height / _spriteHeight);
			var counter = 0;
			var spriteList = new List<Sprite>();
			for (var i = 0; i < spriteCount; i++)
			{
				for (var j = 0; j < 3; j++)
				{
					var sprite = Sprite.Create(texture,
						new Rect(j * width, i * height, width, height),
						new Vector2(0.5f, 0.5f));
					spriteList.Add(sprite);
				}
			}

			_drinkIcons = spriteList;
		}

		private IEnumerator GetSprites()
		{
			DownloadHandler request = null;
			var succeed = false;
			yield return GetTexture("likewater-flowers.png", (success, handler) =>
			{
				succeed = success; 
				request = handler;
			});

			if (request == null) yield break;

			var texture = ((DownloadHandlerTexture) request).texture;

			var spriteCount = Mathf.RoundToInt((float) texture.height / _spriteHeight);
			var spriteWidth = Mathf.RoundToInt((float) texture.width / _spriteWidth);
			for (var i = 0; i < spriteCount; i++)
			{
				var spriteList = new List<Sprite>();
				for (var j = 0; j < spriteWidth; j++)
				{
					var sprite = Sprite.Create(texture,
						new Rect(j * _spriteWidth, i * _spriteHeight, _spriteWidth, _spriteHeight),
						new Vector2(0.5f, 0.5f));
					spriteList.Add(sprite);
				}

				Sprites[i] = spriteList;
			}

		}

		private string GetImageLink(string url)
		{
			if (File.Exists(Application.persistentDataPath +"/Images" +  UrlParser(url)))
				return Application.persistentDataPath + "/Images" + UrlParser(url);
			return url;
		}
		
		private IEnumerator GetImage(string url, Action<Sprite> onComplete)
		{
			if (File.Exists(Application.persistentDataPath +"/Images" +  UrlParser(url)))
			{
				var sprite = LoadSprite(Application.persistentDataPath + "/Images" + UrlParser(url));
				onComplete(sprite);
				yield break;
			}

			var request = UnityWebRequestTexture.GetTexture(url);
			yield return request.SendWebRequest();

			try
			{
				var texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
				var sprite = Extensions.Texture2DToSprite(texture);
				onComplete(sprite);

				try
				{
					var fileName = Path.GetFileName(request.uri.LocalPath);
					if (!Directory.Exists(Application.persistentDataPath + "/Images/" +
					                      request.uri.LocalPath.Replace(fileName, string.Empty)))
					{
						Directory.CreateDirectory(Application.persistentDataPath + "/Images/" +
						                          request.uri.LocalPath.Replace(fileName, string.Empty));
					}

					File.WriteAllBytes(Application.persistentDataPath + "/Images/" + request.uri.LocalPath,
						request.downloadHandler.data);
				}
				catch (Exception e)
				{
					Debug.LogError(e);
				}

			}
			catch (Exception e)
			{
				Debug.LogError("The url didn't download correctly " + url);
			}

			yield return null;
		}

		private string UrlParser(string path)
		{
			var newString = "";
			foreach (var character in path)
			{
				if (character == '\'')
					newString += "/";
				else
					newString += character;
			}

			var parts = newString.Split('/');
			var localPath = "";
			var found = false;
			foreach (var part in parts)
			{
				if (found)
					localPath += "/" + part;
				if (part.Contains(".com"))
				{
					found = true;
				}
			}

			return localPath;
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
