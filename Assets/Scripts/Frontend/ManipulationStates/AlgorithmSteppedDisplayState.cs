using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlgorithmSteppedDisplayState : ManipulationState
{

    private (StepType, List<Vertex>, List<Edge>, string) currentStep;
    private List<EdgeObj> highlightedEdges;
    private List<VertexObj> highlightedVertices;

    public override void OnStateEnter()
    {
        if (AlgorithmsPanel.Singleton.CurrentStep != null) {
            this.currentStep = ((StepType, List<Vertex>, List<Edge>, string)) AlgorithmsPanel.Singleton.CurrentStep;
        }
        else {
            Logger.Log("Steps not found for current algorith.", this, LogType.ERROR);
        }

        this.highlightedEdges = new List<EdgeObj>();
        this.highlightedVertices = new List<VertexObj>();

        AlgorithmsPanel.Singleton.stepByStepPanel.SetActive(true);
        
        if (this.currentStep.Item3 != null) {
            foreach (EdgeObj edgeObj in Controller.Singleton.EdgeObjs)
            {
                if (this.currentStep.Item3.Contains(edgeObj.Edge))
                {
                    edgeObj.IsAlgorithmResult = true;
                    this.highlightedEdges.Add(edgeObj);
                }
            }
        }

        if (this.currentStep.Item2 != null) {
            foreach (VertexObj vertexObj in Controller.Singleton.VertexObjs)
            {
                if (this.currentStep.Item2.Contains(vertexObj.Vertex))
                {
                    vertexObj.IsAlgorithmResult = true;
                    this.highlightedVertices.Add(vertexObj);
                }
            }
        }
    }

    public override void OnStateExit()
    {
        this.highlightedEdges.ForEach(e => e.IsAlgorithmResult = false);
        this.highlightedVertices.ForEach(v => v.IsAlgorithmResult = false);

        AlgorithmsPanel.Singleton.stepByStepPanel.SetActive(false);
    }
}
