using System;

[System.Serializable]
public class RadiusAlgorithm : Algorithm
{
    public float? Radius { get; private set; }

    public RadiusAlgorithm( AlgorithmManager algoManager ) : base( algoManager, algoManager.radiusUI, algoManager.radiusCalc ) { }

    public override void Run()
    {
        if (this.Graph.Directed)
        {
            this.Radius = null;
            return;
        }

        if (this.Graph.Order == 0)
        {
            this.Radius = 0;
            return;
        }

        float radius = float.PositiveInfinity;
        foreach ( Vertex u in this.Graph.Vertices )
        {
            float max_dist = 0;
            foreach ( Vertex v in this.Graph.Vertices )
            {
                this.AlgoManager.RunDijkstras( u, v );
                this.WaitUntilDijkstrasComplete( u, v );
                float cost = ( float ) this.AlgoManager.GetDijkstrasCost( u, v );
                if (cost > max_dist)
                {
                    max_dist = cost;
                }
            }

            if (max_dist < radius)
            {
                radius = max_dist;
            }
        }
        this.Radius = radius;
    }

    private void WaitUntilDijkstrasComplete( Vertex src, Vertex dest )
    {
        this.WaitUntilAlgorithmComplete( DijkstrasAlgorithm.GetHash( src, dest ) );
    }

    public static int GetHash() => typeof ( RadiusAlgorithm ).GetHashCode();

    public override int GetHashCode() => RadiusAlgorithm.GetHash();

}