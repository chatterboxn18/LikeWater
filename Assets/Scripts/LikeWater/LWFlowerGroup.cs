using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LikeWater
{
	public class LWFlowerGroup : MonoBehaviour
	{
		[SerializeField] private Plant[] _plants;

		[Serializable]
		public struct Plant
		{
			public bool isActive;
			public Image Image;
			public Sprite[] Sprites;
			public TextMeshProUGUI DateTag;
			public string Date;
			public SimpleButton Button;
		}

		public bool IsFull
		{
			get
			{
				var isFull = _plants.Count(plant => plant.Image.color.a >= 1);
				return isFull == 3;
			}
		}

		private float _timer = 0;
		private float _spriteSwitchTime = 0.5f;
		private int _currentFrame = 0;

		public void SetPlant(int indexOnShelf, Sprite[] sprites, string date = "")
		{
			if (!string.IsNullOrEmpty(date))
			{
				_plants[indexOnShelf].DateTag.text = date;
			}

			var color = _plants[indexOnShelf].Image.color;
			_plants[indexOnShelf].Image.sprite = sprites[0];
			_plants[indexOnShelf].Image.color = color.SetAlpha(1);
			_plants[indexOnShelf].Sprites = sprites;
			_plants[indexOnShelf].isActive = true;
		}

		private void Evt_OpenPopup(string day)
		{
			
			var date = DateTime.Parse(day, new CultureInfo("en-US"));
			if (date > DateTime.Today)
			{
				LWTransitionController.PopupError(LWTransitionController.Toasts.TextMessage, "Future Date");
				return;
			}
			
			var dict = LWData.current.FlowerDictionary;
			if (dict[date.Month + "/" + date.Year][date.Day - 1].PlantIndex != -1)
			{
				//LWData.current.MainFlower = date.ToShortDateString();
				LWTransitionController.TransitionOn(LWTransitionController.Controllers.Popup, date.ToShortDateString());
			}
			else
			{
				if (date < DateTime.Today.AddDays(-1))
				{
					LWTransitionController.PopupError(LWTransitionController.Toasts.TextMessage, "Past Date");
					return;
				}
				LWTransitionController.TransitionOff(LWTransitionController.Controllers.Pot);
				LWTransitionController.TransitionOn(LWTransitionController.Controllers.Shop, date.ToShortDateString());
			}
		}

		public void SetDate(int indexOnShelf, string date)
		{
			_plants[indexOnShelf].Button.Evt_BasicEvent_Click += () => Evt_OpenPopup(date);
			_plants[indexOnShelf].Date = date;
		}

		public Plant GetPlant(int index)
		{
			if (index < _plants.Length)
				return _plants[index];
			throw new Exception();
		}

		private void Update()
		{
			if (_timer >= _spriteSwitchTime)
			{
				var frame = _currentFrame%2;
				foreach (var item in _plants)
				{
					if (item.isActive)
						item.Image.sprite = item.Sprites[frame];
				}

				_currentFrame++;
				_timer = 0;
				return;
			}

			_timer += Time.deltaTime;

		}
	}
}