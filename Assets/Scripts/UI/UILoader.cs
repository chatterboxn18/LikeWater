using System;
using System.Collections;
using System.IO;
using LikeWater;
using UnityEngine;
using UnityEngine.Networking;

public class UILoader : MonoBehaviour
{
	[SerializeField] private CanvasGroup _loaderGroup;
	private RectTransform _loaderRect;

	protected virtual void Awake()
	{
		_loaderRect = _loaderGroup.GetComponent<RectTransform>();
	}

	protected IEnumerator LoadImage(string path, Action<Sprite> OnComplete)
	{
		LeanTween.rotate(_loaderRect, -360, 1).setLoopClamp();
		if (File.Exists(path))
		{
			var sprite = LWResourceManager.LoadSprite(path);
			LeanTween.alphaCanvas(_loaderGroup, 0, LWConfig.FadeTime).setOnComplete(() =>
			{
				_loaderGroup.gameObject.SetActive(false);
			});
			OnComplete(sprite);
			yield break;
		}
    			
		var request = UnityWebRequestTexture.GetTexture(path);
		yield return request.SendWebRequest();

		while (!request.isDone)
		{
			
		}
		try
		{
			var texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
			var sprite = Extensions.Texture2DToSprite(texture);
			LeanTween.alphaCanvas(_loaderGroup, 0, LWConfig.FadeTime).setOnComplete(() =>
			{
				_loaderGroup.gameObject.SetActive(false);
			});
			OnComplete(sprite);
    
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
