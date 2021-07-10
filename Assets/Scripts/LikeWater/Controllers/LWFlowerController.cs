using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LikeWater
{
	public class LWFlowerController : LWBaseController
	{
		[SerializeField] private LWFlowerGroup _flowerGroupPrefab;
		[SerializeField] private Transform _flowerGroupContainer;
		[SerializeField] private Sprite _emptyPot;

		[SerializeField] private TextMeshProUGUI _titleText;

		private List<LWFlowerGroup> _flowerGroups = new List<LWFlowerGroup>();

		private float _columns = 3;
		private int _displayMonth;
		private int _displayYear;

		private bool _isTransitioning;

		private void OnEnable()
		{
			var todayDate = DateTime.Now;
			_displayMonth = todayDate.Month;
			_displayYear = todayDate.Year;

			SetShelf(_displayYear, _displayMonth);
		}

		public void ButtonEvt_ChangeMonth(bool isPrevious)
		{
			var date = DateTime.Today;
			if (_isTransitioning)
				return;
			if (_displayMonth == date.Month && _displayYear == date.Year && !isPrevious)
				return;
			if (isPrevious)
			{
				if (_displayMonth == 1)
				{
					_displayMonth = 12;
					_displayYear--;
				}
				else
				{
					_displayMonth--;
				}
			}
			else
			{
				if (_displayMonth == 12)
				{
					_displayMonth = 1;
					_displayYear++;
				}
				else
				{
					_displayMonth++;
				}
			}

			SetShelf(_displayYear, _displayMonth);
		}

		private IEnumerator ChangeShelf(int year, int month)
		{
			var tempDate = new DateTime(year, month, 1);
			_titleText.text = tempDate.ToString("MMMM") + " " + year;
			_isTransitioning = true;
			//var flowerList = 
			if (_flowerGroupContainer.childCount > 0)
			{
				CleanShelf();
			}

			while (_flowerGroupContainer.childCount > 0)
			{
				yield return null;
			}

			var days = DateTime.DaysInMonth(year, month);
			var key = tempDate.Month + "/" + tempDate.Year;
			var hasKey = LWData.current.FlowerDictionary.ContainsKey(key);
			if (!hasKey)
			{
				var dict = LWData.current.FlowerDictionary;
				dict.Add(key, new List<LWData.FlowerMonth>());
				LWData.current.FlowerDictionary = dict;
			}

			for (var day = 0; day < days; day++)
			{
				var index = day % 3;
				if (index == 0)
				{
					var flower = Instantiate(_flowerGroupPrefab, _flowerGroupContainer);
					_flowerGroups.Add(flower);
				}

				var groupIndex = Mathf.FloorToInt(day / _columns);
				var currentFlower = _flowerGroups[groupIndex];
				currentFlower.GetPlant(index).DateTag.text = month + "/" + (day + 1);
				if (!hasKey)
				{
					var flower = new LWData.FlowerMonth();
					flower.Date = tempDate.Month + "/" + (day + 1) + "/" + tempDate.Year;
					LWData.current.FlowerDictionary[key].Add(flower);
					var empty = new[] {_emptyPot, _emptyPot};
					currentFlower.SetPlant(index, empty);
					currentFlower.SetDate(index, tempDate.Month + "/" + (day + 1) + "/" + tempDate.Year);
				}
				else
				{
					var data = LWData.current.FlowerDictionary[key][day];
					if (data.PlantIndex == -1)
					{
						var empty = new[] {_emptyPot, _emptyPot};
						currentFlower.SetPlant(index, empty);
						currentFlower.SetDate(index, tempDate.Month + "/" + (day + 1) + "/" + tempDate.Year);
						continue;
					}

					var sprites = LWResourceManager.Sprites[data.PlantIndex];
					var spriteIndex = data.SpriteIndex * 2;
					var sprite = new[] {sprites[spriteIndex], sprites[spriteIndex + 1]};
					_flowerGroups[groupIndex].SetPlant(index, sprite);
					_flowerGroups[groupIndex].SetDate(index, tempDate.Month + "/" + (day + 1) + "/" + tempDate.Year);
				}
			}

			_isTransitioning = false;
		}

		private void SetShelf(int year, int month)
		{
			StartCoroutine(ChangeShelf(year, month));
		}

		private void CleanShelf()
		{
			for (var i = 0; i < _flowerGroupContainer.childCount; i++)
			{
				Destroy(_flowerGroupContainer.GetChild(i).gameObject);
				_flowerGroups.Remove(_flowerGroups[0]);
			}

			Resources.UnloadUnusedAssets();
		}
	}
}