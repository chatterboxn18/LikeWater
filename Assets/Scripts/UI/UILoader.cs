using System;
using System.Collections;
using System.IO;
using LikeWater;
using UnityEngine;
using UnityEngine.Networking;

public class UILoader : MonoBehaviour
{
	[SerializeField] protected CanvasGroup _loaderGroup;
	private RectTransform _loaderRect;
	
	protected virtual void Awake()
	{
		_loaderRect = _loaderGroup.GetComponent<RectTransform>();
	}

	protected virtual IEnumerator Start()
	{
		yield return null;
	}
	
	protected IEnumerator LoadAudio(string path, Action<AudioClip> onComplete)
	{
		var fileService = (FileService) ServiceManager.ServiceCollection[ServiceManager.Services.FileService];
		LeanTween.rotate(_loaderRect, -360, 1).setLoopClamp();
		yield return fileService.GetMedia(path, result =>
		{
			if (result.Clip != null)
				onComplete(result.Clip);
			LeanTween.alphaCanvas(_loaderGroup, 0, LWConfig.FadeTime).setOnComplete(() =>
			{
				_loaderGroup.gameObject.SetActive(false);
			});
		}, FileService.MediaType.Audio);
	}

	protected IEnumerator LoadImage(string path, Action<Sprite> onComplete)
	{
		LeanTween.rotate(_loaderRect, -360, 1).setLoopClamp();
		if (File.Exists(path))
		{
			var sprite = LWResourceManager.LoadSprite(path);
			LeanTween.alphaCanvas(_loaderGroup, 0, LWConfig.FadeTime).setOnComplete(() =>
			{
				_loaderGroup.gameObject.SetActive(false);
			});
			onComplete(sprite);
			yield break;
		}
    			
		var request = UnityWebRequestTexture.GetTexture(path);
		yield return request.SendWebRequest();

		while (!request.isDone)
		{
			yield return null;
		}
		try
		{
			var texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
			var sprite = Extensions.Texture2DToSprite(texture);
			LeanTween.alphaCanvas(_loaderGroup, 0, LWConfig.FadeTime).setOnComplete(() =>
			{
				_loaderGroup.gameObject.SetActive(false);
			});
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
			Debug.LogError("The url didn't download correctly " + path + e);
		}	
	}

}
