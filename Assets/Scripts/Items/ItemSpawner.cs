using System.Collections;
using System.Collections.Generic;
using BK.Items;
using UnityEngine;

namespace BK
{
	public class ItemSpawner: MonoBehaviour
	{
		private float _timeSinceLast;
		private float _timerTarget;

		private void Start()
		{
			SpawnObstacle(transform.position + Vector3.back * 40f);
		}

		private void Update()
		{
			_timeSinceLast += Time.deltaTime;

			if (!(_timeSinceLast > _timerTarget)) return;

			Spawn();
			_timeSinceLast = 0;
			_timerTarget = GetTimerTarget();
		}

		private float GetTimerTarget()
		{
			return Random.Range(Game.Instance.minTime, Game.Instance.maxTime);
		}

		private void Spawn()
		{
			if (Random.Range(0, 1f) < Game.Instance.bias)
			{
				SpawnCollectible(RandomPosition());
			}
			else
			{
				SpawnObstacle(RandomPosition());
			}
		}

		private void SpawnObstacle(Vector3 pos)
		{
			var item = Game.Instance.obstacles[Random.Range(0, Game.Instance.obstacles.Length)];
			Instantiate(item, pos, Quaternion.identity, transform);
		}

		private void SpawnCollectible(Vector3 pos)
		{
			var items = GetAvailableCollectibles();
			var item = items[Random.Range(0, items.Length)];
			Instantiate(item, pos, Quaternion.identity, transform);
		}

		private Item[] GetAvailableCollectibles()
		{
			// TODO: Remove already collected items and return a new array or list
			return Game.Instance.collectibles;
		}

		private Vector3 RandomPosition()
		{
			return transform.position;
		}
	}
}
