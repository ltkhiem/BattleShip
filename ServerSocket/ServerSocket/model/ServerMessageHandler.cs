using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ServerSocket.json;

namespace ServerSocket.model
{
    class ServerMessageHandler
    {
        internal string header { get; set; }
        internal List<KeyValuePair<string, string>> cont { get; set; }
        internal StreamWriter swSender { get; set; }
        internal StreamWriter swReceiver { get; set; }


        internal ServerMessageHandler()
        {
            cont = new List<KeyValuePair<string, string>>();
            swSender = swReceiver = null;
        }
        internal ServerMessageHandler(string header, List<KeyValuePair<string, string>> cont, StreamWriter sw = null, StreamWriter sw2 = null)
        {
            this.header = header;
            this.cont = cont;
            this.swReceiver = sw;
            this.swSender = sw2;
        }

        public void ProcessServerMessage()
        {
            try
            {
                switch (header)
                {
                    case "CheckPlayerName":
                    case "MatchOpponentResponse":
                    case "ReadyToPlay":
                    case "UpdateBattleInfo":
                    case "Crashed":
                        if (this.swReceiver != null) SendServerMessage(this.swReceiver);
                        if (this.swSender != null) SendServerMessage(this.swSender);
                        break;
                }
            }
            catch { }
        }

        void SendServerMessage(StreamWriter sw)
        {
            JsonGenerator jsonGen = new JsonGenerator();
            jsonGen.AddJsonObject("header", header);
            foreach (KeyValuePair<string, string> kp in cont)
            {
                jsonGen.AddJsonObject(kp.Key, kp.Value);
            }
            sw.Write(jsonGen.GenerateJson() + "\n$\n");
            Console.WriteLine("Send: " + jsonGen.GenerateJson());
        }
    }
}
