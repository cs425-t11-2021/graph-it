
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
    public List< Algorithm > Complete
    {
        get => this.running.Values.ToList();
    }

    public void Initiate( Graph graph )
    {
        Controller.Singleton.OnGraphModified += OnGraphModified;
        this.graph = graph;
        this.graphCopy = new Graph( graph );
    }

    public void RunAll()
    {
        this.RunMinDegree();
        this.RunMaxDegree();
        this.RunRadius();
        this.RunDiameter();
        this.RunCyclic();
        this.RunChromatic();
        // this.RunPrims();
        this.RunKruskals();
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
        
        if ( this.IsComplete( hash ) && display ) {
            RunInMain.Singleton.queuedTasks.Enqueue( () => GraphInfo.Singleton.UpdateGraphInfoResults( this.complete.GetValue( hash ) ) );
            RunInMain.Singleton.queuedTasks.Enqueue( () => AlgorithmsPanel.Singleton.UpdateGraphDisplayResults( this.complete.GetValue( hash ), this.complete.GetValue( hash ).vertexParms ) );
        }
        
    }

    public void RunMinDegree( bool display=true ) => this.EnsureRunning( typeof ( MinDegreeAlgorithm ), display );

    public void RunMaxDegree( bool display=true ) => this.EnsureRunning( typeof ( MaxDegreeAlgorithm ), display );

    public void RunRadius( bool display=true ) => this.EnsureRunning( typeof ( RadiusAlgorithm ), display );

    public void RunDiameter( bool display=true ) => this.EnsureRunning( typeof ( DiameterAlgorithm ), display );

    public void RunChromatic( bool display=true ) => this.EnsureRunning( typeof ( ChromaticAlgorithm ), display );

    public void RunBipartite( bool display=true ) => this.EnsureRunning( typeof ( BipartiteAlgorithm ), display );

    public void RunCyclic( bool display=true ) => this.EnsureRunning( typeof ( CyclicAlgorithm ), display );

    public void RunFleurys( bool display=true ) => this.EnsureRunning( typeof ( FleurysAlgorithm ), display );

    public void RunPrims( Vertex vert, bool display=true ) => this.EnsureRunning( typeof ( PrimsAlgorithm ), display, vert );

    public void RunKruskals( bool display=true ) => this.EnsureRunning( typeof ( KruskalsAlgorithm ), display );

    public void RunDijkstras( Vertex src, Vertex dest, bool display=true ) => this.EnsureRunning( typeof ( DijkstrasAlgorithm ), display, src, dest );

    public void RunBellmanFords( Vertex src, Vertex dest, bool display=true ) => this.EnsureRunning( typeof ( BellmanFordsAlgorithm ), display, src, dest );

    public void RunDepthFirstSearch( Vertex vert, Action< Edge, Vertex > action, bool display=true ) => this.EnsureRunning( typeof ( DepthFirstSearchAlgorithm ), display, vert, action );

    public void RunBreadthFirstSearch( Vertex vert, Action< Edge, Vertex > action, bool display=true ) => this.EnsureRunning( typeof ( BreadthFirstSearchAlgorithm ), display, vert, action );

    public int? GetMinDegree() => ( ( MinDegreeAlgorithm ) this.complete.GetValue( MinDegreeAlgorithm.GetHash() ) )?.MinDegree;

    public int? GetMaxDegree() => ( ( MaxDegreeAlgorithm ) this.complete.GetValue( MaxDegreeAlgorithm.GetHash() ) )?.MaxDegree;

    public float? GetRadius() => ( ( RadiusAlgorithm ) this.complete.GetValue( RadiusAlgorithm.GetHash() ) )?.Radius;

    public float? GetDiameter() => ( ( DiameterAlgorithm ) this.complete.GetValue( DiameterAlgorithm.GetHash() ) )?.Diameter;

    public int? GetChromaticNumber() => ( ( ChromaticAlgorithm ) this.complete.GetValue( ChromaticAlgorithm.GetHash() ) )?.ChromaticNumber;

    // TODO: rename this more appropriately
    public bool? GetBipartite() => ( ( BipartiteAlgorithm ) this.complete.GetValue( BipartiteAlgorithm.GetHash() ) )?.IsBipartite;

    // TODO: rename this more appropriately
    public bool? GetCyclic() => ( ( CyclicAlgorithm ) this.complete.GetValue( CyclicAlgorithm.GetHash() ) )?.IsCyclic;

    // TODO: rename this more appropriately
    public bool? GetFleurys() => ( ( FleurysAlgorithm ) this.complete.GetValue( FleurysAlgorithm.GetHash() ) )?.EulerianCircuitExists;

    public List< Edge > GetPrimsMST( Vertex root ) => ( ( PrimsAlgorithm ) this.complete.GetValue( PrimsAlgorithm.GetHash( root ) ) )?.Mst;

    public List< Edge > GetKruskalsMST() => ( ( KruskalsAlgorithm ) this.complete.GetValue( KruskalsAlgorithm.GetHash() ) )?.Mst;

    public float? GetDijkstrasCost( Vertex src, Vertex dest ) => ( ( DijkstrasAlgorithm ) this.complete.GetValue( DijkstrasAlgorithm.GetHash( src, dest ) ) )?.Cost;

    public List< Edge > GetDijkstrasPath( Vertex src, Vertex dest ) => ( ( DijkstrasAlgorithm ) this.complete.GetValue( DijkstrasAlgorithm.GetHash( src, dest ) ) )?.Path;

    public float? GetBellmanFordsCost( Vertex src, Vertex dest ) => ( ( BellmanFordsAlgorithm ) this.complete.GetValue( BellmanFordsAlgorithm.GetHash( src, dest ) ) )?.Cost;

    public List< Edge > GetBellmanFordsPath( Vertex src, Vertex dest ) => ( ( BellmanFordsAlgorithm ) this.complete.GetValue( BellmanFordsAlgorithm.GetHash( src, dest ) ) )?.Path;

    public List< Edge > GetDepthFirstSearchTree( Vertex root ) => ( ( DepthFirstSearchAlgorithm ) this.complete.GetValue( DepthFirstSearchAlgorithm.GetHash( root ) ) )?.Tree;

    public List< Edge > GetBreadthFirstSearchTree( Vertex root ) => ( ( BreadthFirstSearchAlgorithm ) this.complete.GetValue( BreadthFirstSearchAlgorithm.GetHash( root ) ) )?.Tree;

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

    public void Clear()
    {
        this.KillAll();
        this.running.Clear();
        this.complete.Clear();
    }

    public void OnGraphModified()
    {
        Clear();
        
        Logger.Log("Copying Graph DS.", this, LogType.INFO);
        this.graphCopy = new Graph( this.graph );
    }

    public void KillAll()
    {
        Logger.Log( "Stopping all algorithms.", this, LogType.INFO );
        foreach ( KeyValuePair< int, Algorithm > kvp in this.running.ToList() )
            kvp.Value?.Kill();
    }

    ~AlgorithmManager()
    {
        this.KillAll();
        Controller.Singleton.OnGraphModified -= OnGraphModified;
    }

}
