using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenController : MonoBehaviour
{
	private CanvasGroup _canvasGroup;
	private RectTransform _rectTransform;
	[SerializeField] private SimpleButton _simpleButton;

	private void Awake()
	{
		_canvasGroup = GetComponent<CanvasGroup>();
		_rectTransform = GetComponent<RectTransform>();
	}

	public void Evt_OpenScreen()
	{
		Evt_Focus();
		_canvasGroup.alpha = 0.5f;
		_rectTransform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
		gameObject.SetActive(true);
		LeanTween.alphaCanvas(_canvasGroup, 1, HelloConfig.FadeTime);
		LeanTween.scale(_rectTransform, Vector3.one, HelloConfig.FadeTime);
	}

	public void Evt_UnFocused()
	{
		_simpleButton.gameObject.SetActive(true);
	}

	public void Evt_Focus()
	{
		_simpleButton.gameObject.SetActive(false);
		gameObject.transform.SetSiblingIndex(transform.parent.childCount - 1);
	}
	
	public void Evt_CloseScreen()
	{
		LeanTween.alphaCanvas(_canvasGroup, 0, HelloConfig.FadeTime);
		LeanTween.scale(_rectTransform, new Vector3(0.7f, 0.7f, 0.7f), HelloConfig.FadeTime).setOnComplete(() =>
		{
			gameObject.SetActive(false);
			_simpleButton.gameObject.SetActive(true);
		});
	}
}
