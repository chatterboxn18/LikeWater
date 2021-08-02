using System;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonQuest
{
    
	public class DQCharacterController : MonoBehaviour
	{
		[SerializeField] private float _attackSpeed;
		[SerializeField] private float _attackDamage;
		[SerializeField] private int _magicDamage; 
		[SerializeField] private float _critChance;
		[SerializeField] private int _pierceChance;
		private int _pierceCounter;
		[SerializeField] private ParticleSystem _attackParticles;
		
		// upgrades 
		private float _upgradeDamage = 1;
		private int _upgradeMagic = 1;

		[SerializeField] private Image _sprite;
		private Sprite[] _sprites;
		public float AttackDamage => _upgradeDamage * _attackDamage;
		public float AttackSpeed => _attackSpeed;
		public int MagicDamage => _upgradeMagic * _magicDamage;
		public float CritChance => _critChance;
		public bool IsPierce => _pierceCounter == _pierceChance;
		private int _level;
		public int Level => _level;

		public Action<int, int> Evt_AutoDamage;

		private bool _isInit;

		private RectTransform _rectTransform;
		private float _originalX;
		
		private int _characterIndex;
		private int _skinIndex;
		
		private float _actionTimer;
		private void Start()
		{
			_rectTransform = GetComponent<RectTransform>();
			_originalX = _rectTransform.anchoredPosition.x;
			_actionTimer = 0f;
			_level = 1;
		}

		public bool GetPierce()
		{
			if (_pierceCounter == _pierceChance)
			{
				_pierceCounter = 0;
				return true;
			}

			_pierceCounter++;
			return false;
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
				Evt_AutoDamage(_characterIndex, MagicDamage);
				LeanTween.moveX(_rectTransform, _originalX + 5f, 0.1f).setLoopPingPong(1);
				_attackParticles.Play();
				_actionTimer = timeStamp; 
			}
		}

		public void AddDamage(DQResourceManager.UpgradeCard card)
		{
			if (card.Level > 98)
				return;
			card.Level++;	
			switch (card.Type)
			{
				case DQResourceManager.BoostType.Attack:
					UpdateDamage(card.BoostAmount <= 0 ? card.BoostPercent : card.BoostAmount, _attackDamage, card.Level);
					break;
				case DQResourceManager.BoostType.Magic:
					UpdateMagic(card.BoostAmount <= 0 ? card.BoostPercent: card.BoostAmount, _magicDamage, card.Level);
					break;
				case DQResourceManager.BoostType.Speed:
					UpdateSpeed(card.BoostPercent);
					break;
			}
		}

		private void UpdateSpeed(float amount)
		{
			_attackSpeed *= amount;
		}
		
		private void UpdateDamage(float amount, float variable, int level)
		{
			if (amount - 1 < 1)
			{
				_upgradeDamage = Mathf.Pow(amount, level);
				return;
			}
			_attackDamage = variable + amount * level;
		}
		
		private void UpdateMagic(float amount, float variable, int level)
		{
			if (amount - 1 < 1)
			{
				_upgradeMagic = Mathf.RoundToInt(Mathf.Pow(amount, level));
				return;
			}
			_magicDamage = Mathf.RoundToInt(variable + amount * level);
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