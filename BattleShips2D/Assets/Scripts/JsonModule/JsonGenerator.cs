using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonGenerator : MonoBehaviour {

    Dictionary<string, object> result;
    static char[] specialCharV1 = { '\"', '\\' };
    static char[] specialCharV2 = { '\n', '\r', '\t', '\b', '\f', '\v' };
    static char[] replaceChar = { 'n', 'r', 't', 'b', 'f', 'v' };

    public JsonGenerator()
    {
        result = new Dictionary<string, object>();
    }
    public void AddJsonObject(string name, object value)
    {
        result.Add(name, value);
    }

    public void Clear()
    {
        result.Clear();
    }

    public string GenerateJson()
    {
        String jsonResult = GenerateJsonObject(result);
        return "{\n" + jsonResult + "\n}";
    }

    public string GenerateJsonComponents()
    {
        String jsonResult = GenerateJsonObject(result);
        return jsonResult + ",\n";
    }

    private string GenerateJsonObject(Dictionary<string, object> listObject)
    {
        String result = "";
        int cnt = 0;
        foreach (var item in listObject)
        {
            KeyValuePair<string, object> kvPair = item;
            result += string.Format("\t\"{0}\":", kvPair.Key);
            if (kvPair.Value == null)
                result += "null";
            else
            {
                if (kvPair.Value is string || kvPair.Value is String)
                    result += string.Format("\"{0}\"", ValidateString(kvPair.Value.ToString()));
                else if (kvPair.Value is IDictionary)
                    result += string.Format("{0}", GenerateJsonObject((Dictionary<string, object>)kvPair.Value));
                else if (kvPair.Value is IList)
                    result += string.Format("{0}", GenerateJsonArray((IList)kvPair.Value));
                else
                    result += string.Format("{0}", kvPair.Value);
            }
            if (cnt < listObject.Count - 1)
                result += ",\n";
            cnt++;
        }
        return result;
    }

    private string GenerateJsonArray(IList listObject)
    {
        String result = "";
        for (int i = 0; i < listObject.Count; i++)
        {
            object currentObject = listObject[i];
            if (currentObject == null)
                result += "null";
            else
            {
                if (currentObject is string || currentObject is String)
                    result += string.Format("\"{0}\"", ValidateString(currentObject.ToString()));
                else if (currentObject is IDictionary)
                    result += string.Format("{0}", GenerateJsonObject((Dictionary<string, object>)currentObject));
                else if (currentObject is IList)
                    result += string.Format("{0}", GenerateJsonArray((List<object>)currentObject));
                else
                    result += string.Format("{0}", currentObject.ToString());
                if (i < listObject.Count - 1)
                    result += ",\n";
            }
        }
        return "[\n" + result + "\n]";
    }

    public static string ValidateString(string descriptionString)
    {
        string updatedString = "";
        for (int i = 0; i < descriptionString.Length; i++)
        {
            bool isSpecialChar = false;
            for (int j = 0; j < specialCharV1.Length; j++)
            {
                if (descriptionString[i] == specialCharV1[j])
                {
                    updatedString += '\\';
                    updatedString += specialCharV1[j];
                    isSpecialChar = true;
                    break;
                }
            }
            for (int j = 0; j < specialCharV2.Length; j++)
            {
                if (descriptionString[i] == specialCharV2[j])
                {
                    updatedString += '\\';
                    updatedString += replaceChar[j];
                    isSpecialChar = true;
                    break;
                }
            }
            if (!isSpecialChar) updatedString += descriptionString[i];
        }
        return updatedString;
    }
}
