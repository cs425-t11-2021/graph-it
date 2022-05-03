using System;
using System.Collections.Concurrent;

// Utility class used by algorithms to run actions on the main Unity thread. Required for multi-threading purposes.
public class RunInMain : SingletonBehavior<RunInMain>
{
    // Queue of actions that needs to be run on the main thread
    public ConcurrentQueue<Action> queuedTasks;

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