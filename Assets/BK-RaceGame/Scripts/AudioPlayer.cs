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
		private AudioSource _moving, _effect, _voice;
		private float _maxVol, _movingVol;

		private void Awake()
		{
			_moving = transform.GetChild(0).GetComponent<AudioSource>();
			_effect = transform.GetChild(1).GetComponent<AudioSource>();
			_voice = transform.GetChild(2).GetComponent<AudioSource>();
		}

		private void Start()
		{
			_maxVol = Game.Instance.masterVolume;
			SetVolumeVariables();
			_moving.loop = true;
			_effect.loop = false;
			_voice.loop = false;
			Game.Instance.Player.triggerSound += PlayAudio;
		}

		private void OnDisable()
		{
			Game.Instance.Player.triggerSound -= PlayAudio;
		}

		private void SetVolumeVariables()
		{
			_maxVol = Game.Instance.masterVolume;
			_movingVol = Game.Instance.movingVolume * _maxVol;
			_effect.volume = Game.Instance.effectVolume * _maxVol;
			_voice.volume = Game.Instance.voiceVolume * _maxVol;
		}
		
		public void SetMoveVolumeAndPitch(float vol)
		{
			var bend = Game.Instance.movingPitchBend;
			_moving.volume = Mathf.Lerp(0, _movingVol, vol);
			_moving.pitch = Mathf.LerpUnclamped(1 - bend, 1 + bend, vol);
		}

		public void PlayAudio(Sound sound)
		{
			switch (sound.Type)
			{
				case SoundType.Moving:
					if (_moving.clip == sound.clip) return;
					PlayFromSource(sound.clip, _moving);
					break;
				case SoundType.Effect:
					PlayFromSource(sound.clip, _effect);
					break;
				case SoundType.Voice:
					if (_voice.isPlaying) return;
					PlayFromSource(sound.clip, _voice);
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