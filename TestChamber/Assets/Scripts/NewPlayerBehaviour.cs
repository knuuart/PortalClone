﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerBehaviour : MonoBehaviour {

    public float groundPower = 20f, airPower = 3f;
    public float moveMax;
    public float mouseSensitivity;
    private float mouseSensitivityY;
    public bool mInvert;
    public float defaultRotation = 0f, rotationSpeed = 0.1f;
    public float groundSpeedLimit;
    public float airSpeedLimit;
    public bool grounded, jumped;
    public Camera cam;
    Rigidbody rb;
    float movePower;
    public float sphereRadius, sphereDistance, jumpForce;
	public Animator anim;
	public GameObject playerGraphics;
    escapeMenu escMenu;
    public AudioClip[] jumps;

    private Vector3 playerpos;

    // Use this for initialization
    void Awake() {
        rb = GetComponent<Rigidbody>();
        playerpos = transform.position;
        escMenu = GameObject.Find("Escape Menu Handler").GetComponent<escapeMenu>();
    }
    void Update() {

        //mouse invert stuff

        if (escMenu.mouseInvert.isOn) {
            mInvert = true;
        } else { mInvert = false; }

        //updating mouse sensitivity according to options
        if (!mInvert) {
            mouseSensitivityY = mouseSensitivity;
        } else { mouseSensitivityY = mouseSensitivity * -1; }
        

		 


        //Uprighting the camera
        var angles = cam.transform.localEulerAngles;
        angles.z = 0f;
        cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, Quaternion.Euler(angles), rotationSpeed * Time.deltaTime);

        if (!escMenu.menusOpen) {

            if (Vector3.Angle(cam.transform.forward, Vector3.up) < .5f) {
                cam.transform.Rotate(cam.transform.right, .5f, Space.World);
            }

            if (Vector3.Angle(cam.transform.forward, Vector3.down) < .5f) {
                cam.transform.Rotate(cam.transform.right, -.5f, Space.World);
            }

            var RotX = Input.GetAxis("Mouse X") * mouseSensitivity;
            var RotY = Input.GetAxis("Mouse Y") * mouseSensitivityY;



            // RotY = Mathf.Clamp (RotY, minY, maxY);
            var qx = Quaternion.AngleAxis(RotX, Vector3.up);
            cam.transform.rotation = qx * cam.transform.rotation;

            var qy = Quaternion.AngleAxis(RotY, Vector3.ProjectOnPlane(-cam.transform.right, Vector3.up));
            var xzFwd = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up);
            var xzFwdAfter = Vector3.ProjectOnPlane(qy * cam.transform.forward, Vector3.up);
            // would be <90, but Vector3.Angle seems to return 90 for zero vectors,
            // this way no need to handle the vertical corner case where projections approach zero
            if (Vector3.Angle(xzFwd, xzFwdAfter) < 90f && xzFwdAfter.magnitude > 0.001f) {
                cam.transform.rotation = qy * cam.transform.rotation;
            }
            playerGraphics.transform.eulerAngles = new Vector3(0, cam.transform.eulerAngles.y, 0);
            if (grounded) {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
					anim.SetTrigger ("Hyppää");
                    jumped = true;
                    AudioSource.PlayClipAtPoint(jumps[Random.Range(0, jumps.Length)], transform.position, 0.5f);
                }
            }
            
        }
        
    }
    void FixedUpdate() {
        if (!escMenu.menusOpen) {
            //Getting movement inputs
            float MoveX = Input.GetAxis("Horizontal");
            float MoveZ = Input.GetAxis("Vertical");

            if (grounded) {
                movePower = groundPower;
            } else {
                movePower = airPower;
            }
			if (grounded) {
				if (MoveX != 0 || MoveZ != 0) {
					anim.SetBool ("Juoksee", true);
				} else {
					anim.SetBool ("Juoksee", false);
				}
			} else {
				anim.SetBool ("Juoksee", false);
			}


            //Movement
            var XZForward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up);
            var XZRight = Vector3.ProjectOnPlane(cam.transform.right, Vector3.up);

            var aa = (XZRight * MoveX + XZForward * MoveZ).normalized * movePower * Time.deltaTime;

            var newVelocity = rb.velocity + aa;

            newVelocity = Vector3.ClampMagnitude(newVelocity, grounded ? groundSpeedLimit : airSpeedLimit);

            rb.velocity = newVelocity;
        }
    }
    void OnDrawGizmos() {
        //Drawing a gizmo representing the groundcheck
        Gizmos.DrawSphere(transform.position - transform.up * sphereDistance, sphereRadius);
    }

    private void OnCollisionStay(Collision collision) {
        //grounded requires the collider and the ray to be touching something
        //no more jumping on portals
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, sphereRadius, Vector3.down, out hit, sphereDistance) && (rb.velocity.y > -5 && rb.velocity.y < 5)) {
            grounded = true;
        } else {
            grounded = false;
        }
    }
    private void OnCollisionEnter(Collision collision) {
        if (jumped) {
            if (collision.gameObject.transform.forward == Vector3.down) {
                AudioSource.PlayClipAtPoint(jumps[Random.Range(1, 2)], transform.position, 0.5f);
                jumped = false;
            }
        }
        
        
    }
    private void OnCollisionExit(Collision collision) {

        grounded = false;
    }

    public void MouseInvert () {

            mouseSensitivityY *= -1;

    }
}
