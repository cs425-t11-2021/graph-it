using System;

[System.Serializable]
public class RadiusAlgorithm : Algorithm
{
    public float Radius { get; private set; }

    public RadiusAlgorithm(
        Graph graph,
        Action updateUI,
        Action updateCalc,
        Action< Algorithm > markRunning,
        Action< Algorithm > markComplete,
        Action< Algorithm > unmarkRunning )
            : base(
                graph,
                updateUI,
                updateCalc,
                markRunning,
                markComplete,
                unmarkRunning ) { }
    

    public override void Run()
    {
        if (this.Graph.Vertices.Count == 0)
        {
            this.Radius = 0;
            return;
        }

        DijkstrasAlgorithm dijkstra = new DijkstrasAlgorithm();

        float radius = float.PositiveInfinity;
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

            if (max_dist < radius)
            {
                radius = max_dist;
            }
        }

        this.Radius = radius;
    }

    public static int GetHash() => typeof ( RadiusAlgorithm ).GetHashCode();

    public override int GetHashCode() => RadiusAlgorithm.GetHash();

}