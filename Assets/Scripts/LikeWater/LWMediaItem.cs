using System;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace LikeWater
{
	public class LWMediaItem : UILoader
	{
		[SerializeField] private Image _thumbnail;
		[SerializeField] private TextMeshProUGUI _titleText;
		[SerializeField] private SimpleButton _button;
		private RectTransform _rectTransform;
		private float _videoHeight = 575f;

		protected override void Awake()
		{
			base.Awake();
			_rectTransform = GetComponent<RectTransform>();
			_button = GetComponent<SimpleButton>();
		}

		
		public void SetItem(bool isNews, string name, string url, string sprite = "")
		{
			if (!isNews)
			{
				var size = _rectTransform.sizeDelta;
				_rectTransform.sizeDelta = new Vector2(size.x, _videoHeight);
				_thumbnail.transform.parent.gameObject.SetActive(true);
				StartCoroutine(LoadImage(sprite, (item) =>
				{
					_thumbnail.sprite = item;
					LeanTween.alpha(_thumbnail.rectTransform, 1, LWConfig.FadeTime).setOnComplete(() =>
					{
						_thumbnail.gameObject.SetActive(true);
					});
				}));
			}

			_titleText.text = name;
			_button.Evt_BasicEvent_Click += () => Application.OpenURL(url);
		}

	}

}

