
// All code developed by Team 11

using System;

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
    private double? x, y; // TODO: change double to float, remove nullable
    public double? X
    {
        get => this.x;
        set => this.SetX( value );
    }
    public double? Y
    {
        get => this.y;
        set => this.SetY( value );
    }

    // TODO: make enums
    public uint style;
    public uint color;

    // default value position is null
    public Vertex( string label="", double? x=null, double? y=null, uint style=0, uint color=0 )
    {
        this.id = Vertex.idCount++;
        this.label = label;
        this.x = x;
        this.y = y;
        this.style = style;
        this.color = color;
    }

    public Vertex( Action< Modification, Object > createMod, string label="", double? x=null, double? y=null, uint style=0, uint color=0 ) : this( label, x, y, style, color )
    {
        this.createMod = createMod;
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

    public void SetX( double? x, bool recordChange=true )
    {
        // if ( recordChange )
            // this.createMod( Modification.VERTEX_POS, ( this, this.x, this.y, x, this.y ) );
        this.x = x;
    }

    public void SetY( double? y, bool recordChange=true )
    {
        // if ( recordChange )
            // this.createMod( Modification.VERTEX_LABEL, ( this, this.label, label ) );
        this.y = y;
    }

    public uint GetId() => this.id; // temp

    public override bool Equals( object obj ) => obj is Vertex other && this.Equals( other );

    public bool Equals( Vertex vert ) => this.id == vert.id;

    public override int GetHashCode() => ( int ) this.id;

    public static bool operator ==( Vertex lhs, Vertex rhs ) => lhs.Equals( rhs );

    public static bool operator !=( Vertex lhs, Vertex rhs) => !( lhs == rhs );

    public static bool operator <( Vertex lhs, Vertex rhs ) => lhs.id < rhs.id;

    public static bool operator >( Vertex lhs, Vertex rhs ) => lhs.id > rhs.id;

    public override string ToString() => String.Format( "id: {0}, label: {1}, x: {2}, y: {3}, style: {4}, color: {5}", this.id, this.label, this.x, this.y, this.style, this.color );
}
