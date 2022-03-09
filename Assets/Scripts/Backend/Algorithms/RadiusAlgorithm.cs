
using System;
using System.Threading;

[System.Serializable]
public class RadiusAlgorithm : Algorithm
{
    public float radius { get; private set; }

    public RadiusAlgorithm(
        Graph graph,
        CancellationToken token,
        Action updateUI,
        Action updateCalc,
        Action< Algorithm > markRunning,
        Action< Algorithm > markComplete,
        Action< Algorithm > unmarkRunning )
            : base(
                graph,
                token,
                updateUI,
                updateCalc,
                markRunning,
                markComplete,
                unmarkRunning ) { }
    

    public override void Run()
    {
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