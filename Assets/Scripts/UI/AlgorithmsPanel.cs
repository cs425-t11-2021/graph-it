//All code developed by Team 11
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Object = System.Object;

[System.Serializable]
public class GraphDisplayAlgorithmAssociation
{
    public string algorithmClass = "";
    public bool multiThreaded = false;
    public ToggleButton activationButton;
    public int requiredVertices = 0;
    public int requiredEdges = 0;
    public string activationMethod = "";
    public string completedMethod = "";

    public Action<Vertex[]> OnCompleteUpdateDisplay
    {
        get
        {
            return (vertexParms) =>
            {
                object result = Type.GetType("AlgorithmManager").GetMethod(completedMethod)
                    .Invoke(Controller.Singleton.AlgorithmManager, (Object[]) vertexParms);
                
                if (result == null)
                {
                    Logger.Log("Graph display algorithm " + algorithmClass + " returned a null result.", this, LogType.ERROR);
                }
                else
                {
                    SelectionManager.Singleton.DeSelectAll();
                    
                    AlgorithmsPanel.Singleton.AlgorithmResult = (List<Edge>) result;
                    NotificationManager.Singleton.CreateNoficiation(algorithmClass + " finished.", 3);
                    AlgorithmsPanel.Singleton.resultButton.interactable = true;
                }
            };
        }
    }
}

public class AlgorithmsPanel : SingletonBehavior<AlgorithmsPanel>
{
    [SerializeField] private GraphDisplayAlgorithmAssociation[] associations;
    
    // // Reference of the button of prim
    // [SerializeField] private Button primButton;
    // // Reference of the kruskal button
    // [SerializeField] private Button kruskalButton;
    // // Reference of the button of dijkstra
    // [SerializeField] private Button dijkstraButton;
    // // Reference of the button of Bellman Ford
    // [SerializeField] private Button bellmanButton;
    // // Reference of the Eulerian circuit button
    // [SerializeField] private Button eulerianButton;
    // // Reference of the Maximum Indepdent Set button
    // [SerializeField] private Button maxIndSetButton;
    // // Reference of the Maximum Matching button
    // [SerializeField] private Button maxMatchingButton;
    //[SerializeField] private Button algClosePanel;
    //Reference to the button to close the algorithm info panels
    [SerializeField] private Button algOpenPanel;
    //Reference to the button to open the algorithm info panels

    [SerializeField] public Button resultButton;

    public GraphDisplayAlgorithmAssociation CurrentlySelectedAlgorithm {get; private set;}

    public List<Edge> AlgorithmResult {get; set;}


    // Property for whether or not the algorithm buttons are enabled
    public bool AlgorithmButtonsEnabled
    {
        get; set;
    }

    private void Awake() {
        SelectionManager.Singleton.OnSelectionChange += OnSelectionChange;
        Array.ForEach(this.associations, a => a.activationButton.UpdateStatus(false));
        this.resultButton.interactable = false;
    }

    // Function called when the selection is changed
    private void OnSelectionChange(int selectedVertexCount, int selectedEdgeCount) {
        // // Only allow the prim button to be pressed if there is exactly one vertex selected
        // this.primButton.interactable = selectedVertexCount == 1 && selectedEdgeCount == 0;
        // // Only allow dijkstra if exactly two vertices are selected
        // this.dijkstraButton.interactable = selectedVertexCount == 2 && selectedEdgeCount == 0;
        // // Only allow Bellman Ford if exactly one vertex is selected
        // this.bellmanButton.interactable = selectedVertexCount == 1 && selectedEdgeCount == 0;
        foreach (GraphDisplayAlgorithmAssociation association in this.associations)
        {
            // if (association.requiredEdges != selectedEdgeCount || association.requiredVertices != selectedVertexCount)
            // {
            //     association.activationButton.interactable = false;
            // }
            // else
            // {
            //     association.activationButton.interactable = true;
            // }
        }
    }

    public void UpdateGraphDisplayResults(Algorithm algorithm, Vertex[] vertexParms)
    {
        string algorithmName = algorithm.GetType().ToString();

        foreach (GraphDisplayAlgorithmAssociation association in this.associations)
        {
            if (association.algorithmClass == algorithmName)
            {
                association.OnCompleteUpdateDisplay(vertexParms);
                return;
            }
        }
        
        Logger.Log("No algorithm association found for " + algorithmName + ".", this, LogType.ERROR);
    }

    public void RunGraphDisplayAlgorithm(GraphDisplayAlgorithmAssociation association)
    {
        if (association.requiredVertices > 0)
        {
            Object[] vertices = new Object[association.requiredVertices];
            for (int i = 0; i < association.requiredVertices; i++)
            {
                vertices[i] = SelectionManager.Singleton.SelectedVertices[i].Vertex;
            }
            Type.GetType("AlgorithmManager").GetMethod(association.activationMethod).Invoke(Controller.Singleton.AlgorithmManager, vertices);
        }
        else
        {
            Type.GetType("AlgorithmManager").GetMethod(association.activationMethod).Invoke(Controller.Singleton.AlgorithmManager, null);
        }

        DeselectAllAlgorithms();
    }

    public void SelectAlgorithm(string algorithmName) {
        foreach (GraphDisplayAlgorithmAssociation association in this.associations)
        {
            if (association.algorithmClass == algorithmName)
            {
                if (this.CurrentlySelectedAlgorithm != association) {
                    this.CurrentlySelectedAlgorithm = association;
                    association.activationButton.UpdateStatus(true);
                }
                else {
                    this.CurrentlySelectedAlgorithm = null;
                    association.activationButton.UpdateStatus(false);
                }
            }
            else {
                association.activationButton.UpdateStatus(false);
            }
        }
    }

    public void DeselectAllAlgorithms() {
        foreach (GraphDisplayAlgorithmAssociation association in this.associations) {
            association.activationButton.UpdateStatus(false);
        }
        this.CurrentlySelectedAlgorithm = null;
    }

    public void StartAlgorithmInitiation() {
        if (this.CurrentlySelectedAlgorithm != null) {
            ManipulationStateManager.Singleton.ActiveState = ManipulationState.algorithmInitiationState;
        }
    }

    public void DisplayAlgorithmResult() {
        ManipulationStateManager.Singleton.ActiveState = ManipulationState.algorithmDisplayState;
    }

    // public void UpdatePrimsResult() { }
    //
    // public void UpdateKruskalsResult() { }
    //
    // public void UpdateDepthFirstSearchResult() { }
    //
    // public void UpdateBreadthFirstSearchResult() { }
    //
    // public void UpdatePrimsCalculating() { }
    //
    // public void UpdateKruskalsCalculating() { }
    //
    // public void UpdateDepthFirstSearchCalculating() { }
    //
    // public void UpdateBreadthFirstSearchCalculating() { }

    //deactivate the graphInfo panel and display the open panel button for the user to access
    public void CloseAlgorithmInfoPanel(){
        //GetComponent<RectTransform>().position = new Vector3(-577.1f,293.79f,0); //moves the panel off the screen (TEMPORARY FIX) and shows the button to open the graph info panel
        this.gameObject.SetActive(false); 
        algOpenPanel.gameObject.SetActive(true);
    }

    //activate the graphInfo panel and prevent access to the open panel button
    public void OpenAlgorithmInfoPanel(){
        //GetComponent<RectTransform>().position = new Vector3(500f,0,0); //moves the panel back onto the screen (TEMPORARY FIX) and make the open panel button not accessible
        //this.transform.position = new Vector3 (0f,293.79f,0);
        this.gameObject.SetActive(true);
        algOpenPanel.gameObject.SetActive(false);
    }

}
