using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BKRacing.Items
{
	[RequireComponent(typeof(EdgeCollider2D))]
	public class DecorationSpawner : MonoBehaviour
	{
		private EdgeCollider2D _edge;
		
		private void Awake()
		{
			_edge = GetComponent<EdgeCollider2D>();
		}

		private void Start()
		{
			var initSpawnCount = Game.Instance.decorationDensity * 10;

			for (int i = 0; i < initSpawnCount; i++)
			{
				Spawn(RandomPosition() + Vector3.back * Random.Range(0, transform.position.z));
			}
		}
		
		private void Spawn(Vector3 pos)
		{
			int i = Random.Range(0, Game.Instance.Decorations.Length);
			var decoration = Game.Instance.Decorations[i];
			var go = Instantiate(decoration, pos, Quaternion.identity, transform);
			go.SetSpawner(this);
			go.gameObject.SetActive(true);
		}

		public Vector3 RandomPosition()
		{
			var bounds = _edge.bounds;
			return new Vector3(
				Random.Range(bounds.min.x, bounds.max.x),
				Random.Range(bounds.min.y, bounds.max.y),
				Random.Range(bounds.min.z, bounds.max.z)
			);
		}
	
	}
}