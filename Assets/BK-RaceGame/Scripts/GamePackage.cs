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
		public Sound[] collisionSounds;
	}

	[Serializable]
	public class Sound
	{
		public AudioClip clip;
		public SoundType type;

		public Sound(SoundType type)
		{
			this.type = type;
		}
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
		public Sound forwardMovement = new Sound(SoundType.Moving);
		public Sound sidewaysMovement = new Sound(SoundType.Moving);
		public Sound endFanfare = new Sound(SoundType.Effect);
		[Header("Voice alternatives to play in the end screen (set type to Voice):")]
		public Sound[] endVoice;
		[Header("Voice alternatives to play when colliding (set type to Voice):")]
		public Sound[] collisionVoice;
		[Header("Voice alternatives to play when collecting (set type to Voice):")]
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

		[Header("The things to be collected by Character")]
		public GameObject collectibleAccentEffect;
		public List<RoadItemSprite> collectibleSprites;

		[Header("Obstacles that slow down the Character")]
		public List<RoadItemSprite> obstacleSprites;

		[Header("Textures that appear outside the road")]
		public List<ItemSprite> decorationSprites;

		[Header("Items that appear from the collectibles")]
		public Color uncollectedColor;
		[Range(0.05f, 0.3f), Tooltip("Size of the image as fraction of screen width")]
		public float itemSize = 0.125f;
		public List<ItemSprite> itemSprites;

		[Header("Prefabs for effects")]
		public CollisionEffects collisionEffects;
		public List<GameObject> weatherEffects;

		[Header("Sounds:")]
		public SoundCollection soundCollection;
	}
}
