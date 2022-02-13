
using System;
using System.Threading;

public abstract class Algorithm
{
    protected Graph graph;
    private Thread currThread;
    private Action updateUI;
    private Action< Algorithm > markRunning;
    private Action< Algorithm > markComplete;
    protected bool running;
    protected bool complete;

    public Algorithm( Graph graph, Action updateUI, Action< Algorithm > markRunning, Action< Algorithm > markComplete )
    {
        this.graph = graph;
        this.currThread = null;
        this.updateUI = updateUI;
        this.markRunning = markRunning;
        this.markComplete = markComplete;
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
            // TODO: update ui to calculating
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
            // TODO: unmark running
            this.currThread.Abort();
        }
    }
}
