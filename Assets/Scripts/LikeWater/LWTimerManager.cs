using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Jobs;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
using UnityEngine;

public class LWTimerManager : MonoBehaviour
{
	private DateTime _futureTime;
	[SerializeField] private TextMeshProUGUI _homeTimer;
	[SerializeField] private CanvasGroup _timerCanvas;
	
	private bool _isRunning;
	
	public Action<string, bool> Evt_UpdateTime = delegate {  };

	[SerializeField] private AudioController _audioController;

	private int _currentNotification;

	private void Start()
	{
		Evt_UpdateTime += UpdateHomeTimer;
	}
	
	private void CreateNotification(float time)
	{
#if UNITY_ANDROID
		var notification = new AndroidNotification();
		notification.Title = "Like Water Reminder";
		notification.Text = "It's okay to take a break!";
		notification.SmallIcon = "icon_0";
		notification.ShouldAutoCancel = true;
		notification.FireTime = DateTime.Now.AddMinutes(time);

		_currentNotification = AndroidNotificationCenter.SendNotification(notification, LWConfig.NotificationChannel);
#endif
	}
	public void DisplayTimer(bool on)
	{
		if (!_isRunning) return;
		StartCoroutine(FadeGroup(on));
	}

	private IEnumerator FadeGroup(bool on)
	{
		var time = 0f;
		if (on)
		{
			if (_timerCanvas.alpha >= 1) 
				yield break;
			while (time < 0.2f)
			{
				time += Time.deltaTime;
				_timerCanvas.alpha = Mathf.Lerp(0, 1, time / 0.2f);
				yield return null;
			}
		}
		else
		{
			if (_timerCanvas.alpha <= 0) 
				yield break;
			while (time < 0.2f)
			{
				time += Time.deltaTime;
				_timerCanvas.alpha = Mathf.Lerp(1, 0, time / 0.2f);
				yield return null;
			}
		}
	}
	
	private void UpdateHomeTimer(string time, bool isDone)
	{
		_homeTimer.text = time;
	}
	
	public void Evt_StartTimer(float time, bool hasNotif = false, bool hasAudio = false)
	{
		if (hasNotif)
			CreateNotification(time);
		_futureTime = DateTime.Now.AddMinutes(time);
		_isRunning = true;
		if (hasAudio)
			_audioController.FadeAudio(true, 0.2f);
	}

	public void Evt_StopTimer()
	{
#if UNITY_ANDROID
		if (_currentNotification != -1)
			AndroidNotificationCenter.CancelNotification(_currentNotification);
#endif
		DisplayTimer(false);
		_isRunning = false;
		Evt_UpdateTime(PlayerPrefs.HasKey(LWConfig.Timer) ? PlayerPrefs.GetString(LWConfig.Timer) : "00:00:00", true);
		if (_audioController.Source.isPlaying)
			_audioController.FadeAudio(false, 0.2f);
	}

	private void Update()
	{
		if (!_isRunning)
			return;
		var time = _futureTime.Subtract(DateTime.Now);
		
		if (time.Minutes <= 0 && time.Hours <= 0 && time.Seconds <= 0)
		{
			DisplayTimer(false);
			_isRunning = false;
			Evt_UpdateTime(PlayerPrefs.HasKey(LWConfig.Timer) ? PlayerPrefs.GetString(LWConfig.Timer) : "00:00:00", true);
			if (_audioController.Source.isPlaying)
				_audioController.FadeAudio(false, 0.2f);
			return;
		}
		Evt_UpdateTime($"{time.Hours:00}:{time.Minutes:00}:{time.Seconds:00}", false);
	}
}
