using System;
using System.Collections;
using System.Collections.Generic;
using BKRacing.Environment;
using BKRacing.GUI;
using BKRacing.Items;
using TMPro.EditorUtilities;
using UnityEngine;

namespace BKRacing
{
	public class Game : MonoBehaviour
	{
		private static Game _instance;
		private Camera _cam;
		private CollectibleDisplay _collectibleDisplay;

		[SerializeField]
		private GraphicsPackage graphicsPackage;

		public static Game Instance => _instance;
		public RoadTexture RoadTexture => graphicsPackage.roadTexture;
		public GroundMaterial GroundMaterial => graphicsPackage.groundMaterial;

		private Collectible[] _collectibles;
		private Obstacle[] _obstacles;
		private Decoration[] _decorations;
		private Character _player;
		private float _originalSpeed;
		private float _roadWidth;

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
		public Character Player => _player;
		public float RoadWidth => _roadWidth;

		private void Awake()
		{
			_instance = this;
			_cam = Camera.main;
			_collectibleDisplay = FindObjectOfType<CollectibleDisplay>();
			_originalSpeed = forwardSpeed;
			_roadWidth = FindObjectOfType<Road>().transform.localScale.x * 0.5f;

			if (graphicsPackage == null) { Debug.LogError("No Graphics Package found in Game component!"); }

			InitGraphics();
			_player = FindObjectOfType<Character>();
			Time.fixedDeltaTime = 1 / (float) fixedTimeStep;
		}

		private void InitGraphics()
		{
			_collectibles = InitItemArray<Collectible>(GetSprites(graphicsPackage.collectibleSprites));
			_obstacles = InitItemArray<Obstacle>(GetSprites(graphicsPackage.obstacleSprites));
			_decorations = InitItemArray<Decoration>(GetSprites(graphicsPackage.decorationSprites));
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

		public void Collect(Vector3 worldPosition)
		{
			Vector2 screenPosition = _cam.WorldToScreenPoint(worldPosition);
			forwardSpeed += forwardSpeedIncrease;
		}

		public void Collide(Vector3 worldPosition)
		{
			StartCoroutine(nameof(SlowDown));
		}

		private IEnumerator SlowDown()
		{
			float time = 0;

			while (time < stoppingSpeed)
			{
				forwardSpeed = Mathf.Lerp(_originalSpeed, 0, time / stoppingSpeed);
				time += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}

			forwardSpeed = 0;
			time = 0;
			yield return new WaitForSeconds(waitAfterCollision);
			

			while (time < speedUpAfterCollision)
			{
				forwardSpeed = Mathf.Lerp(0, _originalSpeed, time / speedUpAfterCollision);
				time += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
		}
	}
}
