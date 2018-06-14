using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using ServerSocket.json;
using ServerSocket.model;

namespace ServerSocket
{
    class Program
    {
        static object locker = new object();
        static TcpListener listener;
        static int PLAYER_LIMIT = 10;
        internal static Queue<int> freeGameStateManagerId;
        internal static Dictionary<string, int> playerNameList { get; set; }
        internal static Dictionary<string, int> waitingList { get; set; }
        internal static Dictionary<int, StreamWriter> activeThread { get; set; }
        internal static Dictionary<int, GameStateManager> activeGameState { get; set; } 
        internal static Dictionary<string, int> playerGameStateIdMapping { get; set; }
        internal static Dictionary<string, PlayerLoginInfo> playerPriority { get; set; }
        static void Main(string[] args)
        {
            listener = new TcpListener(8888);
            listener.Start();
            playerNameList = new Dictionary<string, int>();
            waitingList = new Dictionary<string, int>();
            activeThread = new Dictionary<int, StreamWriter>();
            activeGameState = new Dictionary<int, GameStateManager>();
            freeGameStateManagerId = new Queue<int>();
            playerPriority = new Dictionary<string, PlayerLoginInfo>();
            playerGameStateIdMapping = new Dictionary<string, int>();
            for (int i=0; i<PLAYER_LIMIT; i++)
            {
                Thread t = new Thread(ReceiveRequestFromClient);
                t.Start();
            }
            for (int i = 0; i < 500 / 2; i++) freeGameStateManagerId.Enqueue(i);
            Console.WriteLine("Start Server");
            while (true)
            {
                ServerMatchingPlayer();
            }
        }

        static void ServerMatchingPlayer()
        {
            if (waitingList.Count >= 2)
            {
                int player1ThreadID, player2ThreadID;
                List<KeyValuePair <string, string>> cont = MatchPlayer(out player1ThreadID, out player2ThreadID);
                StreamWriter swPlayer1 = activeThread[player1ThreadID];
                StreamWriter swPlayer2 = activeThread[player2ThreadID];
                //Console.WriteLine(player1ThreadID + " " + player2ThreadID);
                ServerMessageHandler smsg = new ServerMessageHandler("MatchOpponentResponse", cont, swPlayer1, swPlayer2);
                smsg.ProcessServerMessage();
            }
        }

        static List<KeyValuePair<string, string>> MatchPlayer(out int player1ThreadID, out int player2TheadID)
        {
            lock (locker)
            {
                string[] waitingPlayer = Program.waitingList.Keys.ToArray();
                Shuffle(waitingPlayer);
                string player1 = waitingPlayer[0];
                string player2 = waitingPlayer[1];
                PlayerLoginInfo[] priority = { playerPriority[player1], playerPriority[player2] };
                if (priority[0].Compare(priority[1]) == false)
                {
                    string temp = player1;
                    player1 = player2;
                    player2 = temp;
                }
                ////Dummy for testing
                //player1 = "a";
                //player2 = "b";
                ////*************
                Console.WriteLine("Game Queue: " + freeGameStateManagerId.Count);
                while (freeGameStateManagerId.Count == 0) ;
                int gameStateManagerId = freeGameStateManagerId.Dequeue();
                GameStateManager gameStateManager = new GameStateManager(player1, player2);
                activeGameState.Add(gameStateManagerId, gameStateManager);
                playerGameStateIdMapping.Add(player1, gameStateManagerId);
                playerGameStateIdMapping.Add(player2, gameStateManagerId);
                player1ThreadID = waitingList[player1];
                player2TheadID = waitingList[player2];
                Program.waitingList.Remove(player1);
                Program.waitingList.Remove(player2);
                int tableSize = GenerateTableSize();
                List<KeyValuePair<string, string>> listArguments = new List<KeyValuePair<string, string>>();
                listArguments.Add(new KeyValuePair<string, string>("player1", player1));
                listArguments.Add(new KeyValuePair<string, string>("player2", player2));
                listArguments.Add(new KeyValuePair<string, string>("tableSize", tableSize.ToString()));
                listArguments.Add(new KeyValuePair<string, string>("gameStateManagerId", gameStateManagerId.ToString()));
                Console.WriteLine(player1 + " " + player2);
                return listArguments; 
            }
        }

        static int GenerateTableSize()
        {
            Random r = new Random();
            int tableSize = r.Next(6, 9);
            return tableSize;
        }

        static void Shuffle<T>(T[] array)
        {
            Random _random = new Random();
            int arraySize = array.Length;
            for (int i = 0; i < arraySize; i++)
            {
                int r = i + _random.Next(arraySize - i);
                T t = array[r];
                array[r] = array[i];
                array[i] = t;
            }
        }

        static void ReceiveRequestFromClient()
        {
            while (true)
            {
                Socket soc = listener.AcceptSocket();
                Console.WriteLine("Connected: {0}", soc.RemoteEndPoint);
                try
                {
                    object locker = new object();
                    Stream s = new NetworkStream(soc);
                    StreamReader sr = new StreamReader(s);
                    StreamWriter sw = new StreamWriter(s);
                    sw.AutoFlush = true;
                    lock (locker) {
                        activeThread.Add(Thread.CurrentThread.ManagedThreadId, sw);
                    }
                    //Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                    while (soc.Connected)
                    {
                        string recv = "";
                        while (soc.Connected)
                        {
                            string temp = sr.ReadLine();
                            if (temp == "$" || temp == "" || temp == null) break;
                            else recv += temp;
                        }
                        Console.WriteLine("Receive: " + recv);
                        JsonParser parser = new JsonParser(recv);
                        ClientMessageHandler msg = new ClientMessageHandler(parser.Parse(), sw);
                        ServerMessageHandler smsg = msg.ProcessClientMessage();
                        smsg.ProcessServerMessage();
                    }
                }
                catch { }
                Console.WriteLine("Disconnected: {0}", soc.RemoteEndPoint);
                lock (locker)
                {
                    try
                    {
                        activeThread.Remove(Thread.CurrentThread.ManagedThreadId);
                        var playerName = playerNameList.FirstOrDefault(x => x.Value == Thread.CurrentThread.ManagedThreadId).Key;
                        if (playerName != null)
                        {
                            playerNameList.Remove(playerName);
                            playerPriority.Remove(playerName);
                            if (waitingList.ContainsKey(playerName)) waitingList.Remove(playerName);
                            if (playerGameStateIdMapping.ContainsKey(playerName))
                            {
                                int gameStateIdMapping = playerGameStateIdMapping[playerName];
                                playerGameStateIdMapping.Remove(playerName);
                                if (activeGameState.ContainsKey(gameStateIdMapping))
                                {
                                    GameStateManager gameStateManager = activeGameState[gameStateIdMapping];
                                    string opponentPlayerName;
                                    if (gameStateManager.player1.playerName == playerName)
                                    {
                                        gameStateManager.player1.isOnline = false;
                                        opponentPlayerName = gameStateManager.player2.playerName;
                                    }
                                    else
                                    {
                                        gameStateManager.player2.isOnline = false;
                                        opponentPlayerName = gameStateManager.player1.playerName;
                                    }
                                    if (playerNameList.ContainsKey(opponentPlayerName))
                                    {
                                        int opponentThreadId = playerNameList[opponentPlayerName];
                                        StreamWriter targetSW = activeThread[opponentThreadId];
                                        ServerMessageHandler smsg = new ServerMessageHandler();
                                        smsg.header = "Crashed";
                                        smsg.swReceiver = targetSW;
                                        smsg.ProcessServerMessage();
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
                soc.Close();
            }
        }
    }
}
