using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AdvanceButton : SimpleButton
{
	[SerializeField] private bool _isActive;

	public bool Active => _isActive;
	[SerializeField] private CanvasGroup _activeGroup;
	[SerializeField] private CanvasGroup _inactiveGroup;

	public Action<bool> Evt_Toggle = delegate(bool b) {  }; 

	private float _activeTime = .2f;
	private float _activeTimer = 0f;

	public void SetActive(bool on)
	{
		_isActive = on;
	}

	public void SetToggle(bool on)
	{
		_isActive = on;
		Evt_Toggle(on);
	}
	
	public void Toggle()
	{
		_isActive = !_isActive;
		Evt_Toggle(_isActive);
	}
	
	protected override void Update()
	{
		base.Update();
		if (_isActive)
		{
			if (_activeGroup.alpha < 1)
			{
				_activeGroup.alpha = _activeTimer/_activeTime;
				_inactiveGroup.alpha = 1 - _activeTimer/_activeTime;
				_activeTimer += Time.deltaTime;
			}
			else
			{
				_activeGroup.alpha = 1;
				_inactiveGroup.alpha = 0;
				_activeTimer = 0f;
			}
			
		}

		if (!_isActive)
		{
			if (_activeGroup.alpha > 0)
			{
				_inactiveGroup.alpha = (_activeTimer/_activeTime);
				_activeGroup.alpha = 1 - _activeTimer / _activeTime;
				_activeTimer += Time.deltaTime;
			}
			else
			{
				_activeTimer = 0f;
				_activeGroup.alpha = 0;
				_inactiveGroup.alpha = 1;
			}
		}
	}
}
