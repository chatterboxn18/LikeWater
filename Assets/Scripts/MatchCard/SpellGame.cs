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
		}

		[Header("Items")] 
		[SerializeField] private MagicItem _itemDisplay;
		private List<Item> _itemCards = new List<Item>();
		private int _level = 1;
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
			SetItemList();
		}

		private void SetItemList(){
			_currentItemAmount = 20;
			for (var i = 0; i < _currentItemAmount; i++)
			{
				var value = Random.Range(10, 20 * _level);
				var item = new Item();
				item.Value = value;
				_itemCards.Add(item);
				_itemDisplay.SetText(value);
			}
		}

		private void AddNewCard()
		{
			var card = Instantiate(_spellCardPrefab, _spellCardContainer);
			card.Setup(Random.Range(2, 4), Random.Range(1,5));
			_spellList.Add(card);
		}

		private void CastOnItem(SpellCard card)
		{

			var newValue = _itemDisplay.Value - card.Value;
			if (newValue <= 0){
				_itemDisplay.gameObject.SetActive(false);
				_itemCards.Remove(_itemCards[0]);
				if (_itemCards.Count == 0){
					Debug.Log("Finished the whole list");
					return;
				}
				_itemDisplay.SetText(_itemCards[0].Value);
				_itemDisplay.gameObject.SetActive(true);
				return;
			}
			_itemDisplay.SetText(newValue);

		}
		
		public void ButtonEvt_Cast(int index)
		{
			var upIndex = false;
			var isCasting = false;
			var spellsCast = new List<SpellCard>();
			foreach (var spell in _spellList){
				if (spell.Evt_Match(index, _currentIndex))
				{
					upIndex = true;
					if (spell.SpellList.Count-1 == _currentIndex)
					{
						isCasting = true;
						spellsCast.Add(spell);
					}
				}
			}
			if (isCasting)
			{
				foreach (var spell in spellsCast)
				{
					CastOnItem(spell);
					_spellList.Remove(spell);
					Destroy(spell.gameObject);
					AddNewCard();
				}
				_currentIndex = 0;
				Resources.UnloadUnusedAssets();
			}
			else if (upIndex)
			{
				_currentIndex++;
			}
			else
			{
				_currentIndex = 0;
			}

    }
	}
}