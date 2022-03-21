
// All code developed by Team 11

using System;

public static class PresetGraph
{
    public static Graph Complete( int n )
    {
        Graph graph = new Graph();
        for ( int i = 0; i < n; ++i )
        {
            double theta = 2 * Math.PI * i / n;
            graph.AddVertex( ( float ) ( 5 * Math.Cos( theta ) ), ( float ) ( 5 * Math.Sin( theta ) ) );
        }

        foreach ( Vertex vert1 in graph.Vertices )
        {
            foreach ( Vertex vert2 in graph.Vertices )
            {
                if ( vert1 != vert2 )
                    graph.AddEdge( vert1, vert2 );
            }
        }
        return graph;
    }

    public static Graph CompleteBipartite( int n )
    {
        Graph graph = new Graph();
        for ( int i = 0; i < 2 * n; ++i )
            graph.AddVertex( i < n ? -2 : 2, 2 * ( i % n ) - n + 1 );

        for ( int i = 0; i < n; ++i )
        {
            for ( int j = n; j < 2 * n; ++j )
                graph.AddEdge( graph.Vertices[ i ], graph.Vertices[ j ] );
        }
        return graph;
    }

    public static Graph Cycle( int n )
    {
        Graph graph = new Graph();
        for ( int i = 0; i < n; ++i )
        {
            double theta = 2 * Math.PI * i / n;
            graph.AddVertex( ( float ) ( 5 * Math.Cos( theta ) ), ( float ) ( 5 * Math.Sin( theta ) ) );
        }

        for ( int i = 0; i < n; ++i )
            graph.AddEdge( graph.Vertices[ i ], graph.Vertices[ ( i + 1 ) % n ] );
        return graph;
    }

    public static Graph Path( int n )
    {
        Graph graph = new Graph();
        for ( int i = 0; i < n; ++i )
            graph.AddVertex( 2 * i - n + 1, 0 );

        for ( int i = 1; i < n; ++i )
            graph.AddEdge( graph.Vertices[ i - 1 ], graph.Vertices[ i ] );
        return graph;
    }

    public static Graph Star( int n )
    {
        Graph graph = new Graph();
        Vertex origin = graph.AddVertex( 0, 0 );
        for ( int i = 0; i < n; ++i )
        {
            double theta = 2 * Math.PI * i / n;
            Vertex vert = graph.AddVertex( ( float ) ( 5 * Math.Cos( theta ) ), ( float ) ( 5 * Math.Sin( theta ) ) );
            graph.AddEdge( origin, vert );
        }
        return graph;
    }

    // TODO: hypercube
}
