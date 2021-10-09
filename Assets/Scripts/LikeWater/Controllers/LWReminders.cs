using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
using UnityEngine;

namespace LikeWater
{
	public class LWReminders : MonoBehaviour
	{
		[SerializeField] private AdvanceButton[] _weekButtons;
		[SerializeField] private AdvanceButton[] _typeButtons;
		[SerializeField] private Sprite[] _typeSprites;
		[SerializeField] private Color _selectedColor;
		[SerializeField] private TextMeshProUGUI _timerText;
		[SerializeField] private Transform _reminderContainer;
		[SerializeField] private LWReminderItem _reminderPrefab;
		[SerializeField] private CanvasGroup _reminderEditor;
		private List<LWReminderItem> _reminderItems = new List<LWReminderItem>();
		private bool _isChanged;
		private bool _isNew;
		private int _selectedIndex;
		private int _currentEditorIndex;
		private string _selectedType;

		[SerializeField] private Transform _reminders;
		[SerializeField] private Transform _notSupported;
		
		protected  void Start()
		{
#if !UNITY_IOS || !UNITY_ANDROID
			_reminders.gameObject.SetActive(false);
			_notSupported.gameObject.SetActive(true);
			return;
#endif
			
			var counter = 0;
			foreach (var reminder in LWData.current.Notifications)
			{
				CreateReminder(reminder, counter);
				counter++;
			}
		}

		private void CreateReminder(LWData.NotificationData reminder, int counter)
		{
			var reminderItem = Instantiate(_reminderPrefab, _reminderContainer);
			UpdateReminder(reminderItem, reminder, counter);
			reminderItem.Evt_Selected += Evt_SelectedReminder;
			reminderItem.Evt_Edit += ButtonEvt_Edit;
			_reminderItems.Add(reminderItem);
		}

		private void UpdateReminder(LWReminderItem item, LWData.NotificationData reminder, int counter)
		{
			var days = "";
			foreach (var minder in reminder.Notifications)
			{
				var key = minder.Key.ToString().Substring(0,3);
				days += key + "  ";
			}

			Enum.TryParse(reminder.Type, false, out LWNotificationManager.NotificationType type);
			item.SetReminder(reminder.Time, days, counter, reminder.Ids.ToArray(), reminder.IsActive, _typeSprites[(int)type]);
		}
		
		private void Evt_SelectedReminder(int index)
		{
			_selectedIndex = index;
			for (var i = 0; i < _reminderItems.Count; i++)
			{
				_reminderItems[i].Outline.color = index == i ? _selectedColor : Color.white;
			}
		}

		private void Evt_OpenEditor(bool on)
		{
			if (!on)
			{
				foreach (var button in _weekButtons)
				{
					button.SetActive(false);
				}
				foreach (var button in _typeButtons)
					button.SetActive(false);
			}
			_reminderEditor.gameObject.SetActive(on);
		}

		public void ButtonEvt_Delete()
		{
			var item = _reminderItems[_selectedIndex];
			_reminderItems[_selectedIndex].Evt_Edit -= ButtonEvt_Edit;
			_reminderItems[_selectedIndex].Evt_Selected -= Evt_SelectedReminder;
			item.CleanUp();
			_reminderItems.Remove(item);
			LWData.current.Notifications.Remove(LWData.current.Notifications[_selectedIndex]);
			SerializationManager.Save(LWConfig.DataSaveName, LWData.current);
			var selected =_reminderContainer.GetChild(_selectedIndex + 1);
			Destroy(selected.gameObject);
			for (var i = _selectedIndex + 1; i < _reminderItems.Count + 1; i++)
			{
				_reminderItems[i-1].SetIndex(i - 1);
			}
		}

		public void ButtonEvt_Edit(int index)
		{
			_isNew = false;
			_currentEditorIndex = index;
			var notification = LWData.current.Notifications[index];
			_timerText.text = notification.Time;
			foreach (var item in notification.Notifications)
			{
				_weekButtons[(int) item.Key].SetActive(true);
			}
			Enum.TryParse(notification.Type, false, out LWNotificationManager.NotificationType result);
			_selectedType = result.ToString();
			_typeButtons[(int) result].SetActive(true);
			Evt_OpenEditor(true);
		}
		
		public void ButtonEvt_CreateNew()
		{
			_isNew = true;
			Evt_OpenEditor(true);
			_selectedType = LWNotificationManager.NotificationType.Water.ToString();
			Debug.Log(_selectedType);
			_typeButtons[(int) LWNotificationManager.NotificationType.Water].SetActive( true);
			_currentEditorIndex = _reminderItems.Count;
		}
	
		#region ReminderEditor
	
