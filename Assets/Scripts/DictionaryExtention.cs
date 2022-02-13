
using System.Collections.Generic;

public static class DictionaryExtension
{
	// credit: stackoverflow
    public static Value GetValue< Key, Value >( this IDictionary< Key, Value > dict, Key key, Value default_value=default( Value ) )
    {
        Value value;
        return dict.TryGetValue( key, out value ) ? value : default_value;
    }
}
