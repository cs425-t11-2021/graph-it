
using System;
using System.Linq;
using System.Collections.Generic;

// TODO: algorithm manager needs to know when graph is updated so that it can kill all running algorithms and remove all completed
public class AlgorithmManager
{
    private Graph graph;
    private Action chromaticUI;
    private Action bipartiteUI;
    private Action primsUI;
    private Action kruskalsUI;
    private Action chromaticCalc;
    private Action bipartiteCalc;
    private Action primsCalc;
    private Action kruskalsCalc;
    private Dictionary< int, Algorithm > running; 
    public List< Algorithm > Running
    {
        get => this.running.Values.ToList();
    }
    private Dictionary< int, Algorithm > complete;
    public List< Algorithm > Complete
    {
        get => this.running.Values.ToList();
    }

    public AlgorithmManager( Graph graph, Action chromaticUI, Action bipartiteUI, Action primsUI, Action kruskalsUI, Action chromaticCalc, Action bipartiteCalc, Action primsCalc, Action kruskalsCalc )
    {
        this.graph = graph;
        this.chromaticUI = chromaticUI;
        this.bipartiteUI = bipartiteUI;
        this.primsUI = primsUI;
        this.kruskalsUI = kruskalsUI;
        this.chromaticCalc = chromaticCalc;
        this.bipartiteCalc = bipartiteCalc;
        this.primsCalc = primsCalc;
        this.kruskalsCalc = kruskalsCalc;
        this.running  = new Dictionary< int, Algorithm >();
        this.complete = new Dictionary< int, Algorithm >();
    }

    public void RunAll()
    {
        this.RunChromatic();
        this.RunPrims();
        this.RunKruskals();
    }

    public void RunChromatic()
    {
        new ChromaticAlgorithm( this.graph, this.chromaticUI, this.chromaticCalc, this.MarkRunning, this.MarkComplete, this.UnmarkRunning ).RunThread();
    }

    public void RunBipartite()
    {
        // EnsureChromaticRunning();
        new BipartiteAlgorithm( this.graph, this.bipartiteUI, this.bipartiteCalc, this.MarkRunning, this.MarkComplete, this.UnmarkRunning ).RunThread();
    }

    public void RunPrims()
    {
        // TODO: retrieve vert from selection manager and check if only a single vertex is selected, maybe subscribe to OnSelectionChange
        // new PrimsAlgorithm( this.graph, vert, this.primsUI, this.primsCalc, this.MarkRunning, this.MarkComplete ).Run();
    }

    public void RunKruskals()
    {
        new KruskalsAlgorithm( this.graph, this.kruskalsUI, this.kruskalsCalc, this.MarkRunning, this.MarkComplete, this.UnmarkRunning ).RunThread();
    }

    public void EnsureChromaticRunning()
    {
        int hash = ChromaticAlgorithm.GetHash();
        if ( !this.IsRunning( hash ) && !this.IsComplete( hash ) )
            new ChromaticAlgorithm( this.graph, this.chromaticUI, this.chromaticCalc, this.MarkRunning, this.MarkComplete, this.UnmarkRunning ).RunThread();
    }

    public void EnsurePrimsRunning( Vertex vert )
    {
        int hash = PrimsAlgorithm.GetHash( vert );
        if ( !this.IsRunning( hash ) && !this.IsComplete( hash ) )
            new PrimsAlgorithm( this.graph, vert, this.primsUI, this.primsCalc, this.MarkRunning, this.MarkComplete, this.UnmarkRunning ).RunThread();
    }

    public void EnsureKruskalsRunning()
    {
        int hash = KruskalsAlgorithm.GetHash();
        if ( !this.IsRunning( hash ) && !this.IsComplete( hash ) )
            new KruskalsAlgorithm( this.graph, this.kruskalsUI, this.kruskalsCalc, this.MarkRunning, this.MarkComplete, this.UnmarkRunning ).RunThread();
    }

    public int? GetChromaticNumber() => ( ( ChromaticAlgorithm ) this.complete.GetValue( ChromaticAlgorithm.GetHash() ) )?.ChromaticNumber;

    public bool? GetBipartite() => ( ( BipartiteAlgorithm ) this.complete.GetValue( BipartiteAlgorithm.GetHash() ) )?.IsBipartite;

    public void MarkRunning( Algorithm algo )
    {
        this.running[ algo.GetHashCode() ] = algo;
    }

    public void MarkComplete( Algorithm algo )
    {
        this.UnmarkRunning( algo );
        this.complete[ algo.GetHashCode() ] = algo;
    }

    public void UnmarkRunning( Algorithm algo )
    {
        this.running.Remove( algo.GetHashCode() );
    }

    // public bool IsRunning( Type Algo ) => this.IsRunning( Algo.GetHash() );

    public bool IsRunning( Algorithm algo ) => this.IsRunning( algo.GetHashCode() );

    public bool IsRunning( int key ) => this.running.ContainsKey( key );

    // public bool IsComplete( Type Algo ) => this.IsComplete( Algo.GetHash() );

    public bool IsComplete( Algorithm algo ) => this.IsComplete( algo.GetHashCode() );

    public bool IsComplete( int key ) => this.complete.ContainsKey( key );

    public void KillAll()
    {
        foreach ( KeyValuePair< int, Algorithm > kvp in this.running )
            kvp.Value.Kill();
    }
}
