//All code developed by Team 11
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlgorithmsPanel : SingletonBehavior<AlgorithmsPanel>
{
    // Reference to the text display of chromatic number
    [SerializeField] private TMP_Text chromaticText;
    // Reference to the text display of bipartite
    [SerializeField] private TMP_Text bipartiteText;
    // Reference to the text display of the graph order
    [SerializeField] private TMP_Text orderText;
    // Reference of the text display of the graph size
    [SerializeField] private TMP_Text sizeText;
    // Reference of the button of prim
    [SerializeField] private Button primButton;
    // Reference of the kruskal button
    [SerializeField] private Button kruskalButton;
    // Reference of the button of dijkstra
    [SerializeField] private Button dijkstraButton;
    //[SerializeField] private Button algClosePanel;
    //Reference to the button to close the algorithm info panels
    [SerializeField] private Button algOpenPanel;
    //Reference to the button to open the algorithm info panels
    // Start is called before the first frame update

    // public AlgorithmManager algorithmManager;

     // Property for whether or not the algorithm buttons are enabled
    public bool AlgorithmButtonsEnabled {
        set {
            primButton.enabled = value;
            dijkstraButton.enabled = value;
            kruskalButton.enabled = value;
        }
    }

    private void Awake() {
        SelectionManager.Singleton.OnSelectionChange += OnSelectionChange;
        this.primButton.interactable = false;
    }

    // Function called when the selection is changed
    private void OnSelectionChange(int selectedVertexCount, int selectedEdgeCount) {
        // Only allow the prim button to be pressed if there is exactly one vertex selected
        this.primButton.interactable = selectedVertexCount == 1 && selectedEdgeCount == 0;
        // Only allow dijkstra if exactly two vertices are selected
        this.dijkstraButton.interactable = selectedVertexCount == 2 && selectedEdgeCount == 0;
    }
    
    /*public void UpdateGraphInfo() {
        this.orderText.text = "Order: " + Controller.Singleton.Graph.Vertices.Count;
        this.sizeText.text = "Size: " + Controller.Singleton.Graph.Adjacency.Count;

        // Run multithreaded chromatic
        // this.algorithmManager.RunChromatic();
        this.algorithmManager.RunBipartite();
    }*/

    public void UpdateChromaticResult() {
        int? chromaticNumber = AlgorithmManager.Singleton.GetChromaticNumber();
        if ( chromaticNumber is null )
            this.chromaticText.text = "Chromatic Number: Error";
        else
            this.chromaticText.text = "Chromatic Number: " + chromaticNumber;
    }

    public void UpdateBipartiteResult() {
        this.bipartiteText.text = "Bipartite: " + ( AlgorithmManager.Singleton.GetBipartite() ?? false ? "Yes" : "No" );
    }

    public void UpdatePrimsResult() { }

    public void UpdateKruskalsResult() { }

    public void UpdateDepthFirstSearchResult() { }

    public void UpdateBreadthFirstSearchResult() { }

    public void UpdateChromaticCalculating() {
        this.chromaticText.text = "Chromatic Number: Calculating";
    }

    public void UpdateBipartiteCalculating() {
        this.bipartiteText.text = "Bipartite: Calculating";
    }

    public void UpdatePrimsCalculating() { }

    public void UpdateKruskalsCalculating() { }

    public void UpdateDepthFirstSearchCalculating() { }

    public void UpdateBreadthFirstSearchCalculating() { }

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
