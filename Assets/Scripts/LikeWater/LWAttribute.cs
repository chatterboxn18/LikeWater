using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LWAttribute : MonoBehaviour
{
	[SerializeField] private Image _image;
	private int _attributeCount;
	[SerializeField] private TextMeshProUGUI _amountText;

	public void SetAttributeCount(int amount)
	{
		gameObject.SetActive(amount != 0);
		_attributeCount = amount;
		_amountText.text = "x " + _attributeCount;
	}
}
