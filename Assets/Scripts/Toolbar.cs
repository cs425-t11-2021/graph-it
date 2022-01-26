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
                this.edgeCreationModeButton.Checked = false;
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
                this.edgeCreationModeButton.Checked = false;
            }
            this.createVertexMode = value;
        }
    }

    [SerializeField]
    private Button deleteButton;

    [SerializeField]
    private ToggleButton edgeCreationModeButton;
    private bool edgeCreationMode = false;
    public bool EdgeCreationMode {
        get => this.edgeCreationMode;
        private set {
            if (value) {
                this.selectionModeButton.Checked = false;
                this.createVertexModeButton.Checked = false;
            }
            this.edgeCreationMode = value;
        }
    }

    [SerializeField]
    private Button addEdgeButton;

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
        edgeCreationModeButton.gameObject.SetActive(false);
        addEdgeButton.gameObject.SetActive(false);
    }

    // Turn off all toggles
    public void ResetAll() {
        this.SelectionMode = false;
        this.CreateVertexMode = false;
        this.EdgeCreationMode = false;
    }

    // Enable or disable selection mode
    public void ToggleSelectionMode() {
        this.SelectionMode = !this.SelectionMode;
    }

    // Enable or disable vertex creation mode
    public void ToggleCreateVertexMode() {
        this.CreateVertexMode = !this.CreateVertexMode;
    }

    // Function to enable certain toolbar buttons when a graph component is selected
    private void OnSelectionChange(int selectedVertexCount, int selectedEdgeCount) {
        // Enable delete button if any components are selected
        this.deleteButton.gameObject.SetActive(selectedVertexCount + selectedEdgeCount > 0);

        // Enable the Edge Creation Mode button when one vertex is selected
        if (selectedVertexCount == 1 && selectedEdgeCount == 0) {
            this.edgeCreationModeButton.gameObject.SetActive(true);
        }
        else {
            this.edgeCreationModeButton.Checked = false;
            this.edgeCreationModeButton.gameObject.SetActive(false);
        }

        // Enable the Add Edge button when two vertices are selected
        this.addEdgeButton.gameObject.SetActive(selectedVertexCount == 2 && selectedEdgeCount == 0);
    }

    // Function called by delete button
    public void DeleteSelection() {
        SelectionManager.singleton.DeleteSelection();
    }

    // Enable or disable edge addition mode
    public void ToggleAddEdgeMode() {
        this.EdgeCreationMode = !this.EdgeCreationMode;
    }

    // Function called by Add Edge button
    public void AddEdge() {
        SelectionManager.singleton.AddEdge();
    }
}
