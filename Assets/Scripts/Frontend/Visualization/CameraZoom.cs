//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    // The speed of zooming in and out (default at 100f)
    [SerializeField]
    private float zoomSpeed = 100f;
    // Minimum zoom size
    [SerializeField]
    private float zoomMin = 2f;
    // Maximum zoom size
    [SerializeField]
    private float zoomMax = 20f;

    // Reference to the Camera component attached to this object
    private Camera cam;

    // Store values for different tabes
    private Dictionary<uint, (float, Vector3)> storedZoomValues;

    private void Awake() {
        // Get a reference to the Camera component of the current gameObject
        cam = gameObject.GetComponent<Camera>();

        this.storedZoomValues = new Dictionary<uint, (float, Vector3)>();

        Controller.Singleton.OnInstanceCreated += instance => {
            this.storedZoomValues[instance.id] = (8, new Vector3(0, 0, -10f));
        };

        Controller.Singleton.OnInstanceDeleted += instance => {
            this.storedZoomValues.Remove(instance.id);
        };

        Controller.Singleton.OnInstanceChanged += (previous, current) => {
            if (previous != null && this.storedZoomValues.ContainsKey(previous.id))
                this.storedZoomValues[previous.id] = (this.cam.orthographicSize, this.cam.transform.position);

            this.cam.orthographicSize = this.storedZoomValues[current.id].Item1;
            this.cam.transform.position = this.storedZoomValues[current.id].Item2;
        };
    }

    private void Update() {
        if (UIManager.Singleton.CursorOnUI) return;
        
        // Get the value of the scroll wheel multiplied by zoom speed
        float mouseScrollWheel = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;
        // Use percentage values when zooming in and our rather than absolute values for more consistent speed
        float newZoomLevel = mouseScrollWheel > 0
            ? cam.orthographicSize * (1 - (0.1f * mouseScrollWheel))
            : cam.orthographicSize * (1 - (0.11111f * mouseScrollWheel));

        // Get Mouse Position before zoom
        Vector3 mousePosBefore = cam.ScreenToWorldPoint(Input.mousePosition);
        // Set the camera size to the zoom level, clamped between min zoom size and max zoom size
        cam.orthographicSize = Mathf.Clamp(newZoomLevel, zoomMin, zoomMax);
        // Get Mouse Position after zoom
        Vector3 mousePosAfter = cam.ScreenToWorldPoint(Input.mousePosition);

        // Calculate difference between mouse positions before and after zooming
        Vector3 diff = mousePosBefore - mousePosAfter;

        // Add Difference to Camera Position
        Vector3 camPos = transform.position;
        Vector3 targetPos = new Vector3(camPos.x + diff.x, camPos.y + diff.y, camPos.z);
 
        // Set camera position to new position
        transform.position = targetPos;
    }
}
