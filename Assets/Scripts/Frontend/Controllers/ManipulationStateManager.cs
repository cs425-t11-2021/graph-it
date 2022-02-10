using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipulationStateManager : SingletonBehavior<ManipulationStateManager>
{
    private ManipulationState activeState = null;
    public ManipulationState ActiveState {
        get => this.activeState;
        set {
            if (this.ActiveState != null)
                this.activeState.Active = false;
            Logger.Log("Entering " + value.ToString() + " state.", this, LogType.INFO);
            this.activeState = value;
            this.activeState.Active = true;
        }
    }

    private ManipulationState suspendedState = null;

    private void Awake() {
        ManipulationState.viewState = new GraphViewState();
        ManipulationState.selectionState = new SelectionState();
        ManipulationState.vertexCreationState = new VertexCreationState();
        ManipulationState.edgeCreationState = new EdgeCreationState();
        ManipulationState.disabledState = new DisabledState();

        this.ActiveState = ManipulationState.viewState;
    }

    public void SuspendeManipulationState(bool suspend) {
        if (suspend) {
            if (this.ActiveState != ManipulationState.disabledState) {
                this.suspendedState = this.activeState;
                this.ActiveState = ManipulationState.disabledState;
            }
        }
        else {
            if (this.ActiveState == ManipulationState.disabledState) {
                this.ActiveState = this.suspendedState;
                this.suspendedState = null;
            }
        }
    }
}
