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
		private CollectibleDisplay _collectibleDisplay;
		private Collectible[] _collectibles;
		private Obstacle[] _obstacles;
		private Decoration[] _decorations;
		private Finish _finishLine;
		private Character _player;
		private float _startingSpeed;
		private float _maxSpeed;
		private float _roadWidth;
		private bool _gameStarted;
		private AudioPlayer _audioPlayer;
		private CanvasController _startScreen, _endScreen;
		private static Transform _spawnContainer = null;
		private float _originalFixedTimeStep;
		private ItemSpawner _itemSpawner;
		
		[SerializeField]
		private GamePackage gamePackage;

		[SerializeField]
		private Camera sceneCamera = null;

		public static Game Instance => _instance;
		public bool ControlEnabled { get; private set; }
		public bool ReadyToStart { get; set; } = false;

		[Header("Movement/Character variables:")]
		public float forwardSpeed = 165f;
		[Range(0, 30f)]
		public float forwardSpeedIncrease = 10f;
		public float speedIncreaseTime = 2f;
		[Range(1f, 10f)]
		public float horizontalSpeed = 6;
		[Range(1f, 3f), Tooltip("Time the character is protected from another crash after crashing.")]
		public float crashProtectionTime = 1.5f;
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
		[Range(1, 10), Tooltip("How many obstacles get spawned on road at start")]
		public int initialSpawnCount = 2;
		[Range(0, 10), Tooltip("Ensure at least every n:th item is a collectible. 0 = disabled.")]
		public int ensureCollectibleEvery = 5;
		[Range(0, 10), Tooltip("Spawn at least n obstacles after spawning a collectible. 0 = disabled.")]
		public int obstaclesAfterCollectible = 2;
		
		[Header("Visual variables:")]
		[Range(1f, 200f), Tooltip("How close to the camera item sprites turn towards")]
		public float itemBillboardBend = 100f;
		[Range(1f, 200f), Tooltip("Depth offset behind the camera where decoration sprites turn towards")]
		public float decorationBillboardBendZ = 25f;
		[Range(-50f, 50f), Tooltip("Height offset behind the camera where decoration sprites turn towards")]
		public float decorationBillboardBendY = 2f;
		[Tooltip("Curve for the height of all items after spawning")]
		public AnimationCurve riseCurve;
		[Range(5f,200f), Tooltip("How far from the spawn point the curve affects object height")]
		public float curveDistance = 20f;
		[Range(0, 150f), Tooltip("The height where the background card is drawn")]
		public float backgroundY = 50f;
		[Range(200f, 500f), Tooltip("The depth where the background card is drawn")]
		public float backgroundZ = 300f;
		[Range(0.5f, 4f), Tooltip("Amount to scale the background in size")]
		public float backgroundScale = 1f;
		
		[Header("User Interface variables:")]
		public Color uncollectedColor;
		[Range(0, 1f), Tooltip("Collected item position on the screen")]
		public float itemYPosition = 0.4f, itemXPosition = 0.25f;
		[Range(0.05f, 0.3f), Tooltip("Size of the collected items as fraction of screen width")]
		public float itemSize = 0.2f;
		[Tooltip("Time it takes for an item to move to display and inventory positions")]
		public float moveTime = 0.5f;
		[Tooltip("How long a gained item is displayed on screen")]
		public float waitTime = 1f;
		[Tooltip("How long does it take for a lost item to fly off screen")]
		public float flyTime = 1.5f;
		[Tooltip("Curve of moving item to display position")]
		public AnimationCurve toCenterCurve;
		[Tooltip("Curve of moving item to inventory position")]
		public AnimationCurve toInventoryCurve;
		[Tooltip("Curve of item height when it is lost. Also used to launch obstacles/items when all collected")]
		public AnimationCurve loseItemCurve;
		[Range (2f, 10f), Tooltip("Height of launched items on road when all collected")]
		public float launchHeight = 5f;

		[Header("Audio source variables:")]
		[Range(0, 1f)]
		public float masterVolume = 0.5f;
		[Range(0, 1f)]
		public float movingVolume = 1f;
		[Range(0, 1f)]
		public float effectVolume = 1f;
		[Range(0, 1f)]
		public float voiceVolume = 1f;
		[Range(0, 0.95f)]
		public float movingPitchBend = 0.1f;
		[Range(0, 10f), Tooltip("How many seconds to wait after finish line to trigger ending voice.")]
		public float waitForEndVoice = 4f;
		
		[Header("Optimization variables:")]
		[Range(10, 50), Tooltip("Will be changed back to default when this object is disabled")]
		public int fixedTimeStep = 30;

		public Sprite BackgroundCard => gamePackage.backgroundCard;
		public GroundMaterial GroundMaterial => gamePackage.groundMaterial;
		public EnvironmentTexture RoadTexture => gamePackage.roadTexture;
		public Finish FinishLine => _finishLine;
		public Collectible[] Collectibles => _collectibles;
		public Obstacle[] Obstacles => _obstacles;
		public Decoration[] Decorations => _decorations;
		public Character Player => _player;
		public GameObject CollectibleAccent => gamePackage.collectibleAccentEffect;
		public float RoadWidth => _roadWidth;
		public SoundCollection SoundCollection => gamePackage.soundCollection;

		private void Awake()
		{
			if (gamePackage == null) { Debug.LogError("No Graphics Package found in Game component!"); }
			if (sceneCamera == null) { Debug.LogError("No Camera set in Game component!");}

			_instance = this;
			gamePackage.SetSoundTypes();
			_startScreen = GameObject.FindGameObjectWithTag("RacingStartScreen").GetComponent<CanvasController>();
			_endScreen = GameObject.FindGameObjectWithTag("RacingEndScreen").GetComponent<CanvasController>();
			_endScreen.gameObject.SetActive(false);
			_spawnContainer = new GameObject("SpawnContainer").transform;
			_spawnContainer.SetParent(transform);
			_audioPlayer = sceneCamera.gameObject.GetComponent<AudioPlayer>();
			_collectibleDisplay = FindObjectOfType<CollectibleDisplay>();
			_startingSpeed = forwardSpeed;
			forwardSpeed = 0;
			_maxSpeed = speedUpAfterCollision * (gamePackage.itemSprites.Count - 1) + _startingSpeed;
			_roadWidth = FindObjectOfType<Road>().transform.localScale.x * 0.5f;
			_player = FindObjectOfType<Character>();
			_itemSpawner = FindObjectOfType<ItemSpawner>();
			SetControl(false);
			InitItems();
		}

		private void OnEnable()
		{
			_collectibleDisplay.allCollected += DiscardAllRoadItems;
			_originalFixedTimeStep = Time.fixedDeltaTime;
			Time.fixedDeltaTime = 1 / (float)fixedTimeStep;
		}
		
		private void OnDisable()
		{
			_collectibleDisplay.allCollected -= DiscardAllRoadItems;
			Time.fixedDeltaTime = _originalFixedTimeStep;
		}

		private void Update()
		{
			_audioPlayer.SetMoveVolumeAndPitch(forwardSpeed / _maxSpeed);

			// Initial start of game.
			if (!ReadyToStart) { return; }

			if (Input.touchCount > 0 && !_gameStarted)
			{
				_gameStarted = true;
				StartCoroutine(ChangeSpeed(true, 0, 0, _startingSpeed, 
					speedUpAfterCollision, 0));
				Player.StartMoving();
			}
		}

		private void InitItems()
		{
			_collectibles = InitItemArray<Collectible>(
				GetItemInfo(gamePackage.collectibleSprites, out var m, out var s), m, s);
			_obstacles = InitItemArray<Obstacle>(
				GetItemInfo(gamePackage.obstacleSprites, out m, out s), m, s);
			_decorations = InitItemArray<Decoration>(
				GetItemInfo(gamePackage.decorationSprites, out m, out s), m, s);
			_finishLine = CreateSingle<Finish>(gamePackage.finishLine, false, gamePackage.soundCollection.endFanfare);
			
			foreach (var effect in gamePackage.weatherEffects)
			{
				Instantiate(effect, _spawnContainer);
			}
		}

		private static T[] InitItemArray<T>(Sprite[] sprites, bool[] mirrors, Sound[] sounds) where T : Item
		{
			var items = new T[sprites.Length];

			for (int i = 0; i < sprites.Length; i++)
			{
				var item = CreateSingle<T>(sprites[i], mirrors[i], sounds[i]);
				items[i] = item;
			}

			return items;
		}

		private static T CreateSingle<T>(Sprite sprite, bool mirror, Sound sound) where T : Item
		{
			var go = new GameObject(sprite.name);
			go.transform.SetParent(_spawnContainer);
			var item = go.AddComponent<T>();
			item.Init(sprite, mirror, sound);
			return item;
		}

		private Sprite[] GetItemInfo<T>(List<T> items, out bool[] mirrors, out Sound[] sounds) 
			where T : ItemSprite
		{
			var sprites = new Sprite[items.Count];
			mirrors = new bool[items.Count];
			sounds = new Sound[items.Count];

			for (int i = 0; i < items.Count; i++)
			{
				sprites[i] = items[i].sprite;
				mirrors[i] = items[i].randomMirroring;

				if (items[i] is RoadItemSprite item)
				{
					sounds[i] = item.collisionSound;
				}
			}

			return sprites;
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

		public void EndGame()
		{
			Player.DisableCrashing();
			StopAllCoroutines();
			StartCoroutine(ChangeSpeed(false, 1f, forwardSpeed, 0, 1, 0));
			Invoke(nameof(ShowEndScreen), 2f);
		}

		private void ShowEndScreen()
		{
			_endScreen.gameObject.SetActive(true);
			_endScreen.FadeAll(0, 1);
		}

		public Sprite[] GetUiSprites()
		{
			var sprites = new List<Sprite>();

			foreach (var item in gamePackage.itemSprites)
			{
				sprites.Add(item.sprite);
			}

			return sprites.ToArray();
		}

		public void Collect(Vector3 worldPosition)
		{
			StopAllCoroutines();
			_collectibleDisplay.CollectNew(sceneCamera.WorldToScreenPoint(worldPosition));
			var effect = Instantiate(gamePackage.collisionEffects.hitCollectiblePrefab,
				worldPosition,
				Quaternion.identity,
				_spawnContainer);
			Destroy(effect, 2.5f);
			StartCoroutine(ChangeSpeed(true, 0, forwardSpeed,
				forwardSpeed + forwardSpeedIncrease, speedIncreaseTime, 0));
		}

		public void Collide(Vector3 worldPosition)
		{
			StopAllCoroutines();
			_collectibleDisplay.LoseCollected(sceneCamera.WorldToScreenPoint(_player.transform.position));
			var effect = Instantiate(gamePackage.collisionEffects.hitObstaclePrefab,
				worldPosition,
				Quaternion.identity,
				_spawnContainer);
			Destroy(effect, 5f);
			StartCoroutine(ChangeSpeed(false, 0, forwardSpeed, 0, stoppingSpeed,
				waitAfterCollision));
			StartCoroutine(ChangeSpeed(true, stoppingSpeed + waitAfterCollision, 0,
				_startingSpeed, speedUpAfterCollision, 0));
		}

		public void DiscardAllRoadItems()
		{
			var items = _itemSpawner.GetItemsAndStopSpawning();
			StartCoroutine(ChangeSpeed(true, 0, forwardSpeed, forwardSpeed * 2, 
				8f, 0));

			foreach (var item in items)
			{
				item.DiscardItem();
			}
		}
	}
}
