using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlgorithmDisplayState : ManipulationState
{

    List<Edge> algorithmResult;

    public override void OnStateEnter()
    {
        if (AlgorithmsPanel.Singleton.stepByStep)
        {
            RunInMain.Singleton.queuedTasks.Enqueue( () => RunInMain.Singleton.StartCoroutine(DisplayResultWithAnimation(0.1f)) );
            return;
        }
        
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

    private IEnumerator DisplayResultWithAnimation(float delay)
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
            {
                edgeObj.Vertex1.IsAlgorithmResult = true;
                yield return new WaitForSeconds(delay);
                edgeObj.IsAlgorithmResult = true;
                yield return new WaitForSeconds(delay);
                edgeObj.Vertex2.IsAlgorithmResult = true;
                yield return new WaitForSeconds(delay);
            }
                
            // yield return new WaitForSeconds(delay);
        }
        
        // foreach (VertexObj vertexObj in Controller.Singleton.VertexObjs)
        // {
        //     if (resultVertices.Contains(vertexObj.Vertex) && !vertexObj.IsAlgorithmResult)
        //         vertexObj.IsAlgorithmResult = true;
        //     yield return new WaitForSeconds(delay);
        // }
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

        RunInMain.Singleton.queuedTasks.Enqueue(() =>
            RunInMain.Singleton.StopAllCoroutines());

    }
}
