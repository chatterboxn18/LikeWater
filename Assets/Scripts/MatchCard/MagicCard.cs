using System;
using UnityEngine;

namespace Queendom
{
	public class MagicCard : MagicItem
	{
		public Action<QueendomConfig.Character, int, Action> Evt_Upgrade = delegate {  };
		private QueendomConfig.Character _character; 

		public void SetValue(int value, QueendomConfig.Character character)
		{
			base.SetText(value);
			_character = character;
		}

		public void ButtonEvt_Upgrade()
		{
			Evt_Upgrade(_character, Value, Evt_SuccessfulUpgrade);
		}

		private void Evt_SuccessfulUpgrade()
		{
			Destroy(gameObject);
		}
		
	}
}