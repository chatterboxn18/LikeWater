using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace DungeonQuest
{
	public class DQGameManager : MonoBehaviour
	{
		[SerializeField] private Image _healthFill;

		private float _monsterHealth = 100f;
		private float _currentDamage = 0f;

		[SerializeField] private int _coins = 10000;
		[SerializeField] private TextMeshProUGUI _coinText;

		private float _monsterToCoinRatio = 0.1f;

		private int _playerLevel = 1;
		
		// Audio 
		[SerializeField] private AudioController _audioController;
		
		// Characters 
		[SerializeField] private DQCharacterController[] _characters;
		
		// Enemy
		[SerializeField] private DQEnemyBase _enemy;
		[SerializeField] private DQDamage _damagePrefab;
		[SerializeField] private RectTransform _damageContainer;
		[SerializeField] private ColorPalette[] _enemyColors;
		private int _colorIndex;
		
		[Serializable]
		public struct ColorPalette
		{
			public Color[] Colors;
		}
		
		// Bottom Menu
		[SerializeField] private RectTransform _arrowIcon;
		[SerializeField] private RectTransform _buttonMenu;
		[SerializeField] private Image _backgroundMenuColor;
		[SerializeField] private Image _buttonMenuColor;

		[SerializeField] private DQUpgradeItem _upgradePrefab;
		[SerializeField] private Transform _upgradeContainer;
		private List<DQUpgradeItem> _upgrades = new List<DQUpgradeItem>();

		// Data
		private DQCharacterData _data;

		
		// Other
		private float _actionTimer;
		private Action<float> Evt_TimedActions = delegate(float f) {  };

		[SerializeField] private ActionQueue _actionQueue;
		
		private Action<int> Evt_CheckButtonAvailability = delegate {  };

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
			_actionQueue.Init(0.1f);
			ButtonEvt_SelectCharacter("Irene");
		}

		private void Update()
		{
			Evt_TimedActions(_actionTimer);
			Evt_CheckButtonAvailability(_coins);
			_actionTimer += Time.deltaTime;
			
		}

		/// <summary>
		/// Logic ///
		/// </summary>
		/// <param name="damage"></param>
		private void CheckDamage()
		{
			if (_currentDamage >= _monsterHealth)
			{
				Evt_DefeatMonster();
				return;
			}
			_healthFill.fillAmount = 1 - _currentDamage/ _monsterHealth;
		}
		
		public void Evt_DamageMonster(int index, int damage)
		{
			_currentDamage += damage;
			_audioController.CreateAudio(AudioController.ClipName.Magic);
			ShowDamage(index, damage);
			CheckDamage();
		}

		public void ButtonEvt_AttackDown(int characterIndex)
		{
			_characters[characterIndex].Evt_OnAttackDown();
			_enemy.Evt_GetHit();
		}
		
		public void ButtonEvt_Attack(int characterIndex)
		{
			var character = _characters[characterIndex];
			
			var damage = CalculateDamage(character.Level, character.AttackDamage, character.CritChance, true);
			_currentDamage += damage;
			ShowDamage(characterIndex, damage);
			
			character.Evt_OnAttackUp();
			
			ReceiveAttack();
			
			CheckDamage();
		}

		private void ShowDamage(int index, int damage)
		{
			var colors = DQResourceManager.Colors;
			_actionQueue.AddToQueue(() =>
			{
				var damageItem = Instantiate(_damagePrefab, _damageContainer);
				damageItem.Init(damage.ToString(CultureInfo.InvariantCulture), 0.2f, colors[(DQCharacterData.RedVelvet) index]);
			});
			
		}
		
		private int CalculateDamage(int level, float attack, float crit, bool isPierce)
		{
			return Mathf.RoundToInt(level * attack / 1f);
		}

		private void ReceiveAttack()
		{
			_enemy.Evt_FinishHit();
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
			if (_colorIndex == _enemyColors.Length - 1)
			{				
				_colorIndex = 0;
			}
			else
			{
				_colorIndex++;
			}
			_enemy.SetColor(_enemyColors[_colorIndex].Colors);

			_healthFill.fillAmount = 1 - _currentDamage / _monsterHealth;
		}
	
		private void Evt_AddDamage(int index, DQResourceManager.UpgradeCard card)
		{
			
			_characters[index].AddDamage(card);
		}

		public void ButtonEvt_SelectCharacter(string name)
		{
			if (Enum.TryParse(name, out DQCharacterData.RedVelvet character))
			{
				for (var item = 0; item < _upgrades.Count; item++)
				{
					Evt_CheckButtonAvailability -= _upgrades[item].Evt_CheckButtonState;
					Destroy(_upgrades[item].gameObject);
				}
				_upgrades.Clear();
				
				Resources.UnloadUnusedAssets();
				var upgradeSprites = DQResourceManager.Upgrades;
				foreach (var upgrade in DQResourceManager.UpgradeList)
				{
					if (upgrade.Character != character)
						continue;
					
					var button = Instantiate(_upgradePrefab, _upgradeContainer);
					Evt_CheckButtonAvailability += button.Evt_CheckButtonState;
					var text = upgrade.BoostAmount <= 0 ? upgrade.BoostPercent + "%" : upgrade.BoostAmount.ToString(CultureInfo.InvariantCulture);
					button.SetUpgrade(upgradeSprites[(int)upgrade.Character][upgrade.SpriteIndex],"+ " + text + " " + upgrade.Type ,upgrade.BasePrice, upgrade.PriceInflation, upgrade.Level);
					
					button.Button.Evt_BasicEvent_Click += ()=>
					{
						if (button.Price <= _coins)
						{
							_coins -= Mathf.RoundToInt(button.Price);
							_coinText.text = _coins.ToString();
							Evt_AddDamage((int) character, upgrade);
							button.Evt_PriceUp();
							upgrade.BasePrice = button.Price;
						}
					};
					_upgrades.Add(button);
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
				LeanTween.rotateZ(_arrowIcon.gameObject, 0, DQConfig.FadeTime);
				LeanTween.value(_buttonMenu.gameObject, new Vector2(0, 0.35f), new Vector2(-0.35f,0), DQConfig.FadeTime).setOnUpdate((Vector2 update) => _buttonMenu.SetAnchor(update, false));
			}
			else
			{
				LeanTween.rotateZ(_arrowIcon.gameObject, 180, DQConfig.FadeTime);
				LeanTween.value(_buttonMenu.gameObject,  new Vector2(-0.35f,0), new Vector2(0, 0.35f), DQConfig.FadeTime).setOnUpdate((Vector2 update) => _buttonMenu.SetAnchor(update, false));
			}
			
		}
	}


}
