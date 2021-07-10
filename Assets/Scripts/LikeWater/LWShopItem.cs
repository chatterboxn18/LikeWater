using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LikeWater
{
	public class LWShopItem : MonoBehaviour
	{
		[SerializeField] private Image _icon;
		[SerializeField] private TextMeshProUGUI _priceText;
		[SerializeField] private TextMeshProUGUI _earnsText;
		private SimpleButton _button;
		public SimpleButton Button => _button;
		private LWResourceManager.Flower _flower;

		public Action<string> Evt_BoughtFlower = delegate { };
		public Action<string> Evt_SetFlower = delegate { };

		private string _date;

		private LWData.FlowerMonth _currentFlower
		{
			get
			{
				if (string.IsNullOrEmpty(LWData.current.MainFlower))
					return new LWData.FlowerMonth();
				var currentFlower = DateTime.Parse(LWData.current.MainFlower);
				var data =
					LWData.current.FlowerDictionary[currentFlower.Month + "/" + currentFlower.Year][
						currentFlower.Day - 1];
				return data;
			}
		}

		private void Awake()
		{
			_button = GetComponent<SimpleButton>();
		}

		public void SetDate(string date)
		{
			_date = date;
		}
		
		public void SetItem(Sprite sprite, LWResourceManager.Flower flower)
		{
			_icon.sprite = sprite;
			_priceText.text = flower.Cost.ToString();
			_earnsText.text = "+ " + flower.Earns;
			_flower = flower;
		}

		public void ButtonEvt_BuyFlower()
		{
			var coins = LWData.current.Coins;
			if (_flower.Cost > coins)
			{
				LWTransitionController.PopupError(LWTransitionController.Toasts.TextMessage, "Insufficient Coins");
				return;
			}

			LWData.current.Coins -= _flower.Cost;
			//extra check but it should be -1
			var date = DateTime.Parse(_date);
			var dict = LWData.current.FlowerDictionary;
			var currentFlower = dict[date.Month + "/" + date.Year][date.Day - 1];
			if (currentFlower.PlantIndex == -1) //why would the plant index ever be not -1?
			{
				currentFlower.PlantIndex = _flower.Index;
				currentFlower.SpriteIndex = 0;
				LWData.current.MainFlower = currentFlower.Date;
				Debug.Log(currentFlower.Date);
				dict[date.Month + "/" + date.Year][date.Day - 1] = currentFlower;
			}

			LWData.current.FlowerDictionary = dict;
			Evt_BoughtFlower(LWData.current.Coins.ToString());
			SerializationManager.Save(LWConfig.DataSaveName, LWData.current);
			LWTransitionController.TransitionTo(LWTransitionController.Controllers.Shop,
				LWTransitionController.Controllers.Water);
		}
	}
}