using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
	private AudioSource _audioSource;
	[SerializeField] private AudioSource _prefab;

	public AudioSource Source => _audioSource;
	
	private float _volume;
	private float _time = 0.3f;

	
	
	public enum ClipName
	{
		Magic
	}

	[SerializeField] private List<AudioClip> _clips;

	private static Dictionary<ClipName, AudioClip> _audioClips = new Dictionary<ClipName, AudioClip>();
	
	private void Awake()
	{
		_audioSource = GetComponent<AudioSource>();
		for (var i = 0; i < _clips.Count;i++)
		{
			_audioClips.Add((ClipName) i, _clips[i]);
		}
	}

	/*public static void PlayAudio(AudioClip clip, bool on)
	{
		if (on)
			_audioSource.gameObject.LeanValue(0, 1, LWConfig.FadeTime).setOnUpdate((float val) => _audioSource.volume =val);
		else
			_audioSource.gameObject.LeanValue(1, 0, LWConfig.FadeTime).setOnUpdate((float val) => _audioSource.volume =val);
	}*/

	public void CreateAudio(ClipName clip)
	{
		var audio = Instantiate(_prefab);
		audio.clip = _audioClips[clip];
		audio.Play();
		StartCoroutine(DestroyWhenStop(audio));
	}

	private IEnumerator DestroyWhenStop(AudioSource source)
	{
		while (source.isPlaying)
		{
			yield return null;
		}
		Destroy(source.gameObject);
	}
	
	public void FadeAudio(bool isUp, float time, Action onComplete = null)
	{
		if (!isUp && _audioSource.volume <= 0.001f)
		{
			_audioSource.Stop();
			return;
		}
		StartCoroutine(FadeIn(isUp, time, onComplete));
	}

	private IEnumerator FadeIn(bool on, float time, Action onComplete = null)
	{
		var timer = 0f;
		var currentVolume = _audioSource.volume;
		if (on)
		{
			_audioSource.Play();
			while (timer < _time)
			{
				_audioSource.volume = Mathf.Lerp(0,currentVolume, timer/_time);
				timer += Time.deltaTime;
				yield return null;
			}
		}
		else
		{
			while (timer < _time)
			{
				_audioSource.volume = Mathf.Lerp(currentVolume,0, timer/_time);
				timer += Time.deltaTime;
				yield return null;
			}
			_audioSource.Stop();
		}

		onComplete?.Invoke();
	}
}
