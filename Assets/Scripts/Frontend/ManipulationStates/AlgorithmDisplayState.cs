using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class AlgorithmDisplayState : ManipulationState
{

    private object algorithmResult;
    private string[] algorithmExtra;
    private List<EdgeObj> highlightedEdges;
    private List<VertexObj> highlightedVertices;

    public override void OnStateEnter()
    {
        // if (AlgorithmsPanel.Singleton.stepByStep)
        // {
        //     RunInMain.Singleton.queuedTasks.Enqueue( () => RunInMain.Singleton.StartCoroutine(DisplayResultWithAnimation(0.1f)) );
        //     return;
        // }
        
        this.algorithmResult = AlgorithmsPanel.Singleton.AlgorithmResult;
        this.algorithmExtra = AlgorithmsPanel.Singleton.AlgorithmExtra;
        this.highlightedEdges = new List<EdgeObj>();
        this.highlightedVertices = new List<VertexObj>();

        switch (AlgorithmsPanel.Singleton.CurrentlySelectedAlgorithm.resultType)
        {
            case ResultType.EdgeList:
            {
                List<Edge> resultEdges = (List<Edge>) this.algorithmResult;
                List<Vertex> resultVertices = new List<Vertex>();
                foreach (Edge e in resultEdges)
                {
                    resultVertices.Add(e.vert1);
                    resultVertices.Add(e.vert2);
                }

                foreach (EdgeObj edgeObj in Controller.Singleton.EdgeObjs)
                {
                    if (resultEdges.Contains(edgeObj.Edge))
                    {
                        edgeObj.IsAlgorithmResult = true;
                        this.highlightedEdges.Add(edgeObj);
                    }
                }

                foreach (VertexObj vertexObj in Controller.Singleton.VertexObjs)
                {
                    if (resultVertices.Contains(vertexObj.Vertex))
                    {
                        vertexObj.IsAlgorithmResult = true;
                        this.highlightedVertices.Add(vertexObj);
                    }
                }

                break;
            }
            case ResultType.VertexList:
            {
                List<Vertex> resultVertices = (List<Vertex>) this.algorithmResult;
                List<Edge> resultEdges = Controller.Singleton.Graph.GetIncidentEdges(new HashSet<Vertex>(resultVertices)).ToList();
                
                foreach (EdgeObj edgeObj in Controller.Singleton.EdgeObjs)
                {
                    if (resultEdges.Contains(edgeObj.Edge))
                    {
                        edgeObj.IsAlgorithmResult = true;
                        this.highlightedEdges.Add(edgeObj);
                    }
                }

                foreach (VertexObj vertexObj in Controller.Singleton.VertexObjs)
                {
                    if (resultVertices.Contains(vertexObj.Vertex))
                    {
                        vertexObj.IsAlgorithmResult = true;
                        this.highlightedVertices.Add(vertexObj);
                    }
                }
                
                break;
            }
        }
        
        if (this.algorithmExtra != null)
        {
            AlgorithmsPanel.Singleton.extraInfoPanel.GetComponentInChildren<TMP_Text>(true).text = AlgorithmsPanel.Singleton.CurrentlySelectedAlgorithm.algorithmClass + " Extra Info:";
            string output = "";
            Array.ForEach(this.algorithmExtra, s => output += s + "\n");
            AlgorithmsPanel.Singleton.extraInfoPanel.GetComponentInChildren<TMP_InputField>(true).text = output;
            AlgorithmsPanel.Singleton.extraInfoPanel.SetActive(true);           
        }
    }

    // private IEnumerator DisplayResultWithAnimation(float delay)
    // {
    //     // this.algorithmResult = AlgorithmsPanel.Singleton.AlgorithmResult;
    //     //
    //     // List<Vertex> resultVertices = new List<Vertex>();
    //     // foreach (Edge e in algorithmResult) {
    //     //     resultVertices.Add(e.vert1);
    //     //     resultVertices.Add(e.vert2);
    //     // }
    //     //
    //     // foreach (EdgeObj edgeObj in Controller.Singleton.EdgeObjs)
    //     // {
    //     //     if (algorithmResult.Contains(edgeObj.Edge))
    //     //     {
    //     //         edgeObj.Vertex1.IsAlgorithmResult = true;
    //     //         yield return new WaitForSeconds(delay);
    //     //         edgeObj.IsAlgorithmResult = true;
    //     //         yield return new WaitForSeconds(delay);
    //     //         edgeObj.Vertex2.IsAlgorithmResult = true;
    //     //         yield return new WaitForSeconds(delay);
    //     //     }
    //     //         
    //     //     // yield return new WaitForSeconds(delay);
    //     // }
    //     //
    //     // // foreach (VertexObj vertexObj in Controller.Singleton.VertexObjs)
    //     // // {
    //     // //     if (resultVertices.Contains(vertexObj.Vertex) && !vertexObj.IsAlgorithmResult)
    //     // //         vertexObj.IsAlgorithmResult = true;
    //     // //     yield return new WaitForSeconds(delay);
    //     // // }
    // }

    public override void OnStateExit()
    {
        this.highlightedEdges.ForEach(e => e.IsAlgorithmResult = false);
        this.highlightedVertices.ForEach(v => v.IsAlgorithmResult = false);
        
        AlgorithmsPanel.Singleton.extraInfoPanel.GetComponentInChildren<TMP_InputField>(true).text = "";
        AlgorithmsPanel.Singleton.extraInfoPanel.SetActive(false);

        // RunInMain.Singleton.queuedTasks.Enqueue(() =>
        //     RunInMain.Singleton.StopAllCoroutines());

    }
}
