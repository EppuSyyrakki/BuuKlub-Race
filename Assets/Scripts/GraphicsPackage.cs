using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BKRacing
{
	[System.Serializable]
	public class RoadTexture
	{
		public Texture texture;
		[Range(1, 40)]
		public int repeatY = 1;
	}

	[Serializable]
	public class ItemSprite
	{
		public Sprite sprite;
	}

	[Serializable]
	public class ObstacleSprite : ItemSprite
	{
		[Tooltip("If chosen, will stop player on contact. Else just slow down.")]
		public bool stopsPlayer;
	}

	[Serializable]
	public class DecorationSprite : ItemSprite
	{
		[Tooltip("Bigger number is more probable.")]
		public int probability;
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
	
	[CreateAssetMenu(fileName = "Graphics Package", menuName = "New Graphics Package")]
	public class GraphicsPackage : ScriptableObject
	{
		[Header("Background sprite")]
		public Sprite backgroundCard;

		[Header("Data of ground outside the road")]
		public GroundMaterial groundMaterial;

		[Header("The scrolling texture for the road")]
		public RoadTexture roadTexture;
		
		[Header("The things to be collected by Character")]
		public List<ItemSprite> collectibleSprites;

		[Header("Obstacles that slow down the Character")]
		public List<ObstacleSprite> obstacleSprites;

		[Header("Textures that appear outside the road")]
		public List<DecorationSprite> decorationSprites;

		[Header("Items that appear from the collectibles")]
		public List<ItemSprite> itemSprites;

		[Header("Prefabs for effects when colliding with items")]
		public CollisionEffects collisionEffects;
	}
}
