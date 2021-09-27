using System;
using System.Collections;
using System.Collections.Generic;
using BKRacing.Items;
using UnityEngine;

namespace BKRacing
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
		    float touchDelta = Mathf.Abs(tX - cX);

			if (touchDelta < Game.Instance.moveTreshold)
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

        private void Move(float step)
        {
	        Vector3 pos = transform.position;
	        float limit = Game.Instance.roadWidth - 1;
	        var newX = Mathf.Clamp(pos.x + step * Time.deltaTime, -limit, limit);
	        transform.position = new Vector3(newX, pos.y, pos.z);
        }
    }
}
