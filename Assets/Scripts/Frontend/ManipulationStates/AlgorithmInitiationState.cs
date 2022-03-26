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
        if (this.currentAssociation == null) {
            ManipulationStateManager.Singleton.ActiveState = ManipulationState.viewState;
        }

        SelectionManager.Singleton.DeSelectAll();
        this.selectedVertices = 0;

        if (currentAssociation.requiredVertices == 0) {
            AlgorithmsPanel.Singleton.runButton.UpdateStatus(false);
            SelectionManager.Singleton.DeSelectAll();
            AlgorithmsPanel.Singleton.RunGraphDisplayAlgorithm(this.currentAssociation);
            ManipulationStateManager.Singleton.ActiveState = ManipulationState.viewState;
            return;
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
            AlgorithmsPanel.Singleton.runButton.UpdateStatus(false);
            
            SelectionManager.Singleton.DeSelectAll();
            SelectionManager.Singleton.SelectVertex(vertexObj);
            
            ManipulationStateManager.Singleton.ActiveState = ManipulationState.viewState;        
        }
    }

    public override void OnStateExit()
    {
        
    }
}
