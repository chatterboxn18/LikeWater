using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LWBaseController : MonoBehaviour
{
	[SerializeField] private CanvasGroup _overlay;
	[SerializeField] protected CanvasGroup canvasGroup;
	protected virtual void Start()
	{
		//canvasGroup.alpha = 0;
	}

	private void OnDisable()
	{
		//canvasGroup.alpha = 0;
	}

	public void TransitionOn(bool isTo = false)
	{
		if (_overlay != null)
		{
			if (_overlay.gameObject.activeSelf && isTo)
			{
				Transition(true);
				return;
			}
			_overlay.alpha = 0;
			_overlay.gameObject.SetActive(true);
			Debug.Log("Transitioning");
			_overlay.LeanAlpha(1 , LWConfig.FadeTime).setOnComplete(() =>
			{
				Transition(true);
				Debug.Log("It's taking forever to get here");
			});
			return;
		}
		Transition(true);
		//LeanTween.alphaCanvas(canvasGroup, to, LWConfig.FadeTime).setOnComplete(() => { gameObject.SetActive(on);});
	}

	public void TransitionOff(bool isTo = false)
	{
		if (_overlay != null)
		{
			if (_overlay.gameObject.activeSelf && isTo)
			{
				Transition(false);
				return;
			}
			gameObject.SetActive(true);
			Transition(false, () =>
			{
				_overlay.alpha = 1;
				_overlay.LeanAlpha(0, LWConfig.FadeTime).setOnComplete(() =>
				{
					_overlay.gameObject.SetActive(false);
				});
			});
			return;
		}
		Transition(false);
	}

	private void Transition(bool on, Action onComplete = null)
	{
		//transform.localScale = new Vector3(1, on? 0: 1,1); 
		canvasGroup.alpha = on? 0:1;
		Debug.Log("It's this, it's hinging here");
		gameObject.SetActive(true);
		var to = on ? 1 : 0;
		Debug.Log("Instantiating is expensive");
		canvasGroup.LeanAlpha(to, LWConfig.FadeTime).setOnComplete(() =>
		{
			onComplete?.Invoke();
			gameObject.SetActive(on);
		});
		/*transform.LeanScaleY(to, LWConfig.FadeTime).setOnComplete(() =>
		{
			onComplete?.Invoke();
			gameObject.SetActive(on);
		});*/
	}
	
	public void TransitionTo(Action onComplete)
	{
		canvasGroup.alpha = 1;
		canvasGroup.LeanAlpha(0, LWConfig.FadeTime).setOnComplete(() =>
		{
			gameObject.SetActive(false);
			onComplete?.Invoke();
		});
		/*transform.localScale = new Vector3(1, 1,1); 
		transform.LeanScaleY(0, LWConfig.FadeTime).setOnComplete(() =>
		{
			gameObject.SetActive(false);
			onComplete();
		});*/
	}

	public virtual void Evt_ReceiveData(string data)
	{
		
	}

	/*
	protected IEnumerator FadeOn(bool on)
	{
		var timer = 0f;
		while (timer < LWConfig.FadeTime)
		{
			canvasGroup.alpha = @on ? Mathf.Lerp(0, 1, timer / LWConfig.FadeTime) : Mathf.Lerp(1, 0, timer / LWConfig.FadeTime);
			timer += Time.deltaTime;
			yield return null;
		}

		canvasGroup.alpha = on ? 1:0;
		if (!on) gameObject.SetActive(false);
	}*/
}
