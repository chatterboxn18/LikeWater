using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Queendom
{
	public class MagicCardGame : MonoBehaviour
	{
		[Serializable]
		public class CharacterCard
		{			
			public QueendomConfig.Character Name;
			public int MagicLevel;
			public MagicCharacter Card;
		}
		
		[Serializable]
		public class ItemCard
		{
			public int Value;
			public string Name;
			public MagicItem Card;
		}


		[Header("Character Cards")]
		[SerializeField] private List<CharacterCard> CardList;
		private int _selectedCharacters;
		private Dictionary<QueendomConfig.Character, CharacterCard> _charactersInPlay = new Dictionary<QueendomConfig.Character, CharacterCard>();
	
		[Header("Spell Deck")]
		[SerializeField] private MagicCard _characterCard;
		[SerializeField] private Transform _handContainer;
		[SerializeField] private SimpleButton _castButton;
		
		[Header("Items")]
		[SerializeField] private List<ItemCard> _itemCards;
		private int _level = 3;
		private int _currentItemAmount = 0;
		
		
		private void Start()
		{
			_castButton.SetVisibility(false);
			foreach (var card in CardList)
			{
				_charactersInPlay.Add(card.Name, card);	
			}
			foreach (var item in _itemCards)
			{
				item.Card.gameObject.SetActive(false);
			}
			SetEnemies();
			ButtonEvt_FlipDeck();
		}

		public void Setup()
		{
		
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

		public void ButtonEvt_SelectCharacter(int character)
		{
			var card = _charactersInPlay[(QueendomConfig.Character) character].Card;
			if (_selectedCharacters < _currentItemAmount && !card.IsSelected)
			{
				_charactersInPlay[(QueendomConfig.Character) character].Card.ButtonEvt_Select();
				if (card.IsSelected) _selectedCharacters++;
				else _selectedCharacters--;
			}
			else if (card.IsSelected)
			{
				_charactersInPlay[(QueendomConfig.Character) character].Card.ButtonEvt_Select();
				_selectedCharacters--;
			}
			_castButton.SetVisibility(_selectedCharacters == _currentItemAmount);
		}
		
		public void ResetField()
		{
			
		}

		public void ButtonEvt_FlipDeck()
		{
			var amount = 3 - _handContainer.childCount;
			for (var i = 0; i < amount; i++)
			{
				var value = Random.Range(3, 5);
				var character = Random.Range(0, 5);
				var newCard = Instantiate(_characterCard, _handContainer);
				newCard.SetValue(value, (QueendomConfig.Character) character);
				newCard.Evt_Upgrade += Evt_Upgrade;
			}
		}

		private void Evt_Upgrade(QueendomConfig.Character character, int value, Action onSuccess)
		{
			if (!_charactersInPlay.ContainsKey(character)) return;
			_charactersInPlay[character].Card.AddValue(value);
			onSuccess();
		}

		private void ButtonEvt_Cast()
		{
		
		}
	}
}