using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ManipulationState
{
    public static ManipulationState viewState;
    public static ManipulationState selectionState;
    public static ManipulationState vertexCreationState;
    public static ManipulationState edgeCreationState;
    public static ManipulationState disabledState;

    public bool Active {
        set {
            if (value) {
                InputManager.Singleton.OnMouseClick += OnClick;
                InputManager.Singleton.OnMouseDoubleClick += OnDoubleClick;
                InputManager.Singleton.OnMouseHold += OnMouseHold;
                InputManager.Singleton.OnMouseRelease += OnMouseRelease;
                InputManager.Singleton.OnVertexClick += OnVertexClick;

                OnStateEnter();
            }
            else {
                InputManager.Singleton.OnMouseClick -= OnClick;
                InputManager.Singleton.OnMouseDoubleClick -= OnDoubleClick;
                InputManager.Singleton.OnMouseHold -= OnMouseHold;
                InputManager.Singleton.OnMouseRelease -= OnMouseRelease;
                InputManager.Singleton.OnVertexClick -= OnVertexClick;

                OnStateExit();
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
