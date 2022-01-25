using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Toolbar : MonoBehaviour
{
    public static Toolbar singleton;

    [SerializeField]
    private ToggleButton selectionModeButton;
    private bool selectionMode = false;
    public bool SelectionMode { 
        get => this.selectionMode;
        private set {
            if (value) {
                this.createVertexModeButton.Checked = false;
                this.addEdgeModeButton.Checked = false;
            }
            this.selectionMode = value;
        }
    }

    [SerializeField]
    private ToggleButton createVertexModeButton;
    private bool createVertexMode = false;
    public bool CreateVertexMode { 
        get => this.createVertexMode;
        private set { 
            if (value) {
                this.selectionModeButton.Checked = false;
                this.addEdgeModeButton.Checked = false;
            }
            this.createVertexMode = value;
        }
    }

    [SerializeField]
    private Button deleteButton;

    [SerializeField]
    private ToggleButton addEdgeModeButton;
    private bool addEdgeMode = false;
    public bool AddEdgeMode {
        get => this.addEdgeMode;
        private set {
            if (value) {
                this.selectionModeButton.Checked = false;
                this.createVertexModeButton.Checked = false;
            }
            this.addEdgeMode = value;
        }
    }

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

        // Subscribe to OnSelectionChange event
        SelectionManager.singleton.OnSelectionChange += OnSelectionChange;

        // Default configuration
        deleteButton.gameObject.SetActive(false);
        addEdgeModeButton.gameObject.SetActive(false);
    }

    // Turn off all toggles
    public void ResetAll() {
        this.SelectionMode = false;
        this.CreateVertexMode = false;
        this.AddEdgeMode = false;
    }

    // Enable or disable selection mode
    public void ToggleSelectionMode() {
        this.SelectionMode = !this.SelectionMode;
    }

    // Enable or disable vertex creation mode
    public void ToggleCreateVertexMode() {
        this.CreateVertexMode = !this.CreateVertexMode;
    }

    // Function to enable the delete button when a graph component is selected
    private void OnSelectionChange(int selectedVertexCount, int selectedEdgeCount) {
        this.deleteButton.gameObject.SetActive(selectedVertexCount + selectedEdgeCount > 0);
        if (selectedVertexCount == 1 && selectedEdgeCount == 0) {
            this.addEdgeModeButton.gameObject.SetActive(true);
        }
        else {
            this.addEdgeModeButton.Checked = false;
            this.addEdgeModeButton.gameObject.SetActive(false);
        }
        
    }

    // Function called by delete button
    public void DeleteSelection() {
        SelectionManager.singleton.DeleteSelection();
    }

    // Enable or disable edge addition mode
    public void ToggleAddEdgeMode() {
        this.AddEdgeMode = !this.AddEdgeMode;
    }
}
