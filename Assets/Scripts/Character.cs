﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BKRacing
{
    public class Character : MonoBehaviour
    {
	    [SerializeField]
	    private float moveTreshold = 20f;

	    private Camera _cam;
	    private Animator _animator;
	    private ParticleSystem _particles;
	    private bool _crashProtection = false;
	    private int _maxHealth;
	    private int _currentHealth;
	    
	    public ParticleSystem Particles => _particles;
	    public bool Protected => _crashProtection;
	    public event Action<int> HealthChanged;
		
	    private void Awake()
	    {
			_cam = Camera.main;
			_animator = GetComponent<Animator>();
			_particles = GetComponentInChildren<ParticleSystem>();
	    }

	    private void Start()
	    {
		    _maxHealth = Game.Instance.characterMaxHealth;
		    _currentHealth = _maxHealth;
	    }

	    private void Update()
	    {
		    if (Input.touchCount == 0 || !Game.Instance.ControlEnabled)
		    {
				_animator.SetInteger("movement", 0);
			    return;
		    }

		    Touch touch = Input.GetTouch(0);
		    float tX = touch.position.x;
		    float cX = _cam.WorldToScreenPoint(transform.position).x;
		    float touchDelta = Mathf.Abs(tX - cX);

			if (touchDelta < moveTreshold)
			{
				return;
			}

			touchDelta = Mathf.Clamp(touchDelta, 100f, 1000f);
		    if (tX < cX) { MoveLeft(touchDelta); }
		    else if (tX > cX) { MoveRight(touchDelta); }
	    }

	    public void MoveLeft(float touchDelta)
        {
            _animator.SetInteger("movement", -1);
            Move(-Game.Instance.horizontalSpeed * touchDelta / Screen.width * 5f);
        }

        public void MoveRight(float touchDelta)
        {
			_animator.SetInteger("movement", 1);
			Move(Game.Instance.horizontalSpeed * touchDelta / Screen.width * 5f);
        }

        public void Crash()
        {
	        _crashProtection = true;
			ChangeHealth(-1);
	        Invoke(nameof(EnableCrashing), Game.Instance.crashProtectionTime);
        }

        private void Move(float step)
        {
	        Vector3 pos = transform.position;
	        float limit = Game.Instance.RoadWidth - 1;
	        var newX = Mathf.Clamp(pos.x + step * Time.deltaTime, -limit, limit);
	        transform.position = new Vector3(newX, pos.y, pos.z);
        }

		private void EnableCrashing()
        {
	        _crashProtection = false;
        }

		private void ChangeHealth(int amount)
		{
			_currentHealth += amount;
			HealthChanged?.Invoke(_currentHealth);
		}
    }
}
