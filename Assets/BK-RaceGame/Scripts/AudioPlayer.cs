using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

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
		private AudioSource Moving
		{
			get
			{
				if (_moving == null) { _moving = GetComponents<AudioSource>()[0]; }
				return _moving;
			}
		}

		private AudioSource Effect
		{
			get
			{
				if (_effect == null) { _effect = GetComponents<AudioSource>()[1]; }
				return _effect;
			}
		}
		private AudioSource Voice
		{
			get
			{
				if (_voice == null) { _voice = GetComponents<AudioSource>()[2]; }
				return _voice;
			}
		}

		private AudioSource _moving, _effect, _voice;
		private float _maxVol, _movingVol, _effectVol, _voiceVol;
		
		private void Start()
		{
			_maxVol = Game.Instance.masterVolume;
			SetVolumeVariables();
			Moving.loop = true;
			Effect.loop = false;
			Voice.loop = false;
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
			Moving.volume = Mathf.Lerp(0, _maxVol, vol);
			Moving.pitch = Mathf.Lerp(0.9f, 1.1f, vol);
		}

		public void PlayAudio(Sound sound)
		{
			switch (sound.Type)
			{
				case SoundType.Moving:
					if (Moving.clip == sound.clip) return;
					PlayFromSource(sound.clip, Moving);
					break;
				case SoundType.Effect:
					PlayFromSource(sound.clip, Effect);
					break;
				case SoundType.Voice:
					if (Voice.isPlaying) return;
					PlayFromSource(sound.clip, Voice);
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