		public void ButtonEvt_SelectWeek(int index)
		{
			_weekButtons[index].Toggle();
			_isChanged = true;
		}
		
		public void ButtonEvt_SelectType(int index)
		{
			_typeButtons[index].Toggle();
			Debug.Log(((LWNotificationManager.NotificationType) index).ToString());
			_selectedType = ((LWNotificationManager.NotificationType) index).ToString();
			_isChanged = true;
		}
		
		public void ButtonEvt_Add(int index)
		{
			var originalText = _timerText.text;
			var text = originalText.Substring(0,index);
	
			var firstCharacter = int.Parse(originalText[0].ToString());
			var secondCharacter = int.Parse(originalText[1].ToString());
			var character = int.Parse(originalText[index].ToString());
			if (index == 0 && secondCharacter > 4 && character == 1)
				character = 0;
			else if (index == 1 && character == 3 && firstCharacter == 2)
				character = 0;
			else if (character == 9 || (index == 3 && character == 5) || (index == 0 && character == 2) || firstCharacter == 2 && index == 2 && character == 4)
				character = 0;
			else
				character++;
			text += character + originalText.Substring(index + 1, originalText.Length - (index +1));
			_timerText.text = text;
			_isChanged = true;
		}
		
		public void ButtonEvt_Minus(int index)
		{
			var originalText = _timerText.text;
			var text = originalText.Substring(0,index);

			var firstCharacter = int.Parse(originalText[0].ToString());
			var secondCharacter = int.Parse(originalText[1].ToString());
			var character = int.Parse(originalText[index].ToString());
			if (character == 0 && index == 3) character = 5; 
			else if (character == 0 && index == 1 && firstCharacter == 2) character = 3;
			else if (secondCharacter > 4 && index == 0 && character == 0) character = 1;
			else if (character == 0 && index == 0) character = 2; 
			else if (character == 0) character = 9;
			else character--;
			text += character + originalText.Substring(index + 1, originalText.Length - (index +1));
			_timerText.text = text;
			_isChanged = true;
		}

		
		public void ButtonEvt_Save()
		{
			var active = _weekButtons.Any(item => item.Active);
			if (active) 
				Evt_Save(_currentEditorIndex, _timerText.text);
			Evt_OpenEditor(false);
			_isNew = false;
		}
	
		private void Evt_Save(int index, string time)
		{
			if (!_isChanged)
				return;

			if (index < LWData.current.Notifications.Count)
			{
				var oldNotifs = LWData.current.Notifications[index].Ids;
				foreach (var notif in oldNotifs)
				{
					#if UNITY_ANDROID
					AndroidNotificationCenter.CancelNotification(notif);
					#endif
				}
				LWData.current.Notifications[index].Notifications.Clear();
			}
			var newNotif = (_isNew) ? new LWData.NotificationData() {Notifications =  new Dictionary<DayOfWeek, string>()} : LWData.current.Notifications[index];
			Debug.Log("Parsed time is: " + DateTime.Parse(time));
			newNotif.Time = time;
			newNotif.Type = _selectedType;
			newNotif.IsActive = true;
			var inputTime = time.Split(':');		
			for (var i = 0; i < _weekButtons.Length; i++)
			{
				if (_weekButtons[i].Active)
				{
					var today = DateTime.Today;
					var newDatetime = new DateTime(today.Year, today.Month, today.Day, int.Parse(inputTime[0]), int.Parse(inputTime[1]), 0);
					var dayofweek = i;
					var todayDay = (int) today.DayOfWeek;
					newDatetime = todayDay <= dayofweek ? newDatetime.AddDays(dayofweek - todayDay) : newDatetime.AddDays(7 - (todayDay - dayofweek));
					Debug.Log(newDatetime.ToShortTimeString());
					Debug.Log(newDatetime.ToShortDateString());
#if UNITY_ANDROID
					Enum.TryParse(newNotif.Type, false, out LWNotificationManager.NotificationType type);
					var notification = LWNotificationManager.CreateNotification(type, newDatetime);
					newNotif.Notifications.Add((DayOfWeek) i, newDatetime.ToString(CultureInfo.InvariantCulture));
					newNotif.Ids.Add(AndroidNotificationCenter.SendNotification(notification, LWConfig.NotificationChannel));
#endif
				}
			}

			if (_isNew)
			{
				LWData.current.Notifications.Add(newNotif);
				CreateReminder(newNotif, index);
			}
			else
			{
				UpdateReminder(_reminderItems[index], newNotif, index);
			}
			
			SerializationManager.Save(LWConfig.DataSaveName, LWData.current);
	
			_isChanged = false;
			_isNew = false;
		}	
	
		#endregion
	
	}

}

