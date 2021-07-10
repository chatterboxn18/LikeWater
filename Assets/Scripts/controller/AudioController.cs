using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
	private AudioSource _audioSource;

	public AudioSource Source => _audioSource;
	
	private bool isUp = false;

	private float _volume;
	private float _time = 0.3f;

	public enum AudioClip
	{
		TakeABreak = 0, 
		Wow = 1
	}

	[SerializeField] private List<AudioClip> _clips;

	private static Dictionary<AudioClip, AudioClip> _audioClips = new Dictionary<AudioClip, AudioClip>();
	
	private void Awake()
	{
		_audioSource = GetComponent<AudioSource>();
		for (var i = 0; i < _clips.Count;i++)
		{
			_audioClips.Add((AudioClip) i, _clips[i]);
		}
	}

	/*public static void PlayAudio(AudioClip clip, bool on)
	{
		if (on)
			_audioSource.gameObject.LeanValue(0, 1, LWConfig.FadeTime).setOnUpdate((float val) => _audioSource.volume =val);
		else
			_audioSource.gameObject.LeanValue(1, 0, LWConfig.FadeTime).setOnUpdate((float val) => _audioSource.volume =val);
	}*/
	
	public void FadeAudio(bool isUp, float time)
	{
		StartCoroutine(FadeIn(isUp, time));
	}

	private IEnumerator FadeIn(bool on, float time)
	{
		var timer = 0f;
		if (on)
		{
			_audioSource.Play();
			while (timer < _time)
			{
				_audioSource.volume = Mathf.Lerp(0,1, timer/_time);
				timer += Time.deltaTime;
				yield return null;
			}
		}
		else
		{
			while (timer < _time)
			{
				_audioSource.volume = Mathf.Lerp(1,0, timer/_time);
				timer += Time.deltaTime;
				yield return null;
			}
			_audioSource.Stop();
		}
	}
}
