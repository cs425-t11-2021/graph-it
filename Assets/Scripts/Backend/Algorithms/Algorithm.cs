
using System;
using System.Threading;
using UnityEngine;

public abstract class Algorithm
{
    protected Graph Graph { get; private set; }
    private Thread currThread;
    private CancellationToken token;
    private Action updateUI;
    private Action updateCalc;
    private Action< Algorithm > markRunning;
    private Action< Algorithm > markComplete;
    private Action< Algorithm > unmarkRunning;
    protected bool running;
    protected bool complete;

    public Algorithm( Graph graph, CancellationToken token, Action updateUI, Action updateCalc, Action< Algorithm > markRunning, Action< Algorithm > markComplete, Action< Algorithm > unmarkRunning )
    {
        this.Graph = graph;
        this.currThread = null;
        this.token = token;
        this.updateUI = updateUI;
        this.updateCalc = updateCalc;
        this.markRunning = markRunning;
        this.markComplete = markComplete;
        this.unmarkRunning = unmarkRunning;
        this.running = false;
        this.complete = false;
    }

    public abstract void Run();

    public void RunThread()
    {
        this.Kill();

        this.currThread = new Thread( new ThreadStart( this.RunWrapper ) );
        this.currThread.Start();
    }

    private void RunWrapper()
    {
        try
        {
            this.running = true;
            this.markRunning( this );
            RunInMain.Singleton.queuedTasks.Enqueue( this.updateCalc );
            this.Run();
            if ( this.IsKillRequested() )
                this.Kill();
            this.running = false;
            this.complete = true;
            this.markComplete( this );
            RunInMain.Singleton.queuedTasks.Enqueue( this.updateUI );
        }
        catch ( OperationCanceledException )
        {
            Logger.Log("Killing thread.", this, LogType.INFO);
        }
    }

    protected void WaitUntil( Func< bool > condition )
    {
        Func< bool > conditionOrKill = () => condition() || this.IsKillRequested();
        SpinWait.SpinUntil( conditionOrKill );
    }

    protected virtual void Kill()
    {
        if ( this.currThread?.IsAlive ?? false )
        {
            this.running = false;
            this.complete = false;
            this.unmarkRunning( this );
            this.token.ThrowIfCancellationRequested();
        }
    }

    protected bool IsKillRequested() => this.token.IsCancellationRequested;

    public new abstract int GetHashCode();
}
