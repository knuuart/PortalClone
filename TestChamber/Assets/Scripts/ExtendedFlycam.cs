﻿using UnityEngine;

public class ExtendedFlycam : MonoBehaviour
{
    /*
	EXTENDED FLYCAM (http://wiki.unity3d.com/index.php/FlyCam_Extended)
		Desi Quintans (CowfaceGames.com), 17 August 2012.
		Based on FlyThrough.js by Slin (http://wiki.unity3d.com/index.php/FlyThrough), 17 May 2011.
 
	LICENSE
		Free as in speech, and free as in beer.
 
	FEATURES
		WASD/Arrows:    Movement
		          E:    Climb
		          Q:    Drop
                      Shift:    Move faster
                    Control:    Move slower
                        End:    Toggle cursor locking to screen (you can also press Ctrl+P to toggle play mode on and off).
	*/

    public float cameraSensitivity = 90;
    public float climbSpeed = 4;
    public float normalMoveSpeed = 10;
    public float slowMoveFactor = 0.25f;
    public float fastMoveFactor = 3;
	public Transform cameraOffset, cameraTransform;

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    void Start()
    {
        rotationX = transform.rotation.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
        rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
        rotationY = Mathf.Clamp(rotationY, -90, 90);

        cameraTransform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
		cameraTransform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
			transform.position += cameraTransform.forward * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
			transform.position += cameraTransform.right * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            transform.position += cameraTransform.forward * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += cameraTransform.right * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
        }
        else
        {
			transform.position += cameraTransform.forward * normalMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
			transform.position += cameraTransform.right * normalMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
        }


		if (Input.GetKey(KeyCode.E)) { transform.position += cameraTransform.up * climbSpeed * Time.deltaTime; }
		if (Input.GetKey(KeyCode.Q)) { transform.position -= cameraTransform.up * climbSpeed * Time.deltaTime; }

        if (Input.GetKeyDown(KeyCode.End))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}