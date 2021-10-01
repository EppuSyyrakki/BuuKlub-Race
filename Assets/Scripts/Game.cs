﻿using System;
using System.Collections;
using System.Collections.Generic;
using BKRacing.Environment;
using BKRacing.GUI;
using BKRacing.Items;
using UnityEngine;

namespace BKRacing
{
	public class Game : MonoBehaviour
	{
		private static Game _instance;
		private Camera _cam;
		private CollectibleDisplay _collectibleDisplay;
		private Collectible[] _collectibles;
		private Obstacle[] _obstacles;
		private Decoration[] _decorations;
		private UiItem[] _items;
		private Character _player;
		private float _originalSpeed;
		private float _roadWidth;
		private bool _gameStarted;

		[SerializeField]
		private GraphicsPackage graphicsPackage;

		public static Game Instance => _instance;
		public RoadTexture RoadTexture => graphicsPackage.roadTexture;
		public GroundMaterial GroundMaterial => graphicsPackage.groundMaterial;
		public bool ControlEnabled { get; private set; }

		[Header("Movement variables:")]
		public float forwardSpeed = 150f;
		[Range(0, 20f)]
		public float forwardSpeedIncrease = 10f;
		public float horizontalSpeed = 2f;
		[SerializeField, Range(0.1f, 1f), Tooltip("How fast a collision with obstacle stops the player")]
		private float stoppingSpeed = 0.5f;
		[SerializeField, Range(0.1f, 3f), Tooltip("Seconds to wait after colliding")]
		private float waitAfterCollision = 1f;
		[SerializeField, Range(0, 1f), Tooltip("Time it takes to get back up to starting speed after colliding")]
		private float speedUpAfterCollision = 1f;

		[Header("Spawner variables:")]
		[Range(0, 1), Tooltip("0 = only obstacles, 1 = only collectibles")]
		public float collectibleBias = 0.5f;
		[Range(0.25f, 1.95f), Tooltip("Minimum time between spawns")]
		public float minSpawnTime = 1f;
		[Range(2, 4), Tooltip("Maximum time between spawns")]
		public float maxSpawnTime = 2.5f;
		[Range(0, 10), Tooltip("Density of decorations outside the road area")]
		public int decorationDensity = 5;
		[Range(1, 5), Tooltip("How many obstacles get spawned on road at start")]
		public int initialSpawnCount = 2;

		[Header("Visual variables:")]
		[Range(1f,200f), Tooltip("How close to the camera all sprites turn towards")]
		public float billboardBend = 100f;
		[Tooltip("Curve for the height of all items after spawning")]
		public AnimationCurve riseCurve;
		[Range(5f,200f), Tooltip("How far from the spawn point the curve affects object height")]
		public float curveDistance = 20f;

		[Header("Optimization variables:")]
		[Range(10, 50)]
		public int fixedTimeStep = 30;

		public Collectible[] Collectibles => _collectibles;
		public Obstacle[] Obstacles => _obstacles;
		public Decoration[] Decorations => _decorations;
		public UiItem[] Items => _items;
		public Character Player => _player;
		public float RoadWidth => _roadWidth;

		private void Awake()
		{
			_instance = this;
			_cam = Camera.main;
			_collectibleDisplay = FindObjectOfType<CollectibleDisplay>();
			_originalSpeed = forwardSpeed;
			forwardSpeed = 0;
			_roadWidth = FindObjectOfType<Road>().transform.localScale.x * 0.5f;

			if (graphicsPackage == null) { Debug.LogError("No Graphics Package found in Game component!"); }

			_player = FindObjectOfType<Character>();
			Time.fixedDeltaTime = 1 / (float) fixedTimeStep;
			SetControl(false);
			InitGraphics();
		}

		private void Update()
		{
			// Initial start of game.

			if (Input.touchCount > 0 && !_gameStarted)
			{
				_gameStarted = true;
				StartCoroutine(ChangeSpeed(true, 0, 0, _originalSpeed, 
					speedUpAfterCollision, 0));
			}
		}

		private void InitGraphics()
		{
			_collectibles = InitItemArray<Collectible>(GetSprites(graphicsPackage.collectibleSprites));
			_obstacles = InitItemArray<Obstacle>(GetSprites(graphicsPackage.obstacleSprites));
			_decorations = InitItemArray<Decoration>(GetSprites(graphicsPackage.decorationSprites));
			_items = InitItemArray<UiItem>(GetSprites(graphicsPackage.itemSprites), _collectibleDisplay.transform);
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

		private static T[] InitItemArray<T>(Sprite[] sprites, Transform parent = null) where T : Item
		{
			var items = new List<T>();

			foreach (var sprite in sprites)
			{
				var go = new GameObject(sprite.name);
				var item = go.AddComponent<T>();
				item.Init(sprite);
				items.Add(item);

				if (parent != null) { go.transform.SetParent(parent, false); }
			}

			return items.ToArray();
		}

		public void Collect(Vector3 worldPosition)
		{
			Vector2 screenPosition = _cam.WorldToScreenPoint(worldPosition);
			forwardSpeed += forwardSpeedIncrease;
		}

		public void Collide(Vector3 worldPosition)
		{
			var effect = Instantiate(graphicsPackage.collisionEffects.hitObstaclePrefab,
				worldPosition,
				Quaternion.identity,
				null);
			Destroy(effect, 5f);
			StartCoroutine(ChangeSpeed(false, 0, forwardSpeed, 0, stoppingSpeed, 
				waitAfterCollision));
			StartCoroutine(ChangeSpeed(true, stoppingSpeed + waitAfterCollision, 0,
				_originalSpeed, speedUpAfterCollision, 0));
		}

		private IEnumerator ChangeSpeed(bool control, float preWait, float from, float to, float time, float postWait)
		{
			if (!control) { SetControl(false); }

			float t = 0;
			yield return new WaitForSeconds(preWait);
			
			if (control) { SetControl(true); }

			while (t < time)
			{
				forwardSpeed = Mathf.Lerp(from, to, t / time);
				t += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}

			yield return new WaitForSeconds(postWait);
		}

		private void SetControl(bool enable)
		{
			ControlEnabled = enable;
			if (Player.Particles != null) { Player.Particles.gameObject.SetActive(enable); }
		}
	}
}
