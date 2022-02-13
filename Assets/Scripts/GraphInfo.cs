//All code developed by Team 11
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraphInfo : SingletonBehavior<GraphInfo>
{

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

    private AlgorithmManager algorithmManager;
    // private ChromaticAlgorithm chromaticAlgorithm;

    private void Awake() {
        this.algorithmManager = new AlgorithmManager( Controller.Singleton.Graph, ( Action ) this.UpdateChromaticInfo, ( Action ) this.UpdatePrimsInfo, ( Action ) this.UpdateKruskalsInfo );
        // this.chromaticAlgorithm = new ChromaticAlgorithm(Controller.Singleton.Graph, UpdateChromaticInfo, this.algorithmManager.MarkRunning, this.algorithmManager.MarkComplete );

        this.primButton.interactable = false;
        UpdateGraphInfo();
    }

    private void FixedUpdate() {
        // Only allow the prim button to be pressed if there is exactly one vertex selected
        if (SelectionManager.Singleton.SelectedVertexCount() == 1 && SelectionManager.Singleton.SelectedEdgeCount() == 0) {
            this.primButton.interactable = true;
        }   
        else {
            this.primButton.interactable = false;
        }

        // Only allow dijkstra if exactly two vertices are selected
        if (SelectionManager.Singleton.SelectedVertexCount() == 2 && SelectionManager.Singleton.SelectedEdgeCount() == 0) {
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

        this.orderText.text = "Order: " + Controller.Singleton.Graph.vertices.Count;
        this.sizeText.text = "Size: " + Controller.Singleton.Graph.adjacency.Count;

        this.chromaticText.text = "Chromatic Number: Calculating";
        this.bipartiteText.text = "Bipartite: Calculating";
        // Run multithreaded chromatic
        // chromaticAlgorithm.RunThread();
        this.algorithmManager.RunChromatic();
    }

    public void UpdateChromaticInfo() {
        int? chromaticNumber = this.algorithmManager.GetChromaticNumber();
        if ( !( chromaticNumber is null ) )
        {
            this.chromaticText.text = "Chromatic Number: " + chromaticNumber;
            this.bipartiteText.text = "Bipartite: " + ( chromaticNumber == 2 ? "Yes" : "No" ); // temp until BipartiteAlgorithm
        }
    }

    public void UpdatePrimsInfo() { } // temp for algorithm manager

    public void UpdateKruskalsInfo() { } // temp for algorithm manager
}
