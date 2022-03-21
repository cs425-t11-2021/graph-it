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
    public Button activationButton;
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
                    
                    List<Edge> resultEdges = (List<Edge>) result;
                    List<Vertex> resultVertices = new List<Vertex>();
                    foreach (Edge e in resultEdges) {
                        resultVertices.Add(e.vert1);
                        resultVertices.Add(e.vert2);
                    }
                    
                    foreach (EdgeObj edgeObj in Controller.Singleton.EdgeObjs)
                    {
                        if (resultEdges.Contains(edgeObj.Edge))
                            SelectionManager.Singleton.SelectEdge(edgeObj);
                    }
                    
                    // ((List<EdgeObj>) Controller.Singleton.EdgeObjs.Where((e) => resultEdges.Contains(e.Edge))).ForEach(e => SelectionManager.Singleton.SelectEdge(e));

                    foreach (VertexObj vertexObj in Controller.Singleton.VertexObjs)
                    {
                        if (resultVertices.Contains(vertexObj.Vertex))
                            SelectionManager.Singleton.SelectVertex(vertexObj);
                    }
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

    // Property for whether or not the algorithm buttons are enabled
    public bool AlgorithmButtonsEnabled
    {
        get; set;
    }

    private void Awake() {
        SelectionManager.Singleton.OnSelectionChange += OnSelectionChange;

        foreach (GraphDisplayAlgorithmAssociation association in this.associations)
        {
            association.activationButton.interactable = false;
        }
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
            if (association.requiredEdges != selectedEdgeCount || association.requiredVertices != selectedVertexCount)
            {
                association.activationButton.interactable = false;
            }
            else
            {
                association.activationButton.interactable = true;
            }
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

    public void RunGraphDisplayAlgorithm(string algorithmName)
    {
        foreach (GraphDisplayAlgorithmAssociation association in this.associations)
        {
            if (association.algorithmClass == algorithmName)
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
                return;
            }
        }
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
