using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

[Serializable]
public class GraphDisplayAlgorithmAssociation
{
    public string algorithmClass = "";
    public string displayName = "";
    public bool enabled = false;
    public int requiredVertices = 0;
    public string activationMethod = "";
    public string completedMethod = "";
    public string description = "";
    // public string[] results;
    // public DisplayAlgorithmExtraInfo[] extraInfo;
    
    [HideInInspector] public ToggleButton activationButton;

    public Action<Vertex[]> OnCompleteUpdateDisplay
    {
        get
        {
            return (vertexParms) =>
            {
                AlgorithmResult result = (AlgorithmResult) Type.GetType("AlgorithmManager").GetMethod(completedMethod)
                    .Invoke(Controller.Singleton.AlgorithmManager, (Object[]) vertexParms);
                
                if (result.type == AlgorithmResultType.ERROR)
                {
                    Logger.Log("Graph display algorithm " + algorithmClass + " errored.", this, LogType.ERROR);
                    NotificationManager.Singleton.CreateNotification(string.Format("{0} <color=red>Error: {1}</color>", this.displayName, result.desc), 3);
                }
                else
                {
                    foreach (KeyValuePair<string, (object, Type)> kvp in result.results)
                    {
                        
                    }
                    
                    AlgorithmsPanel.Singleton.StoreAlgorithmResult(this.algorithmClass, result, vertexParms);
                    if (AlgorithmsPanel.Singleton.CurrentlySelectedAlgorithm == this) {
                        AlgorithmsPanel.Singleton.AlgorithmResult = result;
                        AlgorithmsPanel.Singleton.AlgorithmVertexPrams = vertexParms;
                    }

                    NotificationManager.Singleton.CreateNotification("<#0000FF>" + this.displayName + "</color> finished.", 3);
                }
            };
        }
    }
}