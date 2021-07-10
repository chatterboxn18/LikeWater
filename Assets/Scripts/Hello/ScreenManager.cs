using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
	[SerializeField] private List<ScreenController> _screens;

	public void ButtonEvt_OpenScreen(int index)
	{
		_screens[index].Evt_OpenScreen();
		for (int i = 0; i < _screens.Count; i++)
		{
			if (i == index)
				_screens[i].Evt_Focus();
			else
				_screens[i].Evt_UnFocused();
		}
	}

	public void ButtonEvt_FocusScreen(int index)
	{
		for (int i = 0; i < _screens.Count; i++)
		{
			if (i == index)
				_screens[i].Evt_Focus();
			else
				_screens[i].Evt_UnFocused();
		}
	}
}
