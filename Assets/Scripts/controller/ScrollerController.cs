using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

[RequireComponent(typeof(ScrollView))]
public class ScrollerController : MonoBehaviour
{
	private ScrollRect _scroller;
	private RectTransform _scrollerRect;
	[SerializeField] private Image _topGradient;
	[SerializeField] private Image _bottomGradient;

	private float _fadeTime = 0.2f;

	private bool _isFading;

	private float _offset; 

	private void Awake()
	{
		_scroller = GetComponent<ScrollRect>();
		_scrollerRect = _scroller.GetComponent<RectTransform>();
	}

	private IEnumerator FadeGradient(bool isTop, bool on)
	{
		_isFading = true;
		var image = isTop ? _topGradient : _bottomGradient;
		var timer = 0f;
		while (timer < _fadeTime)
		{
			var alpha = on ? Mathf.Lerp(0, 1, timer / _fadeTime) : Mathf.Lerp(1, 0, timer / _fadeTime);
			image.color = image.color.SetAlpha(alpha);
			timer += Time.deltaTime;
			yield return null;
		}

		_isFading = false;
	}

	private void Update()
	{
		var content = _scroller.content;
		var topAlpha = content.anchoredPosition.y / 50f;
		if (topAlpha >= .5f) topAlpha = .5f;
		if (topAlpha < 0) topAlpha = 0;
		_topGradient.color = _topGradient.color.SetAlpha(topAlpha);
		var bottomAlpha = ((content.rect.height - _scrollerRect.rect.height) -content.anchoredPosition.y)/50;
		if (bottomAlpha >= .5f) bottomAlpha = .5f;
		if (bottomAlpha < 0) bottomAlpha = 0;
		_bottomGradient.color = _bottomGradient.color.SetAlpha(bottomAlpha);
	}
}
