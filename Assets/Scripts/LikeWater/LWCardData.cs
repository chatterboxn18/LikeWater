using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LikeWater
{
	[Serializable]
	public class LWCardData
	{
		private static LWCardData _current;

		public static LWCardData current
		{
			get
			{
				if (_current == null)
					_current = new LWCardData();
				return _current;
			}
		}

		public void Setup(LWCardData data)
		{
			_current = data;
		}

		[Serializable]
		public class FlowerCard
		{
			public int AmountCollected;
			public int CollectTotal;
			public int PlantIndex;
			public string Member;
			public int X;
			public int Y;
			public string SheetName;
		}

		// string is the path to the sprite sheet 
		public Dictionary<string, Dictionary<int, FlowerCard>> CollectedCards = new Dictionary<string, Dictionary<int, FlowerCard>>();

	}
}