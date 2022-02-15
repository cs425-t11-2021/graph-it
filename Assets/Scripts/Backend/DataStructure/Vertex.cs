
// All code developed by Team 11

using System;

[System.Serializable]
public class Vertex
{
    static private uint idCount;

    private uint id;
    public string label;
    public double? x, y;

    public uint style;
    public uint color;
    public uint labelStyle;

    // default value position is null
    public Vertex( string label="", double? x=null, double? y=null, uint style=0, uint color=0, uint labelStyle=0 )
    {
        this.id = Vertex.idCount++;
        this.label = label;
        this.x = x;
        this.y = y;
        this.style = style;
        this.color = color;
        this.labelStyle = labelStyle;
    }

    public uint GetId() => this.id; // temp

    public override bool Equals( object obj ) => obj is Vertex other && this.Equals( other );

    public bool Equals( Vertex vert ) => this.id == vert.id;

    public override int GetHashCode() => ( label, x, y, style, color, labelStyle ).GetHashCode();

    public static bool operator ==( Vertex lhs, Vertex rhs ) => lhs.Equals( rhs );

    public static bool operator !=( Vertex lhs, Vertex rhs) => !( lhs == rhs );

    public static bool operator <( Vertex lhs, Vertex rhs ) => lhs.id < rhs.id;

    // public static bool operator <=( Vertex lhs, Vertex rhs ) => lhs < rhs || lhs == rhs;

    public static bool operator >( Vertex lhs, Vertex rhs ) => lhs.id > rhs.id;

    // public static bool operator >=( Vertex lhs, Vertex rhs ) => !( lhs < rhs );

    public override string ToString() => String.Format( "id: {0}, label: {1}, x: {2}, y: {3}, style: {4}, color: {5}, label style: {6}", this.id, this.label, this.x, this.y, this.style, this.color, this.labelStyle );
}
