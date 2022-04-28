using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

// Class used by algorithms to run actions on the main Unity thread
public class RunInMain : SingletonBehavior<RunInMain>
{
    // Queue of actions that needs to be run on the main thread
    public ConcurrentQueue<Action> queuedTasks;

    // public void AddToQueue(Action action) {
    //     Debug.Log("adding " + action.Method.Name);
    //     this.queuedTasks.Enqueue(action);
    // }

    private void Awake() {
        queuedTasks = new ConcurrentQueue<Action>();
    }

    private void Update() {
        if (this.queuedTasks.Count > 0) {
            if (this.queuedTasks.TryDequeue(out Action f)) {
                f();
            }

        }
    }

    
}