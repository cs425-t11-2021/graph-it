
using System;
using System.Collections.Generic;
using UnityEngine;

public enum Modification
{
    ADD_VERTEX,         // modified is added vertex
    VERTEX_LABEL,       // modified is tuple consisting of vertex, oldLabel, newLabel
    VERTEX_POS,         // modified is tuple consisting of vertex, x0, y0, x1, y1
    VERTEX_STYLE,       // modified is tuple consisting of vertex, oldStyle, newStyle
    VERTEX_SIZE,        // modified is tuple consisting of vertex, oldSize, newSize
    VERTEX_COLOR,       // modified is tuple consisting of vertex, oldColor, newColor

    ADD_EDGE,           // modified is added edge
    EDGE_DIRECTED,      // modified is a pair of vertex, newDirected. Guaranteed be called when directed value is different
    EDGE_LABEL,         // modified is tuple consisting of edge, oldLabel, newLabel
    EDGE_STYLE,         // modified is tuple consisting of edge, oldStyle, newStyle
    EDGE_COLOR,         // modified is tuple consisting of edge, oldColor, newColor
    EDGE_THICKNESS,     // modified is tuple consisting of edge, oldThickness, newThickness
    EDGE_CURVATURE,     // modified is tuple consisting of edge, oldCurvature, newCurvature
    EDGE_TAIL_STYLE,    // modified is tuple consisting of edge, oldTail, newTail
    EDGE_HEAD_STYLE,    // modified is tuple consisting of edge, oldHead, newHead
    EDGE_REVERSE,       // modified is reversed edge

    REMOVE_COLLECTION   // modified is tuple consisting of hash set of vertices, hash set of edges
}

public class GraphModification
{
    private Graph graph;
    public Modification Mod { get; private set; }
    public object Modified { get; private set; }

    public GraphModification( Graph graph, Modification mod, System.Object modified )
    {
        this.graph = graph;
        this.Mod = mod;
        this.Modified = modified;
        this.graph.Changes.Push( this );
        this.graph.UndoneChanges.Clear();
        // Graph.PrintStack( this.graph.Changes );
        // Debug.Log( "" );
    }

    public void Undo()
    {
        switch ( this.Mod )
        {
            case Modification.ADD_VERTEX:
                this.UndoAddVertex();
                break;
            case Modification.VERTEX_LABEL:
                this.UndoVertexLabel();
                break;
            case Modification.VERTEX_POS:
                this.UndoVertexPos();
                break;
            case Modification.VERTEX_STYLE:
                this.UndoVertexStyle();
                break;
            case Modification.VERTEX_SIZE:
                this.UndoVertexSize();
                break;
            case Modification.VERTEX_COLOR:
                this.UndoVertexColor();
                break;
            case Modification.ADD_EDGE:
                this.UndoAddEdge();
                break;
            case Modification.EDGE_DIRECTED:
                this.UndoEdgeDirected();
                break;
            case Modification.EDGE_LABEL:
                this.UndoEdgeLabel();
                break;
            case Modification.EDGE_STYLE:
                this.UndoEdgeStyle();
                break;
            case Modification.EDGE_COLOR:
                this.UndoEdgeColor();
                break;
            case Modification.EDGE_THICKNESS:
                this.UndoEdgeThickness();
                break;
            case Modification.EDGE_CURVATURE:
                this.UndoEdgeCurvature();
                break;
            case Modification.EDGE_TAIL_STYLE:
                this.UndoEdgeTailStyle();
                break;
            case Modification.EDGE_HEAD_STYLE:
                this.UndoEdgeHeadStyle();
                break;
            case Modification.EDGE_REVERSE:
                this.UndoEdgeReverse();
                break;
            case Modification.REMOVE_COLLECTION:
                this.UndoRemoveCollection();
                break;
        }
    }

    private void UndoAddVertex()
    {
        this.graph.Remove( ( Vertex ) this.Modified, false );
        
        // Update front end
        Controller.Singleton.RemoveVertex(Controller.Singleton.GetVertexObj( ( Vertex ) this.Modified), false );
    }

