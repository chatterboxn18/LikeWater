using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTween : MonoBehaviour
{
	[SerializeField] private SimpleButton _button;

	private bool _isPressed;
	
	protected virtual void Start()
	{
		if (_button == null)
		{
			Debug.LogError("Why is button null man?");
		}

		_button.Evt_BasicEvent_Down += Evt_OnButtonDown;
		_button.Evt_BasicEvent_Up += Evt_OnButtonUp;
	}

	private void Evt_OnButtonDown()
	{
		if (_isPressed) return;
		Evt_TweenEffect(true);
		_isPressed = true;
	}

	private void Evt_OnButtonUp()
	{
		Evt_TweenEffect(false);
		_isPressed = false;			
	}
	
	protected virtual void Evt_TweenEffect(bool on)
	{
		//place effect here
	}
}
