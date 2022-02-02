
// All code developed by Team 11

using System;


[System.Serializable]
public class Vertex
{
    static private uint id_count;

    private uint id;
    public string label;
    public double? x_pos, y_pos;

    public uint style;
    public uint color;
    public uint label_style;

    // default value position is null
    public Vertex( string label="", double? x_pos=null, double? y_pos=null, uint style=0, uint color=0, uint label_style=0 )
    {
        this.id = Vertex.id_count++;
        this.label = label;
        this.x_pos = x_pos;
        this.y_pos = y_pos;
        this.style = style;
        this.color = color;
        this.label_style = label_style;
    }

    public uint GetId() => this.id; // temp

    public override bool Equals( object obj ) => obj is Vertex other && this.Equals( other );

    public bool Equals( Vertex vert ) => this.id == vert.id;

    public override int GetHashCode() => ( label, x_pos, y_pos, style, color, label_style ).GetHashCode();

    public static bool operator ==( Vertex lhs, Vertex rhs ) => lhs.Equals( rhs );

    public static bool operator !=( Vertex lhs, Vertex rhs) => !( lhs == rhs );

    public static bool operator <( Vertex lhs, Vertex rhs ) => lhs.id < rhs.id;

    // public static bool operator <=( Vertex lhs, Vertex rhs ) => lhs < rhs || lhs == rhs;

    public static bool operator >( Vertex lhs, Vertex rhs ) => lhs.id > rhs.id;

    // public static bool operator >=( Vertex lhs, Vertex rhs ) => !( lhs < rhs );

    public override string ToString() => String.Format( "id: {0}, label: {1}, x_pos: {2}, y_pos: {3}, style: {4}, color: {5}, label_style: {6}", this.id, this.label, this.x_pos, this.y_pos, this.style, this.color, this.label_style );
}
