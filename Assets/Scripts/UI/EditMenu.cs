//All code developed by Team 11

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
                if (Input.GetKey(KeyCode.LeftShift)) {
                    Redo();
                }
                else {
                    Undo();
                }
            }

            if (Input.GetKeyDown(KeyCode.A)) {
                SelectAll();
            }

            if (Input.GetKeyDown(KeyCode.D)) {
                DeselectAll();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                Copy();
            }
            else if (Input.GetKeyDown(KeyCode.V))
            {
                Paste();
            }
        }

        if (Controller.Singleton.Graph.Changes.Count == 0) {
            undoButton.interactable = false;
            undoButton.GetComponentInChildren<TMP_Text>().text = "Undo";
            undoButton.GetComponentInChildren<TMP_Text>().color = Color.gray;
        }
        else {
            undoButton.GetComponentInChildren<TMP_Text>().text = "Undo " + GraphModification.aliases[Controller.Singleton.Graph.Changes.Peek().Mod];
            undoButton.interactable = true;
            undoButton.GetComponentInChildren<TMP_Text>().color = Color.black;
        }

        if (Controller.Singleton.Graph.UndoneChanges.Count == 0) {
            redoButton.interactable = false;
            redoButton.GetComponentInChildren<TMP_Text>().text = "Redo";
            redoButton.GetComponentInChildren<TMP_Text>().color = Color.gray;
        }
        else {
            redoButton.GetComponentInChildren<TMP_Text>().text = "Redo " + GraphModification.aliases[Controller.Singleton.Graph.UndoneChanges.Peek().Mod];
            redoButton.interactable = true;
            redoButton.GetComponentInChildren<TMP_Text>().color = Color.black;
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
