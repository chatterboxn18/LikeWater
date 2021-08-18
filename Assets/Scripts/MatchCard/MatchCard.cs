using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchCard : MonoBehaviour
{
	[SerializeField] private Image _image;
	[SerializeField] private RectTransform _rectTransform;
	private int _cardNumber;
	private int _position;
	public RectTransform RectTransform => _rectTransform;

	public Action<MatchCard,int> Evt_CardPresed;
	public Action Evt_EndGame = delegate {  };

	private bool _isFront;
	[SerializeField] private RectTransform _frontGroup;
	[SerializeField] private RectTransform _backGroup;

	private bool _isMatched;
	
	public void SetCard(Sprite sprite, int number, int x)
	{
		_image.sprite = sprite;
		_cardNumber = number;
		_position = x;
	}

	public void Evt_IsMatched()
	{
		_isMatched = true;
	}
	
	public void ButtonEvt_CardPressed()
	{
		if (_isMatched)
			return;
		Evt_Flip(true);
		/*if (!_isFront)
		{
			//Evt_Flip(false);
			//StartCoroutine(FlipCard(false));
			_isFront = true;
		}*/
		Evt_CardPresed(this, _cardNumber);
	}

	public void Evt_Flip(bool toFront)
	{
		_isFront = !toFront;
		var front = toFront ? _backGroup.gameObject : _frontGroup.gameObject;
		var back = toFront ? _frontGroup.gameObject : _backGroup.gameObject;
		LeanTween.rotateY(front, 90, .2f).setOnComplete(() =>
		{
			back.transform.rotation= new Quaternion(0,90,0, 0);
			back.SetActive(toFront);
			front.SetActive(!toFront);
			LeanTween.rotateY(back, 180, .2f);
		});
	}

	private IEnumerator FlipCard(bool toFront)
	{
		var timer = 0f;
		while (timer < 0.2f)
		{
			var angle = Mathf.Lerp(0, 90, timer/0.2f);
			_backGroup.transform.rotation= new Quaternion(0,angle,0, 0);
			timer += Time.deltaTime;
			yield return null;
		}
		_frontGroup.transform.Rotate(new Vector3(0,90,0));
		_frontGroup.gameObject.SetActive(!toFront);
		_backGroup.gameObject.SetActive(toFront);
		timer = 0f;
		while (timer < 0.2f)
		{
			var angle = Mathf.Lerp(90, 180, timer / 0.2f);
			_frontGroup.transform.rotation= new Quaternion(0,angle,0, 0);
			timer += Time.deltaTime;
			yield return null;
		}
	}

}
