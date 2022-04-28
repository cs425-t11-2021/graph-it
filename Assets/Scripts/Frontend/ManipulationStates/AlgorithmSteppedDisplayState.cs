using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AlgorithmSteppedDisplayState : ManipulationState
{
    private AlgorithmResult algorithmResult;
    private AlgorithmStep currentStep;
    private List<EdgeObj> highlightedEdges;
    private List<VertexObj> highlightedVertices;
    private List<string> infoResults;

    public override void OnStateEnter()
    {
        this.currentStep =  AlgorithmsPanel.Singleton.CurrentStep;
        this.algorithmResult = AlgorithmsPanel.Singleton.AlgorithmResult;
        this.highlightedEdges = new List<EdgeObj>();
        this.highlightedVertices = new List<VertexObj>();
        this.infoResults = new List<string>();

        AlgorithmsPanel.Singleton.stepByStepPanel.SetActive(true);

        foreach (EdgeObj edgeObj in Controller.Singleton.EdgeObjs)
        {
            bool contained = false;
            if (this.currentStep.considerEdges?.Contains(edgeObj.Edge) ?? false)
            {
                edgeObj.AlgorithmResultLevel = 1;
                this.highlightedEdges.Add(edgeObj);
                contained = true;
            }
            if (this.currentStep.resultEdges?.Contains(edgeObj.Edge) ?? false)
            {
                edgeObj.AlgorithmResultLevel = 3;
                this.highlightedEdges.Add(edgeObj);
                contained = true;
            }
            if (this.currentStep.newEdges?.Contains(edgeObj.Edge) ?? false)
            {
                edgeObj.AlgorithmResultLevel = 2;
                this.highlightedEdges.Add(edgeObj);
                contained = true;
            }
            
            if (!contained)
            {
                edgeObj.AlgorithmResultLevel = 0;
            }
        }
        
        foreach (VertexObj vertexObj in Controller.Singleton.VertexObjs)
        {
            bool contained = false;
            if (this.currentStep.considerVertices?.Contains(vertexObj.Vertex) ?? false)
            {
                vertexObj.AlgorithmResultLevel = 1;
                this.highlightedVertices.Add(vertexObj);
                contained = true;
            }
            if (this.currentStep.resultVertices?.Contains(vertexObj.Vertex) ?? false)
            {
                vertexObj.AlgorithmResultLevel = 3;
                this.highlightedVertices.Add(vertexObj);
                contained = true;
            }
            if (this.currentStep.newVertices?.Contains(vertexObj.Vertex) ?? false)
            {
                vertexObj.AlgorithmResultLevel = 2;
                this.highlightedVertices.Add(vertexObj);
                contained = true;
            }
            
            if (!contained)
            {
                vertexObj.AlgorithmResultLevel = 0;
            }
        }
        
        foreach (KeyValuePair<string, (object, Type)> kvp in this.algorithmResult.results)
        {
            string resultID = kvp.Key;
            Type resultType = kvp.Value.Item2;

            if (resultType == typeof(float) || resultType == typeof(int))
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

        AlgorithmsPanel.Singleton.stepByStepPanel.SetActive(false);
    }
}
