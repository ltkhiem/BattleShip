using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System.Threading;

public class GameStateManager : MonoBehaviour {

    internal Stream s { get; set; }
    StreamWriter sw;
    StreamReader sr;
    internal TcpClient client { get; set; }
    OpeningNavigator navigatorOpening;
    SetupNavigator setupNavigator;
    internal string playerName;
    internal int playerOrder;
    internal string opponentPlayerName;
    internal int tableSize;
    internal int gameStateManagerId;
    int totalScore = 0;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        navigatorOpening = GameObject.Find("OpeningNavigator").GetComponent<OpeningNavigator>();
        string ip = System.IO.File.ReadAllText(@"ipconfig.txt");
        Debug.Log(ip);
        client = new TcpClient(ip, 8888);
        try
        {
            s = client.GetStream();
            sr = new StreamReader(s);
            sw = new StreamWriter(s);
            sw.AutoFlush = true;
        }
        catch {
            Debug.Log("Error");
        }

    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    bool CheckCrash(Dictionary<string, object> response)
    {
        Debug.Log("Header " + response["header"]);
        return (response["header"].ToString() == "Crashed");
    }

    internal void SendPlayerName(string playerName)
    {
        this.playerName = playerName;
        List<KeyValuePair<string, string>> cont = new List<KeyValuePair<string, string>>();
        cont.Add(new KeyValuePair<string, string>("playerName", playerName));
        string request = GenerateClientRequest("CheckPlayerName", cont);
        Debug.Log("adasdas");
        SendRequest(request);
        Debug.Log("Sent");
        Thread t = new Thread(ResponsePlayerNameCheck);
        t.Start();
        //t.Join();
    }

    internal void ResponsePlayerNameCheck()
    {
        string response = ReceiveResponse();
        Dictionary<string, object> responseCont = AnalyzeServerResponse(response);
        bool validPlayerName = bool.Parse(responseCont["result"].ToString());
        Debug.Log(validPlayerName);
        navigatorOpening.DisplayCheckNameResult(validPlayerName);
        if (!validPlayerName)
            this.playerName = "";
    }

    // ----------------- Matching ------------------

    internal void SendMatching()
    {
        List<KeyValuePair<string, string>> cont = new List<KeyValuePair<string, string>>();
        cont.Add(new KeyValuePair<string, string>("playerName", playerName));
        string request = GenerateClientRequest("MatchOpponent", cont);
        SendRequest(request);
        Thread t = new Thread(ResponseMatching);
        t.Start();
    }

    internal void ResponseMatching()
    {
        string response = ReceiveResponse();
        Dictionary<string, object> responseCont = AnalyzeServerResponse(response);
        string player1 = responseCont["player1"].ToString();
        string player2 = responseCont["player2"].ToString();
        this.tableSize = int.Parse(responseCont["tableSize"].ToString());
        this.gameStateManagerId = int.Parse(responseCont["gameStateManagerId"].ToString());
        if (player1 == playerName)
        {
            playerOrder = 1;
            opponentPlayerName = player2;
        }
        else
        {
            playerOrder = 2;
            opponentPlayerName = player1;
        }
        navigatorOpening.DisplayMatchingResult();
    }

    // -------------------------- Setup Ship ----------------------------

    internal void SendReadyToPlay(string battleMap)
    {
        List<KeyValuePair<string, string>> cont = new List<KeyValuePair<string, string>>();
        cont.Add(new KeyValuePair<string, string>("sender", playerName));
        cont.Add(new KeyValuePair<string, string>("tableSize", tableSize.ToString()));
        cont.Add(new KeyValuePair<string, string>("gameStateManagerId", gameStateManagerId.ToString()));
        cont.Add(new KeyValuePair<string, string>("battleMap", battleMap.ToString()));
        string request = GenerateClientRequest("ReadyToPlay", cont);
        SendRequest(request);
        //init setupNavigator here since the object is only accessable on the current scene
        setupNavigator = GameObject.Find("SetupNavigator").GetComponent<SetupNavigator>();
        Thread t = new Thread(ResponseReadyToPlay);
        t.Start();
    }

    internal void ResponseReadyToPlay()
    {
        string response = ReceiveResponse();
        Dictionary<string, object> responseCont = AnalyzeServerResponse(response);
        if (CheckCrash(responseCont))
        {
            setupNavigator.gameCrashed();
            return;
        }
        string headerState = responseCont["header"].ToString();
        if (headerState == "ReadyToPlay")
            setupNavigator.ReceiveGameStartSignal();
    }

