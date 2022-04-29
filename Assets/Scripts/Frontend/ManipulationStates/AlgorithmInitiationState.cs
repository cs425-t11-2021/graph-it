using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlgorithmInitiationState : ManipulationState
{

    private GraphDisplayAlgorithmAssociation currentAssociation;
    private int selectedVertices = 0;

    public override void OnStateEnter()
    {
        this.currentAssociation = AlgorithmsPanel.Singleton.CurrentlySelectedAlgorithm;
        Debug.Log(this.currentAssociation);
        if (this.currentAssociation == null) {
            Toolbar.Singleton.EnterViewMode();
        }

        if (currentAssociation.requiredVertices == 0 || (currentAssociation.requiredVertices == SelectionManager.Singleton.SelectedVertexCount() && SelectionManager.Singleton.SelectedEdgeCount() == 0)) {
            AlgorithmsPanel.Singleton.RunGraphDisplayAlgorithm(this.currentAssociation);
            SelectionManager.Singleton.DeSelectAll();
            Toolbar.Singleton.EnterViewMode();
            return;
        }
        
        SelectionManager.Singleton.DeSelectAll();
        this.selectedVertices = 0;

        foreach (VertexObj vertexObj in Controller.Singleton.VertexObjs)
        {
            InWorldHover labelCreator = vertexObj.GetComponent<InWorldHover>();
            labelCreator.enabled = true;
            labelCreator.Label = "Select for " + this.currentAssociation.displayName;
        }

        NotificationManager.Singleton.CreateNotification(string.Format("Please select <#0000FF> {0} </color> vertices for <#0000FF> {1}</color>.", this.currentAssociation.requiredVertices, this.currentAssociation.algorithmClass), 3);
    }

    public override void OnVertexClick(GameObject clicked)
    {
        VertexObj vertexObj = clicked.GetComponent<VertexObj>();
        SelectionManager.Singleton.SelectVertex(vertexObj);
        this.selectedVertices++;

        if (this.selectedVertices == this.currentAssociation.requiredVertices) {
            AlgorithmsPanel.Singleton.RunGraphDisplayAlgorithm(this.currentAssociation);
            
            SelectionManager.Singleton.DeSelectAll();
            SelectionManager.Singleton.SelectVertex(vertexObj);
            
            Toolbar.Singleton.EnterViewMode();       
        }
    }

    public override void OnStateExit()
    {
        AlgorithmsPanel.Singleton.runButton.UpdateStatus(false);
        
        foreach (VertexObj vertexObj in Controller.Singleton.VertexObjs)
        {
            InWorldHover labelCreator = vertexObj.GetComponent<InWorldHover>();
            labelCreator.enabled = false;
        }
    }
}
