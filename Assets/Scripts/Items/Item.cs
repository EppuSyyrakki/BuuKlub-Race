using System;
using System.Collections;
using UnityEngine;
using BKRacing.Environment;

namespace BKRacing.Items
{
	[RequireComponent(typeof(Billboard),
		typeof(SpriteRenderer))]
	public class Item : MonoBehaviour
	{
		protected Transform player;
		protected SpriteRenderer spriteRenderer;
		protected Vector3 newPos;
		private float _objectHeight;
		private AnimationCurve _curve;

		private void Start()
		{
			player = Game.Instance.Player.transform;
			spriteRenderer = GetComponent<SpriteRenderer>();
			var sprite = spriteRenderer.sprite;
			_objectHeight = sprite.rect.height / sprite.pixelsPerUnit;
			_curve = Game.Instance.riseCurve;
		}

		private float GetYPosition(float z)
		{
			var distance = Mathf.Abs(transform.localPosition.z);
			var curveDistance = Game.Instance.curveDistance;

			if (distance > curveDistance)
			{
				return 0;
			}

			return -_objectHeight + _objectHeight * _curve.Evaluate(distance / curveDistance);
		}

		public virtual void Update()
		{
			var self = transform.position;
			var z = self.z - Game.Instance.forwardSpeed * Time.deltaTime * 0.12f;
			var y = GetYPosition(z);
			newPos = new Vector3(self.x, y, z);
		}

		public void Init(Sprite sprite)
		{
			var sr = GetComponent<SpriteRenderer>();
			sr.sprite = sprite;
		}
	}
}