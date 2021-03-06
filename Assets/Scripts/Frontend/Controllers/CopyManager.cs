using System.Collections.Generic;

// Class responsible for implementing the feature of copy and paste in the front end. Links up with corresponding GraphModification
// class in the backend.
public class CopyManager : SingletonBehavior<CopyManager>
{
    // Structure to store the current clipboard
    private (uint, List<Vertex>, List<Edge>)? clipboard;

    private void Awake()
    {
        clipboard = null;
    }
    
    // Save the current selection, including the graph instance id, into the clipboard
    public void CopyCurrentSelection()
    {
        if (SelectionManager.Singleton.SelectedVertices.Count == 0) return;
        
        Dictionary<Vertex, Vertex> vertexCorrespondanceDict = new Dictionary<Vertex, Vertex>();
        List<Vertex> vertices = new List<Vertex>();
        foreach (VertexObj vertexObj in SelectionManager.Singleton.SelectedVertices)
        {
            Vertex newVertex = new Vertex(vertexObj.Vertex);
            vertices.Add(newVertex);
            vertexCorrespondanceDict[vertexObj.Vertex] = newVertex;
        }

        List<Edge> edges = new List<Edge>();
        foreach (EdgeObj edgeObj in SelectionManager.Singleton.SelectedEdges)
        {
            if (SelectionManager.Singleton.SelectedVertices.Contains(edgeObj.Vertex1) &&
                SelectionManager.Singleton.SelectedVertices.Contains(edgeObj.Vertex2))
            {
                Edge newEdge = new Edge(edgeObj.Edge);
                newEdge.vert1 = vertexCorrespondanceDict[newEdge.vert1];
                newEdge.vert2 = vertexCorrespondanceDict[newEdge.vert2];
                edges.Add(newEdge);
            }
        }
        
        this.clipboard = (Controller.Singleton.ActiveGraphInstance.id, vertices, edges);
        
        NotificationManager.Singleton.CreateNotification("Selection copied.", 3);
    }
    
    // Create all objects currently stored in the clipboard in scene
    public void Paste()
    {
        if (this.clipboard == null) return;
        
        Dictionary<Vertex, Vertex> vertexCorrespondanceDict = new Dictionary<Vertex, Vertex>();
        List<Vertex> newVertices = new List<Vertex>();
        List<Edge> newEdges = new List<Edge>();
        foreach (Vertex v in this.clipboard.Value.Item2)
        {
            Vertex newVertex = new Vertex(v);
            vertexCorrespondanceDict[v] = newVertex;
            newVertices.Add(newVertex);
        }

        foreach (Edge e in this.clipboard.Value.Item3)
        {
            Edge newEdge = new Edge(e);
            newEdge.vert1 = vertexCorrespondanceDict[newEdge.vert1];
            newEdge.vert2 = vertexCorrespondanceDict[newEdge.vert2];
            newEdges.Add(newEdge);
        }

        Controller.Singleton.Graph.Add(newVertices, newEdges, true);
        newVertices.ForEach(v => Controller.Singleton.CreateVertexObj(v, false));
        newEdges.ForEach(e => Controller.Singleton.CreateEdgeObj(e, false));
        
        Controller.Singleton.ForceInvokeModificationEvent();
        GraphInfo.Singleton.UpdateGraphInfo();
    }
}
