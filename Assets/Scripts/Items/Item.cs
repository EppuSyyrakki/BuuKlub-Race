using System;
using System.Collections;
using UnityEngine;

namespace BK.Items
{
	[RequireComponent(typeof(Billboard),
		typeof(SpriteRenderer))]
	public class Item : MonoBehaviour
	{
		private Transform _player;
		private SpriteRenderer _sr;

		private void Start()
		{
			_player = Game.Instance.Player.transform;
			_sr = GetComponent<SpriteRenderer>();
		}

		private void Update()
		{
			var self = transform.position;
			var newPos = new Vector3(self.x, self.y , self.z - Game.Instance.forwardSpeed * Time.deltaTime * 0.12f);

			if (newPos.z < -10f)
			{
				Destroy(gameObject);
				return;
			}

			if (newPos.z < _player.position.z)
			{
				_sr.sortingOrder += 100;
			}
			
			transform.position = newPos;
		}

		public void Init(Sprite sprite)
		{
			var sr = GetComponent<SpriteRenderer>();
			sr.sprite = sprite;
		}
	}
}