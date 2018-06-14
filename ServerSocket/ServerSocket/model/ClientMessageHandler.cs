using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ServerSocket.json;
using System.IO;
using System.Text.RegularExpressions;

namespace ServerSocket.model
{
    class ClientMessageHandler
    {
        Dictionary<string, object> recv;
        object locker = new object();
        StreamWriter swSender;
        public ClientMessageHandler(Dictionary<string, object> recvCont, StreamWriter sw)
        {
            recv = recvCont;
            swSender = sw;
        }

        public ServerMessageHandler ProcessClientMessage()
        {
            string header = "error";
            List<KeyValuePair<string, string>> cont = new List<KeyValuePair<string, string>>();
            StreamWriter swReceiver = null;
            try
            {
                string clientHeader = recv["header"].ToString();
                switch (clientHeader){
                    case "CheckPlayerName":
                        lock (locker)
                        {
                            string content = CheckPlayerNameExist().ToString();
                            cont.Add(new KeyValuePair<string, string>("result", content));
                        }
                        break;
                    case "MatchOpponent":
                        lock (locker)
                        {
                            AddPlayerToWaitingList();
                        }
                        break;
                    case "ReadyToPlay":
                        swReceiver = ReadyToPlay();
                        break;
                    case "UpdateBattleInfo":
                        swReceiver = UpdateBattleInfo();
                        InheritAttributeFromRecv(ref cont);
                        break;
                }
                header = recv["header"].ToString();
            }
            catch { }
            return new ServerMessageHandler(header, cont, swReceiver, swSender);
        }

        void InheritAttributeFromRecv(ref List<KeyValuePair<string, string>> cont)
        {
            foreach (KeyValuePair<string, object> kp in recv)
            {
                if (kp.Key != "header") cont.Add(new KeyValuePair<string, string>(kp.Key, kp.Value.ToString()));
            }
        }

        StreamWriter UpdateBattleInfo()
        {
            try
            {
                //Analyze player move
                int gameStateId = -1, x = -1, y = -1;
                int.TryParse(recv["gameStateManagerId"].ToString(), out gameStateId);
                string playerName = recv["sender"].ToString();
                GameStateManager gameStateManager = Program.activeGameState[gameStateId];
                string opponentPlayerName = gameStateManager.GetOpponentPlayerName(playerName);
                int.TryParse(recv["x"].ToString(), out x);
                int.TryParse(recv["y"].ToString(), out y);
                Dictionary<string, string> result = gameStateManager.ProcessPlayerMove(playerName, x, y);
                bool endGame = false;
                bool.TryParse(result["endGame"], out endGame);
                bool disConnected = false;
                //Check if any player disconnected
                if (gameStateManager.Crashed())
                {
                    disConnected = true;
                    ResetGameState(gameStateId);
                }
                //Check if the game ended to send message to both players
                if (endGame)
                {
                    string winner = gameStateManager.GetWinner();
                    string loser = gameStateManager.GetLoser();
                    result.Add("winner", winner);
                    result.Add("loser", loser);
                    ResetGameState(gameStateId);
                }
                string clientHeader = recv["header"].ToString();
                recv.Clear();
                //Console.WriteLine("Disconnected: " + disConnected);
                if (disConnected) recv.Add("header", "Crashed");
                else
                {
                    recv.Add("header", clientHeader);
                    foreach (KeyValuePair<string, string> kp in result) recv.Add(kp.Key, kp.Value);
                }
                int threadId = -1;
                StreamWriter targetStreamWriter = null;
                if (Program.playerNameList.ContainsKey(opponentPlayerName))
                {
                    threadId = Program.playerNameList[opponentPlayerName];
                    targetStreamWriter = Program.activeThread[threadId];
                }
                return targetStreamWriter;
            }
            catch(Exception e)
            {
                //Raise error to user
                Console.WriteLine(e.ToString());
            }
            return null;
        }

        void ResetGameState(int gameStateId)
        {
            lock (locker)
            {
                //Free game state id
                Program.activeGameState.Remove(gameStateId);
                Program.freeGameStateManagerId.Enqueue(gameStateId);
            }
        }
        
        StreamWriter ReadyToPlay()
        {
            try
            {
                //Update battle ship map
                string battleShipMap = recv["battleMap"].ToString();
                string playerName = recv["sender"].ToString();
                int tableSize = 0, gameStateManagerId = -1;
                int.TryParse(recv["tableSize"].ToString(), out tableSize);
                int.TryParse(recv["gameStateManagerId"].ToString(), out gameStateManagerId);
                GameStateManager gameStateManager = Program.activeGameState[gameStateManagerId];
                gameStateManager.SetPlayerShip(playerName, battleShipMap, tableSize);
                //Get Receiver
                string receiver = gameStateManager.GetOpponentPlayerName(playerName);
                int threadId = -1;
                StreamWriter targetStreamWriter = null;
                if (Program.playerNameList.ContainsKey(receiver))
                {
                    threadId = Program.playerNameList[receiver];
                    targetStreamWriter = Program.activeThread[threadId];
                }
                while (!gameStateManager.StartGame() && !gameStateManager.Crashed()) ;
                if (!gameStateManager.Crashed()) swSender = null;
                else
                {
                    recv.Clear();
                    recv.Add("header", "Crashed");
                    ResetGameState(gameStateManagerId);
                }
                return targetStreamWriter;
            }
            catch (Exception e)
            {
                //Raise error to user
                Console.WriteLine(e.ToString());
            }
            return null;
        }

        bool CheckPlayerNameExist()
        {
            try
            {
                Regex regex = new Regex(@"^[a-zA-Z0-9]+$");
                string playerName = recv["playerName"].ToString();
                bool valid = !Program.playerNameList.ContainsKey(playerName) & regex.IsMatch(playerName) & (playerName.Length <= 10);
                if (valid)
                {
                    DateTime now = DateTime.Now;
                    Program.playerNameList.Add(playerName, Thread.CurrentThread.ManagedThreadId);
                    Program.playerPriority.Add(playerName, new PlayerLoginInfo(now.Hour, now.Minute, now.Second, now.Millisecond, now.Day, now.Month, now.Year));
                }
                Console.WriteLine("Finish Check Player Name: " + playerName);
                return valid;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return false;
        }

        void AddPlayerToWaitingList()
        {
            try
            {
                string playerName = recv["playerName"].ToString();
                Program.waitingList.Add(playerName, Thread.CurrentThread.ManagedThreadId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
