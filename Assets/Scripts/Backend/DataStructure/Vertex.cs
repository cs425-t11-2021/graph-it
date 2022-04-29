
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class Vertex
{
    static private uint idCount = 0;

    private uint id;
    public Action< Modification, Object > CreateMod { private get; set; }
    private string label;
    public string Label
    {
        get => this.label;
        set => this.SetLabel( value );
    }
    private System.Numerics.Vector2 pos;
    public System.Numerics.Vector2 Pos
    {
        get => this.pos;
        set => this.SetPos( value );
    }

    private uint style;
    public uint Style
    {
        get => this.style;
        set => this.SetStyle( value );
    }
    private uint size;
    public uint Size
    {
        get => this.size;
        set => this.SetSize( value );
    }
    private uint color;
    public uint Color
    {
        get => this.color;
        set => this.SetColor( value );
    }

    public Vertex( string label="", float x=0, float y=0, uint style=0, uint size=0, uint color=0 )
    {
        this.id = Vertex.idCount++;
        this.label = label;
        this.pos = new System.Numerics.Vector2( x, y );
        this.style = style;
        this.size = size;
        this.color = color;
    }

    public Vertex( Vertex vert ) : this( vert.label, vert.pos.X, vert.pos.Y, vert.style, vert.size, vert.color ) { }

    public void SetLabel( string label, bool recordChange=true )
    {
        if ( recordChange )
        {
            if ( this.CreateMod is null )
                throw new System.Exception( "Vertex label change cannot be recorded." );
            this.CreateMod( Modification.VERTEX_LABEL, ( this, this.label, label ) );
        }
        this.label = label;
    }

    public void SetPos( System.Numerics.Vector2 pos, bool recordChange=true )
    {
        if ( recordChange )
            this.CreateMod( Modification.VERTEX_POSES, ( new List< Vertex >() { this }, new List< System.Numerics.Vector2 >() { this.pos }, new List< System.Numerics.Vector2 >() { pos } ) );
        this.pos = pos;
    }

    public static void SetPoses( List< Vertex > vertices, List< System.Numerics.Vector2 > poses, bool recordChange=true )
    {
        List< System.Numerics.Vector2 > oldPoses = new List< System.Numerics.Vector2 >( vertices.Select( v => v.pos ) );
        if ( recordChange )
            vertices[ 0 ].CreateMod( Modification.VERTEX_POSES, ( vertices, oldPoses, poses ) );
        for ( int i = 0; i < vertices.Count; ++i )
            vertices[ i ].SetPos( poses[ i ], false );
    }

    public void SetStyle( uint style, bool recordChange=true )
    {
        if ( recordChange )
            this.CreateMod( Modification.VERTEX_STYLES, ( new List< Vertex >() { this }, new List< uint >() { this.style }, new List< uint >() { style } ) );
        this.style = style;
    }

    public static void SetStyle( List< Vertex > vertices, uint style, bool recordChange=true )
    {
        List< uint > newStyles = new List< uint >( vertices.Select( v => style ) );
        Vertex.SetStyles( vertices, newStyles, recordChange );
    }

    public static void SetStyles( List< Vertex > vertices, List< uint > styles, bool recordChange=true )
    {
        List< uint > oldStyles = new List< uint >( vertices.Select( v => v.style ) );
        if ( recordChange )
            vertices[ 0 ].CreateMod( Modification.VERTEX_STYLES, ( vertices, oldStyles, styles ) );
        for ( int i = 0; i < vertices.Count; ++i )
            vertices[ i ].SetStyle( styles[ i ], false );
    }

    public void SetSize( uint size, bool recordChange=true )
    {
        if ( recordChange )
            this.CreateMod( Modification.VERTEX_SIZES, ( new List< Vertex >() { this }, new List< uint >() { this.size },new List< uint >() { size } ) );
        this.size = size;
    }

    public static void SetSizes( List< Vertex > vertices, List< uint > sizes, bool recordChange=true )
    {
        List< uint > oldSizes = new List< uint >( vertices.Select( v => v.size ) );
        if ( recordChange )
            vertices[ 0 ].CreateMod( Modification.VERTEX_SIZES, ( vertices, oldSizes, sizes ) );
        for ( int i = 0; i < vertices.Count; ++i )
            vertices[ i ].SetSize( sizes[ i ], false );
    }

    public static void IncrementSize( List< Vertex > vertices, bool recordChange=true )
    {
        List< uint > newSizes = new List< uint >( vertices.Select( v => v.size + 1 ) );
        Vertex.SetSizes( vertices, newSizes, recordChange );
    }

    public static void DecrementSize( List< Vertex > vertices, bool recordChange=true )
    {
        List< uint > newSizes = new List< uint >( vertices.Select( v => v.size - 1 ) );
        Vertex.SetSizes( vertices, newSizes, recordChange );
    }

    public void SetColor( uint color, bool recordChange=true )
    {
        if ( recordChange )
            this.CreateMod( Modification.VERTEX_COLORS, ( new List< Vertex >() { this }, new List< uint >() { this.color }, new List< uint >() { color } ) );
        this.color = color;
    }

    public static void SetColor( List< Vertex > vertices, uint color, bool recordChange=true )
    {
        List< uint > newColors = new List< uint >( vertices.Select( v => color ) );
        Vertex.SetColors( vertices, newColors, recordChange );
    }

    public static void SetColors( List< Vertex > vertices, List< uint > colors, bool recordChange=true )
    {
        List< uint > oldColors = new List< uint >( vertices.Select( v => v.color ) );
        if ( recordChange )
            vertices[ 0 ].CreateMod( Modification.VERTEX_COLORS, ( vertices, oldColors, colors ) );
        for ( int i = 0; i < vertices.Count; ++i )
            vertices[ i ].SetColor( colors[ i ], false );
    }

    public uint GetId() => this.id; // temp

    public override bool Equals( object obj ) => obj is Vertex other && this.Equals( other );

    public bool Equals( Vertex vert ) => this.id == vert.id;

    public override int GetHashCode() => ( int ) this.id;

    public static bool operator ==( Vertex lhs, Vertex rhs ) => lhs.Equals( rhs );

    public static bool operator !=( Vertex lhs, Vertex rhs) => !( lhs == rhs );

    public static bool operator <( Vertex lhs, Vertex rhs ) => lhs.id < rhs.id;

    public static bool operator >( Vertex lhs, Vertex rhs ) => lhs.id > rhs.id;

    public override string ToString() => String.Format( "id: {0}, label: {1}, x: {2}, y: {3}, style: {4}, color: {5}", this.id, this.label, this.pos.X, this.pos.Y, this.style, this.color );
}
