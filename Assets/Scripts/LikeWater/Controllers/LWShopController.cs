using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LikeWater
{
	public class LWShopController : LWBaseController
	{
		[SerializeField] private Transform _shopContainer;
		[SerializeField] private LWShopItem _shopItem;
		[SerializeField] private TextMeshProUGUI _coinText;
		private List<LWShopItem> _shopItems = new List<LWShopItem>();
		private string _startDate; 

		private new IEnumerator Start()
		{
			while (!LWResourceManager.IsLoaded)
				yield return null;
			var flowers = LWResourceManager.Flowers;
			var sprites = LWResourceManager.Sprites;
			foreach (var flower in flowers)
			{
				var item = Instantiate(_shopItem, _shopContainer);
				//_shopItem.MainImage.sprite = sprites[key][0];
				item.SetItem(sprites[flower.Index][0], flower);
				item.Evt_BoughtFlower += (text) => { _coinText.text = text; };
				item.SetDate(_startDate);
				_shopItems.Add(item);
			}
		}

		public override void Evt_ReceiveData(string date)
		{
			foreach (var item in _shopItems)
			{
				item.SetDate(date);
			}

			_startDate = date;
		}
	}
}