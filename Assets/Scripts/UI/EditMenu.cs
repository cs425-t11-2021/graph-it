//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

// Class for hosting functions called by buttons of the Edit dropdown menu, inherits from MenuButton
public class EditMenu : MenuButton
{
    // Reference to child buttons assigned in Unity Inspector
    [SerializeField]
    private Button addEdgeButton;

    [SerializeField] private Button undoButton;
    [SerializeField] private Button redoButton;

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

        if (Controller.Singleton.Graph.Changes.Count == 0) {
            undoButton.interactable = false;
            undoButton.GetComponentInChildren<TMP_Text>().text = "Undo";
        }
        else {
            undoButton.GetComponentInChildren<TMP_Text>().text = "Undo " + Controller.Singleton.Graph.Changes.Peek().Mod.ToString();
            undoButton.interactable = true;
        }

        if (Controller.Singleton.Graph.UndoneChanges.Count == 0) {
            redoButton.interactable = false;
            redoButton.GetComponentInChildren<TMP_Text>().text = "Redo";
        }
        else {
            redoButton.GetComponentInChildren<TMP_Text>().text = "Redo " + Controller.Singleton.Graph.UndoneChanges.Peek().Mod.ToString();
            redoButton.interactable = true;
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
        if (Controller.Singleton.Graph.Changes.Count > 0)
            Controller.Singleton.Graph.Undo();
    }

    public void Redo()
    {
        if (Controller.Singleton.Graph.UndoneChanges.Count > 0)
            Controller.Singleton.Graph.Redo();
    }

    public void Copy()
    {
        CopyManager.Singleton.CopyCurrentSelection();
    }

    public void Paste()
    {
        CopyManager.Singleton.Paste();
    }

    public void ClearAlgorithmResults()
    {
        AlgorithmsPanel.Singleton.ClearAlgorithmResults();
    }

    public void CreateGraphFromSelection()
    {
        if (SelectionManager.Singleton.SelectedVertices.Count > 0)
        {
            Controller.Singleton.CreateInstanceFromSelection();
            NotificationManager.Singleton.CreateNotification("Creating a new graph from selection.", 3);
        }
    }
}
