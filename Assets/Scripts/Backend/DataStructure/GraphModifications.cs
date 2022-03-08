
using System;
using System.Collections.Generic;
using UnityEngine;

public enum Modification
{
    ADD_VERTEX,         // modified is added vertex
    REMOVE_VERTEX,      // modified is removed vertex
    VERTEX_LABEL,       // modified is tuple consisting of vertex, oldLabel, newLabel
    VERTEX_POS,         // modified is tuple consisting of vertex, x0, y0, x1, y1
    VERTEX_STYLE,       // modified is tuple consisting of vertex, oldStyle, newStyle
    VERTEX_SIZE,        // modified is tuple consisting of vertex, oldSize, newSize
    VERTEX_COLOR,       // modified is tuple consisting of vertex, oldColor, newColor

    ADD_EDGE,           // modified is added edge
    REMOVE_EDGE,        // modified is removed edge
    EDGE_DIRECTED,      // modified is a pair of vertex, directed bool. Guaranteed be called when directed value is different
    EDGE_LABEL,         // modified is tuple consisting of edge, oldLabel, newLabel
    EDGE_STYLE,         // modified is tuple consisting of edge, oldStyle, newStyle
    EDGE_COLOR,         // modified is tuple consisting of edge, oldColor, newColor
    EDGE_THICKNESS,     // modified is tuple consisting of edge, oldThickness, newThickness
    EDGE_CURVATURE,     // modified is tuple consisting of edge, oldCurvature, newCurvature
    EDGE_TAIL_STYLE,    // modified is tuple consisting of edge, oldTail, newTail
    EDGE_HEAD_STYLE,    // modified is tuple consisting of edge, oldHead, newHead
    EDGE_REVERSE,       // modified is reversed edge

    REMOVE_COLLECTION,  // modified is collection TODO: when user deletes selection, is remove vertex and edge needed?

    CLEAR_GRAPH,        // modified is a pair of vertices, adjacencies previous to clear
    IMPORT_GRAPH        // modified is a pair of vertices, adjacencies previous to import
}

public class GraphModification
{
    private Graph graph;
    public Modification Mod { get; private set; }
    public System.Object Modified { get; private set; }

