using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LikeWater
{
	public class LWCardItem : PageData
	{
		[SerializeField] private Image _cardImage;
		[SerializeField] private Image _plantImage;
		[SerializeField] private CanvasGroup _lockedGroup;
		[SerializeField] private TextMeshProUGUI _unlockText;
		private int _index;
		public int Index => _index;

		public override void SetPage(int index)
		{
			base.SetPage(index);
		}

		public void AssignData()
		{
			
		}
		
		public void SetImage(LWCardData.FlowerCard card, Sprite sprite, int index)
		{
			if (card.AmountCollected < card.CollectTotal)
			{
				_lockedGroup.alpha = 1;
				_unlockText.text = card.AmountCollected + "/" + card.CollectTotal;
				_plantImage.sprite = LWResourceManager.Sprites[card.PlantIndex][0];
				return;
			}

			LeanTween.alphaCanvas(_lockedGroup, 0, LWConfig.FadeTime);
			_plantImage.sprite = LWResourceManager.Sprites[card.PlantIndex][4];
			_cardImage.sprite = sprite;

			_index = index;
		}

		
	}
}