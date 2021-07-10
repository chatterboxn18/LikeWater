using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LikeWater
{
	public class LWMusicPage : MonoBehaviour
	{
		[SerializeField] private LWMediaCard _mediaPrefab;
		[SerializeField] private Transform _mediaParent;
		[SerializeField] private Transform _musicParent;
		[SerializeField] private TextMeshProUGUI _cardTitle;
		[SerializeField] private LWMediaCard _musicPrefab;


		public void SetPage(int index)
		{
			var videoList = LWResourceManager.MusicList[index].Videos;
			var musicList = LWResourceManager.MusicList;
			_cardTitle.text = musicList[index].Title;
			foreach (var video in videoList)
			{
				var newVideo = Instantiate(_mediaPrefab, _mediaParent);
				newVideo.SetMediaCard(video);
			}

			foreach (var music in musicList[index].Links)
			{
				var musicLinks = Instantiate(_musicPrefab, _musicParent);
				musicLinks.SetMusicLink(music);
			}
		}
	}
}