    // TODO: remove graph, use delegates to push and pop from graph.Changes
    public GraphModification( Graph graph, Modification mod, System.Object modified )
    {
        this.graph = graph;
        this.Mod = mod;
        this.Modified = modified;
        // if ( mod == Modification.VERTEX_POS && this.graph.Changes.TryPeek( out GraphModification graphMod ) && graphMod.Mod == Modification.VERTEX_POS )
        // {
            // this is all messed up, if the user makes multiple movements, they will be absorbed into one which is not good
            // Tuple< Vertex, double?, double?, double?, double? > posData = ( Tuple< Vertex, double?, double?, double?, double? > ) this.graph.Changes.Pop().Modified;
            // this.graph.Changes.Push( new GraphModification( this.graph, Modification.VERTEX_POS, ( posData.Item1,  ) ) );
        // }
        this.graph.Changes.Push( this );
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
            case Modification.REMOVE_VERTEX:
                this.UndoRemoveVertex();
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
            case Modification.REMOVE_EDGE:
                this.UndoRemoveEdge();
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
            case Modification.CLEAR_GRAPH:
                this.UndoClearGraph();
                break;
            case Modification.IMPORT_GRAPH:
                this.UndoImportGraph();
                break;
        }
    }

    private void UndoAddVertex()
    {
        this.graph.RemoveVertex( ( Vertex ) this.Modified, false );
    }

    private void UndoRemoveVertex()
    {
        this.graph.AddVertex( ( Vertex ) this.Modified, false );
    }

    private void UndoVertexLabel()
    {
        Tuple< Vertex, string, string > labelData = ( Tuple< Vertex, string, string > ) this.Modified;
        labelData.Item1.SetLabel( labelData.Item2, false );
    }

    private void UndoVertexPos()
    {
        Tuple< Vertex, System.Numerics.Vector2, System.Numerics.Vector2 > posData = ( Tuple< Vertex, System.Numerics.Vector2, System.Numerics.Vector2 > ) this.Modified;
        posData.Item1.SetPos( posData.Item2, false );
    }

    private void UndoVertexStyle()
    {
        Tuple< Vertex, uint, uint > styleData = ( Tuple< Vertex, uint, uint > ) this.Modified;
        styleData.Item1.SetStyle( styleData.Item2, false );
    }

    private void UndoVertexSize()
    {

    }

    private void UndoVertexColor()
    {
        Tuple< Vertex, uint, uint > colorData = ( Tuple< Vertex, uint, uint > ) this.Modified;
        colorData.Item1.SetStyle( colorData.Item2, false );
    }

    private void UndoAddEdge()
    {
        this.graph.RemoveEdge( ( Edge ) this.Modified, false );
    }

    private void UndoRemoveEdge()
    {
        this.graph.AddEdge( ( Edge ) this.Modified, false );
    }

    private void UndoEdgeDirected()
    {
        Tuple< Edge, bool > directedData = ( Tuple< Edge, bool > ) this.Modified;
        directedData.Item1.SetDirected( directedData.Item2, false );
    }

    private void UndoEdgeLabel()
    {
        Tuple< Edge, string, string > labelData = ( Tuple< Edge, string, string > ) this.Modified;
        labelData.Item1.SetLabel( labelData.Item2, false );
    }

    private void UndoEdgeStyle()
    {
        Tuple< Edge, uint, uint > styleData = ( Tuple< Edge, uint, uint > ) this.Modified;
        styleData.Item1.SetStyle( styleData.Item2, false );
    }

    private void UndoEdgeColor()
    {
        Tuple< Edge, uint, uint > colorData = ( Tuple< Edge, uint, uint > ) this.Modified;
        colorData.Item1.SetColor( colorData.Item2, false );
    }

    private void UndoEdgeThickness()
    {
        Tuple< Edge, uint, uint > thicknessData = ( Tuple< Edge, uint, uint > ) this.Modified;
        thicknessData.Item1.SetThickness( thicknessData.Item2, false );
    }

    private void UndoEdgeCurvature()
    {

    }

    private void UndoEdgeTailStyle()
    {
        Tuple< Edge, uint, uint > tailStyleData = ( Tuple< Edge, uint, uint > ) this.Modified;
        tailStyleData.Item1.SetTailStyle( tailStyleData.Item2, false );
    }

    private void UndoEdgeHeadStyle()
    {
        Tuple< Edge, uint, uint > headStyleData = ( Tuple< Edge, uint, uint > ) this.Modified;
        headStyleData.Item1.SetHeadStyle( headStyleData.Item2, false );
    }

    private void UndoEdgeReverse()
    {
        
    }

    private void UndoClearGraph()
    {

    }

    private void UndoImportGraph()
    {

    }

    public void Redo()
    {
        switch ( this.Mod )
        {
            case Modification.ADD_VERTEX:
                this.RedoAddVertex();
                break;
            case Modification.REMOVE_VERTEX:
                this.RedoRemoveVertex();
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
            case Modification.REMOVE_EDGE:
                this.RedoRemoveEdge();
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
            case Modification.CLEAR_GRAPH:
                this.RedoClearGraph();
                break;
            case Modification.IMPORT_GRAPH:
                this.RedoImportGraph();
                break;
        }
    }

    private void RedoAddVertex()
    {

    }

    private void RedoRemoveVertex()
    {
        
    }

    private void RedoVertexLabel()
    {

    }

    private void RedoVertexPos()
    {
        
    }

    private void RedoVertexStyle()
    {

    }

    private void RedoVertexSize()
    {

    }

    private void RedoVertexColor()
    {
        
    }

    private void RedoAddEdge()
    {

    }

    private void RedoRemoveEdge()
    {
        
    }

    private void RedoEdgeDirected()
    {

    }

    private void RedoEdgeLabel()
    {
        
    }

    private void RedoEdgeStyle()
    {

    }

    private void RedoEdgeColor()
    {
        
    }

    private void RedoEdgeThickness()
    {

    }

    private void RedoEdgeCurvature()
    {

    }

    private void RedoEdgeTailStyle()
    {
        
    }

    private void RedoEdgeHeadStyle()
    {

    }

    private void RedoEdgeReverse()
    {
        
    }

    private void RedoClearGraph()
    {

    }

    private void RedoImportGraph()
    {

    }
}
