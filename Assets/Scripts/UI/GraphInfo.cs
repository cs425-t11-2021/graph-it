
//All code developed by Team 11

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.Reflection;
using Object = System.Object;

public class GraphInfo : SingletonBehavior<GraphInfo>
{

    [SerializeField] private GraphInfoAlgorithmAssociation[] associations;

    //Reference to the button to open the graph info panels
    [SerializeField] private Button openPanel;
    [SerializeField] private TMP_InputField graphInfoField;
    private ConcurrentDictionary<GraphInfoAlgorithmAssociation, string> infoAlgorithmResults;

    private void Awake()
    {
        this.infoAlgorithmResults = new ConcurrentDictionary<GraphInfoAlgorithmAssociation, string>();
        
        Logger.Log("Loading currently enabled graph info algorithms.", this, LogType.INFO);
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

    public void UpdateGraphInfoEstimate(Algorithm algorithm, AlgorithmManager algoMan)
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
                association.OnEstimateUpdateUI();
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
            this.infoAlgorithmResults[association] = association.lead + ": ...";
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