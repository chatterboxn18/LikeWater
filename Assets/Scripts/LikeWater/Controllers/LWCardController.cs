using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace LikeWater
{
	public class LWCardController : LWBaseController
	{
		[SerializeField] private LWCardItem _cardItem;
		[SerializeField] private LWCardManager _cardManager;
		[SerializeField] private LWCardCarousel _cardCarousel;
		
		private Vector2 _nextLocation;
		private Vector2 _prevLocation;

		private int _currentIndex = 0;

		private bool _isTransitioning = false;

		private bool _isFirstLaunch = true; //Need a way to avoid first time enabling, can't think of a better way to do it yet

		protected override void Start()
		{
			base.Start();
			_cardCarousel.AssignData(_currentIndex, _cardManager);
			_cardCarousel.Load(_cardItem, 0); // the int is pointless here 
			_isFirstLaunch = false;
		}

		private void OnEnable()
		{
			if (_isFirstLaunch)
				return;
			_cardCarousel.AssignData(_currentIndex, _cardManager);
			_cardCarousel.Load(_cardItem, 0);
		}

		private void OnDisable()
		{
			_cardCarousel.Unload();
		}

		public void ButtonEvt_Open()
		{
			TransitionOn();
		}

		public void ButtonEvt_Close()
		{
			TransitionOff();
		}

		public override void Evt_ReceiveData(string date)
		{

		}
	}
}