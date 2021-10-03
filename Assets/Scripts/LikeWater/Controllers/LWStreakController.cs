using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace LikeWater
{
	public class LWStreakController : LWBaseController
	{

		[SerializeField] private TextMeshProUGUI _streakText;
		private int _streakNumber;
		[SerializeField] private List<AdvanceButton> _streakButtons;
		[SerializeField] private Image _monthFill;
		[SerializeField] private TextMeshProUGUI _monthStreakText;

		private void OnEnable()
		{
			if (PlayerPrefs.HasKey(LWConfig.StreakCounter))
			{
				var streak = PlayerPrefs.GetInt(LWConfig.StreakCounter);
				_streakNumber = streak;
				
				//todo: additional config text 
			}
			else
			{
				_streakNumber = 1;
			}
			CheckStreak();
			SetText(_streakNumber);
			if (_streakNumber >= 7)
			{
				for (var i = 0; i < 7; i++)
				{
					_streakButtons[i].SetToggle(true);
				}
			}
			else
			{
				for (var i = 0; i < _streakNumber; i++)
				{
					_streakButtons[i].SetToggle(true);
				}
			}

			var todayDate = DateTime.Today;
			var totalDay = DateTime.DaysInMonth(todayDate.Year, todayDate.Month);
			var flowers = LWData.current.FlowerDictionary[todayDate.Month + "/" + todayDate.Year];
			var count = 0;
			foreach (var flower in flowers)
			{
				if (flower.PlantIndex != -1)
					count++;
			}
			_monthFill.fillAmount = count/(float)totalDay;
			_monthStreakText.text = count + " / " + totalDay;
		}

		private void SetText(int day)
		{
			_streakText.text = day.ToString(CultureInfo.InvariantCulture);
		}
		
		private void CheckStreak()
		{
			if (PlayerPrefs.HasKey(LWConfig.StreakDay))
			{
				var date = PlayerPrefs.GetString(LWConfig.StreakDay);
				var hasDate = DateTime.TryParse(date, out var streak);
				if (hasDate)
				{
					var todayDate = DateTime.Today;
					if (streak.Date == DateTime.Today)
						return;
					var yesterday = todayDate.AddDays(-1);
					if (yesterday.Date == streak)
					{
						_streakNumber++;
					}
					else
					{
						_streakNumber = 1;
					}
					PlayerPrefs.SetString(LWConfig.StreakDay, todayDate.ToShortDateString());
					PlayerPrefs.SetInt(LWConfig.StreakCounter, _streakNumber);
				}
				else
				{
					_streakNumber = 1;
					PlayerPrefs.SetString(LWConfig.StreakDay, DateTime.Today.ToShortDateString());
					PlayerPrefs.SetInt(LWConfig.StreakCounter, _streakNumber);
				}
			}
			else
			{
				_streakNumber = 1;
				PlayerPrefs.SetString(LWConfig.StreakDay, DateTime.Today.ToShortDateString());
				PlayerPrefs.SetInt(LWConfig.StreakCounter, _streakNumber);
			}
		}

		public void Evt_OpenPopup()
		{
			TransitionOn();
		}

		public void Evt_ClosePopup()
		{
			TransitionOff();
		}
	}
}