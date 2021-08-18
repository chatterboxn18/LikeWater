using System;
using System.Globalization;
using UnityEngine;
using TMPro;

namespace Queendom
{
	public class MagicCharacter : MagicItem
	{
		private bool _isSelected;
		public bool IsSelected => _isSelected;
		private RectTransform _rectTransform;
		[SerializeField] private int _yPosition;
		[SerializeField] private int _selectedValue;
    [SerializeField] private TextMeshProUGUI _selectedText;

        private void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();
		}

		public void Evt_Select(bool selected, string position = "")
		{
			_isSelected = selected;
			if (_isSelected)
			{
				LeanTween.moveY(_rectTransform, _yPosition + _selectedValue, QueendomConfig.TransitionTime);
        _selectedText.text = position;
			}
			else
			{
				LeanTween.moveY(_rectTransform, _yPosition, QueendomConfig.TransitionTime);
        _selectedText.text = position;
			}
		}

    public void AddValue(int value)
		{
			_value += value;
			_textBox.text = _value.ToString(CultureInfo.InvariantCulture);
		}

	}
}