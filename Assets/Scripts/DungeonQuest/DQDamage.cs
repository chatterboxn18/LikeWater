using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DQDamage : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI _text;
	[SerializeField] private float _timeToLive = 0.1f;
	private float _timer = 0f;
	public void Init(string amount, float time, Color color)
	{
		_text.outlineColor = color;
		_text.outlineWidth = 0.2f;
		_text.text = amount;
		_timeToLive = time;
		LeanTween.alpha(_text.rectTransform, 1, 0.1f).setOnComplete(() =>
			{
				LeanTween.moveY(_text.rectTransform, 100, 0.5f).setOnComplete(() =>
				{
					Destroy(gameObject);
				});
			});
		
	}
	
}
