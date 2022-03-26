
// All code developed by Team 11

using System;

[System.Serializable]
public class DiameterAlgorithm : Algorithm
{
    public float? Diameter { get; private set; }

    public DiameterAlgorithm( AlgorithmManager algoManager, bool display ) : base( algoManager ) { }

    public override void Run()
    {
        if (this.Graph.Directed)
        {
            this.Diameter = null;
            return;
        }

        float diameter = 0;
        foreach ( Vertex u in this.Graph.Vertices )
        {
            foreach ( Vertex v in this.Graph.Vertices )
            {
                this.AlgoManager.RunDijkstras( u, v, false );
                this.WaitUntilDijkstrasComplete( u, v );
                float cost = ( float ) this.AlgoManager.GetDijkstrasCost( u, v );
                if (cost > diameter)
                    diameter = cost;
            }
        }
        this.Diameter = diameter;
    }

    private void WaitUntilDijkstrasComplete( Vertex src, Vertex dest )
    {
        this.WaitUntilAlgorithmComplete( DijkstrasAlgorithm.GetHash( src, dest ) );
    }

    public static int GetHash() => typeof ( DiameterAlgorithm ).GetHashCode();

    public override int GetHashCode() => DiameterAlgorithm.GetHash();

}