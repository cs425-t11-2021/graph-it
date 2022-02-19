
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChromaticAlgorithm : Algorithm
{
    public int ChromaticNumber { get; private set; }

    public ChromaticAlgorithm( Graph graph, Action updateUI, Action updateCalc, Action< Algorithm > markRunning, Action< Algorithm > markComplete, Action< Algorithm > unmarkRunning ) : base( graph, updateUI, updateCalc, markRunning, markComplete, unmarkRunning ) { }

    public override void Run()
    {
        int chi = this.Graph.Vertices.Count;
        HashSet< byte[] > colorings = this.GetAllColorings();
        foreach ( byte[] coloring in colorings )
        {
            int numColors = new HashSet< byte >( coloring ).Count;
            if ( numColors < chi && this.IsProperColoring( coloring ) )
                chi = numColors;
        }

        this.ChromaticNumber = chi;

        BipartiteAlgorithm.SetChromaticNumber( this.Graph, chi );
    }

    private bool IsProperColoring( byte[] coloring )
    {
        foreach ( Edge edge in this.Graph.Adjacency.Values )
        {
            if ( coloring[ this.Graph.Vertices.IndexOf( edge.vert1 ) ] == coloring[ this.Graph.Vertices.IndexOf( edge.vert2 ) ] )
                return false;
        }
        return true;
    }

    private HashSet< byte[] > GetAllColorings()
    {
        HashSet< byte[] > colorings = new HashSet< byte[] >();
        GetAllColoringsHelper( colorings, new List< byte >(), (byte) this.Graph.Vertices.Count, (byte) this.Graph.Vertices.Count );
        return colorings;
    }

    private static void GetAllColoringsHelper( HashSet< byte[] > colorings, List<byte> coloring, byte numVertices, byte numColors )
    {
        if ( coloring.Count >= numVertices )
            colorings.Add( coloring.ToArray() );
        else
        {
            for ( byte i = 0; i < numColors; i++ )
            {
                List< byte > newColoring = new List< byte >( coloring );
                newColoring.Add(i);
                GetAllColoringsHelper( colorings, newColoring, numVertices, numColors );
            }
        }
    }

    public override void Kill()
    {
        base.Kill();
        BipartiteAlgorithm.ClearChromaticNumber( this.Graph );
    }

    public static int GetHash() => typeof ( ChromaticAlgorithm ).GetHashCode();

    public override int GetHashCode() => ChromaticAlgorithm.GetHash();
}
