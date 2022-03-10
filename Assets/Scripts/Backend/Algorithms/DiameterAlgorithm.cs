using System;

[System.Serializable]
public class DiameterAlgorithm : Algorithm
{
    public float Diameter { get; private set; }

    public DiameterAlgorithm(
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
        DijkstrasAlgorithm dijkstra = new DijkstrasAlgorithm();

        float diameter = 0;
        foreach ( Vertex u in this.Graph.Vertices )
        {
            foreach ( Vertex v in this.Graph.Vertices )
            {
                dijkstra.Run(this.Graph, u, v);
                if (dijkstra.cost > diameter)
                {
                    diameter = dijkstra.cost;
                }
            }
        }

        this.Diameter = diameter;
    }

    public static int GetHash() => typeof ( DiameterAlgorithm ).GetHashCode();

    public override int GetHashCode() => DiameterAlgorithm.GetHash();

}