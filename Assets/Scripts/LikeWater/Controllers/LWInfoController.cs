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
		[SerializeField] private SimpleButton _button;
		[SerializeField] private SimpleButton _okayButton;
		[SerializeField] private Image _image;

		public void Build(LWResourceManager.Info info)
		{
			_titleText.text = info.Title;
			_descriptionText.text = info.Description;
			if (info.Image != null)
				_image.sprite = info.Image;
			else
				_button.gameObject.SetActive(false);
			if (info.Link != null)
			{
				_button.Evt_BasicEvent_Click += ()=> Application.OpenURL(info.Link);
			}
			if (info.IsBlocker) _okayButton.SetVisibility(false);
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