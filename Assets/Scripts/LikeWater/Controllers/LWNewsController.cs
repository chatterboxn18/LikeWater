using System.Collections;
using System.Collections.Generic;
using LikeWater;
using UnityEngine;

public class LWNewsController : MonoBehaviour
{
	[SerializeField] private LWMediaItem _mediaItemPrefab;
	[SerializeField] private Transform _mediaContainer;

	private void Start()
	{
		var news = LWResourceManager.News;
		foreach (var item in news)
		{
			var media = Instantiate(_mediaItemPrefab, _mediaContainer);
			if (!item.hasThumbnail)
				media.SetItem(true, item.Title, item.Url);
			else
				media.SetItem(false, item.Title, item.Url, item.Thumbnail);
		}
	}
}
