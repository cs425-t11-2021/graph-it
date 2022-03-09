
using System;
using System.Linq;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// using UnityEngine;

// TODO: algorithm manager needs to know when graph is updated so that it can kill all running algorithms and remove all completed
public class AlgorithmManager
{
    private Graph graph;
    private Action minDegreeUI;
    private Action maxDegreeUI;
    private Action chromaticUI;
    private Action bipartiteUI;
    private Action primsUI;
    private Action kruskalsUI;
    private Action depthFirstSearchUI;
    private Action breadthFirstSearchUI;
    private Action minDegreeCalc;
    private Action maxDegreeCalc;
    private Action chromaticCalc;
    private Action bipartiteCalc;
    private Action primsCalc;
    private Action kruskalsCalc;
    private Action depthFirstSearchCalc;
    private Action breadthFirstSearchCalc;
    private CancellationTokenSource minDegreeCancellationTokenSource;
    private CancellationTokenSource maxDegreeCancellationTokenSource;
    private CancellationTokenSource chromaticCancellationTokenSource;
    private CancellationTokenSource bipartiteCancellationTokenSource;
    private CancellationTokenSource primsCancellationTokenSource;
    private CancellationTokenSource kruskalsCancellationTokenSource;
    private CancellationTokenSource depthFirstSearchCancellationTokenSource;
    private CancellationTokenSource breadthFirstSearchCancellationTokenSource;

    private Dictionary< int, Algorithm > running = new Dictionary< int, Algorithm >(); 
    public List< Algorithm > Running
    {
        get => this.running.Values.ToList();
    }
    private Dictionary< int, Algorithm > complete = new Dictionary< int, Algorithm >();
    public List< Algorithm > Complete
    {
        get => this.running.Values.ToList();
    }

    public void Initiate( Graph graph, Action minDegreeUI, Action maxDegreeUI, Action chromaticUI, Action bipartiteUI, Action primsUI, Action kruskalsUI, Action depthFirstSearchUI, Action breadthFirstSearchUI, Action minDegreeCalc, Action maxDegreeCalc, Action chromaticCalc, Action bipartiteCalc, Action primsCalc, Action kruskalsCalc, Action depthFirstSearchCalc, Action breadthFirstSearchCalc )
    {
        Controller.Singleton.OnGraphModified += Clear;
        this.graph = graph;
        this.minDegreeUI = minDegreeUI;
        this.maxDegreeUI = maxDegreeUI;
        this.chromaticUI = chromaticUI;
        this.bipartiteUI = bipartiteUI;
        this.primsUI = primsUI;
        this.kruskalsUI = kruskalsUI;
        this.depthFirstSearchUI = depthFirstSearchUI;
        this.breadthFirstSearchUI = breadthFirstSearchUI;
        this.minDegreeCalc = minDegreeCalc;
        this.maxDegreeCalc = maxDegreeCalc;
        this.chromaticCalc = chromaticCalc;
        this.bipartiteCalc = bipartiteCalc;
        this.primsCalc = primsCalc;
        this.kruskalsCalc = kruskalsCalc;
        this.depthFirstSearchCalc = depthFirstSearchCalc;
        this.breadthFirstSearchCalc = breadthFirstSearchCalc;
    }

    public void RunAll()
    {
        this.RunMinDegree();
        this.RunMaxDegree();
        this.RunChromatic();
        // this.RunPrims();
        this.RunKruskals();
    }

    public void RunMinDegree()
    {
        this.minDegreeCancellationTokenSource = new CancellationTokenSource();
        new MinDegreeAlgorithm( this.graph, this.minDegreeCancellationTokenSource.Token, this.minDegreeUI, this.minDegreeCalc, this.MarkRunning, this.MarkComplete, this.UnmarkRunning ).RunThread();
    }

    public void RunMaxDegree()
    {
        this.maxDegreeCancellationTokenSource = new CancellationTokenSource();
        new MaxDegreeAlgorithm( this.graph, this.maxDegreeCancellationTokenSource.Token, this.maxDegreeUI, this.maxDegreeCalc, this.MarkRunning, this.MarkComplete, this.UnmarkRunning ).RunThread();
    }

    public void RunChromatic()
    {
        if (!IsComplete(ChromaticAlgorithm.GetHash()))
        {
            this.EnsureMaxDegreeRunning();
            this.chromaticCancellationTokenSource = new CancellationTokenSource();
            new ChromaticAlgorithm( this.graph, this.chromaticCancellationTokenSource.Token, this.chromaticUI, this.chromaticCalc, this.MarkRunning, this.MarkComplete, this.UnmarkRunning ).RunThread();
        }
        else
        {
            this.chromaticUI.Invoke();
        }
        
    }

