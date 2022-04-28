
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class AlgorithmManager
{
    public Graph graph;
    public Graph graphCopy; // temp fix to prevent deadlocking

    private ConcurrentDictionary< int, Algorithm > running = new ConcurrentDictionary< int, Algorithm >(); 
    public List< Algorithm > Running
    {
        get => this.running.Values.ToList();
    }
    private ConcurrentDictionary< int, Algorithm > complete = new ConcurrentDictionary< int, Algorithm >();
    public List< Algorithm > Complete // TODO: algorithms in error state are added to complete, fix?
    {
        get => this.complete.Values.ToList();
    }

    private Action< Edge, Vertex > nothing = ( e, v ) => { };

    public void Initiate( Graph graph )
    {
        Controller.Singleton.OnGraphModified += OnGraphModified;
        this.graph = graph;
        this.graphCopy = new Graph( graph );
    }


    private void RunAlgorithm( Type algorithm, bool display, object[] parms )
    {
        Algorithm algorithmInstance = ( Algorithm ) Activator.CreateInstance( algorithm, parms.Prepend( display ).Prepend( this ).ToArray() );
        algorithmInstance.RunThread();
    }

    private void EnsureRunning( Type algorithm, bool display, params object[] parms )
    {
        int hash = ( int ) algorithm.GetMethod( "GetHash" ).Invoke( null, parms );
        if ( !this.IsRunning( hash ) && !this.IsComplete( hash ) )
            this.RunAlgorithm( algorithm, display, parms );
        else if ( display )
        {
            if ( this.IsComplete( hash ) )
            {
                RunInMain.Singleton.queuedTasks.Enqueue( () => GraphInfo.Singleton.UpdateGraphInfoResults( this.complete.GetValue( hash ), this ) );
                RunInMain.Singleton.queuedTasks.Enqueue( () => AlgorithmsPanel.Singleton.UpdateGraphDisplayResults( this.complete.GetValue( hash ), this.complete.GetValue( hash ).vertexParms, this ) );
            }
            else if ( this.IsRunning( hash ) )
                RunInMain.Singleton.queuedTasks.Enqueue( () => GraphInfo.Singleton.UpdateGraphInfoCalculating( this.running.GetValue( hash ), this ) );
        }
    }

    public void RunAdjacencyMatrix( bool display=true ) => this.EnsureRunning( typeof ( AdjacencyMatrixAlgorithm ), display );

    public void RunWeightMatrix( bool display=true ) => this.EnsureRunning( typeof ( WeightMatrixAlgorithm ), display );

    public void RunMinDegree( bool display=true ) => this.EnsureRunning( typeof ( MinDegreeAlgorithm ), display );

    public void RunMaxDegree( bool display=true )
    {
        Logger.Log( "running max degree in algorithm manager.", this, LogType.INFO );
        this.EnsureRunning( typeof ( MaxDegreeAlgorithm ), display );
    }

    public void RunRadius( bool display=true ) => this.EnsureRunning( typeof ( RadiusAlgorithm ), display );

    public void RunDiameter( bool display=true ) => this.EnsureRunning( typeof ( DiameterAlgorithm ), display );

    public void RunChromatic( bool display=true ) => this.EnsureRunning( typeof ( ChromaticAlgorithm ), display );

    public void RunIndependence( bool display=true ) => this.EnsureRunning( typeof ( IndependenceAlgorithm ), display );

    public void RunClique( bool display=true ) => this.EnsureRunning( typeof ( CliqueAlgorithm ), display );

    public void RunMatching( bool display=true ) => this.EnsureRunning( typeof ( MatchingAlgorithm ), display );

    public void RunBipartite( bool display=true ) => this.EnsureRunning( typeof ( BipartiteAlgorithm ), display );

    public void RunCyclic( bool display=true ) => this.EnsureRunning( typeof ( CyclicAlgorithm ), display );

    public void RunFleurys( bool display=true ) => this.EnsureRunning( typeof ( FleurysAlgorithm ), display );

    public void RunPrims( Vertex vert, bool display=true ) => this.EnsureRunning( typeof ( PrimsAlgorithm ), display, vert );

    public void RunKruskals( bool display=true ) => this.EnsureRunning( typeof ( KruskalsAlgorithm ), display );

    public void RunDijkstras( Vertex src, Vertex dest, bool display=true ) => this.EnsureRunning( typeof ( DijkstrasAlgorithm ), display, src, dest );

    public void RunBellmanFords( Vertex src, Vertex dest, bool display=true ) => this.EnsureRunning( typeof ( BellmanFordsAlgorithm ), display, src, dest );

    public void RunDepthFirstSearch( Vertex vert, bool display=true ) => this.RunDepthFirstSearchWithAction( vert, this.nothing, display );

    public void RunDepthFirstSearchWithAction( Vertex vert, Action< Edge, Vertex > action, bool display=true ) => this.EnsureRunning( typeof ( DepthFirstSearchAlgorithm ), display, vert, action );

    public void RunBreadthFirstSearch( Vertex vert, bool display=true ) => this.RunBreadthFirstSearchWithAction( vert, this.nothing, display );

    public void RunBreadthFirstSearchWithAction( Vertex vert, Action< Edge, Vertex > action, bool display=true ) => this.EnsureRunning( typeof ( BreadthFirstSearchAlgorithm ), display, vert, action );

    public void RunNumberOfSpanningTrees( bool display=true ) => this.EnsureRunning( typeof ( NumberOfSpanningTreesAlgorithm ), display );


    public AlgorithmResult GetResult( Type algorithm, params object[] parms )
    {
        int hash = ( int ) algorithm.GetMethod( "GetHash" ).Invoke( null, parms );
        if ( !this.IsComplete( hash ) )
            throw new System.Exception( "Cannot retrieve algorithm result when algorithm is not complete." ); // TODO: replace with custom error
        return this.complete[ hash ].GetResult();
    }

    public AlgorithmResult GetAdjacencyMatrix() => this.GetResult( typeof ( AdjacencyMatrixAlgorithm ) );

    public AlgorithmResult GetWeightMatrix() => this.GetResult( typeof ( WeightMatrixAlgorithm ) );

    public AlgorithmResult GetMinDegree() => this.GetResult( typeof ( MinDegreeAlgorithm ) );

    public AlgorithmResult GetMaxDegree() => this.GetResult( typeof ( MaxDegreeAlgorithm ) );

    public AlgorithmResult GetRadius() => this.GetResult( typeof ( RadiusAlgorithm ) );

    public AlgorithmResult GetDiameter() => this.GetResult( typeof ( DiameterAlgorithm ) );

    public AlgorithmResult GetChromatic() => this.GetResult( typeof ( ChromaticAlgorithm ) );

    public AlgorithmResult GetIndependence() => this.GetResult( typeof ( IndependenceAlgorithm ) );

    public AlgorithmResult GetClique() => this.GetResult( typeof ( CliqueAlgorithm ) );

    public AlgorithmResult GetMaxMatching() => this.GetResult( typeof ( MatchingAlgorithm ) );

    public AlgorithmResult GetBipartite() => this.GetResult( typeof ( BipartiteAlgorithm ) );

    public AlgorithmResult GetCyclic() => this.GetResult( typeof ( CyclicAlgorithm ) );

    public AlgorithmResult GetFleurys() => this.GetResult( typeof ( FleurysAlgorithm ) );

    public AlgorithmResult GetPrims( Vertex root ) => this.GetResult( typeof ( PrimsAlgorithm ), root );

    public AlgorithmResult GetKruskals() => this.GetResult( typeof ( KruskalsAlgorithm ) );

    public AlgorithmResult GetDijkstras( Vertex src, Vertex dest ) => this.GetResult( typeof ( DijkstrasAlgorithm ), src, dest );

    public AlgorithmResult GetBellmanFords( Vertex src, Vertex dest ) => this.GetResult( typeof ( BellmanFordsAlgorithm ), src, dest );

    public AlgorithmResult GetDepthFirstSearch( Vertex root ) => this.GetResult( typeof ( DepthFirstSearchAlgorithm ), root, this.nothing );

    public AlgorithmResult GetDepthFirstSearchWithAction( Vertex root, Action< Edge, Vertex > action ) => this.GetResult( typeof ( DepthFirstSearchAlgorithm ), root, action );

    public AlgorithmResult GetBreadthFirstSearch( Vertex root ) => this.GetResult( typeof ( BreadthFirstSearchAlgorithm ), root, this.nothing );

    public AlgorithmResult GetBreadthFirstSearchWithAction( Vertex root, Action< Edge, Vertex > action ) => this.GetResult( typeof ( BreadthFirstSearchAlgorithm ), root, action );

    public AlgorithmResult GetNumberOfSpanningTrees() => this.GetResult( typeof ( NumberOfSpanningTreesAlgorithm ) );

    public bool IsNextStepAvailable( Type loggedAlgo, object[] parms ) => !( ( ( LoggedAlgorithm ) this.GetAlgorithm( loggedAlgo, parms ) )?.IsNextStepAvailable() is null );

    public bool IsBackStepAvailable( Type loggedAlgo, object[] parms ) => !( ( ( LoggedAlgorithm ) this.GetAlgorithm( loggedAlgo, parms ) )?.IsBackStepAvailable() is null );

    public bool IsAnyStepAvailable( Type loggedAlgo, object[] parms ) => !( ( ( LoggedAlgorithm ) this.GetAlgorithm( loggedAlgo, parms ) )?.IsFirstStepAvailable() is null );

    public bool IsStepAvailable( int step, Type loggedAlgo, object[] parms ) => !( ( ( LoggedAlgorithm ) this.GetAlgorithm( loggedAlgo, parms ) )?.IsStepAvailable( step ) is null );

    public void NextStep( Type loggedAlgo, object[] parms )
    {
        LoggedAlgorithm algo = ( LoggedAlgorithm ) this.GetAlgorithm( loggedAlgo, parms );
        if ( !algo.IsNextStepAvailable() )
            throw new System.Exception( "The provided algorithm's next step is not available." );
        algo.NextStep();
    }

    public void BackStep( Type loggedAlgo, object[] parms )
    {
        LoggedAlgorithm algo = ( LoggedAlgorithm ) this.GetAlgorithm( loggedAlgo, parms );
        if ( !algo.IsBackStepAvailable() )
            throw new System.Exception( "The provided algorithm's back step is not available." );
        algo.BackStep();
    }

    public void GoToStep( int step, Type loggedAlgo, object[] parms )
    {
        LoggedAlgorithm algo = ( LoggedAlgorithm ) this.GetAlgorithm( loggedAlgo, parms );
        if ( !algo.IsStepAvailable( step ) )
            throw new System.Exception( "The provided algorithm's step is not available." );
        algo.GoToStep( step );
    }

    public AlgorithmStep GetStep( Type loggedAlgo, object[] parms )
    {
        LoggedAlgorithm algo = ( LoggedAlgorithm ) this.GetAlgorithm( loggedAlgo, parms );
        if ( !algo.IsFirstStepAvailable() )
            throw new System.Exception( "The provided algorithm's step is not available." );
        return algo.GetStep();
    }

    private Algorithm GetAlgorithm( Type algorithm, object[] parms )
    {
        int hash = ( int ) algorithm.GetMethod( "GetHash" ).Invoke( null, parms );
        if ( !this.IsRunning( hash ) && !this.IsComplete( hash ) )
            throw new System.Exception( "The provided algorithm is not running nor completed." );
        Algorithm algo = null;
        if ( this.IsRunning( hash ) )
            algo = this.running.GetValue( hash );
        if ( this.IsComplete( hash ) )
            algo = this.complete.GetValue( hash );
        return algo;
    }


    public void MarkRunning( Algorithm algo )
    {
        this.running[ algo.GetHashCode() ] = algo;
    }

    public void MarkComplete( Algorithm algo )
    {
        this.complete[ algo.GetHashCode() ] = algo;
        this.UnmarkRunning( algo );
    }

    public void UnmarkRunning( Algorithm algo )
    {
        this.running.TryRemove( algo.GetHashCode(), out _ );
    }


    public bool IsRunning( Algorithm algo ) => this.IsRunning( algo.GetHashCode() );

    public bool IsRunning( int key ) => this.running.ContainsKey( key );

    public bool IsComplete( Algorithm algo ) => this.IsComplete( algo.GetHashCode() );

    public bool IsComplete( int key ) => this.complete.ContainsKey( key );

    public bool HasError( Algorithm algo ) => this.HasError( algo.GetHashCode() );

    public bool HasError( int key ) => this.IsComplete( key ) && this.complete[ key ].HasError();

    public void Clear()
    {
        this.KillAll();
        this.running.Clear();
        this.complete.Clear();
    }

    private void OnGraphModified()
    {
        Clear();
        
        Logger.Log( "Copying Graph DS.", this, LogType.INFO );
        this.graphCopy = new Graph( this.graph );
    }

    public void KillAll()
    {
        Logger.Log( "Stopping all algorithms.", this, LogType.INFO );
        foreach ( KeyValuePair< int, Algorithm > kvp in this.running.ToList() )
            kvp.Value?.Kill();
    }

    public void KillAlgo( Type algo, params object[] parms )
    {
        int hash = ( int ) algo.GetMethod( "GetHash" ).Invoke( null, parms );
        this.KillAlgo( hash );
    }

    public void KillAlgo( int hash )
    {
        this.running.GetValue( hash )?.Kill();
    }

    ~AlgorithmManager()
    {
        this.KillAll();
        Controller.Singleton.OnGraphModified -= OnGraphModified;
    }

}
