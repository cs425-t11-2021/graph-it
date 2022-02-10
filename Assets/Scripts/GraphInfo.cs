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

    // Reference to the multithreaded chromatic algorithm
    private ChromaticAlgorithm chromaticAlgorithm;

    // Property for whether or not the algorithm buttons are enabled
    public bool AlgorithmButtonsEnabled {
        set {
            primButton.enabled = value;
            dijkstraButton.enabled = value;
            kruskalButton.enabled = value;
        }
    }

    private void Awake() {
        this.chromaticAlgorithm = new ChromaticAlgorithm(Controller.Singleton.Graph, UpdateChromaticInfo);
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

        this.chromaticText.text = "Chromatic Number: Calculating";
        this.bipartiteText.text = "Bipartite: Calculating";
        // Run multithreaded chromatic
        chromaticAlgorithm.RunThread();
    }

    public void UpdateChromaticInfo() {
        Logger.Log("Updated chromatic number and bipartite.", this, LogType.INFO);
        this.chromaticText.text = "Chromatic Number: " + chromaticAlgorithm.ChromaticNumber;
        this.bipartiteText.text = "Bipartite: " + (chromaticAlgorithm.ChromaticNumber == 2 ? "Yes" : "No");
    }
}
