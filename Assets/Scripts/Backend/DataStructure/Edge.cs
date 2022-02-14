
// All code developed by Team 11

using System;

[System.Serializable]
public class Edge
{
    public Vertex vert1, vert2;
    public bool directed;
    public bool weighted;
    public double weight;
    private string _label;
    public string label
    {
        get => this._label;
        set => this.SetLabel( value );
    }

    public uint style;
    public uint color;
    public uint thickness;
    public uint label_style;

    // when undirected, following should be ignored
    public uint? tail_style;
    public uint? head_style;

    public Edge( Vertex vert1, Vertex vert2, bool directed=false, string label="", uint style=0, uint color=0, uint thickness=0, uint label_style=0, uint? tail_style=null, uint? head_style=null )
    {
        this.vert1 = vert1;
        this.vert2 = vert2;
        this.directed = directed;
        this.label = label;
        this.style = style;
        this.color = color;
        this.thickness = thickness;
        this.label_style = label_style;
        this.tail_style = tail_style;
        this.head_style = head_style;
    }

    public void Reverse()
    {
        Vertex temp = vert2;
        vert2 = vert1;
        vert1 = temp;
    }

    public void ResetWeight() => this.weight = 1;

    public bool? IsTail( Vertex vert ) => this.directed ? vert == this.vert1 : ( bool? ) null;

    public bool? IsHead( Vertex vert ) => this.directed ? vert == this.vert2 : ( bool? ) null;

    public bool IncidentOn( Vertex vert ) => vert == this.vert1 || vert == this.vert2;

    public override int GetHashCode() => ( vert1, vert2, directed, label, weight, style, color, thickness ).GetHashCode();

    public override string ToString()
    {
        if ( this.directed )
            return String.Format( "vert1: {0}, vert2: {1}, directed: {2}, label: {3}, style: {4}, color: {5}, thickness: {6}, label_style: {7}, tail_style: {8}, head_style: {9}",  this.vert1.GetId(), this.vert2.GetId(), this.directed, this.label, this.style, this.color, this.thickness, this.label_style, this.tail_style, this.head_style );
        return String.Format( "vert1: {0}, vert2: {1}, directed: {2}, label: {3}, style: {4}, color: {5}, thickness: {6}, label_style: {7}",  this.vert1.GetId(), this.vert2.GetId(), this.directed, this.label, this.style, this.color, this.thickness, this.label_style );
    }

    private void SetLabel( string label )
    {
        label = label.Replace( " ", "" ).Replace( "+", "" ).ToLower();
        if ( label.Equals( "inf" ) || label.Equals( "infinity" ) )
        {
            this.weighted = true;
            this.weight = Double.PositiveInfinity;
        }
        else if ( label.Equals( "-inf" ) || label.Equals( "-infinity" ) )
        {
            this.weighted = true;
            this.weight = Double.NegativeInfinity;
        }
        else if ( Double.TryParse( label, out this.weight ) )
            this.weighted = true;
        else
            this.weight = 1;
        this._label = label;
    }
}
