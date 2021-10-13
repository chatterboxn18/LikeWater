using System.Collections;
using TMPro;
using UnityEngine;

namespace LikeWater
{
	public class LWAudioItem : UILoader
	{
		[SerializeField] private TextMeshProUGUI _title;
		private bool _isLoaded;
		public bool IsLoaded => _isLoaded;
		[SerializeField] private AdvanceButton _toggleButton;
		public AdvanceButton Button => _toggleButton;

		protected override IEnumerator Start()
		{
			yield return base.Start();
			if (!_isLoaded)
				_toggleButton.SetVisibility(false);
		}

		public void SetItem(int index)
		{
			var clipItem = LWResourceManager.AudioClips[index];
			_title.text = clipItem.Name;
			if (clipItem.Clip == null)
			{
				StartCoroutine(LoadAudio(clipItem.ClipLink, clip =>
                {
                    LWResourceManager.AudioClips[index].Clip = clip;
                	_toggleButton.SetVisibility(true);
                	_isLoaded = true;
                }));
			}
			else
			{
				_loaderGroup.gameObject.SetActive(false);
				_toggleButton.SetVisibility(true);
				_isLoaded = true;
			}
			
		}
	}
}