    public void RunBipartite()
    {
        // BipartiteAlgorithm ba = new BipartiteAlgorithm( this.graph, this.bipartiteUI, this.bipartiteCalc, this.MarkRunning, this.MarkComplete, this.UnmarkRunning );
        // foreach ( Vertex vert in this.graph.Vertices )
        // {
        //     EnsureBreadthFirstSearchRunning( vert, ba.  ); 
        // }
        // ba.RunThread();

        if (!IsComplete(BipartiteAlgorithm.GetHash()))
        {
            this.EnsureChromaticRunning();
            this.bipartiteCancellationTokenSource = new CancellationTokenSource();
            new BipartiteAlgorithm(this.graph, this.bipartiteCancellationTokenSource.Token, this.bipartiteUI, this.bipartiteCalc, this.MarkRunning, this.MarkComplete, this.UnmarkRunning).RunThread();
        }
        else
        {
            this.chromaticUI.Invoke();
            this.bipartiteUI.Invoke();
        }
    }

    public void RunPrims( Vertex vert ) // temp parameter
    {
        // TODO: retrieve vert from selection manager and check if only a single vertex is selected, maybe subscribe to OnSelectionChange
        // new PrimsAlgorithm( this.graph, vert, this.primsUI, this.primsCalc, this.MarkRunning, this.MarkComplete, this.UnmarkRunning ).Run();
    }

    public void RunKruskals()
    {
        this.kruskalsCancellationTokenSource = new CancellationTokenSource();
        new KruskalsAlgorithm( this.graph, this.kruskalsCancellationTokenSource.Token, this.kruskalsUI, this.kruskalsCalc, this.MarkRunning, this.MarkComplete, this.UnmarkRunning ).RunThread();
    }

    public void RunDepthFirstSearch( Vertex vert, Action< Edge, Vertex > action ) // temp parameters
    {
        this.depthFirstSearchCancellationTokenSource = new CancellationTokenSource();
        new DepthFirstSearchAlgorithm( this.graph, vert, action, this.depthFirstSearchCancellationTokenSource.Token, this.depthFirstSearchUI, this.depthFirstSearchCalc, this.MarkRunning, this.MarkComplete, this.UnmarkRunning ).RunThread();
    }

    public void RunBreadthFirstSearch( Vertex vert, Action< Edge, Vertex > action ) // temp parameters
    {
        this.breadthFirstSearchCancellationTokenSource = new CancellationTokenSource();
        new BreadthFirstSearchAlgorithm( this.graph, vert, action, this.breadthFirstSearchCancellationTokenSource.Token, this.breadthFirstSearchUI, this.breadthFirstSearchCalc, this.MarkRunning, this.MarkComplete, this.UnmarkRunning ).RunThread();
    }

    public void EnsureMinDegreeRunning()
    {
        int hash = MinDegreeAlgorithm.GetHash();
        if ( !this.IsRunning( hash ) && !this.IsComplete( hash ) )
        {
            this.minDegreeCancellationTokenSource = new CancellationTokenSource();
            new MinDegreeAlgorithm( this.graph, this.minDegreeCancellationTokenSource.Token, this.minDegreeUI, this.minDegreeCalc, this.MarkRunning, this.MarkComplete, this.UnmarkRunning ).RunThread();
        }
    }

    public void EnsureMaxDegreeRunning()
    {
        int hash = MaxDegreeAlgorithm.GetHash();
        if ( !this.IsRunning( hash ) && !this.IsComplete( hash ) )
        {
            this.maxDegreeCancellationTokenSource = new CancellationTokenSource();
            new MaxDegreeAlgorithm( this.graph, this.maxDegreeCancellationTokenSource.Token, this.maxDegreeUI, this.maxDegreeCalc, this.MarkRunning, this.MarkComplete, this.UnmarkRunning ).RunThread();
        }
    }

    public void EnsureChromaticRunning()
    {
        this.EnsureMaxDegreeRunning();

        int hash = ChromaticAlgorithm.GetHash();
        if ( !this.IsRunning( hash ) && !this.IsComplete( hash ) )
        {
            this.chromaticCancellationTokenSource = new CancellationTokenSource();
            new ChromaticAlgorithm( this.graph, this.chromaticCancellationTokenSource.Token, this.chromaticUI, this.chromaticCalc, this.MarkRunning, this.MarkComplete, this.UnmarkRunning ).RunThread();
        }
    }

    public void EnsureBipartiteRunning()
    {
        int hash = BipartiteAlgorithm.GetHash();
        if ( !this.IsRunning( hash ) && !this.IsComplete( hash ) )
        {
            this.bipartiteCancellationTokenSource = new CancellationTokenSource();
            new BipartiteAlgorithm(this.graph, this.bipartiteCancellationTokenSource.Token, this.bipartiteUI, this.bipartiteCalc, this.MarkRunning, this.MarkComplete, this.UnmarkRunning).RunThread();
        }
    }

