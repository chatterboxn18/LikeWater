using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NestedScroller : ScrollRect
{
	public Action<PointerEventData> DragEvt_Begin = data => { };
	public Action<PointerEventData> DragEvt_End = data => { };
	public Action<PointerEventData> DragEvt_Ongoing = data => { };
	
	[SerializeField] private float VerticalMinimum = 1f;
	[SerializeField] private float HorizontalMinimum = .2f;

	public RectTransform RectTransform => _rectTransform;
	private RectTransform _rectTransform;

	protected override void Awake()
	{
		base.Awake();
		_rectTransform = GetComponent<RectTransform>();
	}

	public override void OnDrag(PointerEventData eventData)
	{
		if (Mathf.Abs(eventData.delta.y) > Mathf.Abs(eventData.delta.x))
		{
			var data = eventData;
			data.delta = new Vector2(0, eventData.delta.y);
			DragEvt_Ongoing(data);
		}
		else
			base.OnDrag(eventData);
	}

	public override void OnEndDrag(PointerEventData eventData)
	{
		if (Mathf.Abs(eventData.delta.y) > Mathf.Abs(eventData.delta.x))
		{
			var data = eventData;
			data.delta = new Vector2(0, eventData.delta.y);
			DragEvt_End(data);
		}
		else
			base.OnEndDrag(eventData);
	}

	public override void OnBeginDrag(PointerEventData eventData)
	{
		if (Mathf.Abs(eventData.delta.y) > Mathf.Abs(eventData.delta.x))
		{
			var data = eventData;
			data.delta = new Vector2(0, eventData.delta.y);
			DragEvt_Begin(data);
		}
		else
			base.OnBeginDrag(eventData);
	}
}
