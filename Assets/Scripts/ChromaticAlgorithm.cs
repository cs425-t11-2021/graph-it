
using System;
using System.Threading;

[System.Serializable]
public class ChromaticAlgorithm
{
	private Graph graph;

	public ChromaticAlgorithm( Graph graph )
	{
		this.graph = graph;
		this.Run();
	}

	public int Run()
	{
		// create new thread using GetChromaticNumber
		// save and return result
	}


	public int GetChromaticNumber()
    {
        if ( !( this.chromatic_num is null ) )
            return ( int ) this.chromatic_num;
        int chi = this.vertices.Count;
        HashSet< List< int > > colorings = this.GetAllColorings();
        foreach ( List< int > coloring in colorings )
        {
            int num_colors = ( new HashSet< int >( coloring ) ).Count;
            if ( num_colors < chi && this.IsProperColoring( coloring ) )
                chi = num_colors;
        }
        this.chromatic_num = chi;
        return chi;
    }

    private bool IsProperColoring( List< int > coloring )
    {
        foreach ( Edge edge in this.adjacency.Values )
        {
            if ( coloring[ this.vertices.IndexOf( edge.vert1 ) ] == coloring[ this.vertices.IndexOf( edge.vert2 ) ] )
                return false;
        }
        return true;
    }

    private HashSet< List< int > > GetAllColorings()
    {
        HashSet< List< int > > colorings = new HashSet< List< int > >();
        GetAllColoringsHelper( colorings, new List< int >(), this.vertices.Count, this.vertices.Count );
        return colorings;
    }

    private static void GetAllColoringsHelper( HashSet< List< int > > colorings, List< int > coloring, int num_vertices, int num_colors )
    {
        if ( coloring.Count >= num_vertices )
            colorings.Add( coloring );
        else
        {
            for ( int i = 0; i < num_colors; i++ )
            {
                List< int > new_coloring = new List< int >( coloring );
                new_coloring.Add( i );
                GetAllColoringsHelper( colorings, new_coloring, num_vertices, num_colors );
            }
        }
    }
}
