using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LikeWater
{
	[Serializable]
	public class LWData
	{
		private static LWData _current;

		public static LWData current
		{
			get
			{
				if (_current == null)
					_current = new LWData();
				return _current;
			}
		}

		public void Setup(LWData data)
		{
			_current = data;
		}

		public int Coins;
		public int Goal = 64;

		public Dictionary<string, List<FlowerMonth>> FlowerDictionary = new Dictionary<string, List<FlowerMonth>>();

		public string MainFlower;
		public string DisplayFlower;

		[Serializable]
		public class FlowerMonth
		{
			public int PlantIndex;
			public int SpriteIndex;
			public string Date;
			public string Attributes;
			public int DrinkAmount;
			public int Goal;
			public bool IsComplete;

			public FlowerMonth()
			{
				SpriteIndex = 0;
				PlantIndex = -1;
				Attributes = "";
			}
		}


		[Serializable]
		public class Drink
		{
			public int SpriteIndex;
			public string Color;
			public Dictionary<string, int> Attributes;

			public Drink()
			{
				Color = "#FFFFFF";
				Attributes = new Dictionary<string, int> {{LWConfig.AttributeWaterKey, 8}};
			}
		}

		public List<Drink> DrinkAttributes
		{
			get
			{
				if (_drinkAttributes == null)
				{
					_drinkAttributes = new List<Drink>();
				}

				return _drinkAttributes;
			}
		}

		private List<Drink> _drinkAttributes;

		[Serializable]
		public class NotificationData
		{
			public string Time;
			public bool IsActive;
			public string Type;
			public List<int> Ids = new List<int>();
			public Dictionary<DayOfWeek, string> Notifications = new Dictionary<DayOfWeek, string>();
		}

		public List<NotificationData> Notifications = new List<NotificationData>();
	}
}