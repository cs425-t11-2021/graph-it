using System;

[System.Serializable]
public class DiameterAlgorithm : Algorithm
{
    public float diameter { get; private set; }

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

        this.diameter = 0;
        foreach ( Vertex u in this.Graph.Vertices )
        {
            foreach ( Vertex v in this.Graph.Vertices )
            {
                dijkstra.Run(this.Graph, u, v);
                if (dijkstra.cost > this.diameter)
                {
                    this.diameter = dijkstra.cost;
                }
            }
        }
    }

    public static int GetHash() => typeof ( DiameterAlgorithm ).GetHashCode();

    public override int GetHashCode() => DiameterAlgorithm.GetHash();

}