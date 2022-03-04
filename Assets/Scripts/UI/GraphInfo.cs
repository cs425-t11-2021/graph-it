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
    //[SerializeField] private Button primButton;
    // Reference of the kruskal button
    //[SerializeField] private Button kruskalButton;
    // Reference of the button of dijkstra
    //[SerializeField] private Button dijkstraButton;

    // Property for whether or not the algorithm buttons are enabled
    /*public bool AlgorithmButtonsEnabled {
        set {
            primButton.enabled = value;
            dijkstraButton.enabled = value;
            kruskalButton.enabled = value;
        }
    }*/

    private void Awake() {
        AlgorithmManager.Singleton.Initiate( Controller.Singleton.Graph, ( Action ) this.UpdateMinDegreeResult, ( Action ) this.UpdateMaxDegreeResult, ( Action ) this.UpdateChromaticResult, ( Action ) this.UpdateBipartiteResult, ( Action ) AlgorithmsPanel.Singleton.UpdatePrimsResult, ( Action ) AlgorithmsPanel.Singleton.UpdateKruskalsResult, ( Action ) AlgorithmsPanel.Singleton.UpdateDepthFirstSearchResult, ( Action ) AlgorithmsPanel.Singleton.UpdateBreadthFirstSearchResult, ( Action ) this.UpdateMinDegreeCalculating, ( Action ) this.UpdateMaxDegreeCalculating, ( Action ) this.UpdateChromaticCalculating, ( Action ) this.UpdateBipartiteCalculating, ( Action ) AlgorithmsPanel.Singleton.UpdatePrimsCalculating, ( Action ) AlgorithmsPanel.Singleton.UpdateKruskalsCalculating, ( Action ) AlgorithmsPanel.Singleton.UpdateDepthFirstSearchCalculating, ( Action ) AlgorithmsPanel.Singleton.UpdateBreadthFirstSearchCalculating );
        this.UpdateGraphInfo();
    }
    
    public void UpdateGraphInfo() {
        this.orderText.text = "Order: " + Controller.Singleton.Graph.Order;
        this.sizeText.text = "Size: " + Controller.Singleton.Graph.Size;

        // Run multithreaded algorithms
        AlgorithmManager.Singleton.Clear();
        // this.algorithmManager.RunChromatic();
        AlgorithmManager.Singleton.RunMinDegree();
        AlgorithmManager.Singleton.RunMaxDegree();
        AlgorithmManager.Singleton.RunBipartite(); //TEMPORARY FIX
    }

    public void UpdateMinDegreeResult() { Debug.Log( "Min degree: " + AlgorithmManager.Singleton.GetMinDegree() ); } // temp

    public void UpdateMaxDegreeResult() { Debug.Log( "Max degree: " + AlgorithmManager.Singleton.GetMaxDegree() ); } // temp

    public void UpdateChromaticResult() {
        // Debug.Log("Running UpdateChromaticResult");
        int? chromaticNumber = AlgorithmManager.Singleton.GetChromaticNumber();
        if ( chromaticNumber is null )
            this.chromaticText.text = "Chromatic Number: Error";
        else
            this.chromaticText.text = "Chromatic Number: " + chromaticNumber;
    }

    public void UpdateBipartiteResult() {
        // Debug.Log("Running UpdateBipartiteResult");
        this.bipartiteText.text = "Bipartite: " + ( AlgorithmManager.Singleton.GetBipartite() ?? false ? "Yes" : "No" );
    }

    // public void UpdatePrimsResult() { }

    // public void UpdateKruskalsResult() { }

    // public void UpdateDepthFirstSearchResult() { }

    // public void UpdateBreadthFirstSearchResult() { }

    public void UpdateMinDegreeCalculating() { }

    public void UpdateMaxDegreeCalculating() { }

    public void UpdateChromaticCalculating() {
        this.chromaticText.text = "Chromatic Number: Calculating";
        // Debug.Log("Running UpdateChromaticCalculating");
    }

    public void UpdateBipartiteCalculating() {
        this.bipartiteText.text = "Bipartite: Calculating";
        // Debug.Log("Running UpdateBipartiteCalculating");
    }

    // public void UpdatePrimsCalculating() { }

    // public void UpdateKruskalsCalculating() { }

    // public void UpdateDepthFirstSearchCalculating() { }

    // public void UpdateBreadthFirstSearchCalculating() { }
}