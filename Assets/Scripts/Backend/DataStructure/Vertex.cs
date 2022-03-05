
// All code developed by Team 11

using System;
using System.Numerics;

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
    private Vector2 pos;
    public Vector2 Pos
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
        this.pos = new Vector2( x, y );
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

    public void SetPos( Vector2 pos, bool recordChange=true )
    {
        if ( recordChange )
            this.CreateMod( Modification.VERTEX_POS, ( this, this.pos, pos ) );
        this.pos = pos;
    }

    public void SetStyle( uint style, bool recordChange=true )
    {
        if ( recordChange )
            this.CreateMod( Modification.VERTEX_STYLE, ( this, this.style, style ) );
        this.style = style;
    }

    public void SetSize( uint size, bool recordChange=true )
    {
        if ( recordChange )
            this.CreateMod( Modification.VERTEX_SIZE, ( this, this.size, size ) );
        this.size = size;
    }

    public void SetColor( uint color, bool recordChange=true )
    {
        if ( recordChange )
            this.CreateMod( Modification.VERTEX_COLOR, ( this, this.color, color ) );
        this.color = color;
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
