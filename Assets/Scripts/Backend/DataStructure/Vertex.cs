
// All code developed by Team 11

using System;
using System.Numerics;

[System.Serializable]
public class Vertex
{
    static private uint idCount = 0;

    private uint id;
    private Action< Modification, Object > createMod;
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

    // TODO: make enums
    public uint style;
    public uint color;

    public Vertex( string label="", float x=0, float y=0, uint style=0, uint color=0 )
    {
        this.id = Vertex.idCount++;
        this.label = label;
        this.pos = new Vector2( x, y );
        this.style = style;
        this.color = color;
    }

    public Vertex( Action< Modification, Object > createMod, string label="", float x=0, float y=0, uint style=0, uint color=0 ) : this( label, x, y, style, color )
    {
        this.createMod = createMod;
    }

    public Vertex( Vertex vert )
    {
        this.id = Vertex.idCount++;
        this.createMod = vert.createMod;
        this.label = vert.label;
        this.pos = vert.pos;
        this.style = vert.style;
        this.color = vert.color;
    }

    public void SetLabel( string label, bool recordChange=true )
    {
        if ( recordChange )
        {
            if ( this.createMod is null )
                throw new System.Exception( "Vertex cannot record label change with null reference to graph." );
            this.createMod( Modification.VERTEX_LABEL, ( this, this.label, label ) );
        }
        this.label = label;
    }

    public void SetPos( Vector2 pos, bool recordChange=true )
    {
        if ( recordChange )
            this.createMod( Modification.VERTEX_POS, ( this, this.pos, pos ) );
        this.pos = pos;
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
