using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BKRacing.Items
{
	[RequireComponent(typeof(SphereCollider))]
	public class Collectible : Item
	{
		private void Awake()
		{
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

			if (newPos.z < player.position.z)
			{
				spriteRenderer.sortingOrder += 100;
			}

			transform.position = newPos;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.TryGetComponent<Character>(out var c))
			{
				var animator = c.GetComponent<Animator>();
				animator.SetTrigger("collect");
				Collect();
			}
		}

		private void Collect()
		{
			Game.Instance.Collect(transform.position);
			// TODO: Play effect
			Destroy(gameObject);
		}
	}
}
