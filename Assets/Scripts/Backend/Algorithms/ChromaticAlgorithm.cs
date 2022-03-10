
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

[System.Serializable]
public class ChromaticAlgorithm : Algorithm
{
    private static ConcurrentDictionary< Graph, int > maxDegrees = new ConcurrentDictionary< Graph, int >();
    public int ChromaticNumber { get; private set; }
    public int[] Coloring { get; private set; }

    public ChromaticAlgorithm( Graph graph, Action updateUI, Action updateCalc, Action< Algorithm > markRunning, Action< Algorithm > markComplete, Action< Algorithm > unmarkRunning ) : base( graph, updateUI, updateCalc, markRunning, markComplete, unmarkRunning ) { }

    public override void Run()
    {
        // TODO: if HasLoops, then throw error

        this.Coloring = new int[ this.Graph.Order ];
        int chi = Math.Min( this.Graph.Order, 1 ); // TODO: get lower bound
        this.WaitUntil( () => ChromaticAlgorithm.HasMaxDegree( this.Graph ) );
        int upperBound = ChromaticAlgorithm.maxDegrees[ this.Graph ] + 1;
        while ( !this.IsProperColoring() )
        {
            if ( chi > upperBound ) // something really bad happended
                throw new System.Exception( "Coloring could not be computed." );
            if ( this.Coloring.Min() >= chi - 1 )
            {
                chi++;
                // RunInMain.Singleton.queuedTasks.Enqueue( () => Debug.Log( "increment chi to " + chi ) );
                Array.Clear( this.Coloring, 0, this.Coloring.Length );
            }
            // RunInMain.Singleton.queuedTasks.Enqueue( () => Debug.Log( "update color" ) );
            this.UpdateColoring( chi );
            // foreach ( int c in this.Coloring )
                // RunInMain.Singleton.queuedTasks.Enqueue( () => Debug.Log( c ) );
        }
        // RunInMain.Singleton.queuedTasks.Enqueue( () => Debug.Log( "finished coloring" ) );
        this.ChromaticNumber = chi;

        BipartiteAlgorithm.SetChromaticNumber( this.Graph, chi );
    }

    private bool IsProperColoring()
    {
        foreach ( Edge edge in this.Graph.Adjacency.Values )
        {
            if ( edge.vert1 == edge.vert2 ) // temp
                continue;
            if ( this.Coloring[ this.Graph.Vertices.IndexOf( edge.vert1 ) ] == this.Coloring[ this.Graph.Vertices.IndexOf( edge.vert2 ) ] )
                return false;
        }
        return true;
    }

    private void UpdateColoring( int colors )
    {
        this.UpdateColoringHelper( colors, 0 );
    }

    private void UpdateColoringHelper( int colors, int index )
    {
        if ( index < this.Coloring.Length )
        {
            if ( this.Coloring[ index ] < colors - 1 )
            {
                this.Coloring[ index ]++;
                for ( int i = 0; i < index; ++i )
                    this.Coloring[ i ] = 0;
            }
            else
                this.UpdateColoringHelper( colors, ++index );
        }
    }

    private static bool HasMaxDegree( Graph graph ) => ChromaticAlgorithm.maxDegrees.GetValue( graph, -1 ) != -1;

    public static void SetMaxDegree( Graph graph, int maxDegree ) => ChromaticAlgorithm.maxDegrees[ graph ] = maxDegree;

    public override void Kill()
    {
        base.Kill();
        BipartiteAlgorithm.ClearChromaticNumber( this.Graph );
    }

    public static int GetHash() => typeof ( ChromaticAlgorithm ).GetHashCode();

    public override int GetHashCode() => ChromaticAlgorithm.GetHash();
}
