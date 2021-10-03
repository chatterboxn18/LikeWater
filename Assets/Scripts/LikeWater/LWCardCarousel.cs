using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace LikeWater
{
    

	public class LWCardCarousel : ClickingCarousel
	{
		private Dictionary<int, LWCardData.FlowerCard> _currentList;
		private List<int> _currentKeys;
		private LWCardManager _cardManager;

		private List<LWCardItem> _cardItems = new List<LWCardItem>();
		[SerializeField] private TextMeshProUGUI _setTitle;
		
		//MUST COME BEFORE LOAD
		public void AssignData(int currentSet, LWCardManager manager)
		{
			var dict = LWCardData.current.CollectedCards;
			var key = dict.Keys.ToList()[currentSet];
			_currentList = dict[key];
			_currentKeys = _currentList.Keys.ToList();
			_cardManager = manager;
			_setTitle.text = "Set " + (currentSet + 1);
		}

		public override void Load(PageData prefab, int total)
		{
			for (var i = 0; i < _currentList.Count; i++)
			{
				var page = (LWCardItem) Instantiate(prefab, _pagesContainer);
				page.SetPage(i);
				page.SetImage(_currentList[_currentKeys[i]], _cardManager.GetCardSprite(_currentKeys[i]), i);
				_cardItems.Add(page);
				_pages.Add(i, page.transform);
				if (_spriteIndicator != null)
				{
					var indicator = Instantiate(_spriteIndicator, _indicatorParent);
					_indicators.Add(indicator);
					_hasIndicators = true;
				}
			}
			_pagesContainer.GetChild(_currentIndex).SetParent(_currentPage);
			_isActive = true;
		}

		public override void Unload()
		{
			base.Unload();
			_cardItems.Clear();
		}
	}
}