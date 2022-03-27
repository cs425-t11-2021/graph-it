
//All code developed by Team 11

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.Reflection;
using Object = System.Object;

[System.Serializable]
public class GraphInfoAlgorithmAssociation
{
    public string algorithmClass = "";
    public bool enabled = false;
    public string lead = "";
    public string nullValue = "N/A";
    public string activationMethod = "";
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
                    GraphInfo.Singleton.SetInfoAlgorithmResult(this, lead + ": " + nullValue);
                }
                else
                {
                    GraphInfo.Singleton.SetInfoAlgorithmResult( this, lead + ": " + Convert.ToString(result));
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
                GraphInfo.Singleton.SetInfoAlgorithmResult(this, lead + ": ...");
            };
        }
    }
}

public class GraphInfo : SingletonBehavior<GraphInfo>
{

    [SerializeField] private GraphInfoAlgorithmAssociation[] associations;
    
    // // Reference to the text display of the graph order
    // [SerializeField] private TMP_InputField orderText;
    // // Reference of the text display of the graph size
    // [SerializeField] private TMP_InputField sizeText;
    
    //Reference to the button to open the graph info panels
    [SerializeField] private Button openPanel;
    [SerializeField] private TMP_InputField graphInfoField;
    private ConcurrentDictionary<GraphInfoAlgorithmAssociation, string> infoAlgorithmResults;

    private void Awake()
    {
        this.infoAlgorithmResults = new ConcurrentDictionary<GraphInfoAlgorithmAssociation, string>();

        foreach (GraphInfoAlgorithmAssociation association in this.associations)
        {
            this.infoAlgorithmResults[association] = association.lead + ":";
        }
    }

    public void UpdateGraphInfoResults(Algorithm algorithm, AlgorithmManager algoMan)
    {
        // Fix for #126
        if (algoMan != Controller.Singleton.AlgorithmManager)
        {
            return;
        }
        
        string algorithmName = algorithm.GetType().ToString();

        foreach (GraphInfoAlgorithmAssociation association in this.associations)
        {
            if (association.algorithmClass == algorithmName)
            {
                association.OnCompleteUpdateUI();
                RefreshGraphInfoUI();
                return;
            }
        }
    }

    public void UpdateGraphInfoCalculating(Algorithm algorithm, AlgorithmManager algoMan)
    {
        // Fix for #126
        if (algoMan != Controller.Singleton.AlgorithmManager)
        {
            return;
        }
        
        // Fix strange race condition
        if (algorithm == null) return;
        
        string algorithmName = algorithm.GetType().ToString();

        foreach (GraphInfoAlgorithmAssociation association in this.associations)
        {
            if (association.algorithmClass == algorithmName)
            {
                association.OnCalculatingUpdateUI();
                RefreshGraphInfoUI();
                return;
            }
        }        
    }

    public void SetInfoAlgorithmResult(GraphInfoAlgorithmAssociation association, string result)
    {
        this.infoAlgorithmResults[association] = result;
    }

    public void InitiateAlgorithmManager(AlgorithmManager algoManager) {
        algoManager.Initiate(
            Controller.Singleton.Graph
        );
        this.UpdateGraphInfo();
    }
    
    public void UpdateGraphInfo() {
        // Run multithreaded algorithms
        foreach (GraphInfoAlgorithmAssociation association in this.associations)
        {
            this.infoAlgorithmResults[association] = association.lead + ":";
            if (association.enabled)
            {
                Type.GetType("AlgorithmManager").GetMethod(association.activationMethod)
                    .Invoke(Controller.Singleton.AlgorithmManager, new Object[] {true});
            }
        }
    }

    private void RefreshGraphInfoUI()
    {
        string output = "";
        output += "Order: " + Controller.Singleton.Graph.Order + "\n";
        output += "Size: " + Controller.Singleton.Graph.Size + "\n";
        foreach (GraphInfoAlgorithmAssociation association in this.associations)
        {
            if (association.enabled)
            {
                output += this.infoAlgorithmResults[association] + "\n";
            }
        }

        graphInfoField.text = output;
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