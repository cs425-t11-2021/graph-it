// Taken from https://thomaslevesque.com/2019/11/18/using-foreach-with-index-in-c/

using System.Linq;
using System.Collections.Generic;

public static class ForeachExtension
{
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
    {
        return source.Select((item, index) => (item, index));
    }
}