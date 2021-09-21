using System;
using System.Collections;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BK.Items
{
	[RequireComponent(typeof(EdgeCollider2D))]
	public class DecorationSpawner : ItemSpawner
	{
		private EdgeCollider2D _edge;

		private void Awake()
		{
			_edge = GetComponent<EdgeCollider2D>();
		}

		private void Start()
		{
			initSpawnCount = Game.Instance.decorationDensity * 10;

			for (int i = 0; i < initSpawnCount; i++)
			{
				Spawn(RandomPosition() + Vector3.back * Random.Range(0, initSpawnDistance));
			}
		}

		protected override float GetTimerTarget()
		{
			return Random.Range(Game.Instance.decorationDensity * 0.75f, Game.Instance.decorationDensity * 1.25f) * 0.1f;
		}

		protected override void Spawn(Vector3 pos)
		{
			int i = Random.Range(0, Game.Instance.Decorations.Length);
			var decoration = Game.Instance.Decorations[i];
			CreateItem(pos, decoration);
		}

		protected override Vector3 RandomPosition()
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