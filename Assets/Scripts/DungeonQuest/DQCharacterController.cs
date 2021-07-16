using System;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonQuest
{
    
	public class DQCharacterController : MonoBehaviour
	{
		[SerializeField] private float _attackSpeed;
		[SerializeField] private float _autoAttack;
		[SerializeField] private float _attackDamage;

		[SerializeField] private Image _sprite;
		private Sprite[] _sprites;
		public float AttackDamage => _attackDamage;

		public Action<float> Evt_AutoDamage;
		public Action<float> Evt_TapAttack;

		private bool _isInit;

		private int _characterIndex;
		private int _skinIndex;
		
		private float _actionTimer;
		private void Start()
		{
			_actionTimer = 0f;
		}

		public void Init(int index, int skinIndex =0)
		{
			_characterIndex = index;
			_skinIndex = skinIndex;
			var sprite = DQResourceManager.Sprites[_characterIndex * 2][_skinIndex];
			var sprite2 = DQResourceManager.Sprites[_characterIndex * 2 + 1][_skinIndex];
			_sprites = new[] { sprite, sprite2};
			_sprite.sprite = _sprites[0];
		}

		public void Evt_CheckAutoAttack(float timeStamp)
		{
			if (timeStamp - _actionTimer >= _attackSpeed)
			{
				Evt_AutoDamage(_autoAttack);
				_actionTimer = timeStamp; 
			}
		}

		public void AddDamage(int amount)
		{
			_attackDamage += amount;
		}
		
		public void Evt_OnAttackDown()
		{
			_sprite.sprite = _sprites[1];
		}

		public void Evt_OnAttackUp()
		{
			_sprite.sprite = _sprites[0];
		}

    
	}
}