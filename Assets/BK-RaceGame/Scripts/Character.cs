using System;
using System.Collections;
using System.Collections.Generic;
using BKRacing.Environment;
using BKRacing.Items;
using UnityEngine;

namespace BKRacing
{
	[RequireComponent(typeof(Billboard))]
    public class Character : MonoBehaviour
    {
	    private enum Direction
	    {
			Left,
			Right
	    }

		[SerializeField, Range(0, 0.1f)]
	    private float moveTreshold = 0.0075f;

	    private float _moveTreshold;
	    private Camera _cam;
	    private Animator _animator;
	    private ParticleSystem _particles;
	    private bool _crashProtection = false;

		// Delegate for triggering sounds. AudioPlayer hooks itself up to this.
		public Action<AudioClip, SoundType> triggerSound;

		public ParticleSystem Particles => _particles;
	    public bool Protected => _crashProtection;

	    private void Awake()
	    {
			GetComponent<Billboard>().SetAsItem();
		    _moveTreshold = Screen.width * moveTreshold;
			_cam = Camera.main;
			_animator = GetComponent<Animator>();
			_particles = GetComponentInChildren<ParticleSystem>();
	    }

	    private void Start()
	    {

			// triggerSound();
	    }

	    private void Update()
	    {
		    if (Input.touchCount == 0 || !Game.Instance.ControlEnabled)
		    {
				_animator.SetInteger("movement", 0);
				return;
		    }

		    Touch touch = Input.GetTouch(0);
		    var screenPosX = _cam.WorldToScreenPoint(transform.position).x;

			// if touch is very close to character, don't move
			if (touch.position.x > screenPosX - _moveTreshold && touch.position.x < screenPosX + _moveTreshold)
			{
				_animator.SetInteger("movement", 0);
				return;
			}
			
			Move(touch.position.x < screenPosX ? Direction.Left : Direction.Right);
	    }

	    private void OnTriggerEnter(Collider other)
	    {
		    //var item = other.GetComponent<Item>();

		    //if (item != null)
		    //{

		    //}
	    }

		private void Move(Direction direction)
	    {
		    var amount = direction == Direction.Left ? -1 : 1;
			Vector3 pos = transform.position;
			float limit = Game.Instance.RoadWidth - 1;
			var step = Game.Instance.horizontalSpeed * amount * Time.deltaTime;
			var newX = Mathf.Clamp(pos.x + step, -limit, limit);
			transform.position = new Vector3(newX, pos.y, pos.z);

			if (Mathf.Approximately(newX, pos.x))
			{
				amount = 0;
			}

			_animator.SetInteger("movement", amount);
		}

		private void EnableCrashing()
        {
	        _crashProtection = false;
        }

		public void Crash()
		{
			_crashProtection = true;
			Invoke(nameof(EnableCrashing), Game.Instance.crashProtectionTime);
		}

		public void DisableCrashing()
		{
			_crashProtection = true;
		}
	}
}
