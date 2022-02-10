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
                InputManager.Singleton.OnVertexClick += OnVertexClick;
            }
            else {
                InputManager.Singleton.OnMouseClick -= OnClick;
                InputManager.Singleton.OnMouseDoubleClick -= OnDoubleClick;
                InputManager.Singleton.OnMouseHold -= OnMouseHold;
                InputManager.Singleton.OnMouseRelease -= OnMouseRelease;
                InputManager.Singleton.OnVertexClick -= OnVertexClick;
            }
        }
    }

    public abstract void OnStateEnter();
    public abstract void OnStateExit(); 
    public virtual void OnClick() {}
    public virtual void OnDoubleClick() {}
    public virtual void OnMouseHold() {}
    public virtual void OnMouseRelease() {}
    public virtual void OnVertexClick(GameObject clicked) {}
    public virtual void OnEdgeClick(GameObject clicked) {}
}
