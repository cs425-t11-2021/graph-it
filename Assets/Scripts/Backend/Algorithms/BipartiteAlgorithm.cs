
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;

[System.Serializable]
public class BipartiteAlgorithm : Algorithm
{
    public bool IsBipartite { get; private set; }
    public HashSet< Vertex > Set1 { get; private set; }
    public HashSet< Vertex > Set2 { get; private set; }

    public BipartiteAlgorithm( AlgorithmManager algoManager ) : base( algoManager ) { }

    public override void Run()
    {
        // this.Set1 = new HashSet< Vertex >();
        // this.Set2 = new HashSet< Vertex >();

        // int? chi = BipartiteAlgorithm.chromaticNumbers.GetValue( this.Graph );

        // if ( !( chi is null ) )
            // IsBipartite = chi <= 2;
        // else
        // {
        //     // TODO: need to rework
        //     this.IsBipartite = true;
        //     HashSet< Vertex > visited = new HashSet< Vertex >();
        //     HashSet< Vertex > unvisited = new HashSet< Vertex >();
        //     foreach ( Vertex vert in unvisited )
        //     {
        //         visited.Add( vert );
        //         this.Set1.Add( vert );
        //         this.IsBipartite = this.IsBipartite && this.TwoColorHelper( vert, false, visited );
        //         unvisited.Except( visited );
        //     }
        // }

        // temp
        this.WaitUntilChromaticComplete();
        this.IsBipartite = ( int ) this.AlgoManager.GetChromaticNumber() <= 2;
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
        return new HashSet< Vertex >( this.Graph.GetIncidentEdges( vert ).Select( edge => edge.vert1 == vert ? edge.vert2 : edge.vert1 ) );
    }

    private void WaitUntilChromaticComplete()
    {
        this.WaitUntilAlgorithmComplete( ChromaticAlgorithm.GetHash() );
    }

    public static int GetHash() => typeof ( BipartiteAlgorithm ).GetHashCode();

    public override int GetHashCode() => BipartiteAlgorithm.GetHash();
}
