using System;
using System.Collections;
using System.Collections.Generic;
using BKRacing.Environment;
using BKRacing.Items;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

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
	    private SoundCollection _soundCollection;
	    
		// Delegate for triggering sounds. AudioPlayer hooks itself up to this.
		public Action<Sound> triggerSound;

		public ParticleSystem Particles => _particles;
	    public bool Protected => _crashProtection;
	    public Animator Animator => _animator;

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
		    _soundCollection = Game.Instance.SoundCollection;
	    }

	    private void Update()
	    {
		    if (Input.touchCount == 0 || !Game.Instance.ControlEnabled)
		    {
			    triggerSound(_soundCollection.forwardMovement);
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

			triggerSound(_soundCollection.sidewaysMovement);
			Move(touch.position.x < screenPosX ? Direction.Left : Direction.Right);
	    }

	    private void OnTriggerEnter(Collider other)
	    {
			var item = other.GetComponent<Item>();

			if (item == null || item.Sound == null) { return; }

			triggerSound(item.Sound);

			if (item is Obstacle && !_crashProtection)
			{
				triggerSound(GetRandomSound(_soundCollection.collisionVoice));
			}
			else if (item is Collectible)
			{
				triggerSound(GetRandomSound(_soundCollection.collectVoice));
			}
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

		public void StartMoving()
		{
			triggerSound(GetRandomSound(_soundCollection.startVoice));
			_animator.SetTrigger("collect");
		}

		private Sound GetRandomSound(Sound[] sounds)
		{
			return sounds[Random.Range(0, sounds.Length)];
		}
    }
}
