using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonQuest
{
	public class DQGameManager : MonoBehaviour
	{
		[SerializeField] private Image _healthFill;

		private int _currentAttack = 10;
		private float _monsterHealth = 1000f;
		private int _currentDamage = 0;

		
		// Bottom Menu
		[SerializeField] private RectTransform _buttonMenu;
		[SerializeField] private Image _backgroundMenuColor;
		[SerializeField] private Image _buttonMenuColor;

		[SerializeField] private DQUpgradeItem _upgradePrefab;
		[SerializeField] private Transform _upgradeContainer;

		private DQCharacterData _data;

		private IEnumerator Start()
		{
			while (!DQResourceManager.IsReady)
			{
				yield return null;
			}
			
			ButtonEvt_SelectCharacter("Irene");
		}
		
		public void ButtonEvt_Attack()
		{
			_currentDamage += _currentAttack;
			if (_currentDamage >= _monsterHealth)
			{
				Evt_DefeatMonster();
				return;
			}
			_healthFill.fillAmount = 1 - _currentDamage/ _monsterHealth;
		}

		public void ButtonEvt_AddDamage(int damage)
		{
			Evt_AddDamage(damage);
		}

		private void Evt_DefeatMonster()
		{
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
	
		private void Evt_AddDamage(int addedDamage)
		{
			_currentAttack += addedDamage;
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
					button.Button.Evt_BasicEvent_Click += ()=>Evt_AddDamage(10);
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
				LeanTween.value(_buttonMenu.gameObject, new Vector2(0, 0.3f), new Vector2(-0.3f,0), DQConfig.FadeTime).setOnUpdate((Vector2 update) => _buttonMenu.SetAnchor(update, false));
			}
			else
			{
				LeanTween.value(_buttonMenu.gameObject,  new Vector2(-0.3f,0), new Vector2(0, 0.3f), DQConfig.FadeTime).setOnUpdate((Vector2 update) => _buttonMenu.SetAnchor(update, false));
			}
			
		}
	}


}
