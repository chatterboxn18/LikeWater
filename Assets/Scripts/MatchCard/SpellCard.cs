using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SpellCard : MonoBehaviour
{
	[SerializeField] private Image _iconPrefab;
	[SerializeField] private Transform _spellContainer;

	[SerializeField] private List<Color> _colors;
	
	private int[] _spellList;
	public int[] SpellList => _spellList;

	public Action<int> Evt_Cast = delegate(int i) {  };

	public void Setup(int amount)
	{
		_spellList = new int[amount];
		for (var i = 0; i < amount; i++)
		{
			var spell = Instantiate(_iconPrefab, _spellContainer);
			var index =Random.Range(0, 5);
			_spellList[i] = index;
			spell.color = _colors[index];
		}
	}
}
