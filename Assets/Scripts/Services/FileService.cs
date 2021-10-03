using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class FileService : Service
{
	public IEnumerator DownloadFile(string fileName, Action<bool, string> onComplete)
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
	
}
