using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Toolbar : SingletonBehavior<Toolbar>
{
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

    [SerializeField]
    private Button changeTypeButton;

    private void Awake() {
        // Subscribe to OnSelectionChange event
        SelectionManager.Singleton.OnSelectionChange += OnSelectionChange;

        // Default configuration
        deleteButton.gameObject.SetActive(false);
        edgeCreationModeButton.gameObject.SetActive(false);
        addEdgeButton.gameObject.SetActive(false);
        changeTypeButton.gameObject.SetActive(false);
    }

    private void Update() {
        if (UIManager.Singleton.CursorOnUI) return;
        
        // If a number key is pressed, trigger the corresponding button on the toolbar
        for (int i = 1; i <= 9; i++) {
            // Detect the press of a number key
            if (Input.GetKeyDown(i.ToString())) {
                // Get a list of buttons that are currently active on the toolbar
                List<GameObject> activeTools = new List<GameObject>();
                for (int j = 0; j < this.transform.childCount; j++) {
                    Transform child = this.transform.GetChild(j);
                    if (child.gameObject.activeInHierarchy) {
                        activeTools.Add(child.gameObject);
                    }
                }

                if (i > activeTools.Count) {
                    return;
                }

                // Trigger either the toggleButton or button depending on which one is attached
                ToggleButton toggleButton = activeTools[i - 1].GetComponent<ToggleButton>();
                Button button = activeTools[i - 1].GetComponent<Button>();
                if (toggleButton) {
                    toggleButton.Checked = !toggleButton.Checked;
                }
                else if (button) {
                    button.onClick.Invoke();
                }

                return;
            }
        }
    }

    // Turn off all toggles
    public void ResetAll() {
        this.selectionModeButton.Checked = false;
        this.createVertexModeButton.Checked = false;
        this.edgeCreationModeButton.Checked = false;
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
        this.changeTypeButton.gameObject.SetActive(selectedVertexCount == 0 && selectedEdgeCount > 0);
    }

    // Function called by delete button
    public void DeleteSelection() {
        SelectionManager.Singleton.DeleteSelection();
    }

    // Enable or disable edge addition mode
    public void ToggleAddEdgeMode() {
        this.EdgeCreationMode = !this.EdgeCreationMode;
    }

    // Function called by Add Edge button
    public void AddEdge() {
        SelectionManager.Singleton.AddEdge();
    }

    // Function called by Change Type button
    public void ChangeType() {
        SelectionManager.Singleton.ChangeSelectedEdgesType();
    }
}
