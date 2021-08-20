using System.Globalization;
using TMPro;
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
		[SerializeField] private TextMeshProUGUI _selectedText;
		[SerializeField] private ParticleSystem _upgradeParticles;

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
			_upgradeParticles.Play();
			_value += value;
			_textBox.text = _value.ToString(CultureInfo.InvariantCulture);
		}

	}
}