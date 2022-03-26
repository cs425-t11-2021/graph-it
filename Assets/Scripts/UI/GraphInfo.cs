
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
    }

    public void InitiateAlgorithmManager(AlgorithmManager algoManager) {
        algoManager.Initiate(
            Controller.Singleton.Graph
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
        this.gameObject.SetActive(false); 
        openPanel.gameObject.SetActive(true);
    }

    //activate the graphInfo panel and prevent access to the open panel button
    public void OpenGraphInfoPanel(){
        this.gameObject.SetActive(true);
        openPanel.gameObject.SetActive(false);
    }

}