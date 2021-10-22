using UnityEngine;

namespace BKRacing.Environment
{
	public class Billboard : MonoBehaviour
	{
		private Camera _cam;
		private int _updateEvery = 3;
		private int _frameTimer;
		private bool _isItem = false;

		private void Awake()
		{
			_cam = Camera.main;
			_frameTimer = Random.Range(0, _updateEvery + 1);
		}

		private void LateUpdate()
		{
			if (_frameTimer == _updateEvery)
			{
				Vector3 look;

				if (_isItem)
				{
					look = -_cam.transform.forward * Game.Instance.itemBillboardBend;
					transform.forward = transform.position - look;
					_frameTimer = 0;
					return;
				}

				var offset = new Vector3(0, 
					Game.Instance.decorationBillboardBendY,
					-Game.Instance.decorationBillboardBendZ);
				look = _cam.transform.position + offset;
				transform.forward = transform.position - look;
				_frameTimer = 0;
				return;
			}

			_frameTimer++;
		}

		public void SetAsItem()
		{
			_isItem = true;
		}
	}
}
