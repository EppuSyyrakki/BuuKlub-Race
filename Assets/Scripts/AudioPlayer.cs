using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BKRacing
{
	public enum PlayType
	{
		Moving,
		Effect
	}

	public class AudioPlayer : MonoBehaviour
	{
		private AudioSource _movingSource;
		private AudioSource _effectSource;
		private float _maxVol = 1;

		private void Start()
		{
			AudioSource[] sources = GetComponents<AudioSource>();
			_movingSource = sources[0];
			_effectSource = sources[1];
			_maxVol = Game.Instance.AudioVolume;
			_movingSource.loop = true;
		}

		private void Update()
		{
#if UNITY_EDITOR
			_maxVol = Game.Instance.AudioVolume;
#endif
		}

		public void SetMoveVolumeAndPitch(float vol)
		{
			_movingSource.volume = Mathf.Lerp(0, _maxVol, vol);
			_movingSource.pitch = Mathf.Lerp(0.9f, 1.1f, vol);
		}

		public void PlaySound(AudioClip audioClip, SoundType type)
		{
			if (_movingSource.clip == audioClip) return;

			_movingSource.clip = audioClip;
			_movingSource.Play();
		}
	}
}