    private void UndoVertexLabel()
    {
        ( Vertex, string, string ) labelData = ( ( Vertex, string, string ) ) this.Modified;
        labelData.Item1.SetLabel( labelData.Item2, false );
        
        // Update front end
        Controller.Singleton.GetVertexObj( labelData.Item1 ).labelObj.UpdateVertexLabel(labelData.Item2, false);
    }

    private void UndoVertexPos()
    {
        ( Vertex, System.Numerics.Vector2, System.Numerics.Vector2 ) posData = ( ( Vertex, System.Numerics.Vector2, System.Numerics.Vector2 ) ) this.Modified;
        posData.Item1.SetPos( posData.Item2, false );
        
        // Update front end
        Controller.Singleton.GetVertexObj( posData.Item1 ).transform.position = new Vector3(posData.Item2.X, posData.Item2.Y, 0);
    }

    private void UndoVertexStyle()
    {
        ( Vertex, uint, uint ) styleData = ( ( Vertex, uint, uint ) ) this.Modified;
        styleData.Item1.SetStyle( styleData.Item2, false );
        
        // Update front end
        Controller.Singleton.GetVertexObj( styleData.Item1 ).SetStyle(styleData.Item2, false);
    }

    private void UndoVertexSize()
    {
        ( Vertex, uint, uint ) sizeData = ( ( Vertex, uint, uint ) ) this.Modified;
        sizeData.Item1.SetSize( sizeData.Item2, false );
        
        // Update front end
        // TODO: FRONT END NOT IMPLEMENTED
    }

    private void UndoVertexColor()
    {
        ( Vertex, uint, uint ) colorData = ( ( Vertex, uint, uint ) ) this.Modified;
        colorData.Item1.SetColor( colorData.Item2, false );
        
        // Update front end
        // TODO: FRONT END NOT IMPLEMENTED
    }

    private void UndoAddEdge()
    {
        this.graph.Remove( ( Edge ) this.Modified, false );
        
        // Update front end
        Controller.Singleton.RemoveEdge(Controller.Singleton.GetEdgeObj( ( Edge ) this.Modified), false );
    }

    private void UndoEdgeDirected()
    {
        ( Edge, bool ) directedData = ( ( Edge, bool ) ) this.Modified;
        directedData.Item1.SetDirected( directedData.Item2, false );
        
        // Update front end
        Controller.Singleton.GetEdgeObj( directedData.Item1 ).SetDirectedness(directedData.Item2, false);
    }

    private void UndoEdgeLabel()
    {
        ( Edge, string, string ) labelData = ( ( Edge, string, string ) ) this.Modified;
        labelData.Item1.SetLabel( labelData.Item2, false );
        
        // Update front end
        Controller.Singleton.GetEdgeObj( labelData.Item1 ).labelObj.UpdateEdgeLabel(labelData.Item2, false);
    }

    private void UndoEdgeStyle()
    {
        ( Edge, uint, uint ) styleData = ( ( Edge, uint, uint ) ) this.Modified;
        styleData.Item1.SetStyle( styleData.Item2, false );
    }

    private void UndoEdgeColor()
    {
        ( Edge, uint, uint ) colorData = ( ( Edge, uint, uint ) ) this.Modified;
        colorData.Item1.SetColor( colorData.Item2, false );
    }

    private void UndoEdgeThickness()
    {
        ( Edge, uint, uint ) thicknessData = ( ( Edge, uint, uint ) ) this.Modified;
        thicknessData.Item1.SetThickness( thicknessData.Item2, false );

        // Update front end
        Controller.Singleton.GetEdgeObj( thicknessData.Item1 ).SetThickness(thicknessData.Item2, false);
    }

    private void UndoEdgeCurvature()
    {
        ( Edge, int, int ) curveData = ( ( Edge, int, int ) ) this.Modified;
        curveData.Item1.SetCurvature( curveData.Item2, false );

        // Update front end
        Controller.Singleton.GetEdgeObj( curveData.Item1 ).SetCurvature(curveData.Item2, false);
    }

    private void UndoEdgeTailStyle()
    {
        ( Edge, uint, uint ) tailStyleData = ( ( Edge, uint, uint ) ) this.Modified;
        tailStyleData.Item1.SetTailStyle( tailStyleData.Item2, false );
    }

