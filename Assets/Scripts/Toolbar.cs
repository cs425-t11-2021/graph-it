using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Toolbar : MonoBehaviour
{
    public static Toolbar singleton;

    public bool SelectionMode { get; private set; }

    private void Awake() {
        // Singleton pattern setup
        if (singleton == null) {
            singleton = this;
        }
        else {
            Debug.LogError("[Toolbar] Singleton pattern violation");
            Destroy(this);
            return;
        }
    }

    public void ToggleSelectionMode() {
        this.SelectionMode = !this.SelectionMode;
    }
}
