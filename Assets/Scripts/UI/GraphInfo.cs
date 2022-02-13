//All code developed by Team 11
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraphInfo : SingletonBehavior<GraphInfo>
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

    private AlgorithmManager algorithmManager;

    // Property for whether or not the algorithm buttons are enabled
    public bool AlgorithmButtonsEnabled {
        set {
            primButton.enabled = value;
            dijkstraButton.enabled = value;
            kruskalButton.enabled = value;
        }
    }

    private void Awake() {
        this.algorithmManager = new AlgorithmManager( Controller.Singleton.Graph, ( Action ) this.UpdateChromaticResult, ( Action ) this.UpdateBipartiteResult, ( Action ) this.UpdatePrimsResult, ( Action ) this.UpdateKruskalsResult, ( Action ) this.UpdateDepthFirstSearchResult, ( Action ) this.UpdateBreadthFirstSearchResult, ( Action ) this.UpdateChromaticCalculating, ( Action ) this.UpdateBipartiteCalculating, ( Action ) this.UpdatePrimsCalculating, ( Action ) this.UpdateKruskalsCalculating, ( Action ) this.UpdateDepthFirstSearchCalculating, ( Action ) this.UpdateBreadthFirstSearchCalculating );
        SelectionManager.Singleton.OnSelectionChange += OnSelectionChange;

        this.primButton.interactable = false;
        UpdateGraphInfo();
    }

    // Function called when the selection is changed
    private void OnSelectionChange(int selectedVertexCount, int selectedEdgeCount) {
        // Only allow the prim button to be pressed if there is exactly one vertex selected
        this.primButton.interactable = selectedVertexCount == 1 && selectedEdgeCount == 0;
        // Only allow dijkstra if exactly two vertices are selected
        this.dijkstraButton.interactable = selectedVertexCount == 2 && selectedEdgeCount == 0;
    }
    
    public void UpdateGraphInfo() {
        this.orderText.text = "Order: " + Controller.Singleton.Graph.vertices.Count;
        this.sizeText.text = "Size: " + Controller.Singleton.Graph.adjacency.Count;

        // Run multithreaded chromatic
        // this.algorithmManager.RunChromatic();
        this.algorithmManager.RunBipartite();
    }

    public void UpdateChromaticResult() {
        int? chromaticNumber = this.algorithmManager.GetChromaticNumber();
        if ( chromaticNumber is null )
            this.chromaticText.text = "Chromatic Number: Error";
        else
            this.chromaticText.text = "Chromatic Number: " + chromaticNumber;
    }

    public void UpdateBipartiteResult() {
        this.bipartiteText.text = "Bipartite: " + ( this.algorithmManager.GetBipartite() ?? false ? "Yes" : "No" );
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
