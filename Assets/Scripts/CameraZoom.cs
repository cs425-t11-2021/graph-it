//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    // The speed of zooming in and out (default at 100f)
    public float zoomSpeed = 100f;
    // Minimum zoom size
    public float zoomMin = 2f;
    // Maximum zoom size
    public float zoomMax = 20f;

    // Reference to the Camera component attached to this object
    private Camera camera;

    private void Awake() {
        // Get a reference to the Camera component of the current gameObject
        camera = gameObject.GetComponent<Camera>();
    }

    private void Update() {
        // Get the value of the scroll wheel multiplied by zoom speed
        float mouseScrollWheel = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;
        // Use percentage values when zooming in and our rather than absolute values for more consistent speed
        float newZoomLevel = mouseScrollWheel > 0
            ? camera.orthographicSize * (1 - (0.1f * mouseScrollWheel))
            : camera.orthographicSize * (1 - (0.11111f * mouseScrollWheel));

        // Get Mouse Position before zoom
        Vector3 mousePosBefore = camera.ScreenToWorldPoint(Input.mousePosition);
        // Set the camera size to the zoom level, clamped between min zoom size and max zoom size
        camera.orthographicSize = Mathf.Clamp(newZoomLevel, zoomMin, zoomMax);
        // Get Mouse Position after zoom
        Vector3 mousePosAfter = camera.ScreenToWorldPoint(Input.mousePosition);

        // Calculate difference between mouse positions before and after zooming
        Vector3 diff = mousePosBefore - mousePosAfter;

        // Add Difference to Camera Position
        Vector3 camPos = transform.position;
        Vector3 targetPos = new Vector3(camPos.x + diff.x, camPos.y + diff.y, camPos.z);
 
        // Set camera position to new position
        transform.position = targetPos;
    }

    // // Method which changes the camera's orthographic size to zooom in or out until the zoom target is reached
    // // Uses the zoomSpeed variable to limit the speed at which the camera zoom occurs
    // private void ZoomUntilTarget() {
    //     // Change the camera size based on the difference between target size, current size, and zoom speed
    //     float zoomDifference = zoomTarget - camera.orthographicSize;
    //     camera.orthographicSize += zoomDifference * zoomSpeed * Time.deltaTime;

    //     // Clamp the camera's size to make sure the zoom doesn't overshoot the target
    //     if (zoomDifference > 0) {
    //         camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, 0f, zoomTarget);
    //     }
    //     else {
    //         camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, zoomTarget, float.PositiveInfinity);
    //     }

    //     transform.position = Vector3.MoveTowards(transform.position, zoomCenter, centeringSpeed * Time.deltaTime);
    // }
}
