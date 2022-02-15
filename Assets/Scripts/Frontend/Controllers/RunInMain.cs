using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class used by algorithms to run actions on the main Unity thread
public class RunInMain : SingletonBehavior<RunInMain>
{
    // Queue of actions that needs to be run on the main thread
    public Queue<Action> queuedTasks;

    public void AddToQueue(Action action) {
        Debug.Log("adding " + action.ToString());
        this.queuedTasks.Enqueue(action);
    }

    private void Awake() {
        queuedTasks = new Queue<Action>();
    }

    private void Update() {
        if (this.queuedTasks.Count > 0) {
            Action f = this.queuedTasks.Dequeue();
            Debug.Log( f?.GetHashCode() );
            f();
        }
    }

    
}