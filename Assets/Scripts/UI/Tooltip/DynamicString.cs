using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class DynamicString {
    private const string OPEN_SIGN  = "{{";
    private const string CLOSE_SIGN = "}}";

    [SerializeField] private string                                    _Source;
    [SerializeField] private SerializedDictionary<string, List<string>> _Dict;

    public string Generate(int index) {
        string result = "";

        try {
            for (int i = 0; i < _Source.Length;) {
                int startLast = _Source.IndexOf(OPEN_SIGN, i, StringComparison.Ordinal) + OPEN_SIGN.Length;
                int stopFirst = _Source.IndexOf(CLOSE_SIGN, startLast, StringComparison.Ordinal);

                result += _Source[i..(startLast - OPEN_SIGN.Length)];
                
                if (_Source[startLast] == '$') {
                    var    values  = _Dict[_Source[(startLast + 1)..stopFirst]];
                    string oldItem = values[index];    
                    values[index] =  "<b>" +  oldItem + "</b>";
                    result        += $"[ {string.Join(" | ", values)} ]";
                    values[index] =  oldItem;
                } else result += _Dict[_Source[startLast..stopFirst]][index];

                i = stopFirst + CLOSE_SIGN.Length;
            }
        } catch (Exception e) {
            Debug.LogException(e);
            Debug.LogError("NGDtuanh DynamicString: Fail to generate string, syntax error (see stack trace above for more details)");
        }

        return result;
    }

    private string Source_SafeSubstr(int start, int stop) {
        if (start < 0
         || stop  < 0
         || start >= _Source.Length
         || stop  >= _Source.Length) return "";
        return _Source.Substring(start, stop - start + 1);
    }

    private int Source_SafeIndexOf(in string target, int startIndex) {
        if (startIndex < 0 || _Source.Length <= startIndex) return -1;
        return _Source.IndexOf(target, startIndex, StringComparison.Ordinal);
    }
}