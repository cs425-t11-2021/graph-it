
using System;
using System.Linq;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Concurrent;
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

    // public Action minDegreeUI;
    // public Action minDegreeCalc;
    // public Action maxDegreeUI;
    // public Action maxDegreeCalc;
    // public Action radiusUI;
    // public Action radiusCalc;
    // public Action diameterUI;
    // public Action diameterCalc;
    // public Action chromaticUI;
    // public Action chromaticCalc;
    // public Action bipartiteUI;
    // public Action bipartiteCalc;
    // public Action cyclicUI;
    // public Action cyclicCalc;
    // public Action fleurysUI;
    // public Action fleurysCalc;
    // public Action primsUI;
    // public Action primsCalc;
    // public Action kruskalsUI;
    // public Action kruskalsCalc;
    // public Action dijkstrasUI;
    // public Action dijkstrasCalc;
    // public Action bellmanFordsUI;
    // public Action bellmanFordsCalc;
    // public Action depthFirstSearchUI;
    // public Action depthFirstSearchCalc;
    // public Action breadthFirstSearchUI;
    // public Action breadthFirstSearchCalc;

    public void Initiate(
        Graph graph
        // Action minDegreeUI,
        // Action minDegreeCalc,
        // Action maxDegreeUI,
        // Action maxDegreeCalc,
        // Action radiusUI,
        // Action radiusCalc,
        // Action diameterUI,
        // Action diameterCalc,
        // Action chromaticUI,
        // Action chromaticCalc,
        // Action bipartiteUI,
        // Action bipartiteCalc,
        // Action cyclicUI,
        // Action cyclicCalc,
        // Action fleurysUI,
        // Action fleurysCalc,
        // Action primsUI,
        // Action primsCalc,
        // Action kruskalsUI,
        // Action kruskalsCalc,
        // Action dijkstrasUI,
        // Action dijkstrasCalc,
        // Action bellmanFordsUI,
        // Action bellmanFordsCalc,
        // Action depthFirstSearchUI,
        // Action depthFirstSearchCalc,
        // Action breadthFirstSearchUI,
        // Action breadthFirstSearchCalc
    )
    {
        Controller.Singleton.OnGraphModified += Clear;
        this.graph = graph;
        this.graphCopy = new Graph( graph );

        // this.minDegreeUI = minDegreeUI;
        // this.minDegreeCalc = minDegreeCalc;
        // this.maxDegreeUI = maxDegreeUI;
        // this.maxDegreeCalc = maxDegreeCalc;
        // this.radiusUI = radiusUI;
        // this.radiusCalc = radiusCalc;
        // this.diameterUI = diameterUI;
        // this.diameterCalc = diameterCalc;
        // this.chromaticUI = chromaticUI;
        // this.chromaticCalc = chromaticCalc;
        // this.bipartiteUI = bipartiteUI;
        // this.bipartiteCalc = bipartiteCalc;
        // this.cyclicUI = cyclicUI;
        // this.cyclicCalc = cyclicCalc;
        // this.fleurysUI = fleurysUI;
        // this.fleurysCalc = fleurysCalc;
        // this.primsUI = primsUI;
        // this.primsCalc = primsCalc;
        // this.kruskalsUI = kruskalsUI;
        // this.kruskalsCalc = kruskalsCalc;
        // this.dijkstrasUI = dijkstrasUI;
        // this.dijkstrasCalc = dijkstrasCalc;
        // this.bellmanFordsUI = bellmanFordsUI;
        // this.bellmanFordsCalc = bellmanFordsCalc;
        // this.depthFirstSearchUI = depthFirstSearchUI;
        // this.depthFirstSearchCalc = depthFirstSearchCalc;
        // this.breadthFirstSearchUI = breadthFirstSearchUI;
        // this.breadthFirstSearchCalc = breadthFirstSearchCalc;
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

    private void RunAlgorithm( Type algorithm, object[] parms )
    {
        Algorithm algorithmInstance = ( Algorithm ) Activator.CreateInstance( algorithm, parms.Prepend( this ).ToArray() );
        algorithmInstance.RunThread();
    }

    private void EnsureRunning( Type algorithm, params object[] parms )
    {
        // Debug.Log( algoParms.Length );
        int hash = ( int ) algorithm.GetMethod( "GetHash" ).Invoke( null, parms );
        if ( !this.IsRunning( hash ) && !this.IsComplete( hash ) )
            this.RunAlgorithm( algorithm, parms );
        
        // TODO: check if this is needed, but if algorithm is not complete, then invoke ui delegate
        // JIMSON: I ADDED CODE TO DO THIS BELOW
        if (this.IsComplete(hash))
        {
            GraphInfo.Singleton.UpdateGraphInfoResults(this.complete[hash]);
        }
        
    }

    public void RunMinDegree() => this.EnsureRunning( typeof ( MinDegreeAlgorithm ) );

    public void RunMaxDegree() => this.EnsureRunning( typeof ( MaxDegreeAlgorithm ) );

    public void RunRadius() => this.EnsureRunning( typeof ( RadiusAlgorithm ) );

    public void RunDiameter() => this.EnsureRunning( typeof ( DiameterAlgorithm ) );

    public void RunChromatic() => this.EnsureRunning( typeof ( ChromaticAlgorithm ) );

    public void RunBipartite() => this.EnsureRunning( typeof ( BipartiteAlgorithm ) );

    public void RunCyclic() => this.EnsureRunning( typeof ( CyclicAlgorithm ) );

    public void RunFleurys() => this.EnsureRunning( typeof ( FleurysAlgorithm ) );

    public void RunPrims( Vertex vert ) => this.EnsureRunning( typeof ( PrimsAlgorithm ), vert );

    public void RunKruskals() => this.EnsureRunning( typeof ( KruskalsAlgorithm ) );

    public void RunDijkstras( Vertex src, Vertex dest ) => this.EnsureRunning( typeof ( DijkstrasAlgorithm ), src, dest );

    public void RunBellmanFords( Vertex src, Vertex dest ) => this.EnsureRunning( typeof ( BellmanFordsAlgorithm ), src, dest );

    public void RunDepthFirstSearch( Vertex vert, Action< Edge, Vertex > action ) => this.EnsureRunning( typeof ( DepthFirstSearchAlgorithm ), vert, action );

    public void RunBreadthFirstSearch( Vertex vert, Action< Edge, Vertex > action ) => this.EnsureRunning( typeof ( BreadthFirstSearchAlgorithm ), vert, action );

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
    }

}
