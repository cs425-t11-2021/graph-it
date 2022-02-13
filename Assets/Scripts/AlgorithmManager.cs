
using System;
using System.Linq;
using System.Collections.Generic;

public class AlgorithmManager
{
    private Graph graph;
    private Action chromaticUI;
    private Action primsUI;
    private Action kruskalsUI;
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

    public AlgorithmManager( Graph graph, Action chromaticUI, Action primsUI, Action kruskalsUI )
    {
        this.graph = graph;
        this.chromaticUI = chromaticUI;
        this.primsUI = primsUI;
        this.kruskalsUI = kruskalsUI;
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
        new ChromaticAlgorithm( this.graph, this.chromaticUI, this.MarkRunning, this.MarkComplete ).RunThread();
    }

    public void RunPrims()
    {
        // TODO: retrieve vert from selection manager and check if only a single vertex is selected, maybe subscribe to OnSelectionChange
        // new PrimsAlgorithm( this.graph, vert, this.primsUI, this.MarkRunning, this.MarkComplete ).Run();
    }

    public void RunKruskals()
    {
        new KruskalsAlgorithm( this.graph, this.kruskalsUI, this.MarkRunning, this.MarkComplete ).RunThread();
    }

    public void EnsureChromaticRunning()
    {
        int hash = ChromaticAlgorithm.GetHashCode();
        if ( !this.IsRunning( hash ) && !this.IsComplete( hash ) )
            new ChromaticAlgorithm( this.graph, this.chromaticUI, this.MarkRunning, this.MarkComplete ).RunThread();
    }

    public void EnsurePrimsRunning( Vertex vert )
    {
        int hash = PrimsAlgorithm.GetHashCode( vert );
        if ( !this.IsRunning( hash ) && !this.IsComplete( hash ) )
            new PrimsAlgorithm( this.graph, vert, this.chromaticUI, this.MarkRunning, this.MarkComplete ).RunThread();
    }

    public void EnsureKruskalsRunning()
    {
        int hash = KruskalsAlgorithm.GetHashCode();
        if ( !this.IsRunning( hash ) && !this.IsComplete( hash ) )
            new KruskalsAlgorithm( this.graph, this.chromaticUI, this.MarkRunning, this.MarkComplete ).RunThread();
    }

    // // ensures algorithm Algo is currently running or completed
    // // can only be applied to algorithms without parameters
    // public void EnsureRunning( Type Algo, Action updateUI )
    // {
    //     int hash = Algo.GetHashCode();
    //     if ( !this.IsRunning( hash ) && !this.IsComplete( hash ) )
    //         Activator.CreateInstance( Algo, new Object[] { this.graph, updateUI, ( Action< Algorithm > ) this.MarkRunning, ( Action< Algorithm > ) this.MarkComplete } ).Run();
    // }


    // // can only be applied to algorithms with one parameter
    // public void EnsureRunning( Type Algo, Action updateUI, Object param )
    // {
    //     int hash = Algo.GetHashCode();
    //     if ( !this.IsRunning( hash ) && !this.IsComplete( hash ) )
    //         Activator.CreateInstance( Algo, new Object[] { this.graph, param, updateUI, ( Action< Algorithm > ) this.MarkRunning, ( Action< Algorithm > ) this.MarkComplete } ).Run();
    // }

    public int GetChromaticNumber() {
        if ( this.complete.ContainsKey( ChromaticAlgorithm.GetHashCode() ) )
            return -1;
        return ( ( ChromaticAlgorithm ) this.complete[ ChromaticAlgorithm.GetHashCode() ] ).ChromaticNumber;
    }

    public void MarkRunning( Algorithm algo )
    {
        this.running[ algo.GetHashCode() ] = algo;
    }

    public void MarkComplete( Algorithm algo )
    {
        this.running.Remove( algo.GetHashCode() );
        this.complete[ algo.GetHashCode() ] = algo;
    }

    public bool IsRunning( Type Algo ) => this.IsRunning( Algo.GetHashCode() );

    public bool IsRunning( Algorithm algo ) => this.IsRunning( algo.GetHashCode() );

    public bool IsRunning( int key ) => this.running.ContainsKey( key );

    public bool IsComplete( Type Algo ) => this.IsComplete( Algo.GetHashCode() );

    public bool IsComplete( Algorithm algo ) => this.IsComplete( algo.GetHashCode() );

    public bool IsComplete( int key ) => this.complete.ContainsKey( key );

    public void KillAll()
    {
        foreach ( KeyValuePair< int, Algorithm > kvp in this.running )
            kvp.Value.Kill();
    }
}
