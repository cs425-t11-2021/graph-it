
// All code developed by Team 11

using System;
using System.Threading;
using System.Collections.Generic;

public enum AlgorithmType { INFO, DISPLAY, INTERNAL }

public abstract class Algorithm
{
    protected AlgorithmManager AlgoManager { get; private set; }
    protected Graph Graph { get; private set; }
    private Thread currThread;
    private Action< Algorithm > markRunning;
    private Action< Algorithm > markComplete;
    private Action< Algorithm > unmarkRunning;
    protected bool running;
    protected bool complete;
    
    // Whether ths algorithm is an info or display algorithm
    public AlgorithmType type;
    
    // All the vertex parameters associated with an algorithm
    public Vertex[] vertexParms = null;

    public Algorithm( AlgorithmManager algoManager )
    {
        this.AlgoManager = algoManager;
        this.Graph = algoManager.graphCopy;
        this.currThread = null;
        this.markRunning = algoManager.MarkRunning;
        this.markComplete = algoManager.MarkComplete;
        this.unmarkRunning = algoManager.UnmarkRunning;
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
            
            if ( this.type == AlgorithmType.INFO )
                RunInMain.Singleton.queuedTasks.Enqueue( () => GraphInfo.Singleton.UpdateGraphInfoCalculating( this, this.AlgoManager ) );
            
            this.Run();
            Logger.Log( "Finishing Thread.", this, LogType.DEBUG );
            this.running = false;
            this.complete = true;
            this.markComplete( this );
            
            if ( this.type == AlgorithmType.INFO )
                RunInMain.Singleton.queuedTasks.Enqueue( () => GraphInfo.Singleton.UpdateGraphInfoResults( this, this.AlgoManager ) );
            else if ( this.type == AlgorithmType.DISPLAY )
                RunInMain.Singleton.queuedTasks.Enqueue( () => AlgorithmsPanel.Singleton.UpdateGraphDisplayResults( this, this.vertexParms, this.AlgoManager ) );
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
            this.unmarkRunning( this );
            this.currThread.Abort();
        }
        this.running = false;
        this.complete = false;
    }

    public new abstract int GetHashCode();
}
