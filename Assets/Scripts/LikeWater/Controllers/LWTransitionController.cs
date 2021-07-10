using System;
using System.Collections;
using System.Collections.Generic;
using LikeWater;
using TMPro;
using UnityEngine;

public class LWTransitionController : MonoBehaviour
{
	[Header("Controllers")]
	public LWWaterController waterController; 
	public LWFlowerController potController; 
	public LWMusicController musicController; 
	public LWTimerController timerController; 
	public LWPopupController popController;
	public LWShopController shopController;

	[Header("PopupItems")] 
	public ToastController noCoinError;

	private static bool _errorActive;

	public enum Controllers
	{
		Water = 0, 
		Pot = 1, 
		Music = 2, 
		Timer = 3, 
		Popup = 4,
		Shop = 5
	}
	
	public enum Toasts
	{
		TextMessage = 0
	}

	private static Dictionary<Controllers, LWBaseController> _dictionary;
	private static Dictionary<Toasts, ToastController> _errors;
	
	private void Start()
	{
		_dictionary = new Dictionary<Controllers, LWBaseController>
		{
			{Controllers.Water, waterController},
			{Controllers.Pot, potController},
			{Controllers.Music, musicController},
			{Controllers.Timer, timerController},
			{Controllers.Popup, popController},
			{Controllers.Shop, shopController}
		};
		_errors = new Dictionary<Toasts, ToastController>
		{
			{Toasts.TextMessage, noCoinError}
		};
	}
	
	public static void TransitionTo(Controllers from, Controllers to, string data = "")
	{
		
		_dictionary[from].TransitionTo(() =>
		{
			_dictionary[to].Evt_ReceiveData(data);
			_dictionary[to].TransitionOn(true);
		});
	}

	public static void TransitionOn(Controllers on, string data = "")
	{
		_dictionary[on].Evt_ReceiveData(data);
		_dictionary[on].TransitionOn();
	}
	
	public static void TransitionOff(Controllers off)
	{
		if (_dictionary[off].isActiveAndEnabled)
			_dictionary[off].TransitionOff();
	}

	public static void PopupError(Toasts toast, string message)
	{
		if (!_errorActive)
		{
			_errorActive = true;
			
			var transform = _errors[toast].gameObject;
			var canvas = _errors[toast];
			canvas.SetText(message);
			transform.SetActive(true);
			canvas.CanvasGroup.LeanAlpha( 1, LWConfig.FadeTime);
            canvas.CanvasGroup.LeanAlpha( 0, LWConfig.FadeTime).setDelay(1f).setOnComplete(() =>
            {
	            _errorActive = false;
	            transform.SetActive(false);
            });
		}
		
	}

}
