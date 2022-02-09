using UnityEngine;

public class GraphPhysics : SingletonBehavior<GraphPhysics> {
    // Length of each edge (manually set for now, could implment an algorithm to determine the distance from graph size/shape or whatever)
    private float edgeLength = 5;
    // Timer used for tempoarily enabling graph physics
    private float physicsTimer;
    // Graph physics enabled
    private bool graphPhysicsEnabled = false;

    private void Awake() {
        Controller.Singleton.OnEdgeObjectCreation += OnEdgeObjectCreation;
        Controller.Singleton.OnVertexObjectCreation += OnVertexObjectCreation;
    }

    private void OnVertexObjectCreation(VertexObj vertexObj) {
        
    }

    private void OnEdgeObjectCreation(EdgeObj edgeObj) {
        // Add a DistanceJoint2D which connects the two vertices
        DistanceJoint2D joint = edgeObj.FromVertexObj.gameObject.AddComponent<DistanceJoint2D>();
        // Configure the properties of the joint
        joint.autoConfigureConnectedAnchor = false;
        joint.enableCollision = true;
        joint.distance = this.edgeLength;
        joint.maxDistanceOnly = true;
        joint.autoConfigureDistance = false;
        joint.connectedBody = edgeObj.ToVertexObj.gameObject.GetComponent<Rigidbody2D>();
        // Disable joint by default, the joint will only be enabled when graph physics is in use
        joint.enabled = false;
    }

    private void Update() {
        // If graph physics is currently enabled and the timer isn't set to -1 (indefinite duration), decrease the timer
        if (this.graphPhysicsEnabled && this.physicsTimer != -1)
        {
            if (this.physicsTimer <= 0f)
            {
                // Turn off graph physics once timer hits 0
                SetGraphPhysics(false);
            }
            else
            {
                this.physicsTimer -= Time.deltaTime;
            }
        }
    }

    // Enable/disable the components associated with graph physics
    private void SetGraphPhysics(bool enabled)
    {
        // Turns off the grid if physics is turned on
        if (SettingsManager.Singleton.SnapVerticesToGrid)
        {
            Grid.singleton.ClearGrid();
            Grid.singleton.GridEnabled = !enabled;
        }

        // Find all vertex objects
        VertexObj[] vertexObjs = GameObject.FindObjectsOfType<VertexObj>();
        foreach (VertexObj vertexObj in vertexObjs)
        {
            // Enable the joints connecting vertices when physics is on
            DistanceJoint2D[] joints = vertexObj.GetComponents<DistanceJoint2D>();
            foreach (DistanceJoint2D joint in joints)
            {
                joint.enabled = enabled;
            }
            Rigidbody2D vertexObjRB = vertexObj.GetComponent<Rigidbody2D>();
            vertexObjRB.velocity = Vector3.zero;
            // Set vertex rigidbody to kinematic when physics is off
            vertexObjRB.isKinematic = !enabled;

            if (!enabled && SettingsManager.Singleton.SnapVerticesToGrid)
            {
                // Resnap vertices when physics turns off if snap to grid is enabled
                vertexObj.transform.position = Grid.singleton.FindClosestGridPosition(vertexObj);
            }
        }
        this.graphPhysicsEnabled = enabled;
    }

    // Enables graph physics for a certain duartion
    public void UseGraphPhysics(float duration)
    {
        if (!this.graphPhysicsEnabled)
        {
            SetGraphPhysics(true);
        }
        this.physicsTimer = duration;
    }
}