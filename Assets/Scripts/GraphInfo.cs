//All code developed by Team 11
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraphInfo : MonoBehaviour
{
    // Singleton
    public static GraphInfo singleton;

    [SerializeField]
    private TMP_Text chromaticText;
    [SerializeField]
    private TMP_Text bipartiteText;
    [SerializeField]
    private TMP_Text orderText;
    [SerializeField]
    private TMP_Text sizeText;

    [SerializeField]
    private Button primButton;

    [SerializeField]
    private Button dijkstraButton;

    private ChromaticAlgorithm chromaticAlgorithm;

    private void Awake() {
        this.chromaticAlgorithm = new ChromaticAlgorithm(Controller.singleton.Graph, UpdateChromaticInfo);

        // Singleton pattern setup
        if (singleton == null) {
            singleton = this;
        }
        else {
            Debug.LogError("[GraphInfo] Singleton pattern violation");
            Destroy(this);
            return;
        }

        this.primButton.interactable = false;
        UpdateGraphInfo();
        
        
    }

    private void FixedUpdate() {
        // Only allow the prim button to be pressed if there is exactly one vertex selected
        if (SelectionManager.singleton.SelectedVertexCount() == 1 && SelectionManager.singleton.SelectedEdgeCount() == 0) {
            this.primButton.interactable = true;
        }   
        else {
            this.primButton.interactable = false;
        }

        // Only allow dijkstra if exactly two vertices are selected
        if (SelectionManager.singleton.SelectedVertexCount() == 2 && SelectionManager.singleton.SelectedEdgeCount() == 0) {
            this.dijkstraButton.interactable = true;
        }   
        else {
            this.dijkstraButton.interactable = false;
        }
    }

    public void UpdateGraphInfo() {
        // if (Controller.singleton.Graph.vertices.Count > 6) {
        //     chromaticText.text = "";
        //     bipartiteText.text = "";
        // }
        // else {
        //     int chromaticNum = Controller.singleton.Graph.GetChromaticNumber();
        //     this.chromaticText.text = "Chromatic Number: " + chromaticNum;
        //     this.bipartiteText.text = "Bipartite: " + (chromaticNum == 2 ? "Yes" : "No");
        // }

        this.orderText.text = "Order: " + Controller.singleton.Graph.vertices.Count;
        this.sizeText.text = "Size: " + Controller.singleton.Graph.adjacency.Count;

        this.chromaticText.text = "Chromatic Number: Calculating";
        this.bipartiteText.text = "Bipartite: Calculating";
        // Run multithreaded chromatic
        chromaticAlgorithm.RunThread();
    }

    public void UpdateChromaticInfo() {
        this.chromaticText.text = "Chromatic Number: " + chromaticAlgorithm.chromatic_number;
        this.bipartiteText.text = "Bipartite: " + (chromaticAlgorithm.chromatic_number == 2 ? "Yes" : "No");
    }
}
