using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BKRacing
{
	[Serializable]
	public class ItemSprite
	{
		public Sprite sprite;
		public bool randomMirroring = false;
	}

	[Serializable]
	public class RoadItemSprite : ItemSprite
	{
		public Sound collisionSound;
	}

	[Serializable]
	public class Sound
	{
		public AudioClip clip;
		public SoundType Type { get; set; }
	}

	[Serializable]
	public class EnvironmentTexture
	{
		public Texture texture;
		public Vector2 tiling;

		public EnvironmentTexture(Vector2 tiling)
		{
			this.tiling = tiling;
		}
	}

	[System.Serializable]
	public class GroundMaterial
	{
		public Color color = Color.white;

		[Range(0, 1)]
		public float smoothness, metallic;
	}

	[System.Serializable]
	public class CollisionEffects
	{
		public GameObject hitObstaclePrefab;
		public GameObject hitCollectiblePrefab;
	}

	[Serializable]
	public class SoundCollection
	{
		public Sound forwardMovement;
		public Sound sidewaysMovement;
		public Sound endFanfare;
		[Header("On game start:")]
		public Sound[] startVoice;
		[Header("End screen:")]
		public Sound[] endVoice;
		[Header("On colliding with obstacles:")]
		public Sound[] collisionVoice;
		[Header("On collecting an item:")]
		public Sound[] collectVoice;
	}

	[CreateAssetMenu(fileName = "Game Package", menuName = "New Game Package")]
	public class GamePackage : ScriptableObject
	{
		[Header("Background sprite")]
		public Sprite backgroundCard;

		[Header("The scrolling texture for the road")]
		public EnvironmentTexture roadTexture = new EnvironmentTexture(new Vector2(1f, 15f));

		[Header("Material used in the ground")]
		public GroundMaterial groundMaterial;

		[Header("The goal line spawned when items collected")]
		public Sprite finishLine;

		[Header("The things to be collected by Character")]
		public List<RoadItemSprite> collectibleSprites;

		[Header("Obstacles that stop the Character")]
		public List<RoadItemSprite> obstacleSprites;

		[Header("Textures that appear outside the road")]
		public List<ItemSprite> decorationSprites;

		[Header("Items that appear from the collectibles")]
		public List<ItemSprite> itemSprites;

		[Header("Prefabs for effects")]
		public CollisionEffects collisionEffects;
		public List<GameObject> weatherEffects;
		public GameObject collectibleAccentEffect;

		[Header("Sounds (collision/collection effects are set from items):")]
		public SoundCollection soundCollection;

		private void OnValidate()
		{
			// Set the sound Types that define the audio source used for each sound automatically.
			SetSoundTypes();
		}

		private void SetSoundTypes()
		{
			foreach (var sound in soundCollection.collectVoice) { sound.Type = SoundType.Voice; }
			foreach (var sound in soundCollection.collisionVoice) { sound.Type = SoundType.Voice; }
			foreach (var sound in soundCollection.endVoice) { sound.Type = SoundType.Voice; }
			foreach (var sound in soundCollection.startVoice) { sound.Type = SoundType.Voice; }
			foreach (var item in collectibleSprites) { item.collisionSound.Type = SoundType.Effect; }
			foreach (var item in obstacleSprites) { item.collisionSound.Type = SoundType.Effect; }

			soundCollection.forwardMovement.Type = SoundType.Moving;
			soundCollection.sidewaysMovement.Type = SoundType.Moving;
			soundCollection.endFanfare.Type = SoundType.Effect;
		}
	}
}
