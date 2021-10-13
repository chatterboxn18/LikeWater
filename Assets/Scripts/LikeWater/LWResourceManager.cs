using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;

namespace LikeWater
{
	public class LWResourceManager : MonoBehaviour
	{
		private const string _jarLocation = "jar:file://";

		private static string _serverPath = LWConfig.ServerPath;
		private string _key = "";
		[SerializeField] private GameObject _serverGroup;
		[SerializeField] private CanvasGroup _updateGroup;
		public static string ServerPath
		{
			get
			{
				if (PlayerPrefs.HasKey(LWConfig.ServerKey))
				{
					return _serverPath.Replace("BUCKET", PlayerPrefs.GetString(LWConfig.ServerKey));
				}
				return _serverPath.Replace("BUCKET", LWConfig.KRServer);
			}
		}

		public enum MediaType
		{
			Image, 
			Audio
		}
		
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

		public class Info
		{
			public string Title;
			public string Description;
			public Sprite Image = null;
			public string Link;
			public bool IsBlocker = false;
		}

		private static List<Info> _infoList = new List<Info>();
		public static List<Info> InfoList => _infoList;
		
		//bool for updating files
		private bool _isUpdating;
		private bool _denyUpdate;
		private bool _isCheckUpdate;

		public enum Servers
		{
			Korea = 0, 
			Singapore = 1, 
			California = 2, 
			London = 3
		}
		public void ButtonEvt_SetServer(int index)
		{
			switch ((Servers)index)
			{
				case Servers.Korea:
					_key = LWConfig.KRServer;
					break;
				case Servers.Singapore:
					_key = LWConfig.SEServer;
					break;
				case Servers.California:
					_key = LWConfig.USServer;
					break;
				case Servers.London:
					_key = LWConfig.EUServer;
					break;
			}
			PlayerPrefs.SetString(LWConfig.ServerKey, _key);
			_serverGroup.SetActive(false);
		}

		public void ButtonEvt_CheckUpdate(bool no)
		{
			_denyUpdate = !no;
			_isCheckUpdate = true;
			_updateGroup.LeanAlpha(0, LWConfig.FadeTime).setOnComplete(() =>
			{
				_updateGroup.gameObject.SetActive(false);
			});
		}
		
		private IEnumerator Start()
		{
			if (!PlayerPrefs.HasKey(LWConfig.ServerKey))
			{
				_serverGroup.SetActive(true);
				while (string.IsNullOrEmpty(_key))
				{
					yield return null;
				}
			}
			LWData.current.Setup(
				(LWData) SerializationManager.Load(Application.persistentDataPath + "/saves/likewater.rv"));
			LWCardData.current.Setup(
				(LWCardData) SerializationManager.Load(Application.persistentDataPath + "/saves/likewater-cards.rv"));
			PrepareData();
			yield return DownloadFiles();
			yield return GetVideos();
			yield return GetFlowers();
			yield return GetFlowerSprites();
			yield return GetDrinkIcons();
			yield return GetInstructions();
			yield return GetNews();
			yield return GetStreams();
			yield return GetAudioClips();
			_isLoaded = true;
		}

		private static List<string> _streakTexts;
		public static List<string> StreakTexts => _streakTexts;
		private void GetStreakText(JSONArray config)
		{
			_streakTexts = new List<string>();
			var counter = 1;
			foreach (var value in config)
			{
				var text = value.Value;
				_streakTexts.Add(text["" + counter]);
				counter++;
			}
		}

		private IEnumerator GetPopupInfo(JSONArray config)
		{
			foreach (var value in config)
			{
				var popup = value.Value;
				var info = new Info();
				info.Title = popup["title"];
				info.Description = popup["description"];
				if (!string.IsNullOrEmpty(popup["photo"]))
				{
					yield return StreamImage(popup["photo"], (b, sprite) =>
					{
						if (b)
						{
							info.Image = sprite;
						}
					});
				}
				info.Link = popup["link"];
				if (!string.IsNullOrEmpty(popup["isBlocked"]))
					info.IsBlocker = popup["isBlocked"] == "true";
				_infoList.Add(info);
			}
		}

