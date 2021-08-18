using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchCardGame : MonoBehaviour
{
	private int[] xPositions = {-400, -200, 0, 200, 400};
	private int[] yPositions = {-375, -125, 125, 375};
	[SerializeField] private MatchCard _cardPrefab;
	[SerializeField] private Transform _cardContainer;
	[SerializeField] private List<Sprite> _sprites;

	private int _selectedCard = -1;
	private MatchCard _selectedObject;

	private float _timer;
	[SerializeField] private float _speed;
	private bool _isGameEnd;
	
	private void Start()
	{
		CreateInitialSet();
	}

	private void CreateInitialSet()
	{
		var dict = new Dictionary<int, int>();
		var randomList = new List<int>();
		for (int num = 0; num < _sprites.Count; num++)
		{
			randomList.Add(num);
		}
		for (int i = 0; i < 5; i++)
		{
			for (var j = 0; j < 4; j++)
			{
				var card = Instantiate(_cardPrefab, _cardContainer);
				var number = Random.Range(0, randomList.Count);
				if (!dict.ContainsKey(number)) dict.Add(number, 1);
				else dict[number]++;
				if (dict[number] == 2)
				{
					randomList.Remove(number);
				}
				card.SetCard(_sprites[number], number, xPositions[i]);
				card.Evt_CardPresed += Evt_CheckCard;
				card.Evt_EndGame += Evt_EndGame;
				card.RectTransform.anchoredPosition = new Vector2(xPositions[i], -375 + (card.RectTransform.rect.height * j));
			}
		}
	}

	private void Evt_EndGame()
	{
		_isGameEnd = true;
	}
	
	private void Evt_CheckCard(MatchCard cardObject,int cardNumber)
	{
		if (cardNumber == _selectedCard)
		{
			_selectedObject.Evt_IsMatched();
			cardObject.Evt_IsMatched();
			//Destroy(_selectedObject.gameObject);
			//Destroy(cardObject.gameObject);
			_selectedCard = -1;
			return;
		}
		if (_selectedCard != -1)
		{
			_selectedObject.Evt_Flip(false);
			cardObject.Evt_Flip(false);
			_selectedObject = null;
			_selectedCard = -1;
			return;
		}
		_selectedCard = cardNumber;
		_selectedObject = cardObject;
		_selectedObject.Evt_Flip(true);
	}

	private void Update()
	{
		if (_isGameEnd)
			return;
	}

	private void AddNewRow()
	{
		for (var i = 0; i < 5; i++)
		{
			var card = Instantiate(_cardPrefab, _cardContainer);
			var number = Random.Range(0, 5);
			card.SetCard(_sprites[number], number, xPositions[i]);
			card.Evt_CardPresed += Evt_CheckCard;
			card.Evt_EndGame += Evt_EndGame;
			card.RectTransform.anchoredPosition = new Vector2(xPositions[i], 375 + (card.RectTransform.rect.height * 9));
		}
	}
}