    private void UndoEdgeHeadStyle()
    {
        ( Edge, uint, uint ) headStyleData = ( ( Edge, uint, uint ) ) this.Modified;
        headStyleData.Item1.SetHeadStyle( headStyleData.Item2, false );
    }

    private void UndoEdgeReverse()
    {
        ( ( Edge ) this.Modified ).Reverse( false );

        // Update front end
        Controller.Singleton.GetEdgeObj( ( Edge ) this.Modified ).ReverseEdge(false);
    }

    private void UndoRemoveCollection()
    {
        ( HashSet< Vertex >, HashSet< Edge > ) collection = ( ( HashSet< Vertex >, HashSet< Edge > ) ) this.Modified;
        if ( !( collection.Item1 is null ) )
        {
            foreach ( Vertex vert in collection.Item1 ) {
                this.graph.AddVertex( vert, false );

                // Update front end
                Controller.Singleton.CreateVertexObj( vert );
            }
        }
        if ( !( collection.Item2 is null ) )
        {
            foreach ( Edge edge in collection.Item2 ) {
                this.graph.AddEdge( edge, false );

                // Update front end
                Controller.Singleton.CreateEdgeObj( edge );
            }
        }
    }

    public void Redo()
    {
        switch ( this.Mod )
        {
            case Modification.ADD_VERTEX:
                this.RedoAddVertex();
                break;
            case Modification.VERTEX_LABEL:
                this.RedoVertexLabel();
                break;
            case Modification.VERTEX_POS:
                this.RedoVertexPos();
                break;
            case Modification.VERTEX_STYLE:
                this.RedoVertexStyle();
                break;
            case Modification.VERTEX_SIZE:
                this.RedoVertexSize();
                break;
            case Modification.VERTEX_COLOR:
                this.RedoVertexColor();
                break;
            case Modification.ADD_EDGE:
                this.RedoAddEdge();
                break;
            case Modification.EDGE_DIRECTED:
                this.RedoEdgeDirected();
                break;
            case Modification.EDGE_LABEL:
                this.RedoEdgeLabel();
                break;
            case Modification.EDGE_STYLE:
                this.RedoEdgeStyle();
                break;
            case Modification.EDGE_COLOR:
                this.RedoEdgeColor();
                break;
            case Modification.EDGE_THICKNESS:
                this.RedoEdgeThickness();
                break;
            case Modification.EDGE_CURVATURE:
                this.RedoEdgeCurvature();
                break;
            case Modification.EDGE_TAIL_STYLE:
                this.RedoEdgeTailStyle();
                break;
            case Modification.EDGE_HEAD_STYLE:
                this.RedoEdgeHeadStyle();
                break;
            case Modification.EDGE_REVERSE:
                this.RedoEdgeReverse();
                break;
            case Modification.REMOVE_COLLECTION:
                this.RedoRemoveCollection();
                break;
        }
    }

    private void RedoAddVertex()
    {
        this.graph.AddVertex( ( Vertex ) this.Modified, false );
        
        // Update front end
        Controller.Singleton.CreateVertexObj((Vertex) this.Modified);
    }

    private void RedoVertexLabel()
    {
        ( Vertex, string, string ) labelData = ( ( Vertex, string, string ) ) this.Modified;
        labelData.Item1.SetLabel( labelData.Item3, false );
        
    }

    private void RedoVertexPos()
    {
        ( Vertex, System.Numerics.Vector2, System.Numerics.Vector2 ) posData = ( ( Vertex, System.Numerics.Vector2, System.Numerics.Vector2 ) ) this.Modified;
        posData.Item1.SetPos( posData.Item3, false );

        // Update front end
        Controller.Singleton.GetVertexObj( posData.Item1 ).transform.position =
            new Vector3(posData.Item3.X, posData.Item3.Y, 0);
    }

    private void RedoVertexStyle()
    {
        ( Vertex, uint, uint ) styleData = ( ( Vertex, uint, uint ) ) this.Modified;
        styleData.Item1.SetStyle( styleData.Item3, false );

        // Update front end
        Controller.Singleton.GetVertexObj( styleData.Item1 ).SetStyle(styleData.Item3, false);
    }