    public void EnsurePrimsRunning( Vertex vert )
    {
        int hash = PrimsAlgorithm.GetHash( vert );
        if ( !this.IsRunning( hash ) && !this.IsComplete( hash ) )
        {
            // new PrimsAlgorithm( this.graph, vert, this.primsUI, this.primsCalc, this.MarkRunning, this.MarkComplete, this.UnmarkRunning ).RunThread();
        }
    }

    public void EnsureKruskalsRunning()
    {
        int hash = KruskalsAlgorithm.GetHash();
        if ( !this.IsRunning( hash ) && !this.IsComplete( hash ) )
        {
            this.kruskalsCancellationTokenSource = new CancellationTokenSource();
            new KruskalsAlgorithm( this.graph, this.kruskalsCancellationTokenSource.Token, this.kruskalsUI, this.kruskalsCalc, this.MarkRunning, this.MarkComplete, this.UnmarkRunning ).RunThread();
        }
    }

    public void EnsureDepthFirstSearchRunning( Vertex vert, Action< Edge, Vertex > action )
    {
        int hash = DepthFirstSearchAlgorithm.GetHash( vert );
        if ( !this.IsRunning( hash ) && !this.IsComplete( hash ) )
        {
            this.depthFirstSearchCancellationTokenSource = new CancellationTokenSource();
            new DepthFirstSearchAlgorithm( this.graph, vert, action, this.depthFirstSearchCancellationTokenSource.Token, this.depthFirstSearchUI, this.depthFirstSearchCalc, this.MarkRunning, this.MarkComplete, this.UnmarkRunning ).RunThread();
        }
    }

    public void EnsureBreadthFirstSearchRunning( Vertex vert, Action< Edge, Vertex > action )
    {
        int hash = BreadthFirstSearchAlgorithm.GetHash( vert );
        if ( !this.IsRunning( hash ) && !this.IsComplete( hash ) )
        {
            this.breadthFirstSearchCancellationTokenSource = new CancellationTokenSource();
            new BreadthFirstSearchAlgorithm( this.graph, vert, action, this.breadthFirstSearchCancellationTokenSource.Token, this.breadthFirstSearchUI, this.breadthFirstSearchCalc, this.MarkRunning, this.MarkComplete, this.UnmarkRunning ).RunThread();
        }
    }

    public int? GetMinDegree() => ( ( MinDegreeAlgorithm ) this.complete.GetValue( MinDegreeAlgorithm.GetHash() ) )?.MinDegree;

    public int? GetMaxDegree() => ( ( MaxDegreeAlgorithm ) this.complete.GetValue( MaxDegreeAlgorithm.GetHash() ) )?.MaxDegree;

    public int? GetChromaticNumber() => ( ( ChromaticAlgorithm ) this.complete.GetValue( ChromaticAlgorithm.GetHash() ) )?.ChromaticNumber;

    public bool? GetBipartite() => ( ( BipartiteAlgorithm ) this.complete.GetValue( BipartiteAlgorithm.GetHash() ) )?.IsBipartite;

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
        this.running.Remove( algo.GetHashCode() );
    }

    // public bool IsRunning( Type Algo ) => this.IsRunning( Algo.GetHash() );

    public bool IsRunning( Algorithm algo ) => this.IsRunning( algo.GetHashCode() );

    public bool IsRunning( int key ) => this.running.ContainsKey( key );

    // public bool IsComplete( Type Algo ) => this.IsComplete( Algo.GetHash() );

    public bool IsComplete( Algorithm algo ) => this.IsComplete( algo.GetHashCode() );

    public bool IsComplete( int key ) => this.complete.ContainsKey( key );

    public void Clear()
    {
        this.KillAll();
        this.running.Clear();
        this.complete.Clear();
    }

    public void KillAll()
    {
        Logger.Log("Stopping all algorithms.", this, LogType.INFO);
        // foreach ( KeyValuePair< int, Algorithm > kvp in this.running )
        //     kvp.Value?.RequestKill();
        this.minDegreeCancellationTokenSource?.Cancel();
        this.maxDegreeCancellationTokenSource?.Cancel();
        this.chromaticCancellationTokenSource?.Cancel();
        this.bipartiteCancellationTokenSource?.Cancel();
        this.primsCancellationTokenSource?.Cancel();
        this.kruskalsCancellationTokenSource?.Cancel();
        this.depthFirstSearchCancellationTokenSource?.Cancel();
        this.breadthFirstSearchCancellationTokenSource?.Cancel();
    }

    ~AlgorithmManager()
    {
        this.KillAll();
    }

    
}
