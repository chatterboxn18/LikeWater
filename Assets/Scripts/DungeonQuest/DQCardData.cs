using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonQuest
{
	public class DQCardData
	{
		private static DQCardData _current;
		
		public static DQCardData current
		{
			get
			{
				if (_current == null)
					_current = new DQCardData();
				return _current;
			}
		}

		// retrieves save fild and sets as _current
		public void Setup(DQCardData data)
		{
			_current = data;
		}

		public enum RedVelvet
		{
			Irene = 0, 
			Seulgi = 1, 
			Wendy = 2, 
			Joy = 3, 
			Yeri = 4
		}

		public class Card
		{
			public DQCharacterData.Character Character;
			public int Index;
			public int Level;
			public int Rarity;
			public int Price; 
		}

		private static Dictionary<DQCharacterData.RedVelvet, List<Card>> _cardCollection = new Dictionary<DQCharacterData.RedVelvet, List<Card>>();

		public static Dictionary<DQCharacterData.RedVelvet, List<Card>> CardCollection
		{
			get => _cardCollection;
			set => _cardCollection = value;
		} 
		
		private static Dictionary<DQCharacterData.RedVelvet, List<Card>> _equipList = new Dictionary<DQCharacterData.RedVelvet, List<Card>>();

		public static Dictionary<DQCharacterData.RedVelvet, List<Card>> EquipList
		{
			get => _equipList;
			set => _equipList = value;
		}
	}

}

