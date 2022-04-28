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
    }

    public override void OnStateExit()
    {
        this.highlightedEdges.ForEach(e => e.AlgorithmResultLevel = 0);
        this.highlightedVertices.ForEach(v => v.AlgorithmResultLevel = 0);

        AlgorithmsPanel.Singleton.stepByStepPanel.SetActive(false);
    }
}
