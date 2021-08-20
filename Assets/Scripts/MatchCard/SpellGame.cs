using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Queendom
{
	public class SpellGame : MonoBehaviour
	{
		[SerializeField] private SpellCard _spellCardPrefab;

		[SerializeField] private Transform _spellCardContainer;

		private List<SpellCard> _spellList = new List<SpellCard>();

		private int _currentIndex;

		[Serializable]
		public class Item
		{
			public int Value;
			public string Name;
			public MagicItem Card;
		}


		[Header("Items")] 
		[SerializeField] private List<Item> _itemCards;
		private int _level = 3;
		private int _currentItemAmount = 0;

		private void Start()
		{
			Setup();
		}

		private void Setup()
		{
			for (var i = 0; i < 5; i++)
			{
				AddNewCard();
			}
		}

		private void AddNewCard()
		{
			var card = Instantiate(_spellCardPrefab, _spellCardContainer);
			card.Setup(Random.Range(2, 4));
			_spellList.Add(card);
		}

		private void SetEnemies()
		{
			var random = Random.Range(2, 4);
			_currentItemAmount = random;
			for (var i = 0; i < _currentItemAmount; i++)
			{
				var value = Random.Range(1, _level);
				_itemCards[i].Value = value;
				_itemCards[i].Card.SetText(value);
				_itemCards[i].Card.gameObject.SetActive(true);
			}
		}

		private void CastOnItem(int value)
		{
			if (_currentIndex == _currentItemAmount)
			{
				SetEnemies();
			}
		}
		
		public void ButtonEvt_Cast(int index)
		{
			if (index == _spellList[0].SpellList[_currentIndex])
			{
				_currentIndex++;
				if (_spellList[0].SpellList.Length == _currentIndex)
				{
					var gameObject = _spellList[0];
					_spellList.Remove(gameObject);
					Destroy(gameObject.gameObject);
					AddNewCard();
					_currentIndex = 0;
				}

				return;
			}

			_currentIndex = 0;
		}
	}
}