using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace LikeWater
{
	
	public class LWMain : MonoBehaviour
	{
		[Serializable]
		public struct CharacterAudio
		{
			public string Quote;
			public AudioClip Clip;
		}
	
		[SerializeField] private TextMeshProUGUI _buttonText;
		[SerializeField] private AudioSource _audioSource;
		[SerializeField] private CharacterAudio[] _characterAudio;
		[SerializeField] private Transform _groupBubble;
		private bool _isBubbleVisible;
	
		private bool _isActivePlant;
		[SerializeField] private LWFlowerGroup _activeFlower;
	
		[SerializeField] private TextMeshProUGUI _textCoin;
	
		[SerializeField] private Sprite _emptyPot;
		
		private DateTime _todayDate;

		[SerializeField] private LWStreakController _streakPopup;
		[SerializeField] private LWInfoController _infoPopup;
		[SerializeField] private LWInstructionsController _instructionsPopup;
	
		private LWData.FlowerMonth _currentFlower
		{
			get
			{
				if (string.IsNullOrEmpty(LWData.current.MainFlower))
					return new LWData.FlowerMonth()
					{
						Date = DateTime.Today.ToShortDateString()
					};
				var currentFlower = DateTime.Parse(LWData.current.MainFlower);
				var data = LWData.current.FlowerDictionary[currentFlower.Month + "/" + currentFlower.Year][currentFlower.Day-1];
				return data;
			}
		}
	
	
		private IEnumerator Start()
		{
			while (!LWResourceManager.IsLoaded)
				yield return null;
	
			if (!string.IsNullOrEmpty(LWData.current.MainFlower))
			{
				var dateString = DateTime.Today.ToShortDateString();
				if (LWData.current.MainFlower != dateString)
				{
					LWData.current.MainFlower = dateString;
					SerializationManager.Save(LWConfig.DataSaveName, LWData.current);
				}
			}
			Evt_UpdateActiveFlower();
			Evt_UpdateCoins();
			
			if (PlayerPrefs.HasKey(LWConfig.StreakDay))
			{
				var date = PlayerPrefs.GetString(LWConfig.StreakDay);
				var hasDate = DateTime.TryParse(date, out var streak);
				if (hasDate)
				{
					if (streak.ToShortDateString() != DateTime.Today.ToShortDateString())
						_streakPopup.Evt_OpenPopup();
				}
			}
			else
			{
				_streakPopup.Evt_OpenPopup();
			}

			if (LWResourceManager.InfoList.Count > 0)
			{
				_infoPopup.Build(LWResourceManager.InfoList[0]);
				_infoPopup.Evt_OpenPopup();
			}
			
			if (!PlayerPrefs.HasKey(LWConfig.FirstInstructions))
			{
				_instructionsPopup.Evt_OpenPopup();
				PlayerPrefs.SetInt(LWConfig.FirstInstructions, 1);
			}
			//Show last displayed flower 
			//todo: HERE
	
		}
	
		public void Evt_UpdateActiveFlower()
		{
			var currentFlower = _currentFlower;
			var sprite = new []{_emptyPot, _emptyPot};
			if (currentFlower.PlantIndex != -1)
			{
				var sprites = LWResourceManager.Sprites[_currentFlower.PlantIndex];
				var index = _currentFlower.SpriteIndex * 2;
				sprite = new []{sprites[index], sprites[index+1]};
			}
			
			_activeFlower.SetPlant(0, sprite);
		}
	
		public void Evt_UpdateCoins()
		{
			
			_textCoin.text = LWData.current.Coins.ToString();
		}
		
		public void ButtonEvt_FlowerPot()
		{
			if (_currentFlower.PlantIndex == -1)
			{
				LWTransitionController.TransitionOn(LWTransitionController.Controllers.Shop, _currentFlower.Date);
			}
			else
			{
				LWTransitionController.TransitionOn(LWTransitionController.Controllers.Popup, _currentFlower.Date);
			}
		}
	
		public void ButtonEvt_OpenWater()
		{
			if (_currentFlower.PlantIndex == -1)
			{
				LWTransitionController.TransitionOn(LWTransitionController.Controllers.Shop, _currentFlower.Date);
			}
			else
			{
				LWTransitionController.TransitionOn(LWTransitionController.Controllers.Water);
			}
		}
		
		public void ButtonEvt_PlayAudio()
		{
			if (!_isBubbleVisible) return;
			var randomInt = Random.Range(0, _characterAudio.Length);
			_buttonText.text = _characterAudio[randomInt].Quote;
			_audioSource.clip = _characterAudio[randomInt].Clip;
			_audioSource.Play();
		}
	
		public void ButtonEvt_ToggleBubble()
		{
			_isBubbleVisible = !_isBubbleVisible;
			_groupBubble.gameObject.SetActive(_isBubbleVisible);
			ButtonEvt_PlayAudio();
		}

		public void ButtonEvt_Open(int index)
		{
			LWTransitionController.TransitionOn((LWTransitionController.Controllers) index);
		}

		public void ButtonEvt_Close(int index)
		{
			LWTransitionController.TransitionOff((LWTransitionController.Controllers) index);
		}
	}
}

