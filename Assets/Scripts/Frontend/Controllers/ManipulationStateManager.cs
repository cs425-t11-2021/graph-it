// Class for managing the manipulation state finite state machine. Each state in the FSM corresponds to a mode of graph manipulation.
// Examples of graph manipulation modes include single select mode, multi-select mode, add vertex mode, etc.
public class ManipulationStateManager : SingletonBehavior<ManipulationStateManager>
{
    // Current active manipulation state
    private ManipulationState activeState = null;
    // Public property to get and set the manipulation state
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
    // Stored state if manipuation is suspended
    private ManipulationState suspendedState = null;

    private void Awake() {
        // Create instances of the manipulation states
        ManipulationState.viewState = new GraphViewState();
        ManipulationState.selectionState = new SelectionState();
        ManipulationState.vertexCreationState = new VertexCreationState();
        ManipulationState.edgeCreationState = new EdgeCreationState();
        ManipulationState.edgeDrawingState = new EdgeDrawingState();
        ManipulationState.disabledState = new DisabledState();
        ManipulationState.algorithmInitiationState = new AlgorithmInitiationState();
        ManipulationState.algorithmDisplayState = new AlgorithmDisplayState();
        ManipulationState.algorithmSteppedDisplayState = new AlgorithmSteppedDisplayState();
        
        // Go into view state at program start
        this.ActiveState = ManipulationState.viewState;
    }

    // Suspend or unsuspend the current manipulation state
    public void SuspendManipulationState(bool suspend) {
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
