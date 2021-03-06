using System.Collections;
using BKRacing.Environment;
using UnityEngine;

namespace BKRacing.Items
{
	[RequireComponent(typeof(BoxCollider),
		typeof(Billboard),
		typeof(SpriteRenderer))]
	public class Finish : Item
	{
		private bool _sorted = false;

		private void Awake()
		{
			var col = GetComponent<BoxCollider>();
			col.size = new Vector3(10f, 10f, 10f);
		}

		public override void Update()
		{
			base.Update();

			if (newPos.z < -10f)
			{
				Destroy(gameObject);
				return;
			}

			if (!_sorted && newPos.z < player.position.z)
			{
				spriteRenderer.sortingOrder += 100;
				_sorted = true;
			}

			transform.position = newPos;
		}
	}
}