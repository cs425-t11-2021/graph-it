
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;

[System.Serializable]
public class BipartiteAlgorithm : Algorithm
{
    private bool isBipartite;
    private HashSet< Vertex > set1;
    private HashSet< Vertex > set2;

    public BipartiteAlgorithm( AlgorithmManager algoManager, bool display ) : base( algoManager ) { }

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
        this.AlgoManager.RunChromatic( false );
        this.WaitUntilChromaticComplete();
        foreach (var key in this.AlgoManager.GetChromatic().results.Keys)
        {
            Logger.Log(key, this, LogType.INFO);
        }
        this.isBipartite = ( int ) this.AlgoManager.GetChromatic().results[ "chromatic number" ].Item1 <= 2;
    }

    private bool TwoColorHelper( Vertex vert, bool color, HashSet< Vertex > visited )
    {
        HashSet< Vertex > neighbors = this.GetNeighborhood( vert );
        neighbors.Remove( vert );
        HashSet< Vertex > homogenousNeighbors = new HashSet< Vertex >( neighbors );
        homogenousNeighbors.Intersect( color ? this.set2 : this.set1 );
        if ( homogenousNeighbors.Count > 0 )
            return false;
        neighbors = new HashSet< Vertex >( neighbors.Except( visited ) );
        foreach ( Vertex neighbor in neighbors )
        {
            if ( color )
                this.set1.Add( neighbor ); // set1 has color false
            else
                this.set2.Add( neighbor ); // set2 has color true
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

    public override AlgorithmResult GetResult()
    {
        if ( this.error )
            return this.GetErrorResult();
        if ( this.running )
            return this.GetRunningResult();
        AlgorithmResult result = new AlgorithmResult( AlgorithmResultType.SUCCESS );
        result.results[ "bipartite" ] = ( this.isBipartite, typeof ( bool ) );
        result.results[ "bipartite bipartition 1" ] = ( this.set1, typeof ( HashSet< Vertex > ) );
        result.results[ "bipartite bipartition 2" ] = ( this.set2, typeof ( HashSet< Vertex > ) );
        return result;
    }

    private void WaitUntilChromaticComplete()
    {
        this.WaitUntilAlgorithmComplete( ChromaticAlgorithm.GetHash() );
    }

    public static int GetHash() => typeof ( BipartiteAlgorithm ).GetHashCode();

    public override int GetHashCode() => BipartiteAlgorithm.GetHash();
}
