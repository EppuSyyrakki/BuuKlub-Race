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
			var self = transform.position;
			var newPos = new Vector3(self.x, self.y, self.z - Game.Instance.forwardSpeed * Time.deltaTime * 0.12f);

			if (newPos.z < -10f)
			{
				newPos = _spawner.RandomPosition();
			}
			
			transform.position = newPos;
		}

		public void SetSpawner(DecorationSpawner spawner)
		{
			_spawner = spawner;
		}
	}
}