using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Queendom
{
	public class MagicCard : MagicItem
	{
		public Action<QueendomConfig.Character, int, int, Action> Evt_Upgrade = delegate {  };
		private QueendomConfig.Character _character;
		[SerializeField] private Image _image;
		[SerializeField] private ParticleSystem _fadeParticles;
		private SimpleButton _button;
		private int _index;

		private void Start()
		{
			_button = GetComponent<SimpleButton>();
		}

		public void SetValue(int value, QueendomConfig.Character character, int index, Sprite image)
		{
			base.SetText(value);
			_character = character;
			_image.sprite = image;
			_index = index;
		}

		public void ButtonEvt_Upgrade()
		{
			Evt_Upgrade(_character, Value, _index, Evt_SuccessfulUpgrade);
		}

		public IEnumerator Evt_Destroy(bool effect, Action onTransition)
		{
			if (effect)
			{
				var particles = Instantiate(_fadeParticles, transform);
				particles.Play();
			}
			yield return new WaitForSeconds(0.2f);
			_button.SetVisibility(false);
			if (effect) onTransition();
			yield return new WaitForSeconds(1f);
			Destroy(gameObject);
		}
		
		private void Evt_SuccessfulUpgrade()
		{
			Destroy(gameObject);
		}
		
	}
}