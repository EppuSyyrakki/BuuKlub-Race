using BK;
using UnityEngine;

public class Billboard : MonoBehaviour
{
	[SerializeField]
	private bool flipX;

	private Camera _cam;

	private void Awake()
	{
		if (flipX)
		{
			FlipX();
		}
		_cam = Camera.main;
	}

	private void FlipX()
	{
		var sr = GetComponent<SpriteRenderer>();

		if (sr == null)
		{
			Debug.LogError(gameObject.name + " is trying to flip it's SpriteRenderer but didn't find one!");
		}
		else
		{
			sr.flipX = true;
		}
	}

    private void LateUpdate()
    {
		var look = -_cam.transform.forward * Game.Instance.billboardBend;
		transform.forward = transform.position - look;
	}
}
