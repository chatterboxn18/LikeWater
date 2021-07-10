using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerDownHandler, IPointerUpHandler
{
	[SerializeField] private RectTransform _rectTransform;
	[SerializeField] private BoxCollider2D _boxCollider;
	private Vector2 _lastMousePosition;
	public Action Evt_OnDragEnd = delegate {  };
	public Action Evt_OnDragStart = delegate {  };
	public Action Evt_OnDrag = delegate {  };
	private bool _isDragging;
	private bool _isPressed;
	
	[SerializeField] private float _timeCheck = -1;
	private float _timer;

	[Serializable]
	public class MyUnityEvent : UnityEvent{}

	[SerializeField] private MyUnityEvent onPointerUpEvent;

	private void Update()
	{
		if (_isPressed && _timeCheck != -1)
			_timer += Time.deltaTime;
	}
	
	public virtual void OnDrag(PointerEventData eventData)
	{
		if (_timeCheck != -1 &&_timer < _timeCheck)
			return;
		_isDragging = true;
		_rectTransform.anchoredPosition = _rectTransform.anchoredPosition.AddVector3To2(eventData.position - _lastMousePosition);
		_lastMousePosition = eventData.position;
		Evt_OnDrag();
	}

	public virtual void OnEndDrag(PointerEventData eventData)
	{
		if (_timeCheck != -1 &&_timer < _timeCheck)
			return;
		//if (_boxCollider != null) _boxCollider.isTrigger = false;
		_timer = 0f;
		_isDragging = false;
		Evt_OnDragEnd();
	}

	public virtual void OnBeginDrag(PointerEventData eventData)
	{
		if (_timeCheck != -1 && _timer < _timeCheck)
			return;
		//if (_boxCollider != null) _boxCollider.isTrigger = true;
		Evt_OnDragStart();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		_isPressed = true;
		_lastMousePosition = eventData.position;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		_isPressed = false;
		if (_isDragging)
			return;
		onPointerUpEvent?.Invoke();
	}
}
