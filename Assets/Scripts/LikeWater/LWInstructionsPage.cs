using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LikeWater
{
	public class LWInstructionsPage : PageData
	{
		[SerializeField] private Image _image;


		public override void SetPage(int index)
		{
			var instruction = LWResourceManager.Instructions[index];
			_image.sprite = instruction;
		}
	}
}