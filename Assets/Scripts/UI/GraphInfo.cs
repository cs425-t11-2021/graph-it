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

    public Action OnCompleteUpdateUI
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

    public Action OnCalculatingUpdateUI
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
    
    // Reference to the text display of the graph order
    [SerializeField] private TMP_Text orderText;
    // Reference of the text display of the graph size
    [SerializeField] private TMP_Text sizeText;
    
    //Reference to the button to open the graph info panels
    [SerializeField] private Button openPanel;

    public void UpdateGraphInfoResults(Algorithm algorithm)
    {
        string algorithmName = algorithm.GetType().ToString();

        foreach (GraphInfoAlgorithmAssociation association in this.associations)
        {
            if (association.algorithmClass == algorithmName)
            {
                association.OnCompleteUpdateUI();
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
                association.OnCalculatingUpdateUI();
                return;
            }
        }
        
        Logger.Log("No algorithm association found for " + algorithmName + ".", this, LogType.ERROR);
    }

    public void InitiateAlgorithmManager(AlgorithmManager algoManager) {
        algoManager.Initiate(
            Controller.Singleton.Graph
            // ( Action ) this.UpdateMinDegreeResult,
            // ( Action ) this.UpdateMinDegreeCalculating,
            // ( Action ) this.UpdateMaxDegreeResult,
            // ( Action ) this.UpdateMaxDegreeCalculating,
            // ( Action ) this.UpdateRadiusResult,
            // ( Action ) this.UpdateRadiusCalculating,
            // ( Action ) this.UpdateDiameterResult,
            // ( Action ) this.UpdateDiameterCalculating,
            // ( Action ) this.UpdateChromaticResult,
            // ( Action ) this.UpdateChromaticCalculating,
            // ( Action ) this.UpdateBipartiteResult,
            // ( Action ) this.UpdateBipartiteCalculating,
            // ( Action ) this.UpdateCyclicResult,
            // ( Action ) this.UpdateCyclicCalculating,
            // ( Action ) this.UpdateFleurysResult,
            // ( Action ) this.UpdateFleurysCalculating,
            // ( Action ) AlgorithmsPanel.Singleton.UpdatePrimsResult,
            // ( Action ) AlgorithmsPanel.Singleton.UpdatePrimsCalculating,
            // ( Action ) AlgorithmsPanel.Singleton.UpdateKruskalsResult,
            // ( Action ) AlgorithmsPanel.Singleton.UpdateKruskalsCalculating,
            // ( Action ) this.UpdateDijkstrasResult,
            // ( Action ) this.UpdateDijkstrasCalculating,
            // ( Action ) this.UpdateBellmanFordsResult,
            // ( Action ) this.UpdateBellmanFordsCalculating,
            // ( Action ) AlgorithmsPanel.Singleton.UpdateDepthFirstSearchResult,
            // ( Action ) AlgorithmsPanel.Singleton.UpdateDepthFirstSearchCalculating,
            // ( Action ) AlgorithmsPanel.Singleton.UpdateBreadthFirstSearchResult,
            // ( Action ) AlgorithmsPanel.Singleton.UpdateBreadthFirstSearchCalculating
        );
        this.UpdateGraphInfo();
    }
    
    public void UpdateGraphInfo() {
        this.orderText.text = "Order: " + Controller.Singleton.Graph.Order;
        this.sizeText.text = "Size: " + Controller.Singleton.Graph.Size;

        // Run multithreaded algorithms
        Controller.Singleton.AlgorithmManager.RunChromatic();
        Controller.Singleton.AlgorithmManager.RunMinDegree();
        Controller.Singleton.AlgorithmManager.RunMaxDegree();
        Controller.Singleton.AlgorithmManager.RunRadius();
        Controller.Singleton.AlgorithmManager.RunDiameter();
        Controller.Singleton.AlgorithmManager.RunBipartite();
        Controller.Singleton.AlgorithmManager.RunCyclic();
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

}