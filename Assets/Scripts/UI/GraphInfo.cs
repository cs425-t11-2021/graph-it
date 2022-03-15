//All code developed by Team 11
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.Reflection;

[System.Serializable]
public class GraphInfoAlgorithmAssociation
{
    public string algorithmClass = "";
    public bool multiThreaded = false;
    public TMP_Text graphInfoText;
    public string lead = "";
    public string nullValue = "N/A";
    public string completedMethod = "";

    public Action OnUICompleteAction
    {
        get
        {
            return () =>
            {
                object result = Type.GetType("AlgorithmManager").GetMethod(completedMethod)
                    .Invoke(Controller.Singleton.AlgorithmManager, null);
                
                if (result == null)
                {
                    graphInfoText.text = lead + ": " + nullValue;
                }
                else
                {
                    graphInfoText.text = lead + ": " + Convert.ToString(result);
                }
            };
        }
    }

    public Action OnUICalculatingAction
    {
        get
        {
            return () =>
            {
                graphInfoText.text = lead + ": ...";
            };
        }
    }
}

public class GraphInfo : SingletonBehavior<GraphInfo>
{

    [SerializeField] private GraphInfoAlgorithmAssociation[] associations;
    
    // // Reference to the text display of chromatic number
    // [SerializeField] private TMP_Text chromaticText;
    // // Reference to the text display of bipartite
    // [SerializeField] private TMP_Text bipartiteText;
    // Reference to the text display of the graph order
    [SerializeField] private TMP_Text orderText;
    // Reference of the text display of the graph size
    [SerializeField] private TMP_Text sizeText;
    // Reference of the text display of the graph size
    // [SerializeField] private TMP_Text minDegreeText;
    // // Reference of the text display of the minimum degree
    // [SerializeField] private TMP_Text maxDegreeText;
    // // Reference of the text display of the maximmum degree
    // [SerializeField] private TMP_Text radiusText;
    // // Reference to the text display of radius
    // [SerializeField] private TMP_Text diameterText;
    // // Reference to the text display of diameter
    // [SerializeField] private TMP_Text cyclicText;
    // Reference to the text display of cyclic

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

    public void UpdateGraphInfoResults(Algorithm algorithm)
    {
        string algorithmName = algorithm.GetType().ToString();

        foreach (GraphInfoAlgorithmAssociation association in this.associations)
        {
            if (association.algorithmClass == algorithmName)
            {
                association.OnUICompleteAction();
                return;
            }
        }
    }

    public void UpdateGraphInfoCalculating(Algorithm algorithm)
    {
        string algorithmName = algorithm.GetType().ToString();

        foreach (GraphInfoAlgorithmAssociation association in this.associations)
        {
            if (association.algorithmClass == algorithmName)
            {
                association.OnUICalculatingAction();
                return;
            }
        }
    }

    public void InitiateAlgorithmManager(AlgorithmManager algoManager) {
        algoManager.Initiate(
            Controller.Singleton.Graph,
            ( Action ) AlgorithmsPanel.Singleton.UpdatePrimsResult,
            ( Action ) AlgorithmsPanel.Singleton.UpdateKruskalsResult,
            ( Action ) AlgorithmsPanel.Singleton.UpdateDepthFirstSearchResult,
            ( Action ) AlgorithmsPanel.Singleton.UpdateBreadthFirstSearchResult,
            ( Action ) AlgorithmsPanel.Singleton.UpdatePrimsCalculating,
            ( Action ) AlgorithmsPanel.Singleton.UpdateKruskalsCalculating,
            ( Action ) AlgorithmsPanel.Singleton.UpdateDepthFirstSearchCalculating,
            ( Action ) AlgorithmsPanel.Singleton.UpdateBreadthFirstSearchCalculating
        );
        this.UpdateGraphInfo();
    }
    
    public void UpdateGraphInfo() {
        this.orderText.text = "Order: " + Controller.Singleton.Graph.Order;
        this.sizeText.text = "Size: " + Controller.Singleton.Graph.Size;

        // Run multithreaded algorithms
        Controller.Singleton.AlgorithmManager.RunMinDegree();
        Controller.Singleton.AlgorithmManager.RunMaxDegree();
        Controller.Singleton.AlgorithmManager.RunRadius();
        Controller.Singleton.AlgorithmManager.RunDiameter();
        Controller.Singleton.AlgorithmManager.RunBipartite(); //TEMPORARY FIX
        Controller.Singleton.AlgorithmManager.RunCyclic();
    }

    // public void UpdatePrimsResult() { }

    // public void UpdateKruskalsResult() { }

    // public void UpdateDepthFirstSearchResult() { }

    // public void UpdateBreadthFirstSearchResult() { }

    public void UpdateBellmanFordResult()
    {
        
    }

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

    // public void UpdatePrimsCalculating() { }

    // public void UpdateKruskalsCalculating() { }

    // public void UpdateDepthFirstSearchCalculating() { }

    // public void UpdateBreadthFirstSearchCalculating() { }
}