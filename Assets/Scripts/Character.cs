using System;
using System.Collections;
using System.Collections.Generic;
using BK.Items;
using UnityEngine;

namespace BK
{
    public class Character : MonoBehaviour
    {
	    private Camera _cam;
	    private Animator _animator;

	    private void Awake()
	    {
			_cam = Camera.main;
			_animator = GetComponent<Animator>();
	    }

	    private void Update()
	    {
		    if (Input.touchCount == 0)
		    {
				_animator.SetInteger("movement", 0);
			    return;
		    }

		    Touch touch = Input.GetTouch(0);
		    float tX = touch.position.x;
		    float cX = _cam.WorldToScreenPoint(transform.position).x;

		    if (Mathf.Abs(tX - cX) < Game.Instance.moveTreshold)
		    {
			    return;
		    }

		    if (tX < cX) { MoveLeft(); }
		    else if (tX > cX) { MoveRight(); }
	    }

	    public void MoveLeft()
        {
            _animator.SetInteger("movement", -1);
	        Move(-Game.Instance.horizontalSpeed);
        }

        public void MoveRight()
        {
			_animator.SetInteger("movement", 1);
			Move(Game.Instance.horizontalSpeed);
        }

        private void Move(float step)
        {
	        Vector3 pos = transform.position;
	        float limit = Game.Instance.roadWidth - 1;
	        var newX = Mathf.Clamp(pos.x + step * Time.deltaTime, -limit, limit);
	        transform.position = new Vector3(newX, pos.y, pos.z);
        }
    }
}
