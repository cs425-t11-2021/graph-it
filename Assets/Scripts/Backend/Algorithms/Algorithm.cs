
// All code developed by Team 11

using System;
using System.Threading;
using System.Collections.Generic;

public enum AlgorithmType { INFO, DISPLAY, INTERNAL }

public enum AlgorithmResultType { SUCCESS, RUNNING, ERROR, ESTIMATE }

public class AlgorithmResult
{
    public AlgorithmResultType type;
    public string desc;
    public Dictionary< string, ( object, Type ) > results;

    public AlgorithmResult( AlgorithmResultType type, string desc="" )
    {
        this.type = type;
        this.desc = desc;
        this.results = new Dictionary< string, ( object, Type ) >();
    }
}

public abstract class Algorithm
{
    protected AlgorithmManager AlgoManager { get; private set; }
    protected Graph Graph { get; private set; }
    private Thread currThread;
    protected bool running;
    protected bool complete;
    protected bool estimated;
    protected bool error;
    protected string errorDesc;
    
    // Whether the algorithm is an info or display algorithm
    public AlgorithmType type;
    
    // All the vertex parameters associated with an algorithm
    public Vertex[] vertexParms = null;

    // All the edge parameters associated with an algorithm
    public Edge[] edgeParms = null;

    public Algorithm( AlgorithmManager algoManager )
    {
        this.AlgoManager = algoManager;
        this.Graph = algoManager.graphCopy;
        this.currThread = null;
        this.running = false;
        this.complete = false;
        this.error = false;
        this.errorDesc = null;
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
            this.AlgoManager.MarkRunning( this );

            if ( this.type == AlgorithmType.INFO )
                RunInMain.Singleton.queuedTasks.Enqueue( () => GraphInfo.Singleton.UpdateGraphInfoCalculating( this, this.AlgoManager ) );

            this.Run();
            this.running = false;
            this.complete = true;
            this.error = false;
            this.AlgoManager.MarkComplete( this );
            Logger.Log( "Finishing Thread.", this, LogType.DEBUG );
        }
        catch ( Exception e )
        {
            if ( e is ThreadAbortException )
                Logger.Log( "Killing thread.", this, LogType.DEBUG );
            else
            {
                this.running = false;
                this.complete = true;
                this.error = true;
                this.AlgoManager.MarkComplete( this );
                this.errorDesc = e.Message;
                Logger.Log( string.Format("Algorithm Error: {0} Finishing Thread.", e.Message), this, LogType.ERROR );
            }
        }
        
        if ( this.type == AlgorithmType.INFO )
            RunInMain.Singleton.queuedTasks.Enqueue( () => GraphInfo.Singleton.UpdateGraphInfoResults( this, this.AlgoManager ) );
        else if ( this.type == AlgorithmType.DISPLAY )
            RunInMain.Singleton.queuedTasks.Enqueue( () => AlgorithmsPanel.Singleton.UpdateGraphDisplayResults( this, this.vertexParms, this.AlgoManager ) );
    }

    public abstract AlgorithmResult GetResult();

    protected AlgorithmResult GetErrorResult() => new AlgorithmResult( AlgorithmResultType.ERROR, this.errorDesc );

    protected AlgorithmResult GetRunningResult() => new AlgorithmResult( AlgorithmResultType.RUNNING );

    protected void CreateError( string desc )
    {
        throw new AlgorithmException( desc );
    }

    public bool HasError() => this.error;

    protected void WaitUntil( Func< bool > condition )
    {
        SpinWait.SpinUntil( condition );
    }

    protected void WaitUntilAlgorithmComplete( int hash )
    {
        this.WaitUntil( () => this.AlgoManager.IsComplete( hash ) );
        if ( this.AlgoManager.HasError( hash ) )
            this.CreateError( "Required algorithm failed." );
    }

    public virtual void Kill()
    {
        if ( this.currThread?.IsAlive ?? false )
        {
            this.AlgoManager.UnmarkRunning( this );
            this.currThread.Abort();
        }
        this.running = false;
        this.complete = false;
    }

    public new abstract int GetHashCode();
}

public class AlgorithmException : Exception
{
    public AlgorithmException() : base() { }
    public AlgorithmException( string message ) : base( message ) { }
    public AlgorithmException( string message, Exception inner ) : base( message, inner ) { }

    protected AlgorithmException( System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context ) : base( info, context ) { }
}
