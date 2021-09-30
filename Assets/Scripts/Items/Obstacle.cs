﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BKRacing.Items
{
	[RequireComponent(typeof(SphereCollider))]
	public class Obstacle : Item
	{
		private bool _sorted;

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

			var animator = c.GetComponent<Animator>();
			animator.SetTrigger("hit");
			Game.Instance.Collide(transform.position);
		}
	}
}
