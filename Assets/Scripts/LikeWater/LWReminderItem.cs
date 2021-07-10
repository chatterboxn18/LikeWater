using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using TMPro;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace LikeWater
{
	public class LWReminderItem : MonoBehaviour
	{
		[SerializeField] private Image _check;
		[SerializeField] private TextMeshProUGUI _timeReminder;
		[SerializeField] private TextMeshProUGUI _daysText;
		[SerializeField] private Image _typeImage;
		
		private int[] _notifications;
		private DateTime _scheduledTime;
		private string _type;
		private bool _isActive;
		private int _index = -1;

		public Image Outline;

		public Action<int> Evt_Selected = delegate { };
		public Action<int> Evt_Edit = delegate { };

		public void SetReminder(string time, string days, int index, int[] notifications, bool isActive, Sprite sprite)
		{
			_timeReminder.text = time;
			_daysText.text = days;
			_notifications = notifications;
			_isActive = isActive;
			_check.gameObject.SetActive(_isActive);
			_index = index;
			_typeImage.sprite = sprite;
		}

		public void SetIndex(int index)
		{
			_index = index;
		}

		public void ButtonEvt_Edit()
		{
			Evt_Edit(_index);
		}

		public void ButtonEvt_Selected()
		{
			Evt_Selected(_index);
		}

		private void Start()
		{
			
			
		}

		public void CleanUp()
		{
#if UNITY_ANDROID
			foreach (var notif in _notifications)
			{
				AndroidNotificationCenter.CancelNotification(notif);
			}
#endif
			LWData.current.Notifications[_index].IsActive = false;
			SerializationManager.Save(LWConfig.DataSaveName, LWData.current);
		}

		public void ButtonEvt_EnableNotification()
		{
			_isActive = !_isActive;
			if (_isActive)
			{
				_check.gameObject.SetActive(_isActive);
				var newList = new List<int>();
				foreach (var reminder in LWData.current.Notifications[_index].Notifications)
				{
					var dateTime = DateTime.Parse(reminder.Value);
#if UNITY_ANDROID
					var notification =
						LWNotificationManager.CreateNotification(LWNotificationManager.NotificationType.Water,
							dateTime);
					newList.Add(AndroidNotificationCenter.SendNotification(notification, LWConfig.NotificationChannel));
#endif
				}

				LWData.current.Notifications[_index].Ids.Clear();
				LWData.current.Notifications[_index].Ids = newList;
				LWData.current.Notifications[_index].IsActive = true;
				SerializationManager.Save(LWConfig.DataSaveName, LWData.current);
			}
			else
			{
				_check.gameObject.SetActive(_isActive);
				CleanUp();
			}
		}
	}
}