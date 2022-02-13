
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

[System.Serializable]
public class BipartiteAlgorithm : Algorithm
{
    private static Dictionary< Graph, int? > chromaticNumbers = new Dictionary< Graph, int? >();
    public bool IsBipartite { get; private set; }
    public HashSet< Vertex > Set1 { get; private set; }
    public HashSet< Vertex > Set2 { get; private set; }

    public BipartiteAlgorithm( Graph graph, Action updateUI, Action updateCalc, Action< Algorithm > markRunning, Action< Algorithm > markComplete, Action< Algorithm > unmarkRunning ) : base( graph, updateUI, updateCalc, markRunning, markComplete, unmarkRunning ) { }

    public override void Run() // TODO: index out of range error here?
    {
        this.Set1 = new HashSet< Vertex >();
        this.Set2 = new HashSet< Vertex >();

        int? chi = BipartiteAlgorithm.chromaticNumbers.GetValue( this.graph );
        if ( !( chi is null ) )
            IsBipartite = chi <= 2;
        else
        {
            // TODO: need to rework
            this.IsBipartite = true;
            HashSet< Vertex > visited = new HashSet< Vertex >();
            HashSet< Vertex > unvisited = new HashSet< Vertex >();
            foreach ( Vertex vert in unvisited )
            {
                visited.Add( vert );
                this.Set1.Add( vert );
                this.IsBipartite = this.IsBipartite && this.TwoColorHelper( vert, false, visited );
                unvisited.Except( visited );
            }
        }
    }

    private bool TwoColorHelper( Vertex vert, bool color, HashSet< Vertex > visited )
    {
        HashSet< Vertex > neighbors = this.GetNeighborhood( vert );
        neighbors.Remove( vert );
        HashSet< Vertex > homogenousNeighbors = new HashSet< Vertex >( neighbors );
        homogenousNeighbors.Intersect( color ? Set2 : Set1 );
        if ( homogenousNeighbors.Count > 0 )
            return false;
        neighbors = new HashSet< Vertex >( neighbors.Except( visited ) );
        foreach ( Vertex neighbor in neighbors )
        {
            if ( color )
                this.Set1.Add( neighbor ); // Set1 has color false
            else
                this.Set2.Add( neighbor ); // Set2 has color true
            visited.Add( neighbor );
            if ( !this.TwoColorHelper( neighbor, !color, visited ) )
                return false;
        }
        return true;
    }

    private HashSet< Vertex > GetNeighborhood( Vertex vert )
    {
        HashSet< Vertex > neighbors = new HashSet< Vertex >();
        foreach ( KeyValuePair< ( Vertex, Vertex ), Edge > kvp in this.graph.adjacency )
        {
            if ( kvp.Key.Item1 == vert )
                neighbors.Add( kvp.Key.Item2 );
            else if ( kvp.Key.Item2 == vert )
                neighbors.Add( kvp.Key.Item1 );
        }
        return neighbors;
    }

    public static void SetChromaticNumber( Graph graph, int chromaticNumber )
    {
        BipartiteAlgorithm.chromaticNumbers[ graph ] = chromaticNumber;
    }

    public static int GetHash() => typeof ( BipartiteAlgorithm ).GetHashCode();

    public override int GetHashCode() => BipartiteAlgorithm.GetHash();
}
