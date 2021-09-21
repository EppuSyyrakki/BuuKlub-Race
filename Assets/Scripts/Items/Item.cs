using System.Collections;
using UnityEngine;

namespace BK.Items
{
	[RequireComponent(typeof(Billboard),
		typeof(SpriteRenderer))]
	public class Item : MonoBehaviour
	{
		private void Update()
		{
			var self = transform.position;
			var newPos = new Vector3(self.x, self.y , self.z - Game.Instance.forwardSpeed * Time.deltaTime * 0.12f);
			transform.position = newPos;

			if (transform.position.z < -10f)
			{
				Destroy(gameObject);
			}
		}

		public void Init(Sprite sprite)
		{
			var sr = GetComponent<SpriteRenderer>();
			sr.sprite = sprite;
		}
	}
}