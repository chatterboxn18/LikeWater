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
		[SerializeField] private SimpleButton _editButton;
		[SerializeField] private LWTimerManager _timeController;
		private bool _hasAudio = true;
		[SerializeField] private AudioSource _wendyBuzzer;

		[SerializeField] private AdvanceButton _notificationButton; 
		[SerializeField] private AdvanceButton _soundButton;

		[Header("For Clip Management")]
		[SerializeField] private AdvanceButton _clipToggleButton;
		[SerializeField] private ToggleGroup _toggles;
		[SerializeField] private CanvasGroup _clipsGroup;
		[SerializeField] private CanvasGroup _mainGroup;
		[SerializeField] private SimpleButton _clipModeButton;
		[SerializeField] private TextMeshProUGUI _clipName;
		private bool _isClipEnabled;

		[SerializeField] private Slider _volumeSlider;
		private float _volumeSetting = 1f;

		private bool _hasNotification = true;
		private bool _isButtonPress = false;
		protected override void Start()
		{
			base.Start();
			_timeController.Evt_UpdateTime += UpdateTimer;
			var counter = 0;
			foreach (var item in LWResourceManager.AudioClips)
			{
				var button = Instantiate(_clipToggleButton, _toggles.transform);
				button.GetComponentInChildren<TextMeshProUGUI>().text = item.Name;
				var index = counter;
				button.Evt_BasicEvent_Up += () => ButtonEvt_ToggleIndex(index);
				counter++;
			}
			_toggles.Setup();
			if (PlayerPrefs.HasKey(LWConfig.ClipIndex))
			{
				var audio = LWResourceManager.AudioClips[PlayerPrefs.GetInt(LWConfig.ClipIndex)];
				_clipName.text = audio.Name;
				_timeController.SetClip(audio.Clip);
				_toggles.Evt_Toggle(PlayerPrefs.GetInt(LWConfig.ClipIndex));
			}
			else
			{
				var audio = LWResourceManager.AudioClips[0];
				PlayerPrefs.SetInt(LWConfig.ClipIndex, 0);
				_clipName.text = audio.Name;
				_timeController.SetClip(audio.Clip);
				_toggles.Evt_Toggle(0);
			}
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
					_hasNotification = false;
				}
			}
			else
			{
				_notificationButton.SetActive(false);
				_hasNotification = false;
			}

			if (PlayerPrefs.HasKey(LWConfig.ClipIndex))
			{
				var audio = LWResourceManager.AudioClips[PlayerPrefs.GetInt(LWConfig.ClipIndex)];
				_clipName.text = audio.Name;
				_timeController.SetClip(audio.Clip);
				_toggles.Evt_Toggle(PlayerPrefs.GetInt(LWConfig.ClipIndex));
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
			{
				_wendyBuzzer.Play();
				Evt_FinishTimer();
			}
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
			
			//turn off clip mode
			
			_isButtonPress = false;
			var time = GetMinutes();
			if (time <= 0)
			{
				LWTransitionController.PopupError(LWTransitionController.Toasts.TextMessage, "Edit a time first!");
				return;
			}
			_clipModeButton.SetVisibility(false);
			_editButton.SetVisibility(false);

			_timeController.Evt_StartTimer(time, _hasNotification, _hasAudio);
		}
	
		public void ButtonEvt_StopTimer()
		{
			//need better way to check if there should be audio on stop timer
			_isButtonPress = true;
			Evt_FinishTimer();
			_timeController.Evt_StopTimer();
		}

		private void Evt_FinishTimer()
		{			
			//turn on clip mode
			_clipModeButton.SetVisibility(true);
			_editButton.SetVisibility(true);
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

		public void ButtonEvt_ClipMode()
		{
			EnableClips(_mainGroup.gameObject.activeSelf);
		}

		private void ButtonEvt_ToggleIndex(int index)
		{
			PlayerPrefs.SetInt(LWConfig.ClipIndex, index);
			_timeController.ResetWithClip(LWResourceManager.AudioClips[index].Clip);
		}

		public void ButtonEvt_ClipsClose()
		{
			var audioClips = LWResourceManager.AudioClips;
			if (PlayerPrefs.HasKey(LWConfig.ClipIndex))
			{
				var soundClip = audioClips[PlayerPrefs.GetInt(LWConfig.ClipIndex)];
				_timeController.SetClip(soundClip.Clip);
				_clipName.text = soundClip.Name;
			}
			else
			{
				var soundClip = audioClips[0];
				_timeController.SetClip(soundClip.Clip);
				_clipName.text = soundClip.Name;
			}
			_timeController.StopClip();
			EnableClips(false);
		}

		private void EnableClips(bool on)
		{
			if (on)
			{
				_clipsGroup.gameObject.SetActive(true);
				_clipsGroup.alpha = 0;
				_mainGroup.LeanAlpha(0, LWConfig.FadeTime).setOnComplete(() =>
				{
					_mainGroup.gameObject.SetActive(false);
					_clipsGroup.LeanAlpha(1, LWConfig.FadeTime);
				});
			}
			else
			{
				_mainGroup.gameObject.SetActive(true);
				_mainGroup.alpha = 0;
				_clipsGroup.LeanAlpha(0, LWConfig.FadeTime).setOnComplete(() =>
				{
					_clipsGroup.gameObject.SetActive(false);
					_mainGroup.LeanAlpha(1, LWConfig.FadeTime);
				});
			}
		}
	}
}
