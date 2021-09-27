using UnityEngine;

namespace BKRacing.Environment
{
	public class Billboard : MonoBehaviour
	{
		private Camera _cam;

		private void Awake()
		{
			_cam = Camera.main;
		}

		private void LateUpdate()
		{
			var look = -_cam.transform.forward * Game.Instance.billboardBend;
			transform.forward = transform.position - look;
		}
	}
}
