using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    // The speed of zooming in and out, default at 1f
    public float zoomSpeed = 6f;
    // Amount of zoom change per scroll unit
    public float zoomDelta = 75f;
    // Minimum zoom size
    public float zoomMin = 2f;
    // Maximum zoom size
    public float zoomMax = 20f;

    // Target zoom size of the camera, default at 9f at the start of the program
    private float zoomTarget = 9f;
    // Reference to the Camera component attached to this object
    private Camera camera;

    private void Awake() {
        // Get a reference to the Camera component of the current gameObject
        camera = gameObject.GetComponent<Camera>();
    }

    private void Update() {
        // Increase or decrease the zoom target based on vertical component of scroll wheel vector
        if (Input.mouseScrollDelta.y < 0f) {
            zoomTarget -= zoomDelta * Time.deltaTime;
        }
        else if (Input.mouseScrollDelta.y > 0f) {
            zoomTarget += zoomDelta * Time.deltaTime;
        }

        // Limit the zoom to between a min value and a max value
        zoomTarget = Mathf.Clamp(zoomTarget, zoomMin, zoomMax);

        ZoomUntilTarget();
    }

    // Method which changes the camera's orthographic size to zooom in or out until the zoom target is reached
    // Uses the zoomSpeed variable to limit the speed at which the camera zoom occurs
    private void ZoomUntilTarget() {
        float zoomDifference = zoomTarget - camera.orthographicSize;
        camera.orthographicSize += zoomDifference * zoomSpeed * Time.deltaTime;

        // Clamp the camera's size to make sure the zoom doesn't overshoot the target
        if (zoomDifference > 0) {
            camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, 0f, zoomTarget);
        }
        else {
            camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, zoomTarget, float.PositiveInfinity);
        }
        
    }
}
