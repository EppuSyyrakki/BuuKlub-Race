using System.Collections;
using System.Collections.Generic;
using BK.Items;
using UnityEngine;

namespace BK
{
	public class Game : MonoBehaviour
	{
		private static Game _instance;

		public static Game Instance => _instance;

		private void Awake()
		{
			_instance = this;
		}

		[Header("Movement variables:")]
		public float roadWidth = 6f;
		public float forwardSpeed = 5f;
		public float horizontalSpeed = 2f;

		[Header("Spawner variables:")]
		public Item[] collectibles = null;
		public Item[] obstacles = null;
		[Range(0, 1), Tooltip("0 = only collectibles, 1 = only obstacles")]
		public float bias = 0.5f;
		[Range(2, 4)]
		public float minTime = 2f;
		[Range(4, 6)]
		public float maxTime = 4f;
	}
}
