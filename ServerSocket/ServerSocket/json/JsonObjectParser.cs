using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSocket.json
{
    class JsonObjectParser : JsonParser
    {
        Dictionary<string, object> listObject;

        JsonObjectParser()
        {

        }

        public JsonObjectParser(string jsonContent) : base(jsonContent)
        {
            listObject = new Dictionary<string, object>();
        }

        new public Dictionary<string, object> Parse()
        {
            try
            {
                MatchToken('{');
                while (currentToken != '}')
                {
                    ParsePair();
                    if (ignoreCharacter.Contains(currentToken)) currentToken = NextToken();
                    while (currentToken == ',')
                    {
                        MatchToken(',');
                        ParsePair();
                        if (ignoreCharacter.Contains(currentToken)) currentToken = NextToken();
                    }
                }
            }
            catch
            {
                listObject = null;
            }
            return listObject;
        }

        private string ParseName()
        {
            return ParseString();
        }

        private void ParsePair()
        {
            try
            {
                string key;
                object value;
                key = ParseName();
                if (ignoreCharacter.Contains(currentToken)) currentToken = NextToken();
                MatchToken(':');
                value = ParseValue();
                listObject.Add(key, value);
            }
            catch
            {
                throw;
            }
        }
    }
}
