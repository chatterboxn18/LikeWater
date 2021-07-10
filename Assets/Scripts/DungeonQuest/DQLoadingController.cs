using System.Collections;
using System.Collections.Generic;
using DungeonQuest;
using UnityEngine;
using UnityEngine.Android;

namespace LikeWater
{
	public class DQLoadingController : MonoBehaviour
	{
		private Animator _animator;
		private bool _isLoaded;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
		}

		private void Start()
		{
		}

		private void Update()
		{
			if (_isLoaded)
				return;
			if (DQResourceManager.IsReady)
			{
				_animator.SetTrigger("DoFinish");
				_isLoaded = true;
			}
		}

		public void AnimEvt_CloseLoader()
		{
			gameObject.SetActive(false);
		}
	}
}