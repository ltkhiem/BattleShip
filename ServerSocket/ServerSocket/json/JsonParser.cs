using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ServerSocket.json
{
    class JsonParser
    {
        protected char[] ignoreCharacter = { ' ', '\n', '\t', '\r' };
        protected int currentPosition;
        public int finalPosition { get; set; }
        protected char currentToken = '\0';
        protected string jsonContent;
        private Dictionary<string, object> result = null;

        public JsonParser()
        {
            result = new Dictionary<string, object>();
        }

        public JsonParser(string jsonContent)
        {
            if (jsonContent == null)
                return;
            this.jsonContent = jsonContent;
            currentPosition = 0;
            currentToken = jsonContent[currentPosition];
            result = new Dictionary<string, object>();
        }

        protected void MatchToken(char token)
        {
            if (currentToken == token)
                currentToken = NextToken();
            else
                throw new System.ArgumentException("Current token is not matched", "token");
        }

        protected char NextToken()
        {
            char token = currentToken;
            if (currentPosition + 1 < jsonContent.Length)
                token = jsonContent[++currentPosition];
            else
            {
                token = '\0';
                return token;
            }
            while (ignoreCharacter.Contains(token))
                if (currentPosition + 1 < jsonContent.Length)
                    token = jsonContent[++currentPosition];
                else break;
            if (ignoreCharacter.Contains(token))
                token = '\0';
            return token;
        }

        protected object ParseValue()
        {
            object value = null;
            try
            {
                switch (currentToken)
                {
                    case '\"':
                        value = ParseString();
                        break;
                    case 'T':
                    case 't':
                    case 'f':
                    case 'F':
                        value = ParseBoolean();
                        break;
                    case 'n':
                    case 'N':
                        value = ParseNull();
                        break;
                    case '{':
                        value = ParseJsonObject();
                        break;
                    case '[':
                        value = ParseJsonArray();
                        break;
                    default:
                        value = ParseNumber();
                        break;
                }
            }
            catch
            {
                throw;
            }
            return value;
        }

        protected object ParseNumber()
        {
            string result = "";
            while (currentPosition < jsonContent.Length &&
                  (currentToken >= '0' && currentToken <= '9') || currentToken == '.' ||
                   currentToken == '+' || currentToken == '-' || currentToken == 'e')
            {
                result += currentToken;
                currentToken = jsonContent[++currentPosition];
            }
            try
            {
                if (result.Count(c => c == '.') > 0)
                    return Double.Parse(result);
                return long.Parse(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Invalid Number Cast: {ex}");
                throw;
            }
        }

        protected object ParseNull()
        {
            try
            {
                string nullStr = (currentToken == 'n' ? "null" : "Null");
                for (int i = 0; i < nullStr.Length; i++)
                    MatchToken(nullStr[i]);
                return null;
            }
            catch
            {
                throw;
            }
        }

        protected bool ParseBoolean()
        {
            try
            {
                string boolStr = (currentToken == 't' ? "true" : "false");
                if (currentToken == 'T' || currentToken == 'F')
                    boolStr = (currentToken == 'T' ? "True" : "False");
                for (int i = 0; i < boolStr.Length; i++)
                    MatchToken(boolStr[i]);
                return Boolean.Parse(boolStr);
            }
            catch
            {
                throw;
            }
        }
        protected string ParseString()
        {
            try
            {
                String result = "";
                MatchToken('\"');
                while (currentPosition < jsonContent.Length &&
                      (currentToken = jsonContent[currentPosition]) != '\"')
                {
                    if (jsonContent[currentPosition] == '\\') currentPosition++;
                    result += jsonContent[currentPosition++];
                }
                MatchToken('\"');
                return result;
            }
            catch
            {
                throw;
            }
        }

        protected object ParseJsonArray()
        {
            try
            {
                JsonArrayParser arrParser = new JsonArrayParser(jsonContent.Substring(currentPosition));
                object result = arrParser.Parse();
                currentToken = jsonContent[currentPosition += arrParser.currentPosition];
                MatchToken(']');
                return result;
            }
            catch
            {
                throw;
            }
        }

        protected object ParseJsonObject()
        {
            try
            {
                JsonObjectParser objParser = new JsonObjectParser(jsonContent.Substring(currentPosition));
                object result = objParser.Parse();
                currentToken = jsonContent[currentPosition += objParser.currentPosition];
                MatchToken('}');
                return result;
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<string, object> Parse()
        {
            try
            {
                object parseResult = new object();
                switch (currentToken)
                {
                    case '{':
                        parseResult = ParseJsonObject();
                        break;
                    case '[': //! It can never access here because we are assuming that jsonContent is always a jsonObject
                        parseResult = ParseJsonArray();
                        break;
                    default:
                        break;
                }
                if (currentToken != '\0') return result;
                else
                {
                    //Assume that the parsed json content is an object
                    Dictionary<string, object> listObject = (Dictionary<string, object>)parseResult;
                    foreach (var item in listObject)
                    {
                        KeyValuePair<string, object> jsonObject = item;
                        AddJsonObject(jsonObject.Key, jsonObject.Value);
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return result;
            }
        }
        public void AddJsonObject(string name, object value)
        {
            result.Add(name, value);
        }
        public void SetJsonContent(string jsonContent)
        {
            this.jsonContent = jsonContent;
            currentPosition = 0;
            currentToken = jsonContent[currentPosition];
        }
    }
}
