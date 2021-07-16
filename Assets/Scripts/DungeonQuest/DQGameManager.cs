using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonQuest
{
	public class DQGameManager : MonoBehaviour
	{
		[SerializeField] private Image _healthFill;

		private float _monsterHealth = 1000f;
		private float _currentDamage = 0f;

		private int _coins;
		[SerializeField] private TextMeshProUGUI _coinText;

		private float _monsterToCoinRatio = 0.1f;

		private int _playerLevel = 1;
		
		// Characters 
		[SerializeField] private DQCharacterController[] _characters;
		
		// Enemy
		[SerializeField] private DQEnemyBase _enemy;
		
		// Bottom Menu
		[SerializeField] private RectTransform _buttonMenu;
		[SerializeField] private Image _backgroundMenuColor;
		[SerializeField] private Image _buttonMenuColor;

		[SerializeField] private DQUpgradeItem _upgradePrefab;
		[SerializeField] private Transform _upgradeContainer;

		private DQCharacterData _data;

		private float _actionTimer;
		private Action<float> Evt_TimedActions = delegate(float f) {  };

		private IEnumerator Start()
		{
			while (!DQResourceManager.IsReady)
			{
				yield return null;
			}

			var index = 0;
			foreach (var character in _characters)
			{
				character.Init(index);
				Evt_TimedActions += character.Evt_CheckAutoAttack;
				character.Evt_AutoDamage += Evt_DamageMonster;
				index++;
			}
			
			ButtonEvt_SelectCharacter("Irene");
		}

		private void Update()
		{
			Evt_TimedActions(_actionTimer);
			_actionTimer += Time.deltaTime;
			
		}
		
		public void Evt_DamageMonster(float damage)
		{
			_currentDamage += damage;
			if (_currentDamage >= _monsterHealth)
			{
				Evt_DefeatMonster();
				return;
			}
			_healthFill.fillAmount = 1 - _currentDamage/ _monsterHealth;
		}

		public void ButtonEvt_AttackDown(int characterIndex)
		{
			_characters[characterIndex].Evt_OnAttackDown();
			_enemy.Evt_GetHit();
		}
		
		public void ButtonEvt_Attack(int characterIndex)
		{
			var character = _characters[characterIndex];
			_currentDamage += character.AttackDamage;
			character.Evt_OnAttackUp();
			_enemy.Evt_FinishHit();
			if (_currentDamage >= _monsterHealth)
			{
				Evt_DefeatMonster();
				return;
			}
			_healthFill.fillAmount = 1 - _currentDamage/ _monsterHealth;
		}

		private void Evt_DefeatMonster()
		{
			_coins += Mathf.RoundToInt(_monsterHealth * _monsterToCoinRatio * _playerLevel);
			_coinText.text = _coins.ToString();
			Debug.Log("You defeated the monster");
			NextMonster();
		}

		private void NextMonster()
		{
			_currentDamage = 0;
			var healthBefore = _monsterHealth;
			_monsterHealth = healthBefore * 1.5f;
			//change sprite
			_healthFill.fillAmount = 1 - _currentDamage / _monsterHealth;
		}
	
		private void Evt_AddDamage(int index, int addedDamage)
		{
			_characters[index].AddDamage(addedDamage);
		}

		public void ButtonEvt_SelectCharacter(string name)
		{
			if (Enum.TryParse(name, out DQCharacterData.RedVelvet character))
			{
				for (var item = 0; item < _upgradeContainer.childCount; item++)
				{
					Destroy(_upgradeContainer.GetChild(item).gameObject);
				}

				Resources.UnloadUnusedAssets();
				foreach (var upgrade in DQResourceManager.Upgrades[(int) character])
				{
					var button = Instantiate(_upgradePrefab, _upgradeContainer);
					button.SetUpgrade(upgrade);
					button.Button.Evt_BasicEvent_Click += ()=>Evt_AddDamage((int) character, 10);
				}
				_backgroundMenuColor.color = DQResourceManager.Colors[character];
				_buttonMenuColor.color = DQResourceManager.Colors[character];
			}
		}


		private bool _isBottomMenu;
		public void ButtonEvt_ToggleMenu()
		{
			_isBottomMenu = !_isBottomMenu;
			if (!_isBottomMenu)
			{
				LeanTween.value(_buttonMenu.gameObject, new Vector2(0, 0.35f), new Vector2(-0.35f,0), DQConfig.FadeTime).setOnUpdate((Vector2 update) => _buttonMenu.SetAnchor(update, false));
			}
			else
			{
				LeanTween.value(_buttonMenu.gameObject,  new Vector2(-0.35f,0), new Vector2(0, 0.35f), DQConfig.FadeTime).setOnUpdate((Vector2 update) => _buttonMenu.SetAnchor(update, false));
			}
			
		}
	}


}
