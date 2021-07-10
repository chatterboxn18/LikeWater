using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGroup : MonoBehaviour
{
	private List<AdvanceButton> ToggleButtons = new List<AdvanceButton>();
	private int _activeIndex;
	private void Start()
	{
		var counter = 0;
		foreach (var item in transform.GetComponentsInChildren<AdvanceButton>())
		{
			var index = counter;
			item.Evt_BasicEvent_Click += () => Evt_Toggle(index);
			ToggleButtons.Add(item);
			counter++;

		}
	}

	private void Evt_Toggle(int index)
	{
		_activeIndex = index;
		for (var i = 0; i < ToggleButtons.Count; i++)
		{
			ToggleButtons[i].SetActive(i == index);
		}
	}
}
