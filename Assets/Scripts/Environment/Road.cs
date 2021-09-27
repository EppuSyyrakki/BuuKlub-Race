using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BKRacing.Environment
{
	public class Road : MonoBehaviour
	{
		private Material _material = null;
		private float _repeatY;

		private void Awake()
		{
			var r = GetComponent<MeshRenderer>();
			var m = r.sharedMaterial;
			_material = new Material(m) {mainTextureOffset = Vector2.zero};
			r.material = _material;
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
