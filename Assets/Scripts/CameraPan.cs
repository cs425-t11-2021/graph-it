using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraPan : MonoBehaviour
{
    // Camera panning speed (default 100f)
    public float panSpeed = 100f;

    // Reference to the Camera component attached to this object
    private Camera camera;
    // Previous position of the camera used for panning
    private Vector3 lastPosition;
    // Whether or not cursor is over a collider
    private bool cursorOverCollider = false;

    private void Awake() {
        // Get a reference to the Camera component of the current gameObject
        camera = gameObject.GetComponent<Camera>();
    }

    private void Update() {
        // Left mouse button clicked
        if (Input.GetMouseButtonDown(0)) {
            // At the start of the pan, store cursor position
            lastPosition = camera.ScreenToWorldPoint(Input.mousePosition);

            // Check if cursor is over collider, if so, ignore panning until the mouse button is released
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 11f, LayerMask.GetMask("Vertex", "Edge"));  //11f since camera is at z = -10
            if (hit) {
                cursorOverCollider = true;
                return;
            }
        }

        // Left mouse button held
        if (Input.GetMouseButton(0)) {
            // Do not pan camera if the cursor is over UI elements
            if (EventSystem.current.IsPointerOverGameObject()) {
                return;
            }

            // Do not pan camera if mouse is currently over an object with a collider (eg. vertices and edges)
            if (cursorOverCollider) {
                return;
            }
            
            // Calculate the direction the camera needs to move to move towards the mouse cursor
            Vector3 dir = lastPosition - camera.ScreenToWorldPoint(Input.mousePosition);
            // Translate the camera's position towards the direction, scaled by pan speed
            transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, panSpeed * Time.deltaTime);
            // Update last position of camera
            lastPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        }

        // Left mouse button released
        if (Input.GetMouseButtonUp(0)) {
            if (cursorOverCollider) cursorOverCollider = false;
        }
        
    }
}
