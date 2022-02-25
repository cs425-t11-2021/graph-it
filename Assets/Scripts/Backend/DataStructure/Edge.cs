
// All code developed by Team 11

using System;

[System.Serializable]
public class Edge
{
    public Vertex vert1, vert2; // TODO: private set
    public bool directed;
    public bool weighted; // TODO: private set
    public double weight; // TODO: private set, change to float
    private string label;
    public string Label
    {
        get => this.label;
        set => this.SetLabel( value );
    }

    // TODO: make enums
    public uint style;
    public uint color;
    public uint thickness;

    // when undirected, following should be ignored
    public uint? tailStyle;
    public uint? headStyle;

    public Edge( Vertex vert1, Vertex vert2, bool directed=false, string label="", uint style=0, uint color=0, uint thickness=0, uint? tailStyle=null, uint? headStyle=null )
    {
        this.vert1 = vert1;
        this.vert2 = vert2;
        this.directed = directed;
        this.Label = label;
        this.style = style;
        this.color = color;
        this.thickness = thickness;
        this.tailStyle = tailStyle;
        this.headStyle = headStyle;
    }

    public Edge( Edge edge )
    {
        this.vert1 = edge.vert1;
        this.vert2 = edge.vert2;
        this.directed = edge.directed;
        this.Label = edge.label;
        this.style = edge.style;
        this.color = edge.color;
        this.thickness = edge.thickness;
        this.tailStyle = edge.tailStyle;
        this.headStyle = edge.headStyle;
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
            return String.Format( "vert1: {0}, vert2: {1}, directed: {2}, label: {3}, style: {4}, color: {5}, thickness: {6}, tail style: {7}, head style: {8}",  this.vert1.GetId(), this.vert2.GetId(), this.directed, this.label, this.style, this.color, this.thickness, this.tailStyle, this.headStyle );
        return String.Format( "vert1: {0}, vert2: {1}, directed: {2}, label: {3}, style: {4}, color: {5}, thickness: {6}",  this.vert1.GetId(), this.vert2.GetId(), this.directed, this.label, this.style, this.color, this.thickness );
    }

    private void SetLabel( string label )
    {
        string labelTemp = label.Replace( " ", "" ).Replace( "+", "" ).ToLower();
        if ( label.Equals( "inf" ) || labelTemp.Equals( "infinity" ) )
        {
            this.weighted = true;
            this.weight = Double.PositiveInfinity;
        }
        else if ( labelTemp.Equals( "-inf" ) || labelTemp.Equals( "-infinity" ) )
        {
            this.weighted = true;
            this.weight = Double.NegativeInfinity;
        }
        else if ( Double.TryParse( labelTemp, out this.weight ) )
            this.weighted = true;
        else
        {
            this.weighted = false;
            this.weight = 1;
        }
        this.label = label;
    }
}
