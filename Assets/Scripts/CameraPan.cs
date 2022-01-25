//All code developed by Team 11
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
    // The space that the camera is allowed to pan in
    private Bounds cameraPanBounds;

    private void Awake() {
        // Get a reference to the Camera component of the current gameObject
        camera = gameObject.GetComponent<Camera>();

        cameraPanBounds = new Bounds();
    }

    private void Update() {
        // Disable panning if no graph
        if (Controller.singleton.graphObj.childCount == 0)
        {
            return;
        }

        // Disable panning in selection mode
        if (Toolbar.singleton.SelectionMode) return;

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

            // Update the camera pan bounds according to the position of the vertices
            UpdateBounds();
        }

        // Left mouse button held
        if (Input.GetMouseButton(0)) {
            // Do not pan camera if the cursor is over UI elements
            if (EventSystem.current.IsPointerOverGameObject()) return;

            // Do not pan camera if mouse is currently over an object with a collider (eg. vertices and edges)
            if (cursorOverCollider) return;

            // Do not pan if in vertex creation mode
            if (Toolbar.singleton.CreateVertexMode) return;
            
            // Calculate the direction the camera needs to move to move towards the mouse cursor
            Vector3 dir = lastPosition - camera.ScreenToWorldPoint(Input.mousePosition);
            // Translate the camera's position towards the direction, scaled by pan speed
            Vector3 targetPosition = Vector3.MoveTowards(transform.position, transform.position + dir, panSpeed * Time.deltaTime);
            // Only move the camera if the target position is still in camera pan bounds
            if (cameraPanBounds.Contains(new Vector3(targetPosition.x, targetPosition.y, 0)))
            {
                transform.position = targetPosition;
            }
            
            // Update last position of camera
            lastPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        }

        // Left mouse button released
        if (Input.GetMouseButtonUp(0)) {
            if (cursorOverCollider) cursorOverCollider = false;
        }

    }

    // Set the bounds for camera panning to be slightly bigger than the space taken up by the graph objects
    private void UpdateBounds()
    {
        Transform graphObj = Controller.singleton.graphObj;

        float xMin = float.PositiveInfinity, yMin = float.PositiveInfinity;
        float xMax = float.NegativeInfinity, yMax = float.NegativeInfinity;

        // Find the minimum and maxmimum x and y values of the vertices
        for (int i = 0; i < graphObj.childCount; i++)
        {
            if (graphObj.GetChild(i).position.x < xMin)
            {
                xMin = graphObj.GetChild(i).position.x;
            }
            if (graphObj.GetChild(i).position.x > xMax)
            {
                xMax = graphObj.GetChild(i).position.x;
            }
            if (graphObj.GetChild(i).position.y < yMin)
            {
                yMin = graphObj.GetChild(i).position.y;
            }
            if (graphObj.GetChild(i).position.y > yMax)
            {
                yMax = graphObj.GetChild(i).position.y;
            }
        }

        // Set camera pan bounds
        cameraPanBounds.SetMinMax(new Vector3(xMin - 3, yMin - 3, 0), new Vector3(xMax + 3, yMax + 3, 0));

    }
}
