using UnityEngine;

namespace BKRacing.Environment
{
	public class Billboard : MonoBehaviour
	{
		private Camera _cam;
		private int _updateEvery = 3;
		private int _frameTimer;

		private void Awake()
		{
			_cam = Camera.main;
			_frameTimer = Random.Range(0, _updateEvery + 1);
		}

		private void LateUpdate()
		{
			if (_frameTimer == _updateEvery)
			{
				var look = -_cam.transform.forward * Game.Instance.billboardBend;
				transform.forward = transform.position - look;
				_frameTimer = 0;
				return;
			}

			_frameTimer++;
		}
	}
}
