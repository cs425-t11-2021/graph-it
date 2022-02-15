
using System;
using System.Threading;

public abstract class Algorithm
{
    protected Graph Graph { get; }
    private Thread currThread;
    private Action updateUI;
    private Action updateCalc;
    private Action< Algorithm > markRunning;
    private Action< Algorithm > markComplete;
    private Action< Algorithm > unmarkRunning;
    protected bool running;
    protected bool complete;

    public Algorithm( Graph graph, Action updateUI, Action updateCalc, Action< Algorithm > markRunning, Action< Algorithm > markComplete, Action< Algorithm > unmarkRunning )
    {
        this.Graph = graph;
        this.currThread = null;
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
            this.running = false;
            this.complete = true;
            this.markComplete( this );
            RunInMain.Singleton.queuedTasks.Enqueue( this.updateUI );
        }
        catch ( ThreadAbortException e ) { }
    }

    public void Kill()
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
