using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LikeWater
{
	public class LWMediaCard : UILoader
	{
		[SerializeField] private Image _mediaImage;
		[SerializeField] private TextMeshProUGUI _mediaTitle;
		[SerializeField] private TextMeshProUGUI _mediaDate;
		[SerializeField] private SimpleButton _button;

		private string _imageUrl = "";

		protected override IEnumerator Start()
		{
			while (string.IsNullOrEmpty(_imageUrl))
			{
				yield return null;
			}
			
		}

		public IEnumerator SetMediaCard(LWResourceManager.VideoItem item)
		{

			yield return LoadImage(item.ImageUrl, (sprite) =>
			{
				_mediaImage.sprite = sprite;
				LeanTween.alpha(_mediaImage.rectTransform, 1, LWConfig.FadeTime);
			});
			_mediaTitle.text = item.Title;
			_mediaDate.text = item.Date;
			if (_button)
			{
				_button.Evt_BasicEvent_Click += () => { Application.OpenURL(item.Url); };
			}
		}

		public void SetMusicLink(LWResourceManager.LinkItem item)
		{
			_mediaImage.sprite = item.Icon;
			_button.Evt_BasicEvent_Click += () => Application.OpenURL(item.Link);
		}
	}
}