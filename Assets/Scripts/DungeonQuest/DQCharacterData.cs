using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonQuest
{
	public class DQCharacterData
	{
		private static DQCharacterData _current;
		
		public static DQCharacterData current
		{
			get
			{
				if (_current == null)
					_current = new DQCharacterData();
				return _current;
			}
		}

		// retrieves save fild and sets as _current
		public void Setup(DQCharacterData data)
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

		public class Character
		{
			public RedVelvet Name;
			public int AttackDamage;
			public float AttackSpeed;
			public float CritPercent;
			public int WeaponIndex;
			public int OutfitIndex;
		}

		// This call has a null check so it could be expensive without a reference 
		public Dictionary<RedVelvet, Character> Characters => _characters;
		
		private static Dictionary<RedVelvet, Character> _characters = new Dictionary<RedVelvet, Character>()
		{
			{RedVelvet.Irene, CreateCharacter(RedVelvet.Irene)},
			{RedVelvet.Seulgi, CreateCharacter(RedVelvet.Seulgi)},
			{RedVelvet.Wendy, CreateCharacter(RedVelvet.Wendy)},
			{RedVelvet.Joy, CreateCharacter(RedVelvet.Joy)},
			{RedVelvet.Yeri, CreateCharacter(RedVelvet.Yeri)},
						
		};

		private static Character CreateCharacter(RedVelvet name)
		{
			var character = new Character();
			character.Name = name;
			character.CritPercent = 0.01f;
			character.AttackDamage = 1;
			character.OutfitIndex = 0;
			return character;
		}

		public class Card
		{
			public Character Character;
			public int Index;
			public int Level;
			public int Rarity;
			public int Price; 
		}

		private static Dictionary<RedVelvet, List<Card>> _cardCollection = new Dictionary<RedVelvet, List<Card>>();
		public static Dictionary<RedVelvet, List<Card>> CardCollection => _cardCollection;

	}

}

