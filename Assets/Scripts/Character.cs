using System;
using System.Collections;
using System.Collections.Generic;
using BK.Items;
using UnityEngine;

namespace BK
{
    public class Character : MonoBehaviour
    {
	    public void MoveLeft()
        {
	        Move(-Game.Instance.horizontalSpeed);
        }

        public void MoveRight()
        {
	        Move(-Game.Instance.horizontalSpeed);
        }

        private void Move(float step)
        {
	        Vector3 pos = transform.position;
	        float limit = -Game.Instance.roadWidth / 2;
	        var newX = Mathf.Clamp(pos.x + step, -limit, limit);
	        transform.position = new Vector3(newX, pos.y, pos.z);
        }

        private void OnTriggerEnter(Collider other)
        {
	        var item = other.gameObject.GetComponent<Item>();

	        if (item is Collectible)
	        {
				Debug.Log("Touched Collectible!");
	        }
			else if (item is Obstacle)
	        {
				Debug.Log("Touched Obstacle!");
	        }
        }
    }
}
