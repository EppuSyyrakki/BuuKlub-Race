using System;
using System.Collections;
using UnityEngine;

namespace BKRacing.Items
{
	public abstract class Item : MonoBehaviour
	{
		protected Transform player;
		protected SpriteRenderer spriteRenderer;
		protected Vector3 newPos;
		private float _objectHeight;
		private AnimationCurve _curve;
		
		public virtual Sprite Sprite => spriteRenderer.sprite;
		public bool Mirror { get; private set; }

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

		public virtual void Init(Sprite sprite, bool mirror)
		{
			gameObject.SetActive(false);
			var sr = GetComponent<SpriteRenderer>();
			sr.sprite = sprite;
			Mirror = mirror;
		}
	}
}