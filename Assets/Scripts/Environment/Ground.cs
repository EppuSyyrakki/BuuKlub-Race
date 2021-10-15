using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BKRacing.Environment
{
	public class Ground : MonoBehaviour
	{
		private Material _material = null;

		private void Awake()
		{
			var r = GetComponent<MeshRenderer>();
			var m = r.sharedMaterial;
			_material = new Material(m) { mainTextureOffset = Vector2.zero };
			r.material = _material;
		}

		private void Start()
		{
			var et = Game.Instance.GroundTexture;
			_material.mainTexture = et.texture;
			_material.mainTextureScale = et.tiling;
		}

		private void Update()
		{
			var tiling = Game.Instance.GroundTexture.tiling;
			Vector2 offset = new Vector2(0, Game.Instance.forwardSpeed * tiling.y * 0.00048f * Time.deltaTime);
			_material.mainTextureOffset += offset;
		}
	}
}
