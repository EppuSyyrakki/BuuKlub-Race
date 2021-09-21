using System.Collections;
using System.Collections.Generic;
using BK.Items;
using UnityEngine;

namespace BK
{
	[System.Serializable]
	public class RoadTexture
	{
		public Texture texture;
		[Range(1, 20)]
		public int repeatY;
	}

	[System.Serializable]
	public class GroundMaterial
	{
		public Color color = Color.white;

		[Range(0, 1)]
		public float smoothness, metallic;
	}

	[CreateAssetMenu(fileName = "Graphics Package", menuName = "New Graphics Package")]
	public class GraphicsPackage : ScriptableObject
	{
		[Header("Data of ground outside the road")]
		public GroundMaterial groundMaterial;

		[Header("The scrolling texture for the road")]
		public RoadTexture roadTexture;
		
		[Header("The things to be collected by Character")]
		public Sprite[] collectibleSprites;

		[Header("Obstacles that slow down the Character")]
		public Sprite[] obstacleSprites;

		[Header("Textures that appear outside the road")]
		public Sprite[] decorationSprites;

		[Header("Items that appear from the collectibles")]
		public Sprite[] itemSprites;
	}
}
