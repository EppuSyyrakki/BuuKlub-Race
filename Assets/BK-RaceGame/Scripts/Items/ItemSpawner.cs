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
		private int _obstaclesSpawned;
		
		private void Start()
		{
			var count = Game.Instance.initialSpawnCount;
			var dist = transform.position.z / count;

			for (int i = 1; i <= count; i++)
			{
				var z = dist * i - 10f;
				SpawnObstacle(RandomPosition() + Vector3.back * z);
			}
		}

		private void Update()
		{
			if (!Game.Instance.ControlEnabled) { return; }

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
			if (AreItemsNear(pos)) { return; }

			if (Game.Instance.obstaclesAfterCollectible > 0
			    && _obstaclesSpawned < Game.Instance.obstaclesAfterCollectible)
			{
				SpawnObstacle(pos);
				return;
			}
			
			if (Game.Instance.ensureCollectibleEvery > 0
			         && _obstaclesSpawned + 1 >= Game.Instance.ensureCollectibleEvery)
			{
				SpawnCollectible(pos);
				return;
			}

			if (Random.Range(0, 1f) < Game.Instance.collectibleBias)
			{
				SpawnCollectible(pos);
			}
			else
			{
				SpawnObstacle(pos);
			}
		}

		private bool AreItemsNear(Vector3 pos)
		{
			var cols = Physics.OverlapSphere(pos, 2f);

			foreach (var col in cols)
			{
				var item = col.GetComponent<Item>();

				if (item == null) { continue; }

				return true;
			}

			return false;
		}

		private void SpawnObstacle(Vector3 pos)
		{
			_obstaclesSpawned++;
			var item = Game.Instance.Obstacles[Random.Range(0, Game.Instance.Obstacles.Length)];
			CreateItem(pos, item);
		}

		private void SpawnCollectible(Vector3 pos)
		{
			_obstaclesSpawned = 0;
			var items = Game.Instance.Collectibles;
			var item = items[Random.Range(0, items.Length)];
			CreateItem(pos, item, true);
		}

		private void CreateItem(Vector3 pos, Item item, bool includeEffect = false)
		{
			var go = Instantiate(item, pos + Vector3.down * 10f, Quaternion.identity, transform);
			go.SetSound(item.Sound);
			go.gameObject.SetActive(true);

			if (item.Mirror)
			{
				go.GetComponent<SpriteRenderer>().flipX = Random.Range(0, 1f) <= 0.5f;
			}
			
			if (includeEffect)
			{
				Instantiate(Game.Instance.CollectibleAccent, go.transform);
			}
		}
		
		private Vector3 RandomPosition()
		{
			var v2 = Random.insideUnitCircle * (Game.Instance.RoadWidth - 0.5f);
			return transform.position + new Vector3(v2.x, 0, v2.y);
		}
	}
}
