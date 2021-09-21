using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
	private Camera _cam;

	private void Awake()
	{
		_cam = Camera.main;
	}

	// Update is called once per frame
    void LateUpdate()
    {
	    var look = -_cam.transform.forward * 100f;
	    transform.LookAt(look);
    }
}
