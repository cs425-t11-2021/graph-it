using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    // Singleton
    public static Controller singleton;

    // Main graph DS
    // SET TO PUBLIC FOR TESTING PURPUSES, CHANGE LATER
    public Graph grpah;

    private void Awake() {
        // Singleton pattern setup
        if (singleton == null) {
            singleton = this;
        }
        else {
            Debug.LogError("[Controller] Singleton pattern violation");
            Destroy(this);
            return;
        }
    }

    // Utility method to help get the corresponding world position of the mouse cursor
    public Vector3 GetCursorWorldPosition() {
        return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z));
    }    
}