    // ---------------------------- Shooting ----------------------------
    internal void SendPlayerMove(int x, int y)
    {
        List<KeyValuePair<string, string>> cont = new List<KeyValuePair<string, string>>();
        cont.Add(new KeyValuePair<string, string>("sender", playerName));
        cont.Add(new KeyValuePair<string, string>("gameStateManagerId", gameStateManagerId.ToString()));
        cont.Add(new KeyValuePair<string, string>("x", x.ToString()));
        cont.Add(new KeyValuePair<string, string>("y", y.ToString()));
        string request = GenerateClientRequest("UpdateBattleInfo", cont);
        SendRequest(request);
        Thread t = new Thread(ResponsePlayerMove);
        t.Start();
    }

    void ResponsePlayerMove()
    {
        string response = ReceiveResponse();
        Debug.Log(response);
        Dictionary<string, object> responseCont = AnalyzeServerResponse(response);
        if (CheckCrash(responseCont))
        {
            setupNavigator.gameCrashed();
            return;
        }
        int score = int.Parse(responseCont["score"].ToString());
        bool endGame = bool.Parse(responseCont["endGame"].ToString());
        int win = -1;
        int position = int.Parse(responseCont["position"].ToString());
        int x = position / tableSize;
        int y = position % tableSize;
        if (endGame)
        {
            string winner = responseCont["winner"].ToString();
            if (winner == playerName) win = 1;
            else win = 0;
            setupNavigator.ReceiveEndGameSignal(win);
        }
        Debug.Log("demmmmm");

        if (score > 0)
        {
            int orientation = -1;
            int shipId = -1;
            bool destroyed = bool.Parse(responseCont["destroyed"].ToString());
            Vector2 shipCenterPos = new Vector2();
            if (destroyed)
            {
                orientation = int.Parse(responseCont["orientation"].ToString());
                shipId = int.Parse(responseCont["shipId"].ToString()) - 1;
                int shipCenter = int.Parse(responseCont["shipCenter"].ToString());
                shipCenterPos = new Vector2(shipCenter / tableSize, shipCenter % tableSize);
            }
            this.totalScore += score;
            Debug.Log("Callingggg");
            setupNavigator.ReceiveShootResult(score, x, y, endGame, orientation, destroyed, shipId, shipCenterPos);
        }
        else
        {
            Debug.Log("Callingggg else");
            setupNavigator.ReceiveShootResult(score, x, y);
        }
    }


    // --------------------------- Receiving Damage ------------------------------
    internal void StartReceivingDamage()
    {
        Thread t = new Thread(ResponseReceivingDamage);
        t.Start();
    }

    internal void ResponseReceivingDamage()
    {
        string response = ReceiveResponse();
        Dictionary<string, object> responseCont = AnalyzeServerResponse(response);
        if (CheckCrash(responseCont))
        {
            setupNavigator.gameCrashed();
            return;
        }
        int position = int.Parse(responseCont["position"].ToString());
        int x = position / tableSize;
        int y = position % tableSize;
        int score = int.Parse(responseCont["score"].ToString());
        bool endGame = bool.Parse(responseCont["endGame"].ToString());
        int win = -1;
        if (endGame)
        {
            string winner = responseCont["winner"].ToString();
            if (winner == playerName) win = 1;
            else win = 0;
            setupNavigator.ReceiveEndGameSignal(win);
        }
        setupNavigator.ReceiveDamage(x,y,score,endGame);
    }

    // -------------------------------------------------------------------------------------------

    string GenerateClientRequest(string header, List<KeyValuePair<string, string>> cont)
    {
        JsonGenerator jsonGen = new JsonGenerator();
        jsonGen.AddJsonObject("header", header);
        foreach (KeyValuePair<string, string> kp in cont)
        {
            jsonGen.AddJsonObject(kp.Key, kp.Value);
        }
        string request = jsonGen.GenerateJson();
        return request;
    }

    void SendRequest(string request)
    {
        request += "\n$\n";
        sw.Write(request);
    }

    string ReceiveResponse()
    {
        string response = "";
        while (true)
        {
            string temp = sr.ReadLine();
            if (temp == "$") break;
            else response += temp;
        }
        return response;
    }

    Dictionary<string, object> AnalyzeServerResponse(string response)
    {
        JsonParser parser = new JsonParser(response);
        Dictionary<string, object> responseCont = parser.Parse();
        return responseCont;
    }

    //void OnApplicationQuit()
    //{
    //    string[] str = { "Quit" };
    //    System.IO.File.WriteAllLines(@"log.txt", str);
    //    Debug.Log("On Quit");
    //    s.Close();
    //    client.Close();
    //}

    void OnDestroy()
    {
        client.Close();
        s.Close();
    }
}
