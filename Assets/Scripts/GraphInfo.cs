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
        this.algorithmManager = new AlgorithmManager( Controller.Singleton.Graph, ( Action ) this.UpdateChromaticResult, ( Action ) this.UpdateBipartiteResult, ( Action ) this.UpdatePrimsResult, ( Action ) this.UpdateKruskalsResult, ( Action ) this.UpdateChromaticCalculating, ( Action ) this.UpdateBipartiteCalculating, ( Action ) this.UpdatePrimsCalculating, ( Action ) this.UpdateKruskalsCalculating );

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

    public void UpdateChromaticCalculating() {
        this.chromaticText.text = "Chromatic Number: Calculating";
    }

    public void UpdateBipartiteCalculating() {
        this.bipartiteText.text = "Bipartite: Calculating";
    }

    public void UpdatePrimsCalculating() { }

    public void UpdateKruskalsCalculating() { }
}
