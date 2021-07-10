using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DQGachaSystem : MonoBehaviour
{
	private Dictionary<int, Sprite[]> _cardList = new Dictionary<int, Sprite[]>();
	[SerializeField] private Sprite _TEMPSprite;
	[SerializeField] private DQCardButton _cardPrefab;
	[SerializeField] private HorizontalLayoutGroup _layoutGroup;
	[SerializeField] private Transform _scrollContent;
	
	//full Image
	[SerializeField] private CanvasGroup _fullImageGroup;
	[SerializeField] private Image _fullImage;

	private void Start()
	{
		CreateCardDictionary();
		PrepareCardList();
	}
	
	private void CreateCardDictionary()
	{
		var memberNumber = 5;
		var width = _TEMPSprite.rect.width / memberNumber;
		var height = _TEMPSprite.rect.height / memberNumber;
		for (var i = 0; i < memberNumber; i++)
		{
			var cardList = new List<Sprite>();
			for (var j = 0; j < memberNumber; j++)
			{
				var sprite = Sprite.Create(_TEMPSprite.texture,
					new Rect(j * width, i * height, width, height),
					new Vector2(0.5f, 0.5f));
				cardList.Add(sprite);
			}
			_cardList.Add(i, cardList.ToArray());
		}
	}

	private void PrepareCardList()
	{
		foreach (var key in _cardList)
		{
			var cardGroup = Instantiate(_layoutGroup, _scrollContent);
			foreach (var item in _cardList[key.Key])
			{
				var card = Instantiate(_cardPrefab, cardGroup.transform);
				var sprite = item;
				card.Evt_CardSelected += () => DisplayFullImage(sprite);
				card.SetButton(item, key.Key, item);
			}
		}
	}

	private void DisplayFullImage(Sprite sprite)
	{
		_fullImage.sprite = sprite;
		_fullImage.preserveAspect = true;
		_fullImageGroup.alpha = 0;
		_fullImageGroup.gameObject.SetActive(true);
		LeanTween.alphaCanvas(_fullImageGroup, 1, DQConfig.FadeTime);
	}

	public void ButtonEvt_CloseFullScreen()
	{
		LeanTween.alphaCanvas(_fullImageGroup, 0, DQConfig.FadeTime).setOnComplete(() =>
		{
			_fullImageGroup.gameObject.SetActive(false);
		});
	}
}
