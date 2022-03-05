
// All code developed by Team 11

using System;
using UnityEngine;

[System.Serializable]
public class Edge
{
    public Action< Edge > MakeDirectedInGraph { private get; set; }
    public Action< Edge > MakeUndirectedInGraph { private get; set; }
    public Action< Edge > ReverseInGraph { private get; set; }
    public Action< Modification, System.Object > CreateMod { private get; set; }
    public Vertex vert1; // TODO: private set
    public Vertex vert2; // TODO: private set
    private bool directed;
    public bool Directed
    {
        get => directed;
        set => this.SetDirected( value );
    }
    public bool Weighted { get; private set; }
    public float Weight { get; private set; }
    private string label;
    public string Label
    {
        get => this.label;
        set => this.SetLabel( value );
    }

    private uint style;
    public uint Style
    {
        get => this.style;
        set => this.SetStyle( value );
    }
    private uint color;
    public uint Color
    {
        get => this.color;
        set => this.SetColor( value );
    }
    private uint thickness;
    public uint Thickness
    {
        get => this.thickness;
        set => this.SetThickness( value );
    }
    private int curvature;
    public int Curvature
    {
        get => this.curvature;
        set => this.SetCurvature( value );
    }

    // when undirected, following should be ignored
    private uint tailStyle;
    public uint TailStyle
    {
        get => this.tailStyle;
        set => this.SetTailStyle( value );
    }
    private uint headStyle;
    public uint HeadStyle
    {
        get => this.headStyle;
        set => this.SetHeadStyle( value );
    }

    public Edge( Vertex vert1, Vertex vert2, bool directed=false, string label="", uint style=0, uint color=0, uint thickness=0, int curvature=0, uint tailStyle=0, uint headStyle=0 )
    {
        this.vert1 = vert1;
        this.vert2 = vert2;
        this.directed = directed;
        this.SetLabel( label, false );
        this.style = style;
        this.color = color;
        this.thickness = thickness;
        this.curvature = curvature;
        this.tailStyle = tailStyle;
        this.headStyle = headStyle;
    }

    public Edge( Edge edge ) : this( edge.vert1, edge.vert2, edge.directed, edge.label, edge.style, edge.color, edge.thickness, edge.curvature, edge.tailStyle, edge.headStyle ) { }

    public void Reverse( bool recordChange=true )
    {
        if ( !( this.ReverseInGraph is null ) )
            this.ReverseInGraph( this );

        ( this.vert1, this.vert2 ) = ( this.vert2, this.vert1 );

        if ( recordChange )
            this.CreateMod( Modification.EDGE_REVERSE, this );
    }

    public void SetDirected( bool directed, bool recordChange=true )
    {
        if ( directed ^ this.directed )
        {
            if ( directed )
                this.MakeDirectedInGraph( this );
            else
                this.MakeUndirectedInGraph( this );

            if ( recordChange )
                this.CreateMod( Modification.EDGE_DIRECTED, ( this, this.directed ) );

            this.directed = directed;
        }
    }

    public void SetLabel( string label, bool recordChange=true )
    {
        float weight;
        string labelTemp = label.Replace( " ", "" ).Replace( "+", "" ).ToLower();
        if ( label.Equals( "inf" ) || labelTemp.Equals( "infinity" ) )
        {
            this.Weighted = true;
            this.Weight = Single.PositiveInfinity;
        }
        else if ( labelTemp.Equals( "-inf" ) || labelTemp.Equals( "-infinity" ) )
        {
            this.Weighted = true;
            this.Weight = Single.NegativeInfinity;
        }
        else if ( Single.TryParse( labelTemp, out weight ) )
        {
            this.Weighted = true;
            this.Weight = weight;
        }
        else
        {
            this.Weighted = false;
            this.Weight = 1;
        }

        if ( recordChange )
        {
            if ( this.CreateMod is null )
                // throw new System.Exception( "Edge label change cannot be recorded." );
                Debug.Log( "Edge label change cannot be recorded." );
            // this.CreateMod( Modification.EDGE_LABEL, ( this, this.label, label ) );
        }

        this.label = label;
    }

    public void SetStyle( uint style, bool recordChange=true )
    {
        if ( recordChange )
            this.CreateMod( Modification.EDGE_STYLE, ( this, this.style, style ) );
        this.style = style;
    }

    public void SetColor( uint color, bool recordChange=true )
    {
        if ( recordChange )
            this.CreateMod( Modification.EDGE_COLOR, ( this, this.color, color ) );
        this.color = color;
    }

    public void SetThickness( uint thickness, bool recordChange=true )
    {
        if ( recordChange )
            this.CreateMod( Modification.EDGE_THICKNESS, ( this, this.thickness, thickness ) );
        this.thickness = thickness;
    }

    public void SetCurvature( int curvature, bool recordChange=true )
    {
        if ( recordChange )
            this.CreateMod( Modification.EDGE_CURVATURE, ( this, this.curvature, curvature ) );
        this.curvature = curvature;
    }

    public void SetTailStyle( uint style, bool recordChange=true )
    {
        if ( recordChange )
            this.CreateMod( Modification.EDGE_TAIL_STYLE, ( this, this.tailStyle, style ) );
        this.tailStyle = style;
    }

    public void SetHeadStyle( uint style, bool recordChange=true )
    {
        if ( recordChange )
            this.CreateMod( Modification.EDGE_HEAD_STYLE, ( this, this.headStyle, style ) );
        this.headStyle = style;
    }

    public void ResetWeight() => this.Weight = 1;

    public bool? IsTail( Vertex vert ) => this.directed ? vert == this.vert1 : ( bool? ) null;

    public bool? IsHead( Vertex vert ) => this.directed ? vert == this.vert2 : ( bool? ) null;

    public bool IncidentOn( Vertex vert ) => vert == this.vert1 || vert == this.vert2;

    public override int GetHashCode() => ( this.vert1, this.vert2, this.directed, this.label, this.Weight, this.style, this.color, this.thickness ).GetHashCode();

    public override string ToString()
    {
        if ( this.directed )
            return String.Format( "vert1: {0}, vert2: {1}, directed: {2}, label: {3}, style: {4}, color: {5}, thickness: {6}, tail style: {7}, head style: {8}",  this.vert1.GetId(), this.vert2.GetId(), this.directed, this.label, this.style, this.color, this.thickness, this.tailStyle, this.headStyle );
        return String.Format( "vert1: {0}, vert2: {1}, directed: {2}, label: {3}, style: {4}, color: {5}, thickness: {6}",  this.vert1.GetId(), this.vert2.GetId(), this.directed, this.label, this.style, this.color, this.thickness );
    }
}
