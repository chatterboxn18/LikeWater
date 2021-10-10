using System;
using System.Collections;
using TMPro;
using UnityEngine;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

namespace LikeWater
{
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
			notification.Color = new Color(105, 170, 228);

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
		
		public void SetVolume(float amount)
		{
			_audioController.Source.volume = amount;
		}

		public void SetClip(AudioClip clip)
		{
			if (_audioController.Source.isPlaying)
				_audioController.Source.Stop();
			_audioController.Source.clip = clip;
		}

		public void StopClip()
		{
			_audioController.Source.Stop();
		}
		
		public void ResetWithClip(AudioClip clip)
		{
			_audioController.Source.Stop();
			_audioController.Source.clip = clip;
			_audioController.Source.Play();
		}
		
		public void SetMute(bool on)
		{
			_audioController.Source.mute = on;
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
			_audioController.Source.Play();
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
				_audioController.Source.Stop();
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
					_audioController.Source.Stop();
				return;
			}
			Evt_UpdateTime($"{time.Hours:00}:{time.Minutes:00}:{time.Seconds:00}", false);
		}
	}
}