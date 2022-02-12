
using System.Collections.Generic;
using System;

public static class DictionaryExtension
{
	// credit: stackoverflow
    public static Value GetValue< Key, Value >( this IDictionary< Key, Value > dict, Key key, Value default_value=default( Value ) )
    {
        Value value;
        return dict.TryGetValue( key, out value ) ? value : default_value;
    }

    // https://stackoverflow.com/questions/34416931/dictionarytkey-tvalue-foreach-method
    public static void ForEach<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Action<TKey, TValue> invoke)
    {
        foreach(var kvp in dictionary)
            invoke(kvp.Key, kvp.Value);
    }
}