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
		private Decoration[] _decorations;
		private Character _player;
		private float _startingSpeed;
		private float _roadWidth;
		private bool _gameStarted;
		private AudioPlayer _audioPlayer;
		private readonly Dictionary<SoundType, AudioClip> _sounds = new Dictionary<SoundType, AudioClip>();
		private CanvasController _startScreen, _endScreen;
		private static Transform _spawnContainer = null;
		private float _originalFixedTimeStep;
		
		[SerializeField]
		private GamePackage gamePackage;

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
		[Range(1, 5), Tooltip("How many obstacles get spawned on road at start")]
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
		
		[Header("Optimization variables:")]
		[Range(10, 50)]
		public int fixedTimeStep = 30;

		public Sprite BackgroundCard => gamePackage.backgroundCard;
		public GroundMaterial GroundMaterial => gamePackage.groundMaterial;
		public EnvironmentTexture RoadTexture => gamePackage.roadTexture;
		public Collectible[] Collectibles => _collectibles;
		public Obstacle[] Obstacles => _obstacles;
		public Decoration[] Decorations => _decorations;
		public Character Player => _player;
		public Color UncollectedColor => gamePackage.uncollectedColor;
		public GameObject CollectibleAccent => gamePackage.collectibleAccentEffect;
		public float CollectedSize => gamePackage.itemSize;
		public float RoadWidth => _roadWidth;
		public float AudioVolume => gamePackage.audioVolume;

		private void Awake()
		{
			if (gamePackage == null) { Debug.LogError("No Graphics Package found in Game component!"); }

			_instance = this;
			_startScreen = GameObject.FindGameObjectWithTag("RacingStartScreen").GetComponent<CanvasController>();
			_endScreen = GameObject.FindGameObjectWithTag("RacingEndScreen").GetComponent<CanvasController>();
			_endScreen.gameObject.SetActive(false);
			_spawnContainer = new GameObject("SpawnContainer").transform;
			_spawnContainer.SetParent(transform);
			_cam = Camera.main;
			_audioPlayer = _cam.gameObject.GetComponent<AudioPlayer>();
			_collectibleDisplay = FindObjectOfType<CollectibleDisplay>();
			_startingSpeed = forwardSpeed;
			forwardSpeed = 0;
			_roadWidth = FindObjectOfType<Road>().transform.localScale.x * 0.5f;
			_player = FindObjectOfType<Character>();
			SetControl(false);
			InitGraphics();
			InitSounds();
		}

		private void OnEnable()
		{
			_originalFixedTimeStep = Time.fixedDeltaTime;
			Time.fixedDeltaTime = 1 / (float)fixedTimeStep;
			_collectibleDisplay.gameCompleted += EndGame;
		}

		private void OnDisable()
		{
			_collectibleDisplay.gameCompleted -= EndGame;
			Time.fixedDeltaTime = _originalFixedTimeStep;
		}

		private void Update()
		{
			_audioPlayer.SetMoveVolumeAndPitch(Mathf.Clamp01(forwardSpeed / _startingSpeed));

			// Initial start of game.
			if (!ReadyToStart) { return; }

			if (Input.touchCount > 0 && !_gameStarted)
			{
				_gameStarted = true;
				StartCoroutine(ChangeSpeed(true, 0, 0, _startingSpeed, 
					speedUpAfterCollision, 0));
			}
		}

		private void InitGraphics()
		{
			_collectibles = InitItemArray<Collectible>(
				GetSpritesInfo(gamePackage.collectibleSprites, out var mirrors, out var soundTypes), mirrors, soundTypes);
			_obstacles = InitItemArray<Obstacle>(
				GetSpritesInfo(gamePackage.obstacleSprites, out mirrors, out soundTypes), mirrors, soundTypes);
			_decorations = InitItemArray<Decoration>(
				GetSpritesInfo(gamePackage.decorationSprites, out mirrors, out soundTypes), mirrors, soundTypes);
			
			foreach (var effect in gamePackage.weatherEffects)
			{
				Instantiate(effect, _spawnContainer);
			}
		}

		private void InitSounds()
		{
			foreach (var sound in gamePackage.sounds)
			{
				_sounds.Add(sound.type, sound.clip);
			}
		}

		private Sprite[] GetSpritesInfo<T>(List<T> items, out bool[] mirrors, out SoundType[] soundTypes) 
			where T : ItemSprite
		{
			var sprites = new Sprite[items.Count];
			mirrors = new bool[items.Count];
			soundTypes = new SoundType[items.Count];

			for (int i = 0; i < items.Count; i++)
			{
				sprites[i] = items[i].sprite;
				mirrors[i] = items[i].randomMirroring;
				soundTypes[i] = items[i].associatedSound;
			}

			return sprites;
		}

		private static T[] InitItemArray<T>(Sprite[] sprites, bool[] mirrors, SoundType[] soundTypes) where T : Item
		{
			var items = new T[sprites.Length];

			for (int i = 0; i < sprites.Length; i++)
			{
				var item = CreateSingle<T>(sprites[i], mirrors[i], soundTypes[i]);
				items[i] = item;
			}

			return items;
		}

		private static T CreateSingle<T>(Sprite sprite, bool mirror, SoundType soundType) where T : Item
		{
			var go = new GameObject(sprite.name);
			go.transform.SetParent(_spawnContainer);
			var item = go.AddComponent<T>();
			item.Init(sprite, mirror, soundType);
			return item;
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

		private void EndGame()
		{
			// TODO: make good
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

		public void Collect(Vector3 worldPosition, SoundType soundType)
		{
			PlaySound(soundType);
			StopAllCoroutines();
			_collectibleDisplay.CollectNew(_cam.WorldToScreenPoint(worldPosition));
			var effect = Instantiate(gamePackage.collisionEffects.hitCollectiblePrefab,
				worldPosition,
				Quaternion.identity,
				_spawnContainer);
			Destroy(effect, 2.5f);
			StartCoroutine(ChangeSpeed(true, 0, forwardSpeed,
				forwardSpeed + forwardSpeedIncrease, speedIncreaseTime, 0));
		}

		public void Collide(Vector3 worldPosition, SoundType soundType)
		{
			PlaySound(soundType);
			StopAllCoroutines();
			_collectibleDisplay.LoseCollected(_cam.WorldToScreenPoint(_player.transform.position));
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

		public void PlaySound(SoundType type)
		{
			if (_sounds.ContainsKey(type))
			{
				_audioPlayer.PlaySound(_sounds[type], type);
			}
		}
	}
}
