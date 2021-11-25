using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    // Singleton
    public static Controller singleton;

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
    public Vector2 GetCursorWorldPosition() {
        return (Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }    
}
