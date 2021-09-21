using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Billboard : MonoBehaviour
{
	private Camera _cam;

	private void Awake()
	{
		GetComponent<SpriteRenderer>().flipX = true;
		_cam = Camera.main;
	}

	// Update is called once per frame
    void LateUpdate()
    {
	    var look = -_cam.transform.forward * 100f;
	    transform.LookAt(look);
    }
}
