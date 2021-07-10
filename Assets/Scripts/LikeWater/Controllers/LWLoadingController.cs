using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

namespace LikeWater
{
	public class LWLoadingController : MonoBehaviour
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
			if (LWResourceManager.IsLoaded)
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