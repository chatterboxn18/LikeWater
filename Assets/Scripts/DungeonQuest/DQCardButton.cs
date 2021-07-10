using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DQCardButton : MonoBehaviour
{
	[SerializeField] private Image _image;

	public Action Evt_CardSelected = delegate {  };
	private bool _isCollected;
	private SimpleButton _button;
	private int _setIndex;
	private Sprite _memberSprite;
	
	private void Awake()
	{
		_button = GetComponent<SimpleButton>();
	}

	public void SetButton(Sprite sprite, int set, Sprite member, bool collected = false)
	{
		if (collected)
			_isCollected = collected;
		if (!_isCollected)
			_image.color = _image.color.SetAlpha(0.3f);
		_setIndex = set;
		_memberSprite = member;
		_image.sprite = sprite;
	}

	public void ButtonEvt_CardSelected()
	{
		Evt_CardSelected();
	}
}
