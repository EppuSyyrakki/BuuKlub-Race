using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BKRacing.Environment
{
	public class Road : MonoBehaviour
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
			var et = Game.Instance.RoadTexture;
			_material.mainTexture = et.texture;
			_material.mainTextureScale = et.tiling;
		}

		private void Update()
		{
			_material.mainTextureScale = Game.Instance.RoadTexture.tiling;
			var speed = Game.Instance.ScrollSpeed;
			Vector2 offset = new Vector2(0, Game.Instance.forwardSpeed * speed * Time.deltaTime);
			_material.mainTextureOffset += offset;
		}
	}
}
