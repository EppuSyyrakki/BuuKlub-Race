using System;
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
		private HealthBonus _healthBonus;
		private Decoration[] _decorations;
		private Character _player;
		private HealthBar _healthBar;
		private float _originalSpeed;
		private float _roadWidth;
		private bool _gameStarted;

		[SerializeField]
		private GraphicsPackage graphicsPackage;

		public static Game Instance => _instance;
		public RoadTexture RoadTexture => graphicsPackage.roadTexture;
		public GroundMaterial GroundMaterial => graphicsPackage.groundMaterial;
		public bool ControlEnabled { get; private set; }

		[Header("Movement/Character variables:")]
		public float forwardSpeed = 150f;
		[Range(0, 20f)]
		public float forwardSpeedIncrease = 10f;
		public float speedIncreaseTime = 2f;
		public float horizontalSpeed = 2f;
		[Range(1f, 3f), Tooltip("Time the character is protected from another crash after crashing.")]
		public float crashProtectionTime = 1.5f;
		[SerializeField, Range(0.1f, 1f), Tooltip("How fast a collision with obstacle stops the player")]
		private float stoppingSpeed = 0.5f;
		[SerializeField, Range(0.1f, 3f), Tooltip("Seconds to wait after colliding")]
		private float waitAfterCollision = 1f;
		[SerializeField, Range(0, 1f), Tooltip("Time it takes to get back up to starting speed after colliding")]
		private float speedUpAfterCollision = 1f;
		[Tooltip("How many crashes can the character survive")]
		public int characterMaxHealth = 3;
		[Tooltip("Deselecting will disable the health system completely.")]
		public bool useHealthSystem = true;

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
		[Range(0, 10), Tooltip("Ensure at least every n:th item is a collectible. 0 = disabled.")]
		public int ensureCollectibleEvery = 5;
		[Range(0, 10), Tooltip("Spawn at least n obstacles after spawning a collectible. 0 = disabled.")]
		public int obstaclesAfterCollectible = 2;
		[Range(0, 100), Tooltip("Chance of health bonus spawn instead of collectible if health is less than max")]
		public int healthBonusChance = 25;
		
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
		public HealthBonus HealthBonus => _healthBonus;
		public Character Player => _player;
		public Color UncollectedColor => graphicsPackage.uncollectedColor;
		public GameObject CollectibleAccent => graphicsPackage.collectibleAccentEffect;
		public GameObject CollectedEffect => graphicsPackage.collisionEffects.hitCollectiblePrefab;
		public float CollectedSize => graphicsPackage.itemSize;
		public float RoadWidth => _roadWidth;
		public int PlayerHealth => _player.Health;
		public Sprite HealthSprite => graphicsPackage.healthIcon.sprite;

		private void Awake()
		{
			if (graphicsPackage == null) { Debug.LogError("No Graphics Package found in Game component!"); }

			_instance = this;
			_cam = Camera.main;
			_collectibleDisplay = FindObjectOfType<CollectibleDisplay>();
			_originalSpeed = forwardSpeed;
			forwardSpeed = 0;
			_roadWidth = FindObjectOfType<Road>().transform.localScale.x * 0.5f;
			_player = FindObjectOfType<Character>();
			_healthBar = FindObjectOfType<HealthBar>();
			Time.fixedDeltaTime = 1 / (float) fixedTimeStep;
			SetControl(false);
			InitGraphics();

			if (useHealthSystem)
			{
				_player.HealthChanged += OnHealthChanged;
			}
		}

		private void OnDisable()
		{
			if (useHealthSystem)
			{
				_player.HealthChanged -= OnHealthChanged;
			}
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

		private void OnHealthChanged(int currentHealth)
		{
			_healthBar.SetHealth(currentHealth, characterMaxHealth);
		}

		private void InitGraphics()
		{
			_collectibles = InitItemArray<Collectible>(GetSprites(graphicsPackage.collectibleSprites));
			_obstacles = InitItemArray<Obstacle>(GetSprites(graphicsPackage.obstacleSprites));
			_decorations = InitItemArray<Decoration>(GetSprites(graphicsPackage.decorationSprites));
			_healthBonus = CreateSingle<HealthBonus>(graphicsPackage.healthIcon.sprite);
			
			foreach (var effect in graphicsPackage.weatherEffects)
			{
				Instantiate(effect);
			}
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

		public Sprite[] GetUiSprites()
		{
			return GetSprites(graphicsPackage.itemSprites);
		}

		private static T[] InitItemArray<T>(Sprite[] sprites) where T : Item
		{
			var items = new List<T>();

			foreach (var sprite in sprites)
			{
				var item = CreateSingle<T>(sprite);
				items.Add(item);
			}

			return items.ToArray();
		}

		private static T CreateSingle<T>(Sprite sprite) where T : Item
		{
			var go = new GameObject(sprite.name);
			var item = go.AddComponent<T>();
			item.Init(sprite);
			return item;
		}

		public void CollectHealth(Vector3 worldPosition)
		{
			_collectibleDisplay.CollectNew(_cam.WorldToScreenPoint(worldPosition));
			var effect = Instantiate(graphicsPackage.collisionEffects.hitCollectiblePrefab,
				worldPosition,
				Quaternion.identity,
				null);
			_player.ChangeHealth(1);
		}

		public void Collect(Vector3 worldPosition)
		{
			StopAllCoroutines();
			_collectibleDisplay.CollectNew(_cam.WorldToScreenPoint(worldPosition));
			var effect = Instantiate(graphicsPackage.collisionEffects.hitCollectiblePrefab,
				worldPosition,
				Quaternion.identity,
				null);
			Destroy(effect, 2.5f);
			StartCoroutine(ChangeSpeed(true, 0, forwardSpeed, 
				forwardSpeed + forwardSpeedIncrease, speedIncreaseTime, 0));
		}

		public void Collide(Vector3 worldPosition)
		{
			StopAllCoroutines();
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
