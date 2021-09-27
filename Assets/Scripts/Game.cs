using System;
using System.Collections;
using System.Collections.Generic;
using BKRacing.Items;
using UnityEngine;

namespace BKRacing
{
	public class Game : MonoBehaviour
	{
		private static Game _instance;
		private Camera _cam;

		[SerializeField]
		private GraphicsPackage graphicsPackage;

		public static Game Instance => _instance;
		public RoadTexture RoadTexture => graphicsPackage.roadTexture;
		public GroundMaterial GroundMaterial => graphicsPackage.groundMaterial;

		private Collectible[] _collectibles;
		private Obstacle[] _obstacles;
		private Item[] _decorations;
		private Character _player;

		[Header("Movement variables:")]
		public float roadWidth = 6f;
		public float forwardSpeed = 5f;
		public float horizontalSpeed = 2f;
		public float moveTreshold = 10f;

		[Header("Spawner variables:")]
		[Range(0, 1), Tooltip("0 = only obstacles, 1 = only collectibles")]
		public float collectibleBias = 0.5f;
		[Range(1, 3.95f)]
		public float minSpawnTime = 2f;
		[Range(4, 8)]
		public float maxSpawnTime = 4f;
		[Range(0, 10)]
		public int decorationDensity = 5;

		[Header("Visual variables:")]
		[Range(1f,200f), Tooltip("How close to the camera all sprites turn towards")]
		public float billboardBend = 100f;

		[Header("Optimization variables:")]
		[Range(10, 50)]
		public int fixedTimeStep = 30;

		public Collectible[] Collectibles => _collectibles;
		public Obstacle[] Obstacles => _obstacles;
		public Item[] Decorations => _decorations;
		public Character Player => _player;

		private void Awake()
		{
			_instance = this;
			_cam = Camera.main;

			if (graphicsPackage == null) { Debug.LogError("No Graphics Package found in Game component!"); }

			InitGraphics();
			_player = FindObjectOfType<Character>();
			Time.fixedDeltaTime = 1 / (float) fixedTimeStep;
		}

		private void InitGraphics()
		{
			_collectibles = InitItemArray<Collectible>(GetSprites(graphicsPackage.collectibleSprites));
			_obstacles = InitItemArray<Obstacle>(GetSprites(graphicsPackage.obstacleSprites));
			_decorations = InitItemArray<Item>(GetSprites(graphicsPackage.decorationSprites));
		}

		private Sprite[] GetSprites<T>(List<T> items) where T : ItemSprite
		{
			var sprites = new List<Sprite>();

			foreach (var item in items)
			{
				sprites.Add(item.sprite);
			}

			return sprites.ToArray();
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

		public static void Collect(Vector3 worldPosition)
		{

		}
	}
}
