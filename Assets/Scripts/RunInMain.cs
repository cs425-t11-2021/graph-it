using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunInMain : SingletonBehavior< RunInMain >
{
    public Queue< Action > queuedTasks;

    private void Awake()
    {
        this.queuedTasks = new Queue< Action >();
    }

    private void Update()
    {
        if ( this.queuedTasks.Count > 0 )
        {
            Debug.Log( "Running queued task" );
            this.queuedTasks.Dequeue()();
        }
    }

    
}