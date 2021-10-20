﻿using System;
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
		public SoundType associatedSound;
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
	
	public enum SoundType
	{
		MoveForward,
		MoveSideways,
		PickUp1,
		PickUp2,
		PickUp3,
		Collide1,
		Collide2,
		Collide3,
		WinChime
	}

	[Serializable]
	public class Sound
	{
		public SoundType type;
		public AudioClip clip;
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
		public List<ItemSprite> collectibleSprites;

		[Header("Obstacles that slow down the Character")]
		public List<ItemSprite> obstacleSprites;

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

		[Header("Sounds - make sure no type duplicates exist")]
		[Range(0, 1f)]
		public float audioVolume = 1f;
		public List<Sound> sounds;
	}
}
