using System;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonQuest
{
	

	public class DQEnemyBase : MonoBehaviour
	{
		//Base Enemy Class
		protected string _name = "Enemy";
		protected struct Stats
		{
			private int HP;
			private int Defense;
			private float Dodge;
			private int SpDefense;
		}

		[Serializable]
		protected struct CurrentArt
		{
			public Sprite MainSprite;
			public Sprite[] Extensions;
		}

		[SerializeField] private CurrentArt[] Art;

		[SerializeField] private Image _mainImage;
		[SerializeField] private Image[] _extensions;

		[SerializeField] private Transform _damageParent;
		
		private bool _isHit;
	
		private void Start()
		{
			var currentArt = Art[0];
			_mainImage.sprite = currentArt.MainSprite;
			for (var i = 0 ; i < _extensions.Length; i ++)
			{
				_extensions[i].sprite = currentArt.Extensions[i];
			}
		}

		public void SetColor(Color[] colors)
		{
			if (colors.Length > _extensions.Length)
			{
				Debug.LogError("More colors than extensions"); 
				return;
			}
			for (var i = 0; i < colors.Length; i++)
			{
				_extensions[i].color = colors[i];
			}
		}

		public void Evt_GetHit()
		{
			if (_isHit) return;
			var art = Art[1];
			_mainImage.sprite = art.MainSprite;
			for (var i = 0; i < _extensions.Length; i++)
			{
				_extensions[i].sprite = art.Extensions[i];
			}

			_isHit = true;
		}

		public void Evt_FinishHit()
		{
			var art = Art[0];
			_mainImage.sprite = art.MainSprite;
			for (var i = 0; i < _extensions.Length; i++)
			{
				_extensions[i].sprite = art.Extensions[i];
			}
			_isHit = false;
		}
	}
}