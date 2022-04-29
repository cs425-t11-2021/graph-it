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
    private List<string> infoResults;

    public override void OnStateEnter()
    {
        this.currentStep =  AlgorithmsPanel.Singleton.CurrentStep;
        this.algorithmResult = AlgorithmsPanel.Singleton.AlgorithmResult;
        this.infoResults = new List<string>();

        AlgorithmsPanel.Singleton.stepByStepPanel.SetActive(true);

        foreach (EdgeObj edgeObj in Controller.Singleton.EdgeObjs)
        {
            bool contained = false;
            if (this.currentStep.considerEdges?.Contains(edgeObj.Edge) ?? false)
            {
                edgeObj.visualsAnimator.ChangeState("algorithm_considering");
                contained = true;
            }
            if (this.currentStep.resultEdges?.Contains(edgeObj.Edge) ?? false)
            {
                edgeObj.visualsAnimator.ChangeState("algorithm_result");
                contained = true;
            }
            if (this.currentStep.newEdges?.Contains(edgeObj.Edge) ?? false)
            {
                edgeObj.visualsAnimator.ChangeState("algorithm_new");
                contained = true;
            }
            
            if (!contained)
            {
                edgeObj.visualsAnimator.ChangeState("algorithm_none");
            }
        }
        
        foreach (VertexObj vertexObj in Controller.Singleton.VertexObjs)
        {
            bool contained = false;
            if (this.currentStep.considerVertices?.Contains(vertexObj.Vertex) ?? false)
            {
                vertexObj.visualsAnimator.ChangeState("algorithm_considering");
                contained = true;
            }
            if (this.currentStep.resultVertices?.Contains(vertexObj.Vertex) ?? false)
            {
                vertexObj.visualsAnimator.ChangeState("algorithm_result");
                contained = true;
            }
            if (this.currentStep.newVertices?.Contains(vertexObj.Vertex) ?? false)
            {
                vertexObj.visualsAnimator.ChangeState("algorithm_new");
                contained = true;
            }
            
            if (!contained)
            {
                vertexObj.visualsAnimator.ChangeState("algorithm_none");
            }
        }
        
        foreach (KeyValuePair<string, (object, Type)> kvp in this.algorithmResult.results)
        {
            string resultID = kvp.Key;
            Type resultType = kvp.Value.Item2;

            if (resultType == typeof(float) || resultType == typeof(int) || resultType == typeof(bool))
            {
                this.infoResults.Add(resultID.ToTitleCase() + ": " + Convert.ToString(kvp.Value.Item1));
            }
        }
        
        if (!AlgorithmsPanel.Singleton.ExtraInfoClosed) {
            if (this.infoResults.Count > 0)
            {
                AlgorithmsPanel.Singleton.extraInfoPanel.GetComponentInChildren<TMP_Text>(true).text = AlgorithmsPanel.Singleton.CurrentlySelectedAlgorithm.displayName + " Additional Info:";
                string output = "";
                this.infoResults.ForEach(s => output += s + "\n");
                AlgorithmsPanel.Singleton.extraInfoPanel.GetComponentInChildren<TMP_InputField>(true).text = output;
                AlgorithmsPanel.Singleton.extraInfoPanel.SetActive(true);
            }
        }
    }

    public override void OnStateExit()
    {
        Controller.Singleton.EdgeObjs.ForEach(e => e.visualsAnimator.ChangeState("default"));
        Controller.Singleton.VertexObjs.ForEach(v => v.visualsAnimator.ChangeState("default"));

        AlgorithmsPanel.Singleton.stepByStepPanel.SetActive(false);
        AlgorithmsPanel.Singleton.extraInfoPanel.SetActive(false);
    }
}
