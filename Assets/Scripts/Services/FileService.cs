using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class FileService : Service
{
	private const string _jarLocation = "jar:file://";
	private static string _serverPath = LWConfig.ServerPath;
	
	public struct MediaResult
	{
		public AudioClip Clip;
		public Sprite Sprite;
	}

	public enum MediaType
	{
		Image, 
		Audio
	}
	
	public IEnumerator DownloadFile(string fileName, Action<bool, string> onComplete)
	{
			
		var request = UnityWebRequest.Get(LikeWater.LWResourceManager.ServerPath+ fileName);
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
	
	public IEnumerator GetFile(string path, Action<bool, DownloadHandler> onComplete)
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

	public IEnumerator GetTexture(string path, Action<bool, DownloadHandler> onComplete)
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
	
	public Sprite LoadSprite(string path)
	{
		byte[] bytes = File.ReadAllBytes(path);
		Texture2D texture = new Texture2D(1, 1);
		texture.LoadImage(bytes);
		Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
			new Vector2(0.5f, 0.5f));
		return sprite;

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
	
	public IEnumerator GetMedia(string url, Action<MediaResult> onComplete, MediaType type = MediaType.Image, bool overwrite = false)
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
			Debug.LogError("Retreiving Audio for: " + url);
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
				request = UnityWebRequestTexture.GetTexture(LikeWater.LWResourceManager.ServerPath+ url); // If it was persistent it would have been loaded already so i don't bother
				break;
			case MediaType.Audio:
				folderName = "/Audio/";
				if (!isLocal || overwrite) url = LikeWater.LWResourceManager.ServerPath + "Audio/" + url;
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
}
