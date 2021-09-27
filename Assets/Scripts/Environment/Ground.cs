using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BKRacing.Environment
{
	public class Ground : MonoBehaviour
	{
		private Material _material;
		private GroundMaterial _groundMaterial;

		private void Awake()
		{
			var mr = GetComponent<MeshRenderer>();
			var m = mr.sharedMaterial;
			_material = new Material(m) { mainTextureOffset = Vector2.zero };
			mr.material = _material;
		}

		private void Start()
		{
			_groundMaterial = Game.Instance.GroundMaterial;
			SetMaterialProperties();
		}

		private void Update()
		{
			#if UNITY_EDITOR
			SetMaterialProperties();
			#endif
		}

		private void SetMaterialProperties()
		{
			_material.color = _groundMaterial.color;
			_material.SetFloat("_Glossiness", _groundMaterial.smoothness);
			_material.SetFloat("_Metallic", _groundMaterial.metallic);
		}
	}
}
