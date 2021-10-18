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
		}

		private void Update()
		{
#if UNITY_EDITOR
			_maxVol = Game.Instance.AudioVolume;
#endif
		}

		public void SetMoveVolume(float vol)
		{
			_movingSource.volume = Mathf.Lerp(0, _maxVol, vol);
		}

		public void PlaySound(AudioClip audioClip, SoundType type)
		{
			// Play a movement sound
			if (type == SoundType.MoveForward 
			    || type == SoundType.MoveSideways 
			    || type == SoundType.Collide1 
			    || type == SoundType.Collide2 
			    || type == SoundType.Collide3)
			{
				if (_movingSource.clip == audioClip) { return; }

				_movingSource.Stop();
				_movingSource.clip = audioClip;
				_movingSource.Play();

				if (type == SoundType.MoveForward || type == SoundType.MoveSideways)
				{
					_movingSource.loop = true;
				}

				return;
			}

			if (type == SoundType.PickUp1
			    || type == SoundType.PickUp2
			    || type == SoundType.PickUp3)
			{
				_effectSource.Stop();
				_effectSource.clip = audioClip;
				_effectSource.Play();
				return;
			}

			if (type == SoundType.WinChime)
			{
				_movingSource.Stop();
				_effectSource.Stop();
				_effectSource.clip = audioClip;
				_effectSource.Play();
			}
		}
	}
}