using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// A simple free camera to be added to a Unity game object.
/// 
/// Keys:
///	wasd / arrows	- movement
///	q/e 			- up/down (local space)
///	pageup/pagedown	- up/down (world space)
///	hold shift		- enable fast movement mode
///	right mouse  	- enable free look
///	mouse			- free look / rotation
///     
/// </summary>
public class FreeCam : MonoBehaviour
{
    [HideInInspector]
    public bool isPaused = false;
    private bool looking;
    private bool moving = false;

    void Update()
    {
        if (isPaused) { return; }

        float movementMultiplier = 5 * GlobalVariables.mouseSensitivity;
        float movementDecreaser = 0.05f;

        if (Input.GetKey(KeyCode.A))
        {
            transform.position = transform.position + (-transform.right * Time.deltaTime * movementMultiplier);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.position = transform.position + (transform.right * Time.deltaTime * movementMultiplier);
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.position = transform.position + (transform.forward * Time.deltaTime * movementMultiplier);
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.position = transform.position + (-transform.forward * Time.deltaTime * movementMultiplier);
        }

        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.PageUp))
        {
            transform.position = transform.position + (transform.up * Time.deltaTime * movementMultiplier * 0.25f);
        }

        if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.PageDown))
        {
            transform.position = transform.position + (-transform.up * Time.deltaTime * movementMultiplier * 0.25f);
        }

        if (Input.GetKey(KeyCode.R))
        {
            transform.position = new Vector3(0, 1, 0);
            transform.rotation = new Quaternion(0, 0, 0, 0);
        }

        if (looking)
        {
            float rotationMultiplier = 5 * GlobalVariables.mouseSensitivity;
            float newRotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * rotationMultiplier;
            float newRotationY = transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * rotationMultiplier;
            transform.localEulerAngles = new Vector3(newRotationY, newRotationX, 0f);
        }

        float axis = Input.GetAxis("Mouse ScrollWheel");
        if (axis != 0)
        {
            transform.position = transform.position + transform.forward * axis * GlobalVariables.mouseSensitivity;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            StartLooking();
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            StopLooking();
        }

        if (Input.GetMouseButton(2))
        {
            MouseMiddleButtonClicked();
        }
        else if (Input.GetMouseButtonUp(2))
        {
            ShowAndUnlockCursor();
        }

        void MouseMiddleButtonClicked()
        {
            HideAndLockCursor();
            Vector3 NewPosition = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
            Vector3 pos = transform.position;
            if (NewPosition.x < 0.0f)
            {
                pos += transform.right * movementDecreaser;
            }
            if (NewPosition.x > 0.0f)
            {
                pos -= transform.right * movementDecreaser;
            }
            if (NewPosition.y < 0.0f)
            {
                pos += transform.up * movementDecreaser * 0.5f;
            }
            if (NewPosition.y > 0.0f)
            {
                pos -= transform.up * movementDecreaser * 0.5f;
            }
            //pos.y = transform.position.y;
            transform.position = pos;
        }
    }

    void OnDisable()
    {
        StopLooking();
        StopMoving();
    }

    /// <summary>
    /// Enable free looking.
    /// </summary>
    public void StartLooking()
    {
        looking = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void StartMoving()
    {
        //moving = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Disable free looking.
    /// </summary>
    public void StopLooking()
    {
        looking = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void StopMoving()
    {
        //moving = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void ShowAndUnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    void HideAndLockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}