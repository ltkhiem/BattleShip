using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JsonArrayParser : JsonParser {

    List<object> listValue;
    JsonArrayParser()
    {

    }
    public JsonArrayParser(string jsonContent) : base(jsonContent)
    {
        listValue = new List<object>();
    }
    new public List<object> Parse()
    {
        try
        {
            MatchToken('[');
            while (currentToken != ']')
            {
                ParseElements();
                if (ignoreCharacter.Contains(currentToken)) currentToken = NextToken();
                while (currentToken == ',')
                {
                    MatchToken(',');
                    ParseElements();
                    if (ignoreCharacter.Contains(currentToken)) currentToken = NextToken();
                }
            }
        }
        catch
        {
            listValue = null;
        }
        return listValue;
    }

    private void ParseElements()
    {
        try
        {
            object value = ParseValue();
            listValue.Add(value);
        }
        catch
        {
            throw;
        }
    }
}
