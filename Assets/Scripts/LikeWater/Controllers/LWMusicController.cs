using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LikeWater
{
	public class LWMusicController : LWBaseController
	{
		private List<LWResourceManager.Music> _musicList;

#pragma warning disable 0649
		[SerializeField] private LWMusicPage _musicPagePrefab;
		[SerializeField] private RectTransform _currentPage;
		[SerializeField] private RectTransform _nextPage;

		[SerializeField] private RectTransform _pagesContainer;
#pragma warning restore 0649
		private Dictionary<int, Transform> _pages = new Dictionary<int, Transform>();
		
		private Vector2 _nextLocation;
		private Vector2 _prevLocation;

		private int _currentIndex = 0;

		private bool _isTransitioning = false;

		protected override void Start()
		{
			base.Start();
			var sizeDelta = _currentPage.rect.width;
			_nextLocation = new Vector2(sizeDelta, 0);
			_prevLocation = new Vector2(-1 * sizeDelta, 0);
			_musicList = LWResourceManager.MusicList;
			if (!PlayerPrefs.HasKey(LWConfig.PageIndexName))
			{
				PlayerPrefs.SetInt(LWConfig.PageIndexName, 0);
			}

			for (var i = 0; i < _musicList.Count; i++)
			{
				var page = Instantiate(_musicPagePrefab, _pagesContainer);
				page.SetPage(i);
				_pages.Add(i, page.transform);
			}
			_currentIndex = PlayerPrefs.GetInt(LWConfig.PageIndexName);
			_pagesContainer.GetChild(_currentIndex).SetParent(_currentPage);
		}


		public void ButtonEvt_Next()
		{
			if (_isTransitioning) return;
			_isTransitioning = true;
			if (_currentIndex == _musicList.Count - 1)
				_currentIndex = 0;
			else
				_currentIndex++;
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
			if (_isTransitioning) return;
			_isTransitioning = true;
			if (_currentIndex == 0)
				_currentIndex = _musicList.Count - 1;
			else
				_currentIndex--;
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
}