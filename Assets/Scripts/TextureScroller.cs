using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BK
{
	public class TextureScroller : MonoBehaviour
	{
		private Material _material = null;

		private void Awake()
		{
			_material = GetComponent<MeshRenderer>().sharedMaterial;
			_material.mainTextureOffset = Vector2.zero;
		}

		private void Update()
		{
			Vector2 offset = new Vector2(0, Game.Instance.forwardSpeed * 0.01f * Time.deltaTime);
			_material.mainTextureOffset += offset;
		}
	}
}
