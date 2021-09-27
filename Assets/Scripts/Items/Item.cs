using System;
using System.Collections;
using UnityEngine;
using BKRacing.Environment;

namespace BKRacing.Items
{
	[RequireComponent(typeof(Billboard),
		typeof(SpriteRenderer))]
	public class Item : MonoBehaviour
	{
		protected Transform player;
		protected SpriteRenderer spriteRenderer;

		private void Start()
		{
			player = Game.Instance.Player.transform;
			spriteRenderer = GetComponent<SpriteRenderer>();
		}

		public virtual void Update()
		{
			var self = transform.position;
			var newPos = new Vector3(self.x, self.y, self.z - Game.Instance.forwardSpeed * Time.deltaTime * 0.12f);

			if (newPos.z < -10f)
			{
				Destroy(gameObject);
				return;
			}

			if (newPos.z < player.position.z)
			{
				spriteRenderer.sortingOrder += 100;
			}

			transform.position = newPos;
		}

		public void Init(Sprite sprite)
		{
			var sr = GetComponent<SpriteRenderer>();
			sr.sprite = sprite;
		}
	}
}