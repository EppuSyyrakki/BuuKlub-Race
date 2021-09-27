using System.Collections;
using BKRacing.Environment;
using UnityEngine;

namespace BKRacing.Items
{
	public class Decoration : Item
	{
		private DecorationSpawner _spawner = null;

		public override void Update()
		{
			if (newPos.z < -10f)
			{
				transform.position = _spawner.RandomPosition();
			}

			base.Update();
			transform.position = newPos;
		}

		public void SetSpawner(DecorationSpawner spawner)
		{
			_spawner = spawner;
		}
	}
}