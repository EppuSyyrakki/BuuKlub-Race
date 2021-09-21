using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BK.Items
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
	}
}
