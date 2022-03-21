
using System;
using System.Threading;
using UnityEngine;

public abstract class Algorithm
{
    protected AlgorithmManager AlgoManager { get; private set; }
    protected Graph Graph { get; private set; }
    private Thread currThread;
    private Action< Algorithm > markRunning;
    private Action< Algorithm > markComplete;
    private Action< Algorithm > unmarkRunning;
    private Action updateUI;
    private Action updateCalc;
    protected bool running;
    protected bool complete;

    public Algorithm( AlgorithmManager algoManager, Action updateUI, Action updateCalc )
    {
        this.AlgoManager = algoManager;
        this.Graph = algoManager.graphCopy;
        this.currThread = null;
        this.markRunning = algoManager.MarkRunning;
        this.markComplete = algoManager.MarkComplete;
        this.unmarkRunning = algoManager.UnmarkRunning;
        this.updateUI = updateUI;
        this.updateCalc = updateCalc;
        this.running = false;
        this.complete = false;
    }

    public abstract void Run();

    public void RunThread()
    {
        this.Kill();
        Logger.Log( "Starting Thread.", this, LogType.DEBUG );
        this.currThread = new Thread( new ThreadStart( this.RunWrapper ) );
        this.currThread.Start();
    }

    private void RunWrapper()
    {
        try
        {
            this.running = true;
            this.markRunning( this );
            RunInMain.Singleton.queuedTasks.Enqueue(() => GraphInfo.Singleton.UpdateGraphInfoCalculating(this));
            // RunInMain.Singleton.queuedTasks.Enqueue( this.updateCalc );
            this.Run();
            Logger.Log( "Finishing Thread.", this, LogType.DEBUG );
            this.running = false;
            this.complete = true;
            this.markComplete( this );
            RunInMain.Singleton.queuedTasks.Enqueue(() => GraphInfo.Singleton.UpdateGraphInfoResults(this));
            // RunInMain.Singleton.queuedTasks.Enqueue( this.updateUI );
        }
        catch ( ThreadAbortException )
        {
            Logger.Log( "Killing thread.", this, LogType.DEBUG );
        }
    }

    protected void WaitUntil( Func< bool > condition )
    {
        SpinWait.SpinUntil( condition );
    }

    protected void WaitUntilAlgorithmComplete( int hash )
    {
        this.WaitUntil( () => this.AlgoManager.IsComplete( hash ) );
    }

    public virtual void Kill()
    {
        if ( this.currThread?.IsAlive ?? false )
        {
            this.running = false;
            this.complete = false;
            this.unmarkRunning( this );
            this.currThread.Abort();
        }
    }

    public new abstract int GetHashCode();
}
