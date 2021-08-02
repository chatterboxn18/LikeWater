using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class SimpleButton : Selectable, IPointerClickHandler
{
	private bool _isSelected;
	[SerializeField] private bool _isVisible = true;
	[SerializeField] private bool _isDisabled = false;

	public Image MainImage => targetGraphic.GetComponent<Image>();
	public RectTransform RectTransform => _rectTransform;
	private RectTransform _rectTransform;
	public CanvasGroup canvasGroup;
	private float _visibility;
	private float _fadeTime = 0.1f;
	private float _timer = 0f;

	[Serializable]
	public class ButtonEvt : UnityEvent<PointerEventData> {}

	public ButtonEvt EvtPointerDown = new ButtonEvt();
	public ButtonEvt EvtPointerUp = new ButtonEvt();
	public ButtonEvt EvtPointerClick = new ButtonEvt();
	
	public Action Evt_BasicEvent_Up = delegate {  };
	public Action Evt_BasicEvent_Click = delegate {  };
	public Action Evt_BasicEvent_Down = delegate {  };

	public void SetFadeTime(float time)
	{
		_fadeTime = time;
	}
	
	public void SetVisibility(bool on)
	{
		_isVisible = on;
		canvasGroup.LeanAlpha(to: on ? 1 : 0, time: LWConfig.FadeTime);
	}

	protected override void Start()
	{
		if (!_isVisible)
			canvasGroup.alpha = 0;
	}

	protected override void Awake()
	{
		_rectTransform = GetComponent<RectTransform>();
		
	}

	protected virtual void Update()
	{
		/*if (_isVisible)
		{
			if (_canvasGroup.alpha < 1)
			{
				_canvasGroup.alpha = _timer/_fadeTime;
				_timer += Time.deltaTime;
			}
			else
			{
				_canvasGroup.alpha = 1;
				_timer = 0f;
			}
			
		}

		if (!_isVisible)
		{
			if (_canvasGroup.alpha > 0)
			{
				_canvasGroup.alpha = 1 - (_timer/_fadeTime);
				_timer += Time.deltaTime;
			}
			else
			{
				_timer = 0f;
				_canvasGroup.alpha = 0;
			}
		}*/
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		if (!_isVisible ||_isDisabled) return;
		EvtPointerUp.Invoke(arg0: eventData);
		Evt_BasicEvent_Up();
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		if (!_isVisible || _isDisabled) return;
		EvtPointerDown.Invoke(arg0: eventData);
		Evt_BasicEvent_Down();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!_isVisible || _isDisabled) return;
		EvtPointerClick.Invoke(arg0: eventData);
		Evt_BasicEvent_Click();
	}
}
