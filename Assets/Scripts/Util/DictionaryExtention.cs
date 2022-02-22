
using System;
using System.Collections.Generic;

public static class DictionaryExtension
{
	// https://stackoverflow.com/questions/538729/is-there-an-idictionary-implementation-that-on-missing-key-returns-the-default
    public static Value GetValue< Key, Value >( this IDictionary< Key, Value > dict, Key key, Value default_value=default( Value ) )
    {
        return dict.TryGetValue( key, out Value value ) ? value : default_value;
    }

    // https://stackoverflow.com/questions/34416931/dictionarytkey-tvalue-foreach-method
    public static void ForEach<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Action<TKey, TValue> invoke)
    {
        foreach(var kvp in dictionary)
            invoke(kvp.Key, kvp.Value);
    }
}