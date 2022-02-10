using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ManipulationState
{
    public static ManipulationState viewState;
    public static ManipulationState selectionState;
    public static ManipulationState vertexCreationState;
    public static ManipulationState edgeCreationState;

    public bool Active {
        set {
            if (value) {
                InputManager.Singleton.OnMouseClick += OnClick;
                InputManager.Singleton.OnMouseDoubleClick += OnDoubleClick;
                InputManager.Singleton.OnMouseHold += OnMouseHold;
                InputManager.Singleton.OnMouseRelease += OnMouseRelease;
            }
            else {
                InputManager.Singleton.OnMouseClick -= OnClick;
                InputManager.Singleton.OnMouseDoubleClick -= OnDoubleClick;
                InputManager.Singleton.OnMouseHold -= OnMouseHold;
                InputManager.Singleton.OnMouseRelease -= OnMouseRelease;
            }
        }
    }

    public abstract void OnStateEnter();
    public abstract void OnStateExit(); 
    public virtual void OnClick() {}
    public virtual void OnDoubleClick() {}
    public virtual void OnMouseHold() {}
    public virtual void OnMouseRelease() {}
    public virtual void OnVertexSelect(VertexObj selectedVertex) {}
    public virtual void OnEdgeSelect(EdgeObj selectedEdge) {}
}
