using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlgorithmSteppedDisplayState : ManipulationState
{

    private AlgorithmStep currentStep;
    private List<EdgeObj> highlightedEdges;
    private List<VertexObj> highlightedVertices;

    public override void OnStateEnter()
    {
        this.currentStep =  AlgorithmsPanel.Singleton.CurrentStep;

        this.highlightedEdges = new List<EdgeObj>();
        this.highlightedVertices = new List<VertexObj>();

        AlgorithmsPanel.Singleton.stepByStepPanel.SetActive(true);
        
        if (this.currentStep.considerEdges != null) {
            foreach (EdgeObj edgeObj in Controller.Singleton.EdgeObjs)
            {
                if (this.currentStep.considerEdges.Contains(edgeObj.Edge))
                {
                    edgeObj.IsAlgorithmResult = true;
                    this.highlightedEdges.Add(edgeObj);
                }
            }
        }

        if (this.currentStep.considerVertices != null) {
            foreach (VertexObj vertexObj in Controller.Singleton.VertexObjs)
            {
                if (this.currentStep.considerVertices.Contains(vertexObj.Vertex))
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
