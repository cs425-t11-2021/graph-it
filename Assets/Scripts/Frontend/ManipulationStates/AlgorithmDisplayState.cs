using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class AlgorithmDisplayState : ManipulationState
{

    private AlgorithmResult algorithmResult;
    private List<EdgeObj> highlightedEdges;
    private List<VertexObj> highlightedVertices;
    private List<string> infoResults;

    public override void OnStateEnter()
    {
        this.algorithmResult = AlgorithmsPanel.Singleton.AlgorithmResult;
        this.highlightedEdges = new List<EdgeObj>();
        this.highlightedVertices = new List<VertexObj>();
        this.infoResults = new List<string>();

        foreach (KeyValuePair<string, (object, Type)> kvp in this.algorithmResult.results)
        {
            string resultID = kvp.Key;
            Type resultType = kvp.Value.Item2;

            if (resultType == typeof(List<Vertex>))
            {
                List<Vertex> edgeList = (List<Vertex>) kvp.Value.Item1;
                foreach (VertexObj vertexObj in Controller.Singleton.VertexObjs)
                {
                    if (edgeList.Contains(vertexObj.Vertex))
                    {
                        vertexObj.AlgorithmResultLevel = 3;
                        this.highlightedVertices.Add(vertexObj);
                    }
                }
            }
            else if (resultType == typeof(List<Edge>))
            {
                List<Edge> edgeList = (List<Edge>) kvp.Value.Item1;
                foreach (EdgeObj edgeObj in Controller.Singleton.EdgeObjs)
                {
                    if (edgeList.Contains(edgeObj.Edge))
                    {
                        edgeObj.AlgorithmResultLevel = 3;
                        this.highlightedEdges.Add(edgeObj);
                    }
                }
            }
            else if (resultType == typeof(float) || resultType == typeof(int))
            {
                this.infoResults.Add(resultID.ToTitleCase() + ": " + Convert.ToString(kvp.Value.Item1));
            }
        }

        if (this.infoResults.Count > 0)
        {
            AlgorithmsPanel.Singleton.extraInfoPanel.GetComponentInChildren<TMP_Text>(true).text = AlgorithmsPanel.Singleton.CurrentlySelectedAlgorithm.displayName + " Additional Info:";
            string output = "";
            this.infoResults.ForEach(s => output += s + "\n");
            AlgorithmsPanel.Singleton.extraInfoPanel.GetComponentInChildren<TMP_InputField>(true).text = output;
            AlgorithmsPanel.Singleton.extraInfoPanel.SetActive(true);
        }
    }

    public override void OnStateExit()
    {
        this.highlightedEdges.ForEach(e => e.AlgorithmResultLevel = 0);
        this.highlightedVertices.ForEach(v => v.AlgorithmResultLevel = 0);
        
        AlgorithmsPanel.Singleton.extraInfoPanel.GetComponentInChildren<TMP_InputField>(true).text = "";
        AlgorithmsPanel.Singleton.extraInfoPanel.SetActive(false);
    }
}
