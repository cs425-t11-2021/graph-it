using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class AlgorithmDisplayState : ManipulationState
{

    private AlgorithmResult algorithmResult;
    private List<string> infoResults;
    private Notification notification;

    public override void OnStateEnter()
    {
        this.algorithmResult = AlgorithmsPanel.Singleton.AlgorithmResult;
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
                        vertexObj.visualsAnimator.ChangeState("algorithm_result");
                    }
                    else
                    {
                        vertexObj.visualsAnimator.ChangeState("algorithm_none");
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
                        edgeObj.visualsAnimator.ChangeState("algorithm_result");
                    }
                    else
                    {
                        edgeObj.visualsAnimator.ChangeState("algorithm_none");
                    }
                }
            }
            else if (resultType == typeof(float) || resultType == typeof(int) || resultType == typeof(bool))
            {
                this.infoResults.Add(resultID.ToTitleCase() + ": " + Convert.ToString(kvp.Value.Item1));
            }
            else if (resultType == typeof(List<(int, Edge)>))
            {
                List<(int, Edge)> orderedEdges = ((List<(int, Edge)>) kvp.Value.Item1).OrderBy(pair => pair.Item1).ToList();
                List<EdgeObj> edgeObjs = new List<EdgeObj>();

                foreach ((int, Edge) pair in orderedEdges)
                {
                    edgeObjs.Add(Controller.Singleton.GetEdgeObj(pair.Item2));
                }
                
                foreach (EdgeObj edgeObj in Controller.Singleton.EdgeObjs)
                {
                    if (edgeObjs.Contains(edgeObj))
                    {
                        edgeObj.visualsAnimator.ChangeState("algorithm_new");
                    }
                    else
                    {
                        edgeObj.visualsAnimator.ChangeState("algorithm_none");
                    }
                }
                
                
                ManipulationStateManager.Singleton.StartCoroutine(DisplayEdgesSequentially(0.2f, edgeObjs));
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
        
        this.notification = NotificationManager.Singleton.CreateNotification(string.Format("Showing <#0000FF>{0}</color> results.", AlgorithmsPanel.Singleton.CurrentlySelectedAlgorithm.displayName));
    }

    IEnumerator DisplayEdgesSequentially(float gap, List<EdgeObj> edgeObjs)
    {
        while (true)
        {
            foreach ((EdgeObj e, int i) in edgeObjs.WithIndex())
            {
                e.visualsAnimator.ExpandEffect(edgeObjs.Count() * 0.25f + 1.5f - 0.2f * i, 1.5f, "algorithm_result");
                yield return new WaitForSeconds(gap);
            }
            yield return new WaitForSeconds(3f);
        }
    }

    public override void OnStateExit()
    {
        Controller.Singleton.EdgeObjs.ForEach(e => e.visualsAnimator.ChangeState("default"));
        Controller.Singleton.VertexObjs.ForEach(v => v.visualsAnimator.ChangeState("default"));
        
        AlgorithmsPanel.Singleton.extraInfoPanel.GetComponentInChildren<TMP_InputField>(true).text = "";
        AlgorithmsPanel.Singleton.extraInfoPanel.SetActive(false);
        ManipulationStateManager.Singleton.StopAllCoroutines();
        
        if (this.notification != null)
        {
            Controller.Destroy(this.notification.gameObject);
            this.notification = null;
        }
    }
}
