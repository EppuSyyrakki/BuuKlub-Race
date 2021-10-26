using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BKRacing.Items
{
	public abstract class Item : MonoBehaviour
	{
		protected Transform player;
		protected SpriteRenderer spriteRenderer;
		protected Vector3 newPos;
		protected bool discarded = false;
		private float _objectHeight;
		private AnimationCurve _curve;
		
		public virtual Sprite Sprite => spriteRenderer.sprite;
		public bool Mirror { get; private set; }
		public Sound Sound { get; private set; }

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

		public virtual void Init(Sprite sprite, bool mirror, Sound sound)
		{
			gameObject.SetActive(false);
			var sr = GetComponent<SpriteRenderer>();
			sr.sprite = sprite;
			Mirror = mirror;
			Sound = sound;
		}

		public void SetSound(Sound sound)
		{
			Sound = sound;
		}

		public void DiscardItem()
		{
			discarded = true;
			GetComponent<SphereCollider>().enabled = false;
			StartCoroutine(Launch(Game.Instance.loseItemCurve));
		}

		private IEnumerator Launch(AnimationCurve curve)
		{
			float flyTime = Game.Instance.flyTime;
			float time = 0;
			var height = Game.Instance.launchHeight;
			Vector3 origin = transform.position;
			Vector3 target = GetLaunchTarget();

			while (time < flyTime)
			{
				var t = time / flyTime;
				transform.position = new Vector3(
					Mathf.Lerp(origin.x, target.x, t),
					origin.y + curve.Evaluate(t) * height,
					origin.z);
				time += Time.deltaTime;

				yield return new WaitForEndOfFrame();
			}

			Destroy(gameObject);
		}

		private Vector3 GetLaunchTarget()
		{
			Vector3 target = transform.position;
			float multiplier = target.x < 0 ? -1 : 1;
			target.x = Random.Range(10f, 30f) * multiplier;
			return target;
		}
	}
}