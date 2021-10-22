using System;
using System.Collections;
using System.Collections.Generic;
using BKRacing.Environment;
using UnityEngine;

namespace BKRacing.Items
{
	[RequireComponent(typeof(SphereCollider),
		typeof(Billboard),
		typeof(SpriteRenderer))]
	public class Obstacle : Item
	{
		private bool _sorted;

		private void Awake()
		{
			GetComponent<Billboard>().SetAsItem();
			var col = GetComponent<SphereCollider>();
			col.center = Vector3.up;
			col.radius = 1;
		}

		public override void Update()
		{
			base.Update();

			if (newPos.z < -10f)
			{
				Destroy(gameObject);
				return;
			}

			if (newPos.z < player.position.z && !_sorted)
			{
				spriteRenderer.sortingOrder += 100;
				_sorted = true;
			}

			transform.position = newPos;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!other.gameObject.TryGetComponent<Character>(out var c)) { return; }

			if (c.Protected) { return; }

			var animator = c.GetComponent<Animator>();
			animator.SetTrigger("hit");
			Game.Instance.Collide(transform.position, soundType);
			c.Crash();
		}
	}
}
