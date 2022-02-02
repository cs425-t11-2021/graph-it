using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunInMain : MonoBehaviour
{
    public static RunInMain singleton;


    public Queue<Action> queuedTasks;

    private void Awake() {
        if (singleton == null) {
            singleton = this;
        }
        else {
            Debug.LogError("[RunInMain] Singleton pattern violation");
            Destroy(this);
            return;
        }

        queuedTasks = new Queue<Action>();
    }

    private void Update() {
        if (queuedTasks.Count > 0) {
            Debug.Log("Running queued task");
            var task = queuedTasks.Dequeue();
            task();
        }
    }

    
}