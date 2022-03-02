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
        int? chromaticNumber = Controller.Singleton.AlgorithmManager.GetChromaticNumber();
        if ( chromaticNumber is null )
            this.chromaticText.text = "Chromatic Number: Error";
        else
            this.chromaticText.text = "Chromatic Number: " + chromaticNumber;
    }

    public void UpdateBipartiteResult() {
        this.bipartiteText.text = "Bipartite: " + ( Controller.Singleton.AlgorithmManager.GetBipartite() ?? false ? "Yes" : "No" );
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
}
