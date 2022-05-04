
// All code developed by Team 11

using System;
using System.Collections.Generic;
using UnityEngine;

public enum Modification
{
    VERTEX_LABEL,       // modified is tuple consisting of vertex, oldLabel, newLabel
    VERTEX_POSES,       // modified is tuple consisting of list of vertices, list of oldPoses (Vector2), list of newPoses (Vector2)
    VERTEX_STYLES,      // modified is tuple consisting of list of vertices, list of oldStyle, list of newStyle
    VERTEX_SIZES,       // modified is tuple consisting of list of vertices, list of oldSizes, list of newSizes
    VERTEX_COLORS,      // modified is tuple consisting of list of vertices, list of oldColors, list of newColors

    EDGE_DIRECTED,    // modified is tuple consisting of edge, newDirected. Guaranteed be called when directed value is different ** TODO: support a collection of edges, this will require some modifications to how we change direction in front end
    EDGE_LABEL,         // modified is tuple consisting of edge, oldLabel, newLabel
    EDGE_STYLES,        // modified is tuple consisting of list of edges, list of oldStyles, list of newStyles **
    EDGE_COLORS,        // modified is tuple consisting of edge, oldColor, newColor **
    EDGE_THICKNESSES,   // modified is tuple consisting of list of edges, list of oldThicknesses, list of newThickness
    EDGE_CURVATURES,    // modified is tuple consisting of list of edges, list of oldCurvatures, list of newCurvatures
    EDGE_TAIL_STYLES,   // modified is tuple consisting of edge, oldTail, newTail **
    EDGE_HEAD_STYLES,   // modified is tuple consisting of edge, oldHead, newHead **
    EDGE_REVERSE,       // modified is reversed edge ** TODO: support a colleciton of edges

    ADD_COLLECTION,     // modified is tuple consisting of hash set of vertices, hash set of edges
    REMOVE_COLLECTION   // modified is tuple consisting of hash set of vertices, hash set of edges
}

