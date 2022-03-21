using System;

[System.Serializable]
public class RadiusAlgorithm : Algorithm
{
    public float radius { get; private set; }

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

        DijkstrasAlgorithm dijkstra = new DijkstrasAlgorithm();

        this.radius = float.PositiveInfinity;
        foreach ( Vertex u in this.Graph.Vertices )
        {
            float max_dist = 0;
            foreach ( Vertex v in this.Graph.Vertices )
            {
                dijkstra.Run(this.Graph, u, v);
                if (dijkstra.cost > max_dist)
                {
                    max_dist = dijkstra.cost;
                }
            }

            if (max_dist < this.radius)
            {
                this.radius = max_dist;
            }
        }
    }

    public static int GetHash() => typeof ( RadiusAlgorithm ).GetHashCode();

    public override int GetHashCode() => RadiusAlgorithm.GetHash();

}