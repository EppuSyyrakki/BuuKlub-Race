using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BKRacing
{
	public enum SoundType
	{
		Moving,
		Effect,
		Voice
	}

	public class AudioPlayer : MonoBehaviour
	{
		private AudioSource _movingSource;
		private AudioSource _effectSource;
		private AudioSource _voiceSource;
		private float _maxVol, _movingVol, _effectVol, _voiceVol;
		
		private void Start()
		{
			AudioSource[] sources = GetComponents<AudioSource>();
			_movingSource = sources[0];
			_effectSource = sources[1];
			_voiceSource = sources[2];
			_maxVol = Game.Instance.masterVolume;
			SetVolumeVariables();
			_movingSource.loop = true;
			_effectSource.loop = false;
			_voiceSource.loop = false;
		}

		private void OnEnable()
		{
			Game.Instance.Player.triggerSound += PlayAudio;
		}

		private void OnDisable()
		{
			Game.Instance.Player.triggerSound -= PlayAudio;
		}

		private void Update()
		{
#if UNITY_EDITOR
			_maxVol = Game.Instance.masterVolume;
			SetVolumeVariables();
#endif
		}

		private void SetVolumeVariables()
		{
			_movingVol = Game.Instance.movingVolume;
			_effectVol = Game.Instance.effectVolume;
			_voiceVol = Game.Instance.voiceVolume;
		}
		
		public void SetMoveVolumeAndPitch(float vol)
		{
			_movingSource.volume = Mathf.Lerp(0, _maxVol, vol);
			_movingSource.pitch = Mathf.Lerp(0.9f, 1.1f, vol);
		}

		public void PlayAudio(AudioClip clip, SoundType type)
		{
			switch (type)
			{
				case SoundType.Moving:
					if (_movingSource.clip == clip) return;
					PlayFromSource(clip, _movingSource);
					break;
				case SoundType.Effect:
					PlayFromSource(clip, _effectSource);
					break;
				case SoundType.Voice:
					if (_voiceSource.isPlaying) return;
					PlayFromSource(clip, _voiceSource);
					break;
			}
		}

		private void PlayFromSource(AudioClip clip, AudioSource source)
		{
			source.Stop();
			source.clip = clip;
			source.Play();
		}
	}
}