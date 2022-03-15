using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Toolbar : SingletonBehavior<Toolbar>
{
    [SerializeField]
    private ToggleButton selectionModeButton;

    [SerializeField]
    private ToggleButton createVertexModeButton;

    [SerializeField]
    private Button deleteButton;

    [SerializeField]
    private ToggleButton edgeCreationModeButton;

    [SerializeField]
    private Button addEdgeButton;

    [SerializeField]
    private Button changeTypeButton;

    [SerializeField] private ToggleButton edgeThicknessButton;

    [SerializeField] private ToggleButton edgeCurvatureButton;

    [SerializeField] private Button changeVertexStyleButton;

    private void Awake() {
        // Subscribe to OnSelectionChange event
        SelectionManager.Singleton.OnSelectionChange += OnSelectionChange;
        
        // Subscribe to Onclick event
        InputManager.Singleton.OnMouseClick += CloseSubpanels;

        // Default configuration
        deleteButton.gameObject.SetActive(false);
        // edgeCreationModeButton.gameObject.SetActive(false);
        addEdgeButton.gameObject.SetActive(false);
        changeTypeButton.gameObject.SetActive(false);
        this.edgeThicknessButton.gameObject.SetActive(false);
        this.edgeCurvatureButton.gameObject.SetActive(false);
        this.changeVertexStyleButton.gameObject.SetActive(false);
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
    
    // Turn off the edge thickness and curvature subpanels
    private void CloseSubpanels()
    {
        if (this.edgeThicknessButton.Checked)
            this.edgeThicknessButton.Checked = false;
        if (this.edgeCurvatureButton.Checked)
            this.edgeCurvatureButton.Checked = false;
    }

    // Turn off all toggles
    public void ResetAll() {
        this.selectionModeButton.Checked = false;
        this.createVertexModeButton.Checked = false;
        this.edgeCreationModeButton.Checked = false;
    }

    // Enable or disable selection mode
    public void ToggleSelectionMode() {
        if (ManipulationStateManager.Singleton.ActiveState != ManipulationState.selectionState) {
            ManipulationStateManager.Singleton.ActiveState = ManipulationState.selectionState;
            this.createVertexModeButton.UpdateStatus(false);
            this.edgeCreationModeButton.UpdateStatus(false);
        }
        else {
            ManipulationStateManager.Singleton.ActiveState = ManipulationState.viewState;
        }
        
        CloseSubpanels();
    }

    // Enable or disable vertex creation mode
    public void ToggleCreateVertexMode() {
        if (ManipulationStateManager.Singleton.ActiveState != ManipulationState.vertexCreationState) {
            ManipulationStateManager.Singleton.ActiveState = ManipulationState.vertexCreationState;
            this.selectionModeButton.UpdateStatus(false);
            this.edgeCreationModeButton.UpdateStatus(false);
        }
        else {
            ManipulationStateManager.Singleton.ActiveState = ManipulationState.viewState;
        }
        
        CloseSubpanels();
    }

    // Function to enable certain toolbar buttons when a graph component is selected
    private void OnSelectionChange(int selectedVertexCount, int selectedEdgeCount) {
        // Enable delete button if any components are selected
        this.deleteButton.gameObject.SetActive(selectedVertexCount + selectedEdgeCount > 0);

        // // Enable the Edge Creation Mode button when nothing is selected
        // if (selectedVertexCount == 0 && selectedEdgeCount == 0) {
        //     this.edgeCreationModeButton.gameObject.SetActive(true);
        // }
        // else {
        //     this.edgeCreationModeButton.Checked = false;
        //     this.edgeCreationModeButton.gameObject.SetActive(false);
        // }

        // Enable the Add Edge button when two vertices are selected
        // this.addEdgeButton.gameObject.SetActive(selectedVertexCount == 2 && selectedEdgeCount == 0);
        this.changeTypeButton.gameObject.SetActive(selectedVertexCount == 0 && selectedEdgeCount > 0);
        
        // Enable the thickness and curvature toggles when only edges are selected
        if (selectedVertexCount == 0 && selectedEdgeCount > 0)
        {
            this.edgeThicknessButton.gameObject.SetActive(true);
            this.edgeCurvatureButton.gameObject.SetActive(true);
        }
        else
        {
            this.edgeThicknessButton.Checked = false;
            this.edgeCurvatureButton.Checked = false;
            this.edgeThicknessButton.gameObject.SetActive(false);
            this.edgeCurvatureButton.gameObject.SetActive(false);
        }

        this.changeVertexStyleButton.gameObject.SetActive(selectedVertexCount > 0 && selectedEdgeCount == 0);
    }

    // Function called by delete button
    public void DeleteSelection() {
        SelectionManager.Singleton.DeleteSelection();
    }

    // Enable or disable edge addition mode
    public void ToggleAddEdgeMode() {
        if (ManipulationStateManager.Singleton.ActiveState != ManipulationState.edgeCreationState) {
            ManipulationStateManager.Singleton.ActiveState = ManipulationState.edgeCreationState;
            this.selectionModeButton.UpdateStatus(false);
            this.createVertexModeButton.UpdateStatus(false);
        }
        else {
            ManipulationStateManager.Singleton.ActiveState = ManipulationState.viewState;
        }
    }

    // Function called by Add Edge button
    public void AddEdge() {
        SelectionManager.Singleton.AddEdgeBetweenTwoSelectedVertices();
    }

    // Function called by Change Type button
    public void ChangeType() {
        SelectionManager.Singleton.ChangeSelectedEdgesType();
    }
    
    // Function called by the edge thickness and curvature buttons to toggle the add/subtract subpanel
    public void ToggleAddSubtractSubpanel(GameObject subpanel)
    {
        if (subpanel.activeInHierarchy)
        {
            subpanel.SetActive(false);
        }
        else
        {
            subpanel.SetActive(true);
        }
    }
    
    // Function called by the edge thickness plus or minus buttons
    public void ChangeEdgeThickness(int change)
    {
        foreach (EdgeObj edgeObj in SelectionManager.Singleton.SelectedEdges)
        {
            edgeObj.ChangeThickness(change);
        }
    }
    
    // Function called by the edge curvature plus or minus buttons
    public void ChangeEdgeCurvature(int change)
    {
        foreach (EdgeObj edgeObj in SelectionManager.Singleton.SelectedEdges)
        {
            edgeObj.ChangeCurvature(change);
        }
    }

    // Function called by the change vertex style button
    public void ChangeVertexStyle() {
        foreach (VertexObj vertexObj in SelectionManager.Singleton.SelectedVertices) {
            vertexObj.ChangeStyle();
        }
    }
}
