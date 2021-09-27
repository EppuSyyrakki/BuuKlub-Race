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
			Game.Collect(transform.position);
			// TODO: Play effect
		}
	}
}