    private void RedoVertexSize()
    {
        ( Vertex, uint, uint ) sizeData = ( ( Vertex, uint, uint ) ) this.Modified;
        sizeData.Item1.SetSize( sizeData.Item3, false );
    }

    private void RedoVertexColor()
    {
        ( Vertex, uint, uint ) colorData = ( ( Vertex, uint, uint ) ) this.Modified;
        colorData.Item1.SetColor( colorData.Item3, false );
    }

    private void RedoAddEdge()
    {
        this.graph.AddEdge( ( Edge ) this.Modified, false );

        // Update front end
        Controller.Singleton.CreateEdgeObj((Edge) this.Modified);
    }

    private void RedoEdgeDirected()
    {
        ( Edge, bool ) directedData = ( ( Edge, bool ) ) this.Modified;
        directedData.Item1.SetDirected( !directedData.Item2, false );

        // Update front end
        Controller.Singleton.GetEdgeObj( directedData.Item1 ).SetDirectedness(directedData.Item2, false);
    }

    private void RedoEdgeLabel()
    {
        ( Edge, string, string ) labelData = ( ( Edge, string, string ) ) this.Modified;
        labelData.Item1.SetLabel( labelData.Item3, false );

        // Update front end
        Controller.Singleton.GetEdgeObj( labelData.Item1 ).labelObj.UpdateEdgeLabel(labelData.Item3, false);
    }

    private void RedoEdgeStyle()
    {
        ( Edge, uint, uint ) styleData = ( ( Edge, uint, uint ) ) this.Modified;
        styleData.Item1.SetStyle( styleData.Item3, false );
    }

    private void RedoEdgeColor()
    {
        ( Edge, uint, uint ) colorData = ( ( Edge, uint, uint ) ) this.Modified;
        colorData.Item1.SetColor( colorData.Item3, false );
    }

    private void RedoEdgeThickness()
    {
        ( Edge, uint, uint ) thicknessData = ( ( Edge, uint, uint ) ) this.Modified;
        thicknessData.Item1.SetThickness( thicknessData.Item3, false );

        // Update front end
        Controller.Singleton.GetEdgeObj( thicknessData.Item1 ).SetThickness(thicknessData.Item3, false);
    }

    private void RedoEdgeCurvature()
    {
        ( Edge, int, int ) curveData = ( ( Edge, int, int ) ) this.Modified;
        curveData.Item1.SetCurvature( curveData.Item3, false );

        // Update front end
        Controller.Singleton.GetEdgeObj( curveData.Item1 ).SetCurvature(curveData.Item3, false);
    }

    private void RedoEdgeTailStyle()
    {
        ( Edge, uint, uint ) tailStyleData = ( ( Edge, uint, uint ) ) this.Modified;
        tailStyleData.Item1.SetTailStyle( tailStyleData.Item3, false );
    }

    private void RedoEdgeHeadStyle()
    {
        ( Edge, uint, uint ) headStyleData = ( ( Edge, uint, uint ) ) this.Modified;
        headStyleData.Item1.SetHeadStyle( headStyleData.Item3, false );
    }

    private void RedoEdgeReverse()
    {
        this.UndoEdgeReverse();
    }

    private void RedoRemoveCollection()
    {
        ( HashSet< Vertex >, HashSet< Edge > ) collection = ( ( HashSet< Vertex >, HashSet< Edge > ) ) this.Modified;
        
        if (!(collection.Item2 is null))
        {
            this.graph.Remove(new List<Edge>(collection.Item2));
            
            // Update front end
            foreach (Edge e in collection.Item2)
            {
                Controller.Singleton.RemoveEdge(Controller.Singleton.GetEdgeObj( e ), false );
            }
        }
        
        if (!(collection.Item1 is null))
        {
            this.graph.Remove(new List<Vertex>(collection.Item1));
            
            // Update front end
            foreach (Vertex v in collection.Item1)
            {
                Controller.Singleton.RemoveVertex(Controller.Singleton.GetVertexObj( v ), false );
            }
        }
    }
}
