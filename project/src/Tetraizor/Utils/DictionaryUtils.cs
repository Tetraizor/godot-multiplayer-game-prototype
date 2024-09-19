using Godot.Collections;

namespace Tetraizor.Utils;

public static class DictionaryUtils
{
    public static Dictionary Merge(Dictionary dict1, Dictionary dict2)
    {
        Dictionary mergedDict = new Dictionary();

        foreach (var key in dict1.Keys)
        {
            mergedDict[key] = dict1[key];
        }

        foreach (var key in dict2.Keys)
        {
            mergedDict[key] = dict2[key];
        }

        return mergedDict;
    }
}