		private IEnumerator StreamImage(string localPath, Action<bool,Sprite> onComplete)
		{
			var url = ServerPath + localPath;
			var request = UnityWebRequestTexture.GetTexture(url);
			yield return request.SendWebRequest();
			while (!request.isDone)
				yield return null;
			if (!string.IsNullOrEmpty(request.error))
			{
				Debug.LogError("Failed to stream image with error: " + request.error);
				onComplete(false, null);
				yield break;
			}
			var textureHandler = (DownloadHandlerTexture) request.downloadHandler;
			var texture = textureHandler.texture;
			var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
				new Vector2(0.5f, 0.5f));
			onComplete(true, sprite);
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
			yield return DownloadFile(LWConfig.ConfigFile, (success, path) =>
			{
				if (success)
				{
					configPath = path;
				}
			});

			if (string.IsNullOrEmpty(configPath))
			{
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

			GetStreakText(config["streak-notes"].AsArray);
			
			if (PlayerPrefs.HasKey(LWConfig.LastModifiedKey))
			{
				if (configDate == PlayerPrefs.GetString(LWConfig.LastModifiedKey))
				{
					//Don't need to download new files
					yield break;
				}
			}
			_updateGroup.gameObject.SetActive(true);
			_updateGroup.LeanAlpha(1, LWConfig.FadeTime);
			while (!_isCheckUpdate)
			{
				yield return null;
			}
			
			if (_denyUpdate)
				yield break;
			
			// Yield break above if the config isn't new, otherwise bool is true for updating certain media files
			_isUpdating = true;
			
			yield return GetPopupInfo(config["promo-messages"].AsArray);

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
			
			var request = UnityWebRequest.Get(ServerPath+ fileName);
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
		
		private IEnumerator GetTexture(string path, Action<bool, DownloadHandler> onComplete, bool downloaded = false)
		{
				
			var uriBuilder = new UriBuilder(Path.Combine(Application.persistentDataPath,path));
			if (!downloaded)
				uriBuilder = new UriBuilder(Path.Combine(ServerPath, path));
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
					yield return GetMedia(linkValue["icon"], (result) => { item.Icon = result.Sprite; });
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

			if (!succeed)
			{
				Debug.LogError("Texture " + "likewater-drinks.png" + " failed to download");
			}
			
			var texture = ((DownloadHandlerTexture) request).texture;

			var width = 300;
			var height = 300;
			var spriteCount = Mathf.RoundToInt((float) texture.height / _spriteHeight);
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

		#region Clips

		private static List<SoundClip> _audioClips = new List<SoundClip>();
		public static List<SoundClip> AudioClips =>_audioClips;
		
		public class SoundClip
		{
			public string Name;
			public string ClipLink;
			public AudioClip Clip;
		}
		
		private IEnumerator GetAudioClips()
		{
			var succeed = false;
			DownloadHandler dHandler = null;
			yield return GetFile("likewater-clips.json", (success, handler) =>
			{
				succeed = success;
				dHandler = handler;
			});
			if (!succeed)
			{
				Debug.LogError("likewater-clips.json failed to download");
				yield break;
			}
			var data = dHandler.text;
			var audioItems = JSON.Parse(data);
			var index = 0;
			if (PlayerPrefs.HasKey(LWConfig.ClipIndex))
			{
				index = PlayerPrefs.GetInt(LWConfig.ClipIndex);
			}
			foreach (var audio in audioItems["Clips"].AsArray)
			{
				var count = _audioClips.Count;
				var value = audio.Value;
				var clip = new SoundClip
				{
					Name = value["Name"],
					ClipLink = value["Link"]
				};
				if (count == index)
					yield return GetMedia(value["Link"], result => { clip.Clip = result.Clip;}, MediaType.Audio, _isUpdating);
				_audioClips.Add(clip);
			}
		}

		#endregion
		
		#region Instructions
		
		private static List<Sprite> _instructionsScreens;
		public static List<Sprite> Instructions => _instructionsScreens;
		
		private IEnumerator GetInstructions()
		{
			DownloadHandler request = null;
			var succeed = false;
			yield return GetTexture("likewater-instructions.png", (success, handler) =>
			{
				succeed = success; 
				request = handler;
			});
		
			if (request == null) yield break;
		
			if (!succeed)
			{
				Debug.LogError("Texture " + "likewater-instructions.png" + " failed to download");
			}
			
			var texture = ((DownloadHandlerTexture) request).texture;
			
			var width = 1080;
			var height = 1920;
			var spriteCount = Mathf.RoundToInt((float) texture.width / width);
			var spriteList = new List<Sprite>();
			for (var i = 0; i < spriteCount; i++)
			{
				var sprite = Sprite.Create(texture,
					new Rect(i * width, 0, width, height),
					new Vector2(0.5f, 0.5f));
				spriteList.Add(sprite);
			}
		
			_instructionsScreens = spriteList;
		}

		#endregion


		private IEnumerator GetFlowerSprites()
		{
			DownloadHandler request = null;
			yield return GetTexture("likewater-flowers.png", (success, handler) =>
			{
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

		public struct MediaResult
		{
			public AudioClip Clip;
			public Sprite Sprite;
		}

		private IEnumerator GetMedia(string url, Action<MediaResult> onComplete, MediaType type = MediaType.Image, bool overwrite = false)
		{
			// If File Exists in persistent 
			#region Persistent Check

			var isLocal = false;
			if (type == MediaType.Image)
			{
				if (File.Exists(Application.persistentDataPath + "/Images/" + url))
				{
					Sprite sprite = LoadSprite(Application.persistentDataPath + "/Images/" + url);
					onComplete(new MediaResult() {Sprite = sprite});
					yield break;
				}
			}

			if (type == MediaType.Audio)
			{
				if (File.Exists(Application.persistentDataPath + "/Audio/" + url) && !overwrite)
				{
					url = Application.persistentDataPath + "/Audio/" + url;
					isLocal = true;
				}
			}
			#endregion

			#region CheckInternetAndCallHandler
			
			var request = new UnityWebRequest();
			var folderName = "";
			switch (type)
			{
				case MediaType.Image:
					folderName = "/Image/";
					request = UnityWebRequestTexture.GetTexture(ServerPath+ url); // If it was persistent it would have been loaded already so i don't bother
					break;
				case MediaType.Audio:
					folderName = "/Audio/";
					if (!isLocal || overwrite) url = ServerPath + "Audio/" + url;
					var uri = new UriBuilder(url);
					request = UnityWebRequestMultimedia.GetAudioClip(uri.Uri, AudioType.WAV);
					break;
			}
			yield return request.SendWebRequest();
			
			#endregion
			
			#region WriteToPersistent
			try
			{
				switch (type)
				{
					case MediaType.Image:
						var texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
						var sprite = Extensions.Texture2DToSprite(texture);
						onComplete(new MediaResult(){Sprite = sprite});
						if (isLocal) yield break;
						break;
					case MediaType.Audio:
						var clip = ((DownloadHandlerAudioClip) request.downloadHandler).audioClip;
						onComplete(new MediaResult(){Clip = clip});
						if(isLocal) yield break;
						break;
				}

				try
				{
					var fileName = Path.GetFileName(request.uri.LocalPath);
					if (!Directory.Exists(Application.persistentDataPath + folderName))
					{
						Directory.CreateDirectory(Application.persistentDataPath + folderName);
					}
					if (overwrite)
    				{
    					if (File.Exists(Application.persistentDataPath + folderName + fileName))
    						File.Delete(Application.persistentDataPath + folderName + fileName);
    				}
					
					File.WriteAllBytes(Application.persistentDataPath + folderName + fileName,
						request.downloadHandler.data);

					yield break;
				}
				catch (Exception e)
				{
					Debug.LogError(e);
				}
			}
			catch (Exception e)
			{
				Debug.LogError("The url didn't download correctly " + url + " And the error was " + e);
			}
			#endregion

			#region CheckStreaming
			// I actually only want streaming assets as a LAST RESORT 

			if (type == MediaType.Image){
				// If file exists in streaming 
				if (File.Exists(Application.streamingAssetsPath + "/Images" + UrlParser(url)))
				{
					Sprite sprite = LoadSprite(Application.streamingAssetsPath + "/Images" + UrlParser(url));
					onComplete(new MediaResult() {Sprite = sprite});
					yield break;
				}
#if UNITY_ANDROID
				var localRequest =
					UnityWebRequestTexture.GetTexture(new UriBuilder(_jarLocation + Application.dataPath + "!assets" + "/Images/" + url).Uri);
				yield return localRequest.SendWebRequest();
				if (string.IsNullOrEmpty(localRequest.error))
				{
					var handler = (DownloadHandlerTexture) localRequest.downloadHandler;
					if (handler.texture != null)
					{
						var sprite = Extensions.Texture2DToSprite(handler.texture);
						onComplete(new MediaResult() {Sprite = sprite});
						yield break;
					}
					
				}

				localRequest =
					UnityWebRequestTexture.GetTexture(new UriBuilder(Application.streamingAssetsPath + "/Images/" + url).Uri);
				yield return localRequest.SendWebRequest();
				if (string.IsNullOrEmpty(localRequest.error))
				{
					var handler = (DownloadHandlerTexture) localRequest.downloadHandler;
					if (handler.texture != null)
					{
						var sprite = Extensions.Texture2DToSprite(handler.texture);
						onComplete(new MediaResult() {Sprite = sprite});
						yield break;
					}
				}				
#endif
			}
			
			if (type == MediaType.Audio)
			{
				if (File.Exists(Application.streamingAssetsPath + "/Audio/" + url))
				{
					url = Application.streamingAssetsPath + "/Audio/" + url;
				}
#if UNITY_ANDROID
				var localRequest =
					UnityWebRequestMultimedia.GetAudioClip(new UriBuilder(_jarLocation + Application.dataPath + "!assets" + "/Audio/" + url).Uri, AudioType.WAV);
				yield return localRequest.SendWebRequest();
				if (string.IsNullOrEmpty(localRequest.error))
				{
					var handler = (DownloadHandlerAudioClip) localRequest.downloadHandler;
					if (handler.audioClip != null)
					{
						var clip = handler.audioClip;
						onComplete(new MediaResult() {Clip = clip});
						yield break;
					}
					
				}

				localRequest =
					UnityWebRequestMultimedia.GetAudioClip(new UriBuilder(Application.streamingAssetsPath + "/Audio/" + url).Uri, AudioType.WAV);
				yield return localRequest.SendWebRequest();
				if (string.IsNullOrEmpty(localRequest.error))
				{
					var handler = (DownloadHandlerAudioClip) localRequest.downloadHandler;
					if (handler.audioClip != null)
					{
						var clip = handler.audioClip;
						onComplete(new MediaResult() {Clip = clip});
						yield break;
					}
				}				
#endif
				var streamingRequest = new UnityWebRequest();
				switch (type)
				{
					case MediaType.Image:
						streamingRequest = UnityWebRequestTexture.GetTexture(new UriBuilder(url).Uri);
						break;
					case MediaType.Audio:
						streamingRequest = UnityWebRequestMultimedia.GetAudioClip(new UriBuilder(url).Uri, AudioType.WAV);
						break;
				}
				yield return streamingRequest.SendWebRequest();
				
				switch (type)
				{
					case MediaType.Image:
						var texture = ((DownloadHandlerTexture) streamingRequest.downloadHandler).texture;
						var sprite = Extensions.Texture2DToSprite(texture);
						onComplete(new MediaResult(){Sprite = sprite});
						break;
					case MediaType.Audio:
						var clip = ((DownloadHandlerAudioClip) streamingRequest.downloadHandler).audioClip;
						onComplete(new MediaResult(){Clip = clip});
						break;
				}
				
			}
			#endregion
		}

		// Literally only works right now to rid of the web url part of a link
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