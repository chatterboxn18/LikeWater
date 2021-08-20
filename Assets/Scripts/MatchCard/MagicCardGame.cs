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


		[Header("Character Cards")] [SerializeField]
		private List<CharacterCard> CardList;

		private List<MagicCharacter> _selectedCharacters = new List<MagicCharacter>();

		private Dictionary<QueendomConfig.Character, CharacterCard> _charactersInPlay =
			new Dictionary<QueendomConfig.Character, CharacterCard>();

		[Header("Spell Deck")] [SerializeField]
		private MagicCard _characterCard;

		private List<MagicCard> _spellList = new List<MagicCard>();
		[SerializeField] private List<Sprite> _spriteList = new List<Sprite>();
		[SerializeField] private Transform _handContainer;
		[SerializeField] private SimpleButton _castButton;

		[Header("Items")] [SerializeField] private List<ItemCard> _itemCards;
		private int _level = 3;
		private int _currentItemAmount = 0;


		private void Start()
		{
			_castButton.SetVisibility(false);
			foreach (var card in CardList)
			{
				_charactersInPlay.Add(card.Name, card);
				card.MagicLevel = 5; 
				card.Card.AddValue(card.MagicLevel);
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
			if (_selectedCharacters.Count < _currentItemAmount && !card.IsSelected)
			{
				if (!card.IsSelected)
				{
					_selectedCharacters.Add(card);
					_charactersInPlay[(QueendomConfig.Character) character].Card
						.Evt_Select(true, GetPositionString(_selectedCharacters.Count));
				}
				else
				{
					_selectedCharacters.Remove(card);
					_charactersInPlay[(QueendomConfig.Character) character].Card.Evt_Select(false);
					for (var i = 0; i < _selectedCharacters.Count; i++)
					{
						_selectedCharacters[i].Evt_Select(true, GetPositionString(i + 1));
					}
				}
			}
			else if (card.IsSelected)
			{
				_charactersInPlay[(QueendomConfig.Character) character].Card.Evt_Select(false);
				_selectedCharacters.Remove(card);
				for (var i = 0; i < _selectedCharacters.Count; i++)
				{
					_selectedCharacters[i].Evt_Select(true, GetPositionString(i + 1));
				}
			}

			_castButton.SetVisibility(_selectedCharacters.Count == _currentItemAmount);
		}

		public void ResetField()
		{
			/*foreach (var character in _charactersInPlay)
			{
			  character.Value.Card.gameObject.SetActive(true);
			}*/
			_castButton.SetVisibility(false);

			foreach (var select in _selectedCharacters)
			{
				select.Evt_Select(false);
			}

			_selectedCharacters.Clear();
			foreach (var item in _itemCards)
			{
				item.Card.gameObject.SetActive(false);
			}

			SetEnemies();
			ButtonEvt_FlipDeck();
		}

		public void ButtonEvt_FlipDeck()
		{
			var amount = 3 - _handContainer.childCount;
			for (var i = 0; i < amount; i++)
			{
				var value = Random.Range(3, 5);
				var character = Random.Range(0, 5);
				var newCard = Instantiate(_characterCard, _handContainer);
				newCard.SetValue(value, (QueendomConfig.Character) character, i, _spriteList[character]);
				newCard.Evt_Upgrade += Evt_Upgrade;
				_spellList.Add(newCard);
			}
		}

		private void Evt_Upgrade(QueendomConfig.Character character, int value, int index, Action onSuccess)
		{
			if (!_charactersInPlay.ContainsKey(character)) return;
			if (!_charactersInPlay[character].Card.gameObject.activeSelf) return;
			//_charactersInPlay[character].Card.AddValue(value);
			for (var i = 0; i < _spellList.Count; i++)
			{
				StartCoroutine(_spellList[i].Evt_Destroy(i == index, ()=>_charactersInPlay[character].Card.AddValue(value)));
			}
			_spellList.Clear();
			Resources.UnloadUnusedAssets();
		}

		public void ButtonEvt_Cast()
		{
			for (var i = 0; i < _selectedCharacters.Count; i++)
			{
				if (_selectedCharacters[i].Value >= _itemCards[i].Value)
				{
					_selectedCharacters[i].SetText(_selectedCharacters[i].Value - _itemCards[i].Value);
				}
				else
				{
					_selectedCharacters[i].gameObject.SetActive(false);
				}
			}

			ResetField();
		}

		private string GetPositionString(int index)
		{
			switch (index)
			{
				case 1:
					return "1st";
				case 2:
					return "2nd";
				case 3:
					return "3rd";
				default:
					return index + "th";
			}
		}
	}
}