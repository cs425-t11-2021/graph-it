// From https://thomaslevesque.com/2019/11/18/using-foreach-with-index-in-c/

using System.Collections.Generic;
using System.Linq;

// Class to extend all C# IEnumerable data structures (lists, dict, etc) to add an ability to iterate through the 
// data structure and store the index at the same time. To use, do "foreach ((T item, int i) in ds.WithIndex()) {}".
public static class ForeachExtension
{
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
    {
        return source.Select((item, index) => (item, index));
    }
}