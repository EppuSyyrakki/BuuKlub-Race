using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BK
{
	public class Road : MonoBehaviour
	{
		private Material _material = null;
		private float _repeatY;

		private void Awake()
		{
			_material = GetComponent<MeshRenderer>().sharedMaterial;
			_material.mainTextureOffset = Vector2.zero;
		}

		private void Start()
		{
			var rt = Game.Instance.RoadTexture;
			_repeatY = rt.repeatY;
			_material.mainTexture = rt.texture;
			_material.mainTextureScale = new Vector2(1, _repeatY);
		}

		private void Update()
		{
			Vector2 offset = new Vector2(0, Game.Instance.forwardSpeed * _repeatY * 0.00048f * Time.deltaTime);
			_material.mainTextureOffset += offset;
		}
	}
}
