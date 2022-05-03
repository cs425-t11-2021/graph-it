using System;
using System.Collections.Generic;
using TMPro;

// State in the manipulation FSM representing the state in which the result of an algorithm is being displayed and step-through is enabled. All graph
// manipulations are disabled. This state is a temporary solution.
public class AlgorithmSteppedDisplayState : ManipulationState
{
    private AlgorithmResult algorithmResult;
    private AlgorithmStep currentStep;
    private List<string> infoResults;
    private Notification notification;

    public override void OnStateEnter()
    {
        this.currentStep =  AlgorithmsPanel.Singleton.CurrentStep;
        this.algorithmResult = AlgorithmsPanel.Singleton.AlgorithmResult;
        this.infoResults = new List<string>();

        AlgorithmsPanel.Singleton.stepPanelTitle.text = AlgorithmsPanel.Singleton.CurrentlySelectedAlgorithm.displayName + " Steps";
        AlgorithmsPanel.Singleton.stepByStepPanel.SetActive(true);

        foreach (EdgeObj edgeObj in Controller.Singleton.EdgeObjs)
        {
            InWorldHover labelCreator = edgeObj.GetComponent<InWorldHover>();

            bool contained = false;
            if (this.currentStep.considerEdges?.Contains(edgeObj.Edge) ?? false)
            {
                edgeObj.visualsAnimator.ChangeState("algorithm_considering");
                contained = true;

                labelCreator.enabled = true;
                labelCreator.Label = "Edge under consideration";
            }
            if (this.currentStep.resultEdges?.Contains(edgeObj.Edge) ?? false)
            {
                edgeObj.visualsAnimator.ChangeState("algorithm_result");
                contained = true;
                
                labelCreator.enabled = true;
                labelCreator.Label = "Edge in result";
            }
            if (this.currentStep.newEdges?.Contains(edgeObj.Edge) ?? false)
            {
                edgeObj.visualsAnimator.ChangeState("algorithm_new");
                contained = true;
                
                labelCreator.enabled = true;
                labelCreator.Label = "Edge added to result";
            }
            
            if (!contained)
            {
                edgeObj.visualsAnimator.ChangeState("algorithm_none");
                labelCreator.enabled = false;
            }
        }
        
        foreach (VertexObj vertexObj in Controller.Singleton.VertexObjs)
        {
            InWorldHover labelCreator = vertexObj.GetComponent<InWorldHover>();
            
            bool contained = false;
            if (this.currentStep.considerVerts?.Contains(vertexObj.Vertex) ?? false)
            {
                vertexObj.visualsAnimator.ChangeState("algorithm_considering");
                contained = true;
                
                labelCreator.enabled = true;
                labelCreator.Label = "Vertex under consideration";
            }
            if (this.currentStep.resultVerts?.Contains(vertexObj.Vertex) ?? false)
            {
                vertexObj.visualsAnimator.ChangeState("algorithm_result");
                contained = true;
                
                labelCreator.enabled = true;
                labelCreator.Label = "Vertex in result";
            }
            if (this.currentStep.newVerts?.Contains(vertexObj.Vertex) ?? false)
            {
                vertexObj.visualsAnimator.ChangeState("algorithm_new");
                contained = true;
                
                labelCreator.enabled = true;
                labelCreator.Label = "Vertex added to result";
            }
            if (this.currentStep.paramVerts?.Contains(vertexObj.Vertex) ?? false)
            {
                vertexObj.visualsAnimator.ChangeState("algorithm_parameter");
                contained = true;
                
                labelCreator.enabled = true;
                labelCreator.Label = "Vertex parameter";
            }
            
            if (!contained)
            {
                vertexObj.visualsAnimator.ChangeState("algorithm_none");
                labelCreator.enabled = false;
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
        
        this.notification = NotificationManager.Singleton.CreateNotification(string.Format("Showing <#0000FF>{0}</color> results.", AlgorithmsPanel.Singleton.CurrentlySelectedAlgorithm.displayName));
    }

    public override void OnStateExit()
    {
        Controller.Singleton.EdgeObjs.ForEach(e =>
        {
            e.visualsAnimator.ChangeState("default");
            e.GetComponent<InWorldHover>().enabled = false;
        });
        Controller.Singleton.VertexObjs.ForEach(v =>
        {
            v.visualsAnimator.ChangeState("default");
            v.GetComponent<InWorldHover>().enabled = false;
        });

        AlgorithmsPanel.Singleton.stepByStepPanel.SetActive(false);
        AlgorithmsPanel.Singleton.extraInfoPanel.SetActive(false);
        
        if (this.notification != null)
        {
            Controller.Destroy(this.notification.gameObject);
            this.notification = null;
        }
    }
}
