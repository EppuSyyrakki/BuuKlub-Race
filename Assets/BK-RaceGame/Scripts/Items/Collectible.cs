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
	public class Collectible : Item
	{
		private void Awake()
		{
			GetComponent<Billboard>().SetAsItem();
			var col = GetComponent<SphereCollider>();
			col.center = Vector3.up;
			col.radius = 1;
		}

		public override void Update()
		{
			if (discarded) { return; }

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
			if (!other.gameObject.TryGetComponent<Character>(out var c)) { return; }
			
			c.Animator.SetTrigger("collect");
			Game.Instance.Collect(transform.position);
			Destroy(gameObject);
		}
	}
}
