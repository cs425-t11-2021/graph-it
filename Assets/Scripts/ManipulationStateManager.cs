using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipulationStateManager : SingletonBehavior<ManipulationStateManager>
{
    private ManipulationState activeState = null;
    public ManipulationState ActiveState {
        get => this.activeState;
        set {
            this.activeState.OnStateExit();
            this.activeState.Active = false;
            Logger.Log("Entering " + value.ToString() + " state.", this, LogType.INFO);
            this.activeState = value;
            this.activeState.OnStateEnter();
            this.activeState.Active = true;
        }
    }

    private void Awake() {
        ManipulationState.viewState = new GraphViewState();
        ManipulationState.selectionState = new SelectionState();
        ManipulationState.vertexCreationState = new VertexCreationState();
        ManipulationState.edgeCreationState = new EdgeCreationState();


        this.activeState = ManipulationState.viewState;
        this.activeState.Active = true;
    }
}