public class GraphModification
{
    public static readonly Dictionary<Modification, string> aliases = new Dictionary<Modification, string> {
        { Modification.VERTEX_LABEL,      "Change Vertex Label"    },
        { Modification.VERTEX_POSES,      "Change Vertex Position" },
        { Modification.VERTEX_STYLES,     "Change Vertex Style"    },
        { Modification.VERTEX_SIZES,      "Change Vertex Size"     },
        { Modification.VERTEX_COLORS,     "Change Vertex Color"    },
        { Modification.EDGE_DIRECTED,     "Change Edge Directed"   },
        { Modification.EDGE_LABEL,        "Change Edge Label"      },
        { Modification.EDGE_STYLES,       "Change Edge Style"      },
        { Modification.EDGE_COLORS,       "Change Edge Color"      },
        { Modification.EDGE_THICKNESSES,  "Change Edge Thickness"  },
        { Modification.EDGE_CURVATURES,   "Change Edge Curvature"  },
        { Modification.EDGE_TAIL_STYLES,  "Change Edge Tail Style" },
        { Modification.EDGE_HEAD_STYLES,  "Change Edge Head Style" },
        { Modification.EDGE_REVERSE,      "Reverse Edge"           },
        { Modification.ADD_COLLECTION,    "Add Collection"         },
        { Modification.REMOVE_COLLECTION, "Remove Collection"      }
    };

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
    }

    public void Undo()
    {
        switch ( this.Mod )
        {
            case Modification.VERTEX_LABEL:
                this.UndoVertexLabel();
                break;
            case Modification.VERTEX_POSES:
                this.UndoVertexPoses();
                break;
            case Modification.VERTEX_STYLES:
                this.UndoVertexStyles();
                break;
            case Modification.VERTEX_SIZES:
                this.UndoVertexSizes();
                break;
            case Modification.VERTEX_COLORS:
                this.UndoVertexColors();
                break;
            case Modification.EDGE_DIRECTED:
                this.UndoEdgeDirected();
                break;
            case Modification.EDGE_LABEL:
                this.UndoEdgeLabel();
                break;
            case Modification.EDGE_STYLES:
                this.UndoEdgeStyles();
                break;
            case Modification.EDGE_COLORS:
                this.UndoEdgeColors();
                break;
            case Modification.EDGE_THICKNESSES:
                this.UndoEdgeThicknesses();
                break;
            case Modification.EDGE_CURVATURES:
                this.UndoEdgeCurvatures();
                break;
            case Modification.EDGE_TAIL_STYLES:
                this.UndoEdgeTailStyles();
                break;
            case Modification.EDGE_HEAD_STYLES:
                this.UndoEdgeHeadStyles();
                break;
            case Modification.EDGE_REVERSE:
                this.UndoEdgeReverse();
                break;
            case Modification.ADD_COLLECTION:
                this.UndoAddCollection();
                break;
            case Modification.REMOVE_COLLECTION:
                this.UndoRemoveCollection();
                break;
        }
        
        // Update Algorithm Manager
        GraphInfo.Singleton.UpdateGraphInfo();
    }

    private void UndoVertexLabel()
    {
        ( Vertex, string, string ) labelData = ( ( Vertex, string, string ) ) this.Modified;
        labelData.Item1.SetLabel( labelData.Item2, false );
        
        // Update front end
        Controller.Singleton.GetVertexObj( labelData.Item1 ).labelObj.UpdateVertexLabel(labelData.Item2, false);
    }

    private void UndoVertexPoses()
    {
        ( List< Vertex >, List< System.Numerics.Vector2 >, List< System.Numerics.Vector2 > ) posData = ( ( List< Vertex >, List< System.Numerics.Vector2 >, List< System.Numerics.Vector2 > ) ) this.Modified;
        for ( int i = 0; i < posData.Item1.Count; ++i )
        {
            posData.Item1[ i ].SetPos( posData.Item2[ i ], false );

            // TODO: Update front end
            Controller.Singleton.GetVertexObj( posData.Item1[i] ).MovePosition(new Vector3( posData.Item2[i].X, posData.Item2[i].Y, 0 ));
        }
    }

    private void UndoVertexStyles()
    {
        ( List< Vertex >, List< uint >, List< uint > ) styleData = ( ( List< Vertex >, List< uint >, List< uint > ) ) this.Modified;
        for ( int i = 0; i < styleData.Item1.Count; ++i )
        {
            styleData.Item1[ i ].SetStyle( styleData.Item2[ i ], false );

            // TODO: Update front end
            Controller.Singleton.GetVertexObj( styleData.Item1[ i ] ).SetStyle( styleData.Item2[ i ], false );
        }
    }

    private void UndoVertexSizes()
    {
        ( List< Vertex >, List< uint >, List< uint > ) sizeData = ( ( List< Vertex >, List< uint >, List< uint > ) ) this.Modified;
        for ( int i = 0; i < sizeData.Item1.Count; ++i )
        {
            sizeData.Item1[ i ].SetSize( sizeData.Item2[ i ], false );

            // Update front end
            // TODO: FRONT END NOT IMPLEMENTED
        }
        
    }

    private void UndoVertexColors()
    {
        ( List< Vertex >, List< uint >, List< uint > ) colorData = ( ( List< Vertex >, List< uint >, List< uint > ) ) this.Modified;
        for ( int i = 0; i < colorData.Item1.Count; ++i )
        {
            colorData.Item1[ i ].SetColor( colorData.Item2[ i ], false );

            // Update front end
            // TODO: FRONT END NOT IMPLEMENTED
        }
    }

    private void UndoEdgeDirected()
    {
        ( Edge, bool ) directedData = ( ( Edge, bool ) ) this.Modified;
        directedData.Item1.SetDirected( directedData.Item2, false );

        // Update front end
        Controller.Singleton.GetEdgeObj( directedData.Item1 ).SetDirectedness( directedData.Item2, false );
    }

    private void UndoEdgeLabel()
    {
        ( Edge, string, string ) labelData = ( ( Edge, string, string ) ) this.Modified;
        labelData.Item1.SetLabel( labelData.Item2, false );
        
        // Update front end
        Controller.Singleton.GetEdgeObj( labelData.Item1 ).labelObj.UpdateEdgeLabel( labelData.Item2, false );
    }

    private void UndoEdgeStyles()
    {
        ( List< Edge >, List< uint >, List< uint > ) styleData = ( ( List< Edge >, List< uint >, List< uint > ) ) this.Modified;
        for ( int i = 0; i < styleData.Item1.Count; ++i )
        {
            styleData.Item1[ i ].SetStyle( styleData.Item2[ i ], false );

            // TODO: Update front end
            // FRONT END NOT IMPLEMENTED
        }
    }

    private void UndoEdgeColors()
    {
        ( List< Edge >, List< uint >, List< uint > ) colorData = ( ( List< Edge >, List< uint >, List< uint > ) ) this.Modified;
        for ( int i = 0; i < colorData.Item1.Count; ++i )
        {
            colorData.Item1[ i ].SetColor( colorData.Item2[ i ], false );

            // TODO: Update front end
            // FRONT END NOT IMPLEMENTED
        }
    }

    private void UndoEdgeThicknesses()
    {
        ( List< Edge >, List< uint >, List< uint > ) thicknessData = ( ( List< Edge >, List< uint >, List< uint > ) ) this.Modified;
        for ( int i = 0; i < thicknessData.Item1.Count; ++i )
        {
            thicknessData.Item1[i].SetThickness(thicknessData.Item2[ i ], false);
            
            // Update front end
            Controller.Singleton.GetEdgeObj( thicknessData.Item1[ i ] ).UpdateSpline();
        }
    }

    private void UndoEdgeCurvatures()
    {
        ( List< Edge >, List< int >, List< int > ) curveData = ( ( List< Edge >, List< int >, List< int > ) ) this.Modified;
        for ( int i = 0; i < curveData.Item1.Count; ++i )
        {
            curveData.Item1[i].SetCurvature(curveData.Item2[i], false);
            
            // Update front end
            Controller.Singleton.GetEdgeObj( curveData.Item1[i] ).UpdateSpline();
        }
    }

    private void UndoEdgeTailStyles()
    {
        ( List< Edge >, List< uint >, List< uint > ) tailStyleData = ( ( List< Edge >, List< uint >, List< uint > ) ) this.Modified;
        for ( int i = 0; i < tailStyleData.Item1.Count; ++i )
        {
            tailStyleData.Item1[ i ].SetTailStyle( tailStyleData.Item2[ i ], false );

            // TODO: Update front end
            // FRONT END NOT IMPLEMENTED
        }
    }

    private void UndoEdgeHeadStyles()
    {
        ( List< Edge >, List< uint >, List< uint > ) headStyleData = ( ( List< Edge >, List< uint >, List< uint > ) ) this.Modified;
        for ( int i = 0; i < headStyleData.Item1.Count; ++i )
        {
            headStyleData.Item1[ i ].SetHeadStyle( headStyleData.Item2[ i ], false );

            // TODO: Update front end
            // FRONT END NOT IMPLEMENTED
        }
    }

    private void UndoEdgeReverse()
    {
        ( ( Edge ) this.Modified ).Reverse( false );

        // Update front end
        Controller.Singleton.GetEdgeObj( ( Edge ) this.Modified ).ReverseEdge(false);
    }

    private void UndoAddCollection()
    {
        ( HashSet< Vertex >, HashSet< Edge > ) collection = ( ( HashSet< Vertex >, HashSet< Edge > ) ) this.Modified;
        if ( !( collection.Item2 is null ) )
        {
            foreach ( Edge edge in collection.Item2 )
            {
                this.graph.Remove( edge, false );

                // Update front end
                Controller.Singleton.RemoveEdge( Controller.Singleton.GetEdgeObj( edge ), false );
            }
        }
        if ( !( collection.Item1 is null ) )
        {
            foreach ( Vertex vert in collection.Item1 )
            {
                this.graph.Remove( vert, false );

                // Update front end
                Controller.Singleton.RemoveVertex( Controller.Singleton.GetVertexObj( vert ), false );
            }
        }
    }

    private void UndoRemoveCollection()
    {
        ( HashSet< Vertex >, HashSet< Edge > ) collection = ( ( HashSet< Vertex >, HashSet< Edge > ) ) this.Modified;
        if ( !( collection.Item1 is null ) )
        {
            foreach ( Vertex vert in collection.Item1 )
            {
                this.graph.Add( vert, false );

                // Update front end
                Controller.Singleton.CreateVertexObj( vert );
            }
        }
        if ( !( collection.Item2 is null ) )
        {
            foreach ( Edge edge in collection.Item2 )
            {
                this.graph.Add( edge, false );

                // Update front end
                Controller.Singleton.CreateEdgeObj( edge );
            }
        }
    }

    public void Redo()
    {
        switch ( this.Mod )
        {
            case Modification.VERTEX_LABEL:
                this.RedoVertexLabel();
                break;
            case Modification.VERTEX_POSES:
                this.RedoVertexPoses();
                break;
            case Modification.VERTEX_STYLES:
                this.RedoVertexStyles();
                break;
            case Modification.VERTEX_SIZES:
                this.RedoVertexSizes();
                break;
            case Modification.VERTEX_COLORS:
                this.RedoVertexColors();
                break;
            case Modification.EDGE_DIRECTED:
                this.RedoEdgeDirected();
                break;
            case Modification.EDGE_LABEL:
                this.RedoEdgeLabel();
                break;
            case Modification.EDGE_STYLES:
                this.RedoEdgeStyles();
                break;
            case Modification.EDGE_COLORS:
                this.RedoEdgeColors();
                break;
            case Modification.EDGE_THICKNESSES:
                this.RedoEdgeThicknesses();
                break;
            case Modification.EDGE_CURVATURES:
                this.RedoEdgeCurvatures();
                break;
            case Modification.EDGE_TAIL_STYLES:
                this.RedoEdgeTailStyles();
                break;
            case Modification.EDGE_HEAD_STYLES:
                this.RedoEdgeHeadStyles();
                break;
            case Modification.EDGE_REVERSE:
                this.RedoEdgeReverse();
                break;
            case Modification.ADD_COLLECTION:
                this.RedoAddCollection();
                break;
            case Modification.REMOVE_COLLECTION:
                this.RedoRemoveCollection();
                break;
        }

        // Update Algorithm Manager
        GraphInfo.Singleton.UpdateGraphInfo();
    }

    private void RedoVertexLabel()
    {
        ( Vertex, string, string ) labelData = ( ( Vertex, string, string ) ) this.Modified;
        labelData.Item1.SetLabel( labelData.Item3, false );
        
    }

    private void RedoVertexPoses()
    {
        ( List< Vertex >, List< System.Numerics.Vector2 >, List< System.Numerics.Vector2 > ) posData = ( ( List< Vertex >, List< System.Numerics.Vector2 >, List< System.Numerics.Vector2 > ) ) this.Modified;
        for ( int i = 0; i < posData.Item1.Count; ++i )
        {
            posData.Item1[ i ].SetPos( posData.Item3[ i ], false );

            // TODO: Update front end
            Controller.Singleton.GetVertexObj( posData.Item1[i] ).MovePosition(new Vector3( posData.Item3[i].X, posData.Item3[i].Y, 0 ));
        }
    }

    private void RedoVertexStyles()
    {
        ( List< Vertex >, List< uint >, List< uint > ) styleData = ( ( List< Vertex >, List< uint >, List< uint > ) ) this.Modified;
        for ( int i = 0; i < styleData.Item1.Count; ++i )
        {
            styleData.Item1[ i ].SetStyle( styleData.Item3[ i ], false );

            // Update front end
            Controller.Singleton.GetVertexObj( styleData.Item1[ i ] ).SetStyle( styleData.Item3[ i ], false );
        }
    }

    private void RedoVertexSizes()
    {
        ( List< Vertex >, List< uint >, List< uint > ) sizeData = ( ( List< Vertex >, List< uint >, List< uint > ) ) this.Modified;
        for ( int i = 0; i < sizeData.Item1.Count; ++i )
        {
            sizeData.Item1[ i ].SetSize( sizeData.Item3[ i ], false );

            // TODO: Update front end
            // FRONT END NOT IMPLEMENTED
        }
    }

    private void RedoVertexColors()
    {
        ( List< Vertex >, List< uint >, List< uint > ) colorData = ( ( List< Vertex >, List< uint >, List< uint > ) ) this.Modified;
        for ( int i = 0; i < colorData.Item1.Count; ++i )
        {
            colorData.Item1[ i ].SetColor( colorData.Item3[ i ], false );

            // TODO: Update front end
            // FRONT END NOT IMPLEMENTED
        }
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

    private void RedoEdgeStyles()
    {
        ( List< Edge >, List< uint >, List< uint > ) styleData = ( ( List< Edge >, List< uint >, List< uint > ) ) this.Modified;
        for ( int i = 0; i < styleData.Item1.Count; ++i )
        {
            styleData.Item1[ i ].SetStyle( styleData.Item3[ i ], false );

            // TODO: Update front end
            // FRONT END NOT IMPLEMENTED
        }
    }

    private void RedoEdgeColors()
    {
        ( List< Edge >, List< uint >, List< uint > ) colorData = ( ( List< Edge >, List< uint >, List< uint > ) ) this.Modified;
        for ( int i = 0; i < colorData.Item1.Count; ++i )
        {
            colorData.Item1[ i ].SetColor( colorData.Item3[ i ], false );

            // TODO: Update front end
            // FRONT END NOT IMPLEMENTED
        }
    }

    private void RedoEdgeThicknesses()
    {
        ( List< Edge >, List< uint >, List< uint > ) thicknessData = ( ( List< Edge >, List< uint >, List< uint > ) ) this.Modified;
        for ( int i = 0; i < thicknessData.Item1.Count; ++i )
        {
            thicknessData.Item1[i].SetThickness(thicknessData.Item3[i], false);
            
            // Update front end
            Controller.Singleton.GetEdgeObj( thicknessData.Item1[i] ).UpdateSpline();
        }
    }

    private void RedoEdgeCurvatures()
    {
        ( List< Edge >, List< int >, List< int > ) curveData = ( ( List< Edge >, List< int >, List< int > ) ) this.Modified;
        for ( int i = 0; i < curveData.Item1.Count; ++i )
        {
            curveData.Item1[i].SetCurvature(curveData.Item3[i], false);
            
            // Update front end
            Controller.Singleton.GetEdgeObj( curveData.Item1[i] ).UpdateSpline();
        }
    }

    private void RedoEdgeTailStyles()
    {
        ( List< Edge >, List< uint >, List< uint > ) tailStyleData = ( ( List< Edge >, List< uint >, List< uint > ) ) this.Modified;
        for ( int i = 0; i < tailStyleData.Item1.Count; ++i )
        {
            tailStyleData.Item1[ i ].SetTailStyle( tailStyleData.Item3[ i ], false );

            // TODO: Update front end
            // FRONT END NOT IMPLEMENTED
        }
    }

    private void RedoEdgeHeadStyles()
    {
        ( List< Edge >, List< uint >, List< uint > ) headStyleData = ( ( List< Edge >, List< uint >, List< uint > ) ) this.Modified;
        for ( int i = 0; i < headStyleData.Item1.Count; ++i )
        {
            headStyleData.Item1[ i ].SetHeadStyle( headStyleData.Item3[ i ], false );

            // TODO: Update front end
            // FRONT END NOT IMPLEMENTED
        }
    }

    private void RedoEdgeReverse()
    {
        this.UndoEdgeReverse();
    }

    private void RedoAddCollection()
    {
        ( HashSet< Vertex >, HashSet< Edge > ) collection = ( ( HashSet< Vertex >, HashSet< Edge > ) ) this.Modified;
        
        if ( !( collection.Item1 is null ) )
        {
            this.graph.Add( new List< Vertex >( collection.Item1 ), false );

            // Update front end
            foreach ( Vertex v in collection.Item1 )
                // Controller.Singleton.RemoveVertex( Controller.Singleton.GetVertexObj( v ), false );
                Controller.Singleton.CreateVertexObj(v, false);
            Controller.Singleton.ForceInvokeModificationEvent();
        }
        if ( !( collection.Item2 is null ) )
        {
            this.graph.Add( new List< Edge >( collection.Item2 ), false );

            // Update front end
            foreach ( Edge e in collection.Item2 )
                // Controller.Singleton.RemoveEdge( Controller.Singleton.GetEdgeObj( e ), false );
                Controller.Singleton.CreateEdgeObj(e, false);
            Controller.Singleton.ForceInvokeModificationEvent();
        }
    }

    private void RedoRemoveCollection()
    {
        ( HashSet< Vertex >, HashSet< Edge > ) collection = ( ( HashSet< Vertex >, HashSet< Edge > ) ) this.Modified;
        
        if ( !( collection.Item2 is null ) )
        {
            this.graph.Remove( new List< Edge >( collection.Item2 ), false );

            // Update front end
            foreach ( Edge e in collection.Item2 )
                Controller.Singleton.RemoveEdge( Controller.Singleton.GetEdgeObj( e ), false );
        }
        if ( !( collection.Item1 is null ) )
        {
            this.graph.Remove( new List< Vertex >( collection.Item1 ), false );

            // Update front end
            foreach ( Vertex v in collection.Item1 )
                Controller.Singleton.RemoveVertex( Controller.Singleton.GetVertexObj( v ), false );
        }
    }
}
