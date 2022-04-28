
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class GirthAlgorithm : Algorithm
{
    private int girth;
    private List< Vertex > cycleVertices;
    private List< Edge > cycleEdges;

    private Graph graphCopy;

    public GirthAlgorithm( AlgorithmManager algoManager, bool display ) : base( algoManager )
    {
        if ( display )
            this.type = AlgorithmType.DISPLAY;
        else
            this.type = AlgorithmType.INTERNAL;
    }

    public override void Run()
    {
        graphCopy = new Graph(this.Graph);

        this.girth = int.MaxValue;
        this.cycleEdges = new List<Edge>();

        foreach (Vertex u in this.graphCopy.Vertices)
        {
            foreach (Vertex v in this.graphCopy.Vertices)
            {
                if (this.graphCopy.IsAdjacent(u, v))
                {
                    Edge e = this.graphCopy[u, v];
                    this.graphCopy.Remove(e, false);

                    // run dijkstra's
                    List<Edge> edgePath = DijkstrasWithoutWeights(u,v);
                    if (edgePath.Count > 0 && edgePath.Count + 1 < girth)
                    {
                        girth = edgePath.Count + 1;
                        this.cycleEdges = edgePath;
                        this.cycleEdges.Add(e);
                    }

                    this.graphCopy.Add(e, false);
                }
            }
        }

        this.cycleVertices = new List<Vertex> (Edge.GetIncidentVertices( this.cycleEdges ));
    }

    private List< Edge > DijkstrasWithoutWeights(Vertex src, Vertex dest)
    {
        HashSet< Vertex > notVisited = new HashSet< Vertex >( this.graphCopy.Vertices );
        Dictionary< Vertex, float > dist = new Dictionary< Vertex, float >();
        Dictionary< Vertex, Vertex > prev = new Dictionary< Vertex, Vertex >();

        foreach ( Vertex v in this.graphCopy.Vertices )
        {
            prev[ v ] = null;
            dist[ v ] = float.PositiveInfinity;
        }
        dist[ src ] = 0;

        while ( notVisited.Count > 0 )
        {
            // find u in notVisited such that dist[u] is minimal
            Vertex u = notVisited.First();
            foreach ( Vertex v in notVisited )
            {
                if ( dist[ v ] < dist[ u ] )
                    u = v;
            }

            notVisited.Remove( u );

            // update neighbors of u
            foreach ( Vertex v in notVisited )
            {
                if ( this.graphCopy.IsAdjacent( u, v ) )
                {
                    float tmp = dist[ u ] + 1;
                    if ( tmp < dist[ v ] )
                    {
                        dist[ v ] = tmp;
                        prev[ v ] = u;
                    }
                }
            }
        }

        List<Edge> edgePath = new List<Edge>();

        // put together final path 
        Vertex curr = dest;
        while ( curr != src )
        {
            if (prev[curr] is null)
            {
                return new List<Edge>();
            }

            edgePath.Add( this.graphCopy[ prev[ curr ], curr ] );
            curr = prev[ curr ];
        }

        edgePath.Reverse();

        return edgePath;
    }

    public override AlgorithmResult GetResult()
    {
        if ( this.error )
            return this.GetErrorResult();
        if ( this.running )
            return this.GetRunningResult();
        AlgorithmResult result = new AlgorithmResult( AlgorithmResultType.SUCCESS );
        if (this.girth < int.MaxValue)
        {
            result.results[ "girth" ] = ( this.girth, typeof ( int ) );
            result.results[ "smallest cycle vertices" ] = ( this.cycleVertices, typeof ( List< Vertex > ) );
            result.results[ "smallest cycle edges" ] = ( this.cycleEdges, typeof ( List< Edge > ) );
        } else
        {
            result.results[ "girth" ] = ( float.PositiveInfinity, typeof ( float ) );
        }

        return result;
    }

    public static int GetHash() => typeof ( FleurysAlgorithm ).GetHashCode();

    public override int GetHashCode() => FleurysAlgorithm.GetHash();
}
