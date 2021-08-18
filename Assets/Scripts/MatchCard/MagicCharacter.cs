using System;
using System.Globalization;
using UnityEngine;

namespace Queendom
{
	public class MagicCharacter : MagicItem
	{
		private bool _isSelected;
		public bool IsSelected => _isSelected;
		private RectTransform _rectTransform;
		[SerializeField] private int _yPosition;
		[SerializeField] private int _selectedValue;

		private void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();
		}

		public void ButtonEvt_Select()
		{
			_isSelected = !_isSelected;
			if (_isSelected)
			{
				LeanTween.moveY(_rectTransform, _yPosition + _selectedValue, QueendomConfig.TransitionTime);
			}
			else
			{
				LeanTween.moveY(_rectTransform, _yPosition, QueendomConfig.TransitionTime);
			}
		}

		public void AddValue(int value)
		{
			_value += value;
			_textBox.text = _value.ToString(CultureInfo.InvariantCulture);
		}
	}
}