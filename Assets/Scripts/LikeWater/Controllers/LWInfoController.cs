using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace LikeWater
{
	public class LWInfoController : LWBaseController
	{

		[SerializeField] private TextMeshProUGUI _titleText;
		[SerializeField] private TextMeshProUGUI _descriptionText;
		[SerializeField] private Image _image;

		public void Build(string title, string description, Sprite sprite = null)
		{
			_titleText.text = title;
			_descriptionText.text = description;
			if (sprite != null)
				_image.sprite = sprite;
			else
				_image.gameObject.SetActive(false);
		}

		public void Evt_OpenPopup()
		{
			TransitionOn();
		}

		public void Evt_ClosePopup()
		{
			TransitionOff();
		}
	}
}