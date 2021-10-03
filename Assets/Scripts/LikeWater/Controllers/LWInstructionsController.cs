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
	public class LWInstructionsController : LWBaseController
	{

		[SerializeField] private ClickingCarousel _carousel;
		[SerializeField] private PageData _prefab;
		
		protected override void Start()
		{
			base.Start();
			var count = LWResourceManager.Instructions.Count;
			_carousel.Load(_prefab, count);
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