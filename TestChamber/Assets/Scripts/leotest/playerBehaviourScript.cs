﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerBehaviourScript : MonoBehaviour {

    public float movePower;
    //public float moveMax;
    public float mouseSensitivity;
    private float mouseSensitivityY;
    public bool mInvert;
    public float maxY = 60f;
    public float minY = -60f;
    //public float maxX = Mathf.Infinity;
    //public float minX = Mathf.Infinity;


    public float RotX;
    public float RotY;

    public Camera cam;
    Rigidbody rb;

	
	void Start () {
        rb = GetComponent<Rigidbody>();

        mouseSensitivityY = mouseSensitivity;
        if (mInvert) {
            mouseSensitivityY = mouseSensitivityY * -1;
        }
	}
	
	
	void Update () {
		
	}

    private void FixedUpdate() {

        if (mInvert) { mouseSensitivityY = mouseSensitivityY * -1; }

        float MoveX = Input.GetAxis("Horizontal");
        float MoveZ = Input.GetAxis("Vertical");

        RotX += Input.GetAxis("Mouse X") * mouseSensitivity;

        RotY += Input.GetAxis("Mouse Y") * mouseSensitivityY;
        RotY = Mathf.Clamp(RotY, minY, maxY);

        Vector3 oldT = cam.transform.localEulerAngles;
        cam.transform.localEulerAngles = new Vector3(-RotY, oldT.y);

        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 
                                                 RotX, 
                                                 transform.localEulerAngles.z);

        var aa = (transform.right * MoveX +transform.forward * MoveZ).normalized * movePower * Time.deltaTime;

        rb.velocity = new Vector3(aa.x, rb.velocity.y, aa.z);
    }
}