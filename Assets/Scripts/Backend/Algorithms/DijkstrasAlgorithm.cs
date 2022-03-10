using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class DijkstrasAlgorithm
{
    public float cost { get; private set; }
    public List< Vertex > path { get; private set; }

    public void Run( Graph graph, Vertex src, Vertex dest )
    {
        HashSet< Vertex > notVisited = new HashSet< Vertex >( graph.Vertices );
        Dictionary< Vertex, float > dist = new Dictionary< Vertex, float >();
        Dictionary< Vertex, Vertex > prev = new Dictionary< Vertex, Vertex >();

        foreach ( Vertex v in graph.Vertices )
            dist[ v ] = float.PositiveInfinity;

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
                if ( graph.IsAdjacent( u, v ) )
                {
                    float tmp = dist[ u ] + graph[ u, v ].Weight;
                    if ( tmp < dist[ v ] )
                    {
                        dist[ v ] = tmp;
                        prev[ v ] = u;
                    }
                }
            }
        }

        this.cost = dist[ dest ];

        // put together final path 
        this.path = new List< Vertex >();
        Vertex curr = dest;
        while ( curr != src )
        {
            this.path.Add( curr );
            curr = prev[ curr ];
            if ( curr is null )
            {
                this.path = new List<Vertex>();
                return;
            }
        }
        this.path.Add( src );
        this.path.Reverse();
    }

}