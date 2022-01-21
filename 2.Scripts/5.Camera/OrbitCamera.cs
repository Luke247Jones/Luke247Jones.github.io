using UnityEngine;
using System.Collections;

public class OrbitCamera : MonoBehaviour
{

    [SerializeField] GameObject target;

    [Header("Zoom")]
    [SerializeField] public float minDistance = 2f;
    [SerializeField] float maxDistance = 5f;
    private bool isZooming = false;

    void Update()
    {
        CameraControl();
    }

    void CameraControl()
    {
        // Set camera to fixed position
        if (isZooming) {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(0,2.6f,-5.5f), 7 * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(25,0,0), 65 * Time.deltaTime);
            if (transform.rotation == Quaternion.Euler(25, 0, 0) && transform.position == new Vector3(0, 2.6f, -5.5f)) { isZooming = false; }
            return;
        }

        //Rotate the camera
        if (Input.GetMouseButton(1))
        {
            transform.RotateAround(target.transform.position, Vector3.up, ((Input.GetAxisRaw("Mouse X") * Time.deltaTime) * GlobalVariables.mouseSensitivity * 15000));
            transform.RotateAround(target.transform.position, transform.right, -((Input.GetAxisRaw("Mouse Y") * Time.deltaTime) * GlobalVariables.mouseSensitivity * 15000));
        }

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)) {
            Vector3 direction = (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) ? Vector3.right : Vector3.left;
            transform.RotateAround(target.transform.position, direction, ((0.001f * Time.deltaTime) * GlobalVariables.mouseSensitivity * 15000));
        }

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            Vector3 direction = (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) ? Vector3.up : Vector3.down;
            transform.RotateAround(target.transform.position, direction, ((0.001f * Time.deltaTime) * GlobalVariables.mouseSensitivity * 15000));
        }

        /* Zoom the camera */
        ZoomCamera();
    }

    void ZoomCamera()
    {
        // Get Input
        float zoomInput;
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.PageUp) || Input.GetKey(KeyCode.PageDown))
        {
            zoomInput = (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.PageUp)) ? 0.05f : -0.05f;
        }
        else if (MouseScrollDetected()) { zoomInput = Input.GetAxis("Mouse ScrollWheel"); }
        else { return; }

        // Check if within the distance
        if (!WithinDistance(zoomInput)) { return; }

        // Translate camera
        transform.Translate(
            0f,
            0f,
            (zoomInput * Time.deltaTime) * GlobalVariables.mouseSensitivity * 15000 * 0.001f,
            Space.Self
        );
    }

    // Reusables
    bool WithinDistance(float direction) {
        if (Vector3.Distance(transform.position, target.transform.position) <= minDistance && direction > 0f) { return false; }
        if (Vector3.Distance(transform.position, target.transform.position) >= maxDistance && direction < 0f) { return false; }
        return true;
    }

    bool MouseScrollDetected()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f || Input.GetAxis("Mouse ScrollWheel") < 0f) { return true; }
        return false;
    }

    // Public
    public void ZoomOut()
    {
        isZooming = true;
    }
}