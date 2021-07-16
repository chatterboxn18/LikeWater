using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleButtonTween : ButtonTween
{
	[SerializeField] private float _scaleBy;
	[SerializeField] private RectTransform _canvasGroup;
	private Vector3 _originalScale;
	[SerializeField] private float _scaleTime = 0.1f;

	protected override void Start()
	{
		base.Start();
		_originalScale = _canvasGroup.localScale;
	}

	protected override void Evt_TweenEffect(bool on)
	{
		if (on)
			LeanTween.scale(_canvasGroup, new Vector3(_originalScale.x +_scaleBy, _originalScale.y +_scaleBy, 1), _scaleTime);
		else
		{
			LeanTween.scale(_canvasGroup, Vector3.one, _scaleTime);			
		}
	}
}
