//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Class for hosting functions called by buttons of the Edit dropdown menu, inherits from MenuButton
public class EditMenu : MenuButton
{
    // Reference to child buttons assigned in Unity Inspector
    [SerializeField]
    private Button addEdgeButton;

    private void Start() {
        SelectionManager.Singleton.OnSelectionChange += (selectedVertexCount, selectedEdgeCount) => {
            if (selectedVertexCount == 2 && selectedEdgeCount == 0) {
                this.addEdgeButton.interactable = true;
            }
            else {
                this.addEdgeButton.interactable = false;
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Singleton.ControlCommandKeyHeld) {
            if (Input.GetKeyDown(KeyCode.Z)) {
                Undo();
            }

            if (Input.GetKeyDown(KeyCode.X)) {
                Redo();
            }
        }
    }

    // Function called by select all button
    public void SelectAll(){
        SelectionManager.Singleton.SelectAll();
        EventSystem.current.SetSelectedGameObject(null);
    }

    // Function called by deselect all button
    public void DeselectAll(){
        SelectionManager.Singleton.DeSelectAll();
        EventSystem.current.SetSelectedGameObject(null);
    }

    // Function called by the add edge button
    public void AddEdgeButtonFromEditMenu(){
        SelectionManager.Singleton.AddEdgeBetweenTwoSelectedVertices();
        
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Undo()
    {
        Controller.Singleton.Graph.Undo();
    }

    public void Redo()
    {
        Controller.Singleton.Graph.Redo();
    }
}
