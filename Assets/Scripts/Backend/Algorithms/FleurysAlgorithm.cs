using System;

[System.Serializable]
public class FleurysAlgorithm : Algorithm
{
    public bool eulerianCircuitExists { get; private set; }

    public FleurysAlgorithm(
        Graph graph,
        Action updateUI,
        Action updateCalc,
        Action< Algorithm > markRunning, 
        Action< Algorithm > markComplete,
        Action< Algorithm > unmarkRunning)
        : base(
            graph,
            updateUI,
            updateCalc,
            markRunning,
            markComplete,
            unmarkRunning)
    {}

    public override void Run()
    {
        // check first if graph is an Euler graph:
        //      - there are either 0 or 2 vertices with odd degree
        //      - graph is connected

        int numOddDegrees = 0;
        foreach ( Vertex u in this.graph.Vertices )
        {
            if (this.graph.GetVertexDegree(u) % 2 == 1) 
            {
                numOddDegrees++;
            }
        }

        if (numOddDegrees != 0 && numOddDegrees != 2)
        {
            eulerianCircuitExists = false;
            return;
        }

        // TODO: run Depth First search to determine if graph is connected

        // TODO:
        // start from an odd degree vertex (if it exists, otherwise any vertex)
        // traverse through the graph until a circuit is formed, by always choosing a "non-bridge" over a "bridge"


    }
}
