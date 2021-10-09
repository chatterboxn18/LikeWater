using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickingCarousel : MonoBehaviour
{
#pragma warning disable 0649
		[SerializeField] protected RectTransform _currentPage;
		[SerializeField] private RectTransform _nextPage;

		[SerializeField] protected RectTransform _pagesContainer;
		[SerializeField] protected Image _spriteIndicator;
		[SerializeField] protected Color _indicatorOnColor;
		[SerializeField] protected Color _indicatorOffColor;
		[SerializeField] protected RectTransform _indicatorParent;
#pragma warning restore 0649
		protected Dictionary<int, Transform> _pages = new Dictionary<int, Transform>();
		
		private Vector2 _nextLocation;
		private Vector2 _prevLocation;
		protected List<Image> _indicators = new List<Image>();

		protected int _currentIndex = 0;

		private bool _isTransitioning = false;
		protected bool _hasIndicators = false;
		protected bool _isActive;

		public void Start()
		{
			var sizeDelta = _currentPage.rect.width;
			_nextLocation = new Vector2(sizeDelta, 0);
			_prevLocation = new Vector2(-1 * sizeDelta, 0);
		}


		public virtual void Load(PageData prefab, int count)
		{
			for (var i = 0; i < count; i++)
			{
				var page = Instantiate(prefab, _pagesContainer);
				page.SetPage(i);
				_pages.Add(i, page.transform);
				if (_spriteIndicator != null)
				{
					var indicator = Instantiate(_spriteIndicator, _indicatorParent);
					_indicators.Add(indicator);
					_hasIndicators = true;
				}
			}

			if (_hasIndicators)
				_indicators[0].color = _indicatorOnColor;
			_pagesContainer.GetChild(_currentIndex).SetParent(_currentPage);
			_isActive = true;

		}

		public virtual void Unload()
		{
			_isActive = false;
			_pagesContainer.transform.DestroyChildren();
			_currentPage.DestroyChildren();
			_nextPage.DestroyChildren();
			_pages.Clear();
			_currentIndex = 0;
			if (_hasIndicators)
			{
				_indicatorParent.transform.DestroyChildren();
			}
		}
		
		public void ButtonEvt_Next()
		{
			if (_isTransitioning || !_isActive) return;
			_isTransitioning = true;

			// turn previous off
			if (_hasIndicators)
				_indicators[_currentIndex].color = _indicatorOffColor;
			
			if (_currentIndex == _pages.Count - 1)
				_currentIndex = 0;
			else
				_currentIndex++;
			
			//turn next on
			if (_hasIndicators)
				_indicators[_currentIndex].color = _indicatorOnColor;
			
			//var page = Instantiate(_musicPagePrefab, _nextPage);
			_pages[_currentIndex].SetParent(_nextPage, false);
			//page.SetPage(_currentIndex);
			_nextPage.anchoredPosition = _nextLocation;
			LeanTween.moveX(_currentPage, _prevLocation.x, LWConfig.FadeTime);
			LeanTween.moveX(_nextPage, 0, LWConfig.FadeTime).setOnComplete(() =>
			{
				_nextPage.LeanSetLocalPosX(0);
				var child = _currentPage.GetChild(0);
				child.transform.SetParent(_pagesContainer, false);
				_currentPage.anchoredPosition = new Vector2(0, 0);
				_pages[_currentIndex].SetParent(_currentPage, false);
				_isTransitioning = false;
			});
			//_currentPage.LeanSetLocalPosX(_prevLocation.x);
		}

		public void ButtonEvt_Prev()
		{
			if (_isTransitioning || !_isActive) return;
			_isTransitioning = true;
			
			//turn next off
			if (_hasIndicators)
				_indicators[_currentIndex].color = _indicatorOffColor;
			
			if (_currentIndex == 0)
				_currentIndex = _pages.Count - 1;
			else
				_currentIndex--;
			
			//turn next on
			if (_hasIndicators)
				_indicators[_currentIndex].color = _indicatorOnColor;
			
			_nextPage.anchoredPosition = _prevLocation;
			//var page = Instantiate(_musicPagePrefab, _nextPage);
			//page.SetPage(_currentIndex);
			_pages[_currentIndex].SetParent(_nextPage, false);
			LeanTween.moveX(_currentPage, _nextLocation.x, LWConfig.FadeTime);
			LeanTween.moveX(_nextPage, 0, LWConfig.FadeTime).setOnComplete(() =>
			{
				//_nextPage.LeanSetLocalPosX(_nextLocation.x);
				var child = _currentPage.GetChild(0);
				child.transform.SetParent(_pagesContainer, false);
				_currentPage.anchoredPosition = new Vector2(0, 0);
				_pages[_currentIndex].transform.SetParent(_currentPage, false);
				_isTransitioning = false;
			});
		}
}
