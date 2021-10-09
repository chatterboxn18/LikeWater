using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LikeWater
{
	public class LWTimerController : LWBaseController
	{
		[SerializeField] private TextMeshProUGUI _timerText;
		private bool _isEdit;
		private bool _isRunning;
		private float _time;
		[SerializeField] private CanvasGroup _editGroup;
		[SerializeField] private LWTimerManager _timeController;
		private bool _hasAudio = true;
		[SerializeField] private AudioSource _wendyBuzzer;

		[SerializeField] private AdvanceButton _notificationButton; 
		[SerializeField] private AdvanceButton _soundButton;

		[SerializeField] private Slider _volumeSlider;
		private float _volumeSetting = 1f;

		private bool _hasNotification = true;
		private bool _isButtonPress = false;
		protected override void Start()
		{
			base.Start();
			_timeController.Evt_UpdateTime += UpdateTimer;
		}
		
		private void OnEnable()
		{
			_timeController.DisplayTimer(false);
			
			if (PlayerPrefs.HasKey(LWConfig.VolumeSetting))
			{
				var volume = PlayerPrefs.GetFloat(LWConfig.VolumeSetting);
				SliderEvt_Volume(volume);
				_volumeSlider.value = volume;
			}
			// Has Sound after slider so it will still mute accordingly after adjusting volume
			if (PlayerPrefs.HasKey(LWConfig.HasSound))
			{
				var on = PlayerPrefs.GetInt(LWConfig.HasSound);
				if (on == 1)
				{
					_soundButton.SetActive(true);
					_timeController.SetMute(false);
					_hasAudio = true;
				}
				else
				{
					_soundButton.SetActive(false);
					_timeController.SetMute(true);
					_hasAudio = false;
				}
			}
			if (PlayerPrefs.HasKey(LWConfig.HasNotification))
			{
				var on = PlayerPrefs.GetInt(LWConfig.HasNotification);
				if (on == 1)
				{
					_notificationButton.SetActive(true);
					_hasNotification = true;
				}
				else
				{
					_notificationButton.SetActive(false);
					_hasNotification = true;
				}
			}
			if (PlayerPrefs.HasKey(LWConfig.Timer))
				_timerText.text = PlayerPrefs.GetString(LWConfig.Timer);
		}
	
		private void CleanUp()
		{
			//should there ever be a clean up
			_timeController.Evt_UpdateTime -= UpdateTimer;
		}
		
		public void ButtonEvt_Add(int index)
		{
			var originalText = _timerText.text;
			var text = originalText.Substring(0,index);
	
			var character = int.Parse(originalText[index].ToString());
			character = (character == 9 || (index == 3 && character == 5)) ? 0 : (character + 1);
			text += character + originalText.Substring(index + 1, originalText.Length - (index +1));
			_timerText.text = text;
		}
		
		public void ButtonEvt_Minus(int index)
		{
			var originalText = _timerText.text;
			var text = originalText.Substring(0,index);
	
			var character = int.Parse(originalText[index].ToString());
			if (character == 0 && index == 3) character = 5; 
			else if (character == 0) character = 9;
			else character--;
			text += character + originalText.Substring(index + 1, originalText.Length - (index +1));
			_timerText.text = text;
		}
		
		private float GetMinutes()
		{
			var text = _timerText.text.Split(':');
			var hours = int.Parse(text[0]);
			var minutes = int.Parse(text[1]);
			_time = minutes + hours * 60;
			return _time;
		}
	
		public void UpdateTimer(string time, bool isDone = false)
		{
			if (isDone && !_isButtonPress)
				_wendyBuzzer.Play();
			_timerText.text = time;
		}
		
		public void ButtonEvt_Edit()
		{
			if (_isRunning) return;
			_isEdit = !_isEdit;
			if (!_isEdit)
			{
				PlayerPrefs.SetString(LWConfig.Timer, _timerText.text);
			}
			_editGroup.gameObject.SetActive(_isEdit);
		}
	
		public void OnDisable()
		{
			_timeController.DisplayTimer(true);
		}

		public void SliderEvt_Volume(float volume)
		{
			//_volumeSetting = volume;
			
			if (volume <= 0)
			{
				_soundButton.SetActive(false);
				return;
			}

			_timeController.SetVolume(volume);
			if (_hasAudio)
				_soundButton.SetActive(true);
			_volumeSetting = volume;
			PlayerPrefs.SetFloat(LWConfig.VolumeSetting,volume);

		}
		
		public void ButtonEvt_StartTimer()
		{
			if (_isEdit) ButtonEvt_Edit();
			
			_isButtonPress = false;
			var time = GetMinutes();
			if (time <= 0)
			{
				LWTransitionController.PopupError(LWTransitionController.Toasts.TextMessage, "Edit a time first!");
				return;
			}
			_timeController.Evt_StartTimer(time, _hasNotification, _hasAudio);
		}
	
		public void ButtonEvt_StopTimer()
		{
			//need better way to check if there should be audio on stop timer
			_isButtonPress = true;
			_timerText.text = PlayerPrefs.HasKey(LWConfig.Timer) ? PlayerPrefs.GetString(LWConfig.Timer) : "00:00:00";
			_timeController.Evt_StopTimer();
		}

		public void ButtonEvt_EnableNotification()
		{
			_hasNotification = !_hasNotification;
			_notificationButton.SetActive(_hasNotification);
			PlayerPrefs.SetInt(LWConfig.HasNotification, _hasNotification ? 1:0);
		}
		
		public void ButtonEvt_EnableAudio()
		{
			_hasAudio = !_hasAudio;
			//SliderEvt_Volume(_hasAudio ? _volumeSetting:0f);
			_soundButton.SetActive(_hasAudio);
			_timeController.SetMute(!_hasAudio);
			PlayerPrefs.SetInt(LWConfig.HasSound, _hasAudio ? 1:0);
		}
	}
}
