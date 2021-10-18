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

		[SerializeField]
		private GamePackage gamePackage;

		public static Game Instance => _instance;
		public bool ControlEnabled { get; private set; }

		[Header("Movement/Character variables:")]
		public float forwardSpeed = 165f;
		[Range(0, 30f)]
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
		[Range(1f,200f), Tooltip("How close to the camera all sprites turn towards")]
		public float billboardBend = 100f;
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
		public GameObject CollectedEffect => gamePackage.collisionEffects.hitCollectiblePrefab;
		public float CollectedSize => gamePackage.itemSize;
		public float RoadWidth => _roadWidth;
		public Dictionary<SoundType, AudioClip> Sounds => _sounds;
		public float AudioVolume => gamePackage.audioVolume;

		private void Awake()
		{
			if (gamePackage == null) { Debug.LogError("No Graphics Package found in Game component!"); }

			_instance = this;
			_cam = Camera.main;
			_audioPlayer = _cam.gameObject.GetComponent<AudioPlayer>();
			_collectibleDisplay = FindObjectOfType<CollectibleDisplay>();
			_startingSpeed = forwardSpeed;
			forwardSpeed = 0;
			_roadWidth = FindObjectOfType<Road>().transform.localScale.x * 0.5f;
			_player = FindObjectOfType<Character>();
			Time.fixedDeltaTime = 1 / (float) fixedTimeStep;
			SetControl(false);
			InitGraphics();
			InitSounds();
		}

		private void Update()
		{
			// Initial start of game.

			if (Input.touchCount > 0 && !_gameStarted)
			{
				_gameStarted = true;
				StartCoroutine(ChangeSpeed(true, 0, 0, _startingSpeed, 
					speedUpAfterCollision, 0));
			}

			_audioPlayer.SetMoveVolume(Mathf.Clamp01(forwardSpeed / _startingSpeed));

			if (forwardSpeed > 0)
			{
				PlaySound(SoundType.MoveForward);
			}
		}

		private void InitGraphics()
		{
			_collectibles = InitItemArray<Collectible>(
				GetSpritesInfo(gamePackage.collectibleSprites, out var mirrors), mirrors);
			_obstacles = InitItemArray<Obstacle>(
				GetSpritesInfo(gamePackage.obstacleSprites, out mirrors), mirrors);
			_decorations = InitItemArray<Decoration>(
				GetSpritesInfo(gamePackage.decorationSprites, out mirrors), mirrors);
			
			foreach (var effect in gamePackage.weatherEffects)
			{
				Instantiate(effect);
			}
		}

		private void InitSounds()
		{
			foreach (var sound in gamePackage.sounds)
			{
				_sounds.Add(sound.type, sound.clip);
			}
		}

		private Sprite[] GetSpritesInfo<T>(List<T> items, out bool[] mirrors) where T : ItemSprite
		{
			var sprites = new Sprite[items.Count];
			mirrors = new bool[items.Count];

			for (int i = 0; i < items.Count; i++)
			{
				sprites[i] = items[i].sprite;
				mirrors[i] = items[i].randomMirroring;
			}

			return sprites;
		}

		private static T[] InitItemArray<T>(Sprite[] sprites, bool[] mirrors) where T : Item
		{
			var items = new T[sprites.Length];

			for (int i = 0; i < sprites.Length; i++)
			{
				var item = CreateSingle<T>(sprites[i], mirrors[i]);
				items[i] = item;
			}

			return items;
		}

		private static T CreateSingle<T>(Sprite sprite, bool mirror) where T : Item
		{
			var go = new GameObject(sprite.name);
			var item = go.AddComponent<T>();
			item.Init(sprite, mirror);
			return item;
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
			_collectibleDisplay.CollectNew(_cam.WorldToScreenPoint(worldPosition));
			var effect = Instantiate(gamePackage.collisionEffects.hitCollectiblePrefab,
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
			_collectibleDisplay.LoseCollected(_cam.WorldToScreenPoint(_player.transform.position));
			var effect = Instantiate(gamePackage.collisionEffects.hitObstaclePrefab,
				worldPosition,
				Quaternion.identity,
				null);
			Destroy(effect, 5f);
			StartCoroutine(ChangeSpeed(false, 0, forwardSpeed, 0, stoppingSpeed, 
				waitAfterCollision));
			StartCoroutine(ChangeSpeed(true, stoppingSpeed + waitAfterCollision, 0,
				_startingSpeed, speedUpAfterCollision, 0));
		}

		private void PlaySound(SoundType type)
		{
			if (_sounds.ContainsKey(type))
			{
				_audioPlayer.PlaySound(_sounds[type], type);
			}
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
