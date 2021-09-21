using System;
using System.Collections;
using System.Collections.Generic;
using BK.Items;
using UnityEngine;

namespace BK
{
	public class Game : MonoBehaviour
	{
		private static Game _instance;

		[SerializeField]
		private GraphicsPackage graphicsPackage;

		public static Game Instance => _instance;
		public RoadTexture RoadTexture => graphicsPackage.roadTexture;

		private void Awake()
		{
			_instance = this;

			if (graphicsPackage == null) { Debug.LogError("No Graphics Package found in Game component!"); }

			InitGraphics();
		}

		private Collectible[] _collectibles;
		private Obstacle[] _obstacles;
		private Item[] _decorations;

		[Header("Movement variables:")]
		public float roadWidth = 6f;
		public float forwardSpeed = 5f;
		public float horizontalSpeed = 2f;

		[Header("Spawner variables:")]
		[Range(0, 1), Tooltip("0 = only obstacles, 1 = only collectibles")]
		public float collectibleBias = 0.5f;
		[Range(2, 4)]
		public float minSpawnTime = 2f;
		[Range(4, 6)]
		public float maxSpawnTime = 4f;
		[Range(0, 10)]
		public int decorationDensity = 5;

		public Collectible[] Collectibles => _collectibles;
		public Obstacle[] Obstacles => _obstacles;
		public Item[] Decorations => _decorations;

		private void InitGraphics()
		{
			_collectibles = InitItemArray<Collectible>(graphicsPackage.collectibleSprites);
			_obstacles = InitItemArray<Obstacle>(graphicsPackage.obstacleSprites);
			_decorations = InitItemArray<Item>(graphicsPackage.decorationSprites);
		}

		private T[] InitItemArray<T>(Sprite[] sprites) where T : Item
		{
			var items = new List<T>();

			foreach (var sprite in sprites)
			{
				var go = new GameObject(sprite.name);
				var item = go.AddComponent<T>();
				item.Init(sprite);
				items.Add(item);
				go.SetActive(false);
			}

			return items.ToArray();
		}
	}
}
