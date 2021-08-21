using System.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using TMPro;
using Debug = UnityEngine.Debug;

namespace Queendom
{
	
public class SpellCard : MonoBehaviour
{
	[SerializeField] private SpellIcon _iconPrefab;
	[SerializeField] private Transform _spellContainer;
	[SerializeField] private TextMeshProUGUI _textValue;

	[SerializeField] private List<Color> _colors;
	
	private List<SpellIcon> _spellList;
	public List<SpellIcon> SpellList => _spellList;

	private int _value; 
	public int Value => _value;

	public Action<int> Evt_Cast = delegate(int i) {  };

	public void Setup(int amount, int value)
	{
		_spellList = new List<SpellIcon>();
		for (var i = 0; i < amount; i++)
		{
			var spell = Instantiate(_iconPrefab, _spellContainer);
			var index =Random.Range(0, 5);
      spell.SetIndex(index);
		  spell.SetColor(_colors[index]);
      _spellList.Add(spell);
    }
		_value = value;
		_textValue.text = _value.ToString();

	}

	public bool Evt_Match(int index, int currentIndex){
		if (currentIndex >= _spellList.Count)
		{
			foreach (var icon in _spellList)
			{
				icon.Selected(false);
			}
			return false;
		}
		Debug.Log("The index: " + index + " and currentIndex = " + currentIndex +  " Spell list length: " + _spellList.Count);
		if (index == _spellList[currentIndex].Index){
      		_spellList[currentIndex].Selected(true);
			return true;
		}
        foreach (var icon in _spellList)
		{
			icon.Selected(false);
		}
      	return false;
	}
}
}
