
using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class DijkstrasAlgorithm : Algorithm
{
    public float Cost { get; private set; }
    public List< Edge > Path { get; private set; }
    private Vertex src;
    private Vertex dest;

    public DijkstrasAlgorithm( AlgorithmManager algoManager, bool display, Vertex src, Vertex dest ) : base( algoManager )
    {
        this.src = src;
        this.dest = dest;
        
        // Assign the type of the algorithm
        if ( display )
            this.type = AlgorithmType.DISPLAY;
        else
            this.type = AlgorithmType.INTERNAL;
        // Add the root vertex to vertex parms array
        vertexParms = new Vertex[] {this.src, this.dest};
    }

    public override void Run()
    {
        List< Vertex > vertexPath = new List< Vertex >();
        List< Edge > edgePath = new List< Edge >();
        HashSet< Vertex > notVisited = new HashSet< Vertex >( this.Graph.Vertices );
        Dictionary< Vertex, float > dist = new Dictionary< Vertex, float >();
        Dictionary< Vertex, Vertex > prev = new Dictionary< Vertex, Vertex >();

        foreach ( Vertex v in this.Graph.Vertices )
        {
            prev[ v ] = null;
            dist[ v ] = float.PositiveInfinity;
        }

        dist[ this.src ] = 0;

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
                if ( this.Graph.IsAdjacent( u, v ) )
                {
                    float tmp = dist[ u ] + this.Graph[ u, v ].Weight;
                    if ( tmp < dist[ v ] )
                    {
                        dist[ v ] = tmp;
                        prev[ v ] = u;
                    }
                }
            }
        }

        this.Cost = dist[ this.dest ];

        // put together final path 
        Vertex curr = this.dest;
        while ( curr != this.src )
        {
            vertexPath.Add( curr );
            curr = prev[ curr ];
            if ( curr is null )
            {
                vertexPath = new List<Vertex>();
                return;
            }
        }
        vertexPath.Add( this.src );
        vertexPath.Reverse();

        for (int i = 0; i < vertexPath.Count - 1; i++) {
            HashSet<Edge> incidentEdges = Controller.Singleton.Graph.GetIncidentEdges( vertexPath[ i ] );
            foreach (Edge edge in incidentEdges) {
                if (edge.vert1 == vertexPath[i + 1] || edge.vert2 == vertexPath[i + 1]) {
                    edgePath.Add(edge);
                }
            }
        }

        this.Path = edgePath;
    }

    public static int GetHash( Vertex src, Vertex dest ) => ( typeof ( DijkstrasAlgorithm ), src, dest ).GetHashCode();

    public override int GetHashCode() => DijkstrasAlgorithm.GetHash( this.src, this.dest );
}