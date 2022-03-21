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
    // Reference of the text display of the graph size
    [SerializeField] private TMP_Text minDegreeText;
    // Reference of the text display of the minimum degree
    [SerializeField] private TMP_Text maxDegreeText;
    // Reference of the text display of the maximmum degree
    //[SerializeField] private Button closePanel;
    //Reference to the button to close the graph info panels
    [SerializeField] private Button openPanel;
    //Reference to the button to open the graph info panels

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

    public void InitiateAlgorithmManager() {
        Controller.Singleton.AlgorithmManager.Initiate(
            Controller.Singleton.Graph,
            ( Action ) this.UpdateMinDegreeResult,
            ( Action ) this.UpdateMinDegreeCalculating,
            ( Action ) this.UpdateMaxDegreeResult,
            ( Action ) this.UpdateMaxDegreeCalculating,
            ( Action ) this.UpdateRadiusResult,
            ( Action ) this.UpdateRadiusCalculating,
            ( Action ) this.UpdateDiameterResult,
            ( Action ) this.UpdateDiameterCalculating,
            ( Action ) this.UpdateChromaticResult,
            ( Action ) this.UpdateChromaticCalculating,
            ( Action ) this.UpdateBipartiteResult,
            ( Action ) this.UpdateBipartiteCalculating,
            ( Action ) this.UpdateCyclicResult,
            ( Action ) this.UpdateCyclicCalculating,
            ( Action ) this.UpdateFleurysResult,
            ( Action ) this.UpdateFleurysCalculating,
            ( Action ) AlgorithmsPanel.Singleton.UpdatePrimsResult,
            ( Action ) AlgorithmsPanel.Singleton.UpdatePrimsCalculating,
            ( Action ) AlgorithmsPanel.Singleton.UpdateKruskalsResult,
            ( Action ) AlgorithmsPanel.Singleton.UpdateKruskalsCalculating,
            ( Action ) AlgorithmsPanel.Singleton.UpdateDepthFirstSearchResult,
            ( Action ) AlgorithmsPanel.Singleton.UpdateDepthFirstSearchCalculating,
            ( Action ) AlgorithmsPanel.Singleton.UpdateBreadthFirstSearchResult,
            ( Action ) AlgorithmsPanel.Singleton.UpdateBreadthFirstSearchCalculating
        );
        this.UpdateGraphInfo();
    }
    
    public void UpdateGraphInfo() {
        this.orderText.text = "Order: " + Controller.Singleton.Graph.Order;
        this.sizeText.text = "Size: " + Controller.Singleton.Graph.Size;

        // Run multithreaded algorithms
        // Controller.Singleton.AlgorithmManager.Clear();
        Controller.Singleton.AlgorithmManager.RunChromatic();
        Controller.Singleton.AlgorithmManager.RunMinDegree();
        Controller.Singleton.AlgorithmManager.RunMaxDegree();
        Controller.Singleton.AlgorithmManager.RunBipartite(); //TEMPORARY FIX
    }

    public void DisplayGraphInfo()
    {
        UpdateChromaticResult();
        UpdateBipartiteResult();
        UpdateMinDegreeResult();
        UpdateMaxDegreeResult();
    }

    public void UpdateMinDegreeResult() { 
        // Debug.Log( "Min degree: " + AlgorithmManager.Singleton.GetMinDegree() ); 
        this.minDegreeText.text = "Minimum Degree (δ): " + Controller.Singleton.AlgorithmManager.GetMinDegree();
    }

    public void UpdateMaxDegreeResult() { 
        // Debug.Log( "Max degree: " + AlgorithmManager.Singleton.GetMaxDegree() ); 
        this.maxDegreeText.text = "Maximum Degree (Δ): " + Controller.Singleton.AlgorithmManager.GetMaxDegree();
    }

    public void UpdateChromaticResult() {
        Debug.Log("Running UpdateChromaticResult");
        int? chromaticNumber = Controller.Singleton.AlgorithmManager.GetChromaticNumber();
        if ( chromaticNumber is null )
            this.chromaticText.text = "Chromatic Number: Error";
        else
            this.chromaticText.text = "Chromatic Number: " + chromaticNumber;
    }

    public void UpdateBipartiteResult() {
        Debug.Log("Running UpdateBipartiteResult");
        this.bipartiteText.text = "Bipartite: " + ( Controller.Singleton.AlgorithmManager.GetBipartite() ?? false ? "Yes" : "No" );
    }

    public void UpdateCyclicResult() {
        this.cyclicText.text = "Cyclic: " + (Controller.Singleton.AlgorithmManager.GetCyclic() ?? false ? "Yes" : "No");
    }

    public void UpdateFleurysResult() { }

    // public void UpdatePrimsResult() { }

    // public void UpdateKruskalsResult() { }

    // public void UpdateDepthFirstSearchResult() { }

    // public void UpdateBreadthFirstSearchResult() { }

    public void UpdateBellmanFordResult() { }

    public void UpdateMinDegreeCalculating() { }

    public void UpdateMaxDegreeCalculating() { }

    public void UpdateRadiusCalculating() { }

    public void UpdateDiameterCalculating() { }

    public void UpdateChromaticCalculating() {
        this.chromaticText.text = "Chromatic Number: Calculating";
        // Debug.Log("Running UpdateChromaticCalculating");
    }

    public void UpdateBipartiteCalculating() {
        this.bipartiteText.text = "Bipartite: Calculating";
        // Debug.Log("Running UpdateBipartiteCalculating");
    }

    public void UpdateCyclicCalculating() { }

    public void UpdateFleurysCalculating() { }

    // public void UpdatePrimsCalculating() { }

    // public void UpdateKruskalsCalculating() { }

    // public void UpdateDepthFirstSearchCalculating() { }

    // public void UpdateBreadthFirstSearchCalculating() { }

    //deactivate the graphInfo panel and display the open panel button for the user to access
    public void CloseGraphInfoPanel(){
        //GetComponent<RectTransform>().position = new Vector3(-577.1f,293.79f,0); //moves the panel off the screen (TEMPORARY FIX) and shows the button to open the graph info panel
        this.gameObject.SetActive(false); 
        openPanel.gameObject.SetActive(true);
    }

    //activate the graphInfo panel and prevent access to the open panel button
    public void OpenGraphInfoPanel(){
        //GetComponent<RectTransform>().position = new Vector3(500f,0,0); //moves the panel back onto the screen (TEMPORARY FIX) and make the open panel button not accessible
        //this.transform.position = new Vector3 (0f,293.79f,0);
        this.gameObject.SetActive(true);
        openPanel.gameObject.SetActive(false);
    }

}