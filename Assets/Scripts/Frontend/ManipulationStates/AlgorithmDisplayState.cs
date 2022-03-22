using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlgorithmDisplayState : ManipulationState
{

    List<Edge> algorithmResult;

    public override void OnStateEnter()
    {
        this.algorithmResult = AlgorithmsPanel.Singleton.AlgorithmResult;

        List<Vertex> resultVertices = new List<Vertex>();
        foreach (Edge e in algorithmResult) {
            resultVertices.Add(e.vert1);
            resultVertices.Add(e.vert2);
        }
        
        foreach (EdgeObj edgeObj in Controller.Singleton.EdgeObjs)
        {
            if (algorithmResult.Contains(edgeObj.Edge))
                edgeObj.IsAlgorithmResult = true;
        }
        
        foreach (VertexObj vertexObj in Controller.Singleton.VertexObjs)
        {
            if (resultVertices.Contains(vertexObj.Vertex))
                vertexObj.IsAlgorithmResult = true;
        }
    }

    public override void OnDoubleClick() {
        ManipulationStateManager.Singleton.ActiveState = ManipulationState.viewState;
    }

    public override void OnStateExit()
    {
        List<Vertex> resultVertices = new List<Vertex>();
        foreach (Edge e in algorithmResult) {
            resultVertices.Add(e.vert1);
            resultVertices.Add(e.vert2);
        }

        foreach (EdgeObj edgeObj in Controller.Singleton.EdgeObjs)
        {
            if (algorithmResult.Contains(edgeObj.Edge))
                edgeObj.IsAlgorithmResult = false;
        }
        
        foreach (VertexObj vertexObj in Controller.Singleton.VertexObjs)
        {
            if (resultVertices.Contains(vertexObj.Vertex))
                vertexObj.IsAlgorithmResult = false;
        }

    }
}
