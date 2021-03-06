using UnityEngine;

// State in the manipulation FSM representing the state in which the user initiates an algorithm from the algorithm panel. All graph
// manipulations are disabled aside from choosing vertices for the algorithm's parameter. This state is a temporary solution.
public class AlgorithmInitiationState : ManipulationState
{

    private GraphDisplayAlgorithmAssociation currentAssociation;
    private Notification notification;
    private int selectedVertices = 0;

    public override void OnStateEnter()
    {
        this.currentAssociation = AlgorithmsPanel.Singleton.CurrentlySelectedAlgorithm;
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

        this.notification = NotificationManager.Singleton.CreateNotification(string.Format("Please select <#0000FF> {0} </color> vertices for <#0000FF> {1}</color>.", this.currentAssociation.requiredVertices, this.currentAssociation.displayName));
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

        if (this.notification != null)
        {
            Controller.Destroy(this.notification.gameObject);
            this.notification = null;
        }
    }
}
