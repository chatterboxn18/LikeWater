using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LikeWater
{
	public class LWWaterController : LWBaseController
	{
		private Animator _animator;

		[Header("Water Items")] [SerializeField]
		private LWDrinkController[] _drinkControllers;

		private LWDrinkController _selectedDrink;
		[SerializeField] private Transform _attributesContainer;
		private int _goal = 64;
		private int _drinkCount = 0;
		[SerializeField] private Image _fillOutline;
		private int _maxAttributeCount = 6;
		private Dictionary<string, int> _activeAttributes = new Dictionary<string, int>();

		[SerializeField] private LWFlowerGroup _activeFlower;
		[SerializeField] private LWAttribute _attributePrefab;
		[SerializeField] private TextMeshProUGUI _drinkText;
		[SerializeField] private AudioSource _audioSource;

		//Edit Values 
		[Header("Edit Items")] [SerializeField]
		private TMP_InputField _colorInput;

		[SerializeField] private TMP_InputField _goalInput;
		private Color _previousColor = Color.white;
		private bool _isEditMode;
		[SerializeField] private Attributes[] _attributes;
		private int _selectedAttribute;
		[SerializeField] private Color _selectedColor;
		[SerializeField] private Transform _drinkIconContainer;
		[SerializeField] private SimpleButton _drinkIconPrefab;

		private LWData.FlowerMonth _currentFlower
		{
			get
			{
				if (string.IsNullOrEmpty(LWData.current.MainFlower))
					return new LWData.FlowerMonth();
				var currentFlower = DateTime.Parse(LWData.current.MainFlower);
				var data =
					LWData.current.FlowerDictionary[currentFlower.Month + "/" + currentFlower.Year][
						currentFlower.Day - 1];
				return data;
			}
			set => _currentFlower = value;
		}

		[Serializable]
		public struct Attributes
		{
			public string Name;
			public LWAttribute Script;
			public Image Outline;
		}

		private void Awake()
		{
			_animator = GetComponent<Animator>();
		}

		protected override void Start()
		{
			base.Start();
			var drinks = LWResourceManager.DrinkIcons;
			for (var i = 0; i < drinks.Count; i++)
			{
				var drinkButton = Instantiate(_drinkIconPrefab, _drinkIconContainer);
				drinkButton.image.sprite = drinks[i];
				var i1 = i;
				drinkButton.Evt_BasicEvent_Click += () => { ButtonEvt_UpdateDrink(drinks[i1], i1); };
			}
		}


		private void OnEnable()
		{
			var current = _currentFlower;
			if (current.PlantIndex == -1)
			{
				Debug.LogError("Something went wrong, this is an empty flower");
				gameObject.SetActive(false);
			}

			// filing out active Attributes
			_activeAttributes.Clear();
			if (!string.IsNullOrEmpty(current.Attributes))
			{
				foreach (var item in current.Attributes.Split(','))
				{
					var attribute = item.Split(':');
					_activeAttributes.Add(attribute[0], int.Parse(attribute[1]));
				}
			}
			
			_drinkCount = current.DrinkAmount;
			_goal = LWData.current.Goal;
			_fillOutline.fillAmount = _drinkCount / (float) _goal;
			_drinkText.text = _drinkCount + "/" + _goal;
			Update_DrinkFill();

			var index = 0;
			if (_selectedDrink)
			{
				index = _selectedDrink.Index;
			}

			for (var i = 0; i < _drinkControllers.Length; i++)
			{
				var drink = new LWData.Drink();
				if (i < LWData.current.DrinkAttributes.Count)
				{
					drink = LWData.current.DrinkAttributes[i];
				}
				else
				{
					LWData.current.DrinkAttributes.Add(drink);
				}

				_drinkControllers[i].Setup(_attributes);
				_drinkControllers[i].Evt_Selected(i == index);
				_drinkControllers[i].Image.sprite = LWResourceManager.DrinkIcons[drink.SpriteIndex];
				_drinkControllers[i].Evt_UpdateDrinkIndex(drink.SpriteIndex);
			}

			_selectedDrink = _drinkControllers[index];

			SetActiveFlower();
			// fill attributes;
			UpdateEditAttributes();
		}

		private void SetActiveFlower()
		{
			// grabs data from current flower 
			var data = _currentFlower;

			if (data.PlantIndex == -1)
			{
				Debug.LogError("There isn't a current flower? There should have been an earlier check");
				return;
			}


			var spriteIndex = data.SpriteIndex * 2;
			var plantSprites = LWResourceManager.Sprites[data.PlantIndex];
			var sprites = new[] {plantSprites[spriteIndex], plantSprites[spriteIndex + 1]};
			_activeFlower.SetPlant(0, sprites, data.Date);
		}

		public void ButtonEvt_Drink()
		{
			var drinkAmount = 0;
			if (_selectedDrink.Attributes.ContainsKey(LWConfig.AttributeWaterKey))
				drinkAmount = _selectedDrink.Attributes[LWConfig.AttributeWaterKey];

			foreach (var attributes in _selectedDrink.Attributes)
			{
				if (_activeAttributes.ContainsKey(attributes.Key))
					_activeAttributes[attributes.Key] += attributes.Value;
				else
				{
					_activeAttributes.Add(attributes.Key, attributes.Value);
				}
			}

			_drinkCount += drinkAmount;
			_drinkText.text = _drinkCount + "/" + _goal;

			SaveDrink();
			SetActiveFlower();
		}

		private void SaveDrink()
		{
			var newFlower = _currentFlower;
			var date = DateTime.Parse(newFlower.Date);
			newFlower.Goal = _goal;
			var attributeString = "";
			foreach (var attribute in _activeAttributes)
			{
				attributeString += attribute.Key + ":" + attribute.Value + ",";
			}

			attributeString = attributeString.Substring(0, (attributeString.Length - 1));
			newFlower.Attributes = attributeString;
			newFlower.DrinkAmount = _drinkCount;
			newFlower.SpriteIndex = Update_DrinkFill();
			LWData.current.FlowerDictionary[date.Month + "/" + date.Year][date.Day - 1] = newFlower;
			CheckCompletedFlower();
			SerializationManager.Save(LWConfig.DataSaveName, LWData.current);
		}

		private int Update_DrinkFill()
		{
			if (_drinkCount < _goal)
			{
				var fillAmount = (float) _drinkCount / _goal;
				_fillOutline.fillAmount = fillAmount;
				if (fillAmount >= 0.5f)
					return 1;
				return fillAmount >= 1 ? 2 : 0;
			}
			else
			{
				_fillOutline.fillAmount = 1;
				return 2;
			}
		}

		public void ButtonEvt_SelectDrink(int index)
		{
			_selectedDrink = _drinkControllers[index];
			for (int i = 0; i < _drinkControllers.Length; i++)
			{
				_drinkControllers[i].Evt_Selected(i == index);
			}

			UpdateEditAttributes();
		}

		#region Edit

		public void ButtonEvt_Edit(bool on)
		{
			if (on)
			{
				ButtonEvt_SelectAttribute(0);
				_animator.SetBool("IsEdit", on);
			}
			else
			{
				SaveAttributes();
				_animator.SetBool("IsEdit", on);
				UpdateEditAttributes();
				Update_DrinkFill();
				SetActiveFlower();
			}
		}

		private void UpdateEditAttributes()
		{
			for (var i = 0; i < _attributes.Length; i++)
			{
				_attributes[i].Script.SetAttributeCount(_selectedDrink.Attributes[_attributes[i].Name]);
			}
		}

		private void SaveAttributes()
		{
			for (var i = 0; i < _drinkControllers.Length; i++)
			{
				if (i < LWData.current.DrinkAttributes.Count)
				{
					LWData.current.DrinkAttributes[i].SpriteIndex = _drinkControllers[i].SpriteIndex;
					LWData.current.DrinkAttributes[i].Attributes = _drinkControllers[i].Attributes;
				}
				else
				{
					var drink = new LWData.Drink();
					drink.Attributes = _drinkControllers[i].Attributes;
					drink.SpriteIndex = _drinkControllers[i].SpriteIndex;
					LWData.current.DrinkAttributes.Add(drink);
				}

				Debug.Log("Current Data at index " + i + " is: " + LWData.current.DrinkAttributes[i].Attributes);
				SerializationManager.Save(LWConfig.DataSaveName, LWData.current);
			}
		}

		public void ButtonEvt_UpdateGoal()
		{
			if (_goalInput.text.Length > 0)
			{
				var value = _goalInput.text;
				int.TryParse(value, out var goal);
				if (goal <= 0)
				{
					Debug.LogError("Goal can't be 0, that's pretty sad");
					return;
				}

				_goal = goal;
				LWData.current.Goal = _goal;
				var date = DateTime.Parse(LWData.current.MainFlower);
				LWData.current.FlowerDictionary[date.Month + "/" + date.Year][date.Day - 1].DrinkAmount = _drinkCount;
				LWData.current.FlowerDictionary[date.Month + "/" + date.Year][date.Day - 1].Goal = goal;
				LWData.current.FlowerDictionary[date.Month + "/" + date.Year][date.Day - 1].SpriteIndex =
					Update_DrinkFill();
				CheckCompletedFlower();
				SerializationManager.Save(LWConfig.DataSaveName, LWData.current);
				_goalInput.text = "";
				_drinkText.text = _drinkCount + "/" + _goal;
			}
		}

		private void CheckCompletedFlower()
		{
			var date = DateTime.Parse(LWData.current.MainFlower);
			var currentFlower = LWData.current.FlowerDictionary[date.Month + "/" + date.Year][date.Day - 1];
			if (currentFlower.IsComplete) return;
			if (currentFlower.DrinkAmount >= currentFlower.Goal)
			{
				var flower = LWResourceManager.Flowers[currentFlower.PlantIndex];
				LWData.current.FlowerDictionary[date.Month + "/" + date.Year][date.Day - 1].IsComplete = true;
				LWData.current.Coins += flower.Earns;
				LWTransitionController.PopupError(LWTransitionController.Toasts.TextMessage, "+ " + flower.Earns);
				_audioSource.Play();
				SerializationManager.Save(LWConfig.DataSaveName, LWData.current);
				//IMPORTANT: need to place before a save
			}
		}

		private void ButtonEvt_UpdateDrink(Sprite sprite, int index)
		{
			_selectedDrink.Image.sprite = sprite;
			Debug.Log(index);
			_selectedDrink.Evt_UpdateDrinkIndex(index);
		}


		public void ButtonEvt_ChangeAttribute(bool isAdd)
		{
			if (isAdd)
			{
				_selectedDrink.AddAttribute(_attributes[_selectedAttribute].Name);
				//var attribute = Instantiate(_attributePrefab, _attributesContainer);
				//attribute.SetSprite(_selectedAttribute);
			}
			else
			{
				var index = _selectedDrink.RemoveAttribute(_attributes[_selectedAttribute].Name);
				if (index == 0)
				{
					_attributes[_selectedAttribute].Script.gameObject.SetActive(false);
					//Destroy(_attributesContainer.GetChild(index).gameObject);
				}
			}

			UpdateEditAttributes();
		}

		public void ButtonEvt_SelectAttribute(int index)
		{
			for (var i = 0; i < _attributes.Length; i++)
			{
				_attributes[i].Outline.color = (index == i) ? _selectedColor : Color.white;
			}

			_selectedAttribute = index;
		}

		public void Evt_ColorInputChange(string text)
		{
			if (text.Length == 6)
			{
				var hex = "#" + text;
				var value = Color.white;
				if (ColorUtility.TryParseHtmlString(hex, out value))
				{
					_colorInput.text = ColorUtility.ToHtmlStringRGB(value);
					_colorInput.textComponent.color = value;
					_previousColor = value;
				}
				else
				{
					_colorInput.text = ColorUtility.ToHtmlStringRGB(_previousColor);
				}
			}
		}

		#endregion
	}
}