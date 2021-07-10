using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LikeWater
{
	[RequireComponent(typeof(SimpleButton))]
	public class LWDrinkController : MonoBehaviour
	{
		private SimpleButton _button;

		public int Index => _index;
		[SerializeField] private int _index;
		public Image Image => _image;
		private Color _color;
		private Dictionary<string, int> _drinkAttributes = new Dictionary<string, int>();
		public Dictionary<string, int> Attributes => _drinkAttributes;

		private bool _isSelected;

		public int SpriteIndex => _spriteIndex;

		private int _spriteIndex;

		[SerializeField] private Image _outline;
		[SerializeField] private Color _selectedColor;

		[SerializeField] private Image _image;

		private void Awake()
		{
			_button = GetComponent<SimpleButton>();
		}

		public void Evt_UpdateDrinkIndex(int index)
		{
			_spriteIndex = index;
		}

		public void Setup(LWWaterController.Attributes[] attributes)
		{
			// Takes all the known attributes and adds it to the drink controller and if it doens't already have 
			// value then add a new field
			if (_index < LWData.current.DrinkAttributes.Count)
			{
				_drinkAttributes = LWData.current.DrinkAttributes[_index].Attributes;
				foreach (var item in attributes)
				{
					if (!_drinkAttributes.ContainsKey(item.Name))
						_drinkAttributes.Add(item.Name, 0);
				}

				LWData.current.DrinkAttributes[_index].Attributes = _drinkAttributes;
			}
		}

		public void Evt_Selected(bool on)
		{
			_outline.color = on ? _selectedColor : Color.white;
		}

		public int RemoveAttribute(string attribute)
		{
			return _drinkAttributes[attribute] <= 0 ? 0 : _drinkAttributes[attribute]--;
		}

		public void AddAttribute(string attribute)
		{
			_drinkAttributes[attribute]++;
		}
	}
}