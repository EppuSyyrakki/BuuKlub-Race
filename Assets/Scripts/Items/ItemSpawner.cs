using System.Collections;
using System.Collections.Generic;
using BKRacing.Items;
using UnityEngine;

namespace BKRacing
{
	public class ItemSpawner: MonoBehaviour
	{
		private float _timeSinceLast;
		private float _timerTarget;
		
		private void Start()
		{
			var count = Game.Instance.initialSpawnCount;

			for (int i = 0; i < count; i++)
			{
				var z = Random.Range(0, transform.position.z * 0.75f);
				SpawnObstacle(RandomPosition() + Vector3.back * z);
			}
		}

		private void Update()
		{
			_timeSinceLast += Time.deltaTime;

			if (!(_timeSinceLast > _timerTarget)) return;

			Spawn(RandomPosition());
			_timeSinceLast = 0;
			_timerTarget = GetTimerTarget();
		}

		private float GetTimerTarget()
		{
			return Random.Range(Game.Instance.minSpawnTime, Game.Instance.maxSpawnTime);
		}

		private void Spawn(Vector3 pos)
		{
			if (Random.Range(0, 1f) < Game.Instance.collectibleBias)
			{
				SpawnCollectible(pos);
			}
			else
			{
				SpawnObstacle(pos);
			}
		}

		private void SpawnObstacle(Vector3 pos)
		{
			var item = Game.Instance.Obstacles[Random.Range(0, Game.Instance.Obstacles.Length)];
			CreateItem(pos, item);
		}

		private void SpawnCollectible(Vector3 pos)
		{
			var items = GetAvailableCollectibles();
			var item = items[Random.Range(0, items.Length)];
			CreateItem(pos, item);
		}

		private void CreateItem(Vector3 pos, Item item)
		{
			var go = Instantiate(item, pos + Vector3.down * 10f, Quaternion.identity, transform);
			go.gameObject.SetActive(true);
		}

		private Collectible[] GetAvailableCollectibles()
		{
			// TODO: Remove already collected items and return a new array or list
			return Game.Instance.Collectibles;
		}

		private Vector3 RandomPosition()
		{
			var v2 = Random.insideUnitCircle * (Game.Instance.RoadWidth - 0.5f);
			return transform.position + new Vector3(v2.x, 0, v2.y);
		}
	}
}
