using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class SetupNavigator : MonoBehaviour {

    internal GameObject[] arrShip = new GameObject[5];
    internal GameObject[] arrOppoShip = new GameObject[5];
    internal GridLayout gridLayout;
    internal OppoGridLayout oppoGridLayout;
    internal GameStateManager gameStateManager;
    GameObject opponentField, canvas, canvasResult, canvasTurn, playerField;
    Text textTurn, textResult, textResultStatus;
    Button btnHome, btnQuit;

    Queue<GameObject>[] queue = new Queue<GameObject>[2];
    int idQueue = 0;

    GameObject goCross, goSmoke, goExplosion, goWater, goBlueDot;
    Vector3 gridAnimPos = new Vector3();

    private bool canShoot;

    enum UIState { SETUPSHIP, GAMESTART, UIDONE, GAMEEND, UIFINAL };
    UIState uiState = UIState.SETUPSHIP;
    int winningState = -1;

    enum ShootTurn {PLAYERTURN, OPPONENTTURN };
    ShootTurn shootTurn;
    string curTurnText="";

    enum ShootStatus { TARGETHIT, TARGETMISS, WAITFORSHOOT };
    ShootStatus shootStatus = ShootStatus.WAITFORSHOOT;

    enum DestroyShipState { NOTYET, DESTROYONE };
    DestroyShipState destroyShipState = DestroyShipState.NOTYET;
    int destroyedShipId = -1;
    int destroyedShipOrient = 0;
    Vector2 destroyedShipPos;


    void Awake()
    {
        arrShip[0] = GameObject.Find("Mary Celeste");
        arrShip[1] = GameObject.Find("MV Joyita");
        arrShip[2] = GameObject.Find("The Kaz");
        arrShip[3] = GameObject.Find("Octavius");
        arrShip[4] = GameObject.Find("Carol Deering");

        gridLayout = GameObject.Find("Grid System").GetComponent<GridLayout>();
        oppoGridLayout = GameObject.Find("Grid System Opponent").GetComponent<OppoGridLayout>();

        gameStateManager = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();
        opponentField = GameObject.Find("Opponent Field");
        playerField = GameObject.Find("Player Field");
        canvas = GameObject.Find("Canvas");
        canvasResult = GameObject.Find("Canvas Result");
        canvasTurn = GameObject.Find("Canvas Turn");
        textResult = GameObject.Find("Text Result").GetComponent<Text>();
        textResultStatus = GameObject.Find("Text Result Status").GetComponent<Text>();

        goCross = GameObject.Find("Cross");
        goSmoke = GameObject.Find("Smoke");
        goExplosion = GameObject.Find("Explosion");
        goWater = GameObject.Find("Water");
        goBlueDot = GameObject.Find("BlueDot");

        textTurn = GameObject.Find("Text Turn").GetComponent<Text>();

    }

    // Use this for initialization
    void Start () {
        opponentField.SetActive(false);
        if (gameStateManager.playerOrder == 1)
            shootTurn = ShootTurn.PLAYERTURN;
        else shootTurn = ShootTurn.OPPONENTTURN;

        //Transform animation size to fit cell size
        goCross.transform.localScale = GetTransformScale(goCross, 0.9f);
        goExplosion.transform.localScale = GetTransformScale(goExplosion, 2.0f);
        goSmoke.transform.localScale = GetTransformScale(goSmoke, 1.0f);
        goWater.transform.localScale = GetTransformScale(goWater, 2.0f);
        goBlueDot.transform.localScale = GetTransformScale(goBlueDot, 0.5f);
        for (int i = 0; i < 2; i++)
            queue[i] = new Queue<GameObject>();

        canvasResult.SetActive(false);
        canvasTurn.SetActive(false);
    }

    private Vector2 GetTransformScale(GameObject obj, float factor)
    {
        Vector2 newScale = new Vector2();
        newScale.x = (factor * (gridLayout.gridSize.x / gridLayout.rows)) / obj.GetComponent<SpriteRenderer>().bounds.size.x;
        newScale.y = (factor * (gridLayout.gridSize.y / gridLayout.cols)) / obj.GetComponent<SpriteRenderer>().bounds.size.y;
        return newScale;
    }

    internal void PostShipSetup()
    {
        for (int i = 0; i < 5; i++)
            arrShip[i].GetComponent<Collider2D>().enabled = false;

        string battleMap = GenerateBattleMap();
        gameStateManager.SendReadyToPlay(battleMap);
    }

    internal void ReceiveGameStartSignal()
    {
        uiState = UIState.GAMESTART;
    }

    private string GenerateBattleMap()
    {
        int[][] arrMap = new int[10][];
        for (int i = 0; i < 10; i++)
            arrMap[i] = new int[10];

        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
                arrMap[i][j] = 0;

        for (int i=0; i<5; i++)
        {
            ShipInfo ship = arrShip[i].GetComponent<ShipInfo>();
            for (int r = (int) ship.startPos.x; r<= (int) ship.endPos.x; r++)    
                for (int c = (int) ship.startPos.y; c <= (int) ship.endPos.y; c++)
                    arrMap[r][c] = i + 1;
        }

        string res = "";
        for (int i = 0; i < gameStateManager.tableSize; i++)
            for (int j = 0; j < gameStateManager.tableSize; j++)
                res += arrMap[i][j].ToString();
        return res;
    }

    void SwitchTurn()
    {
        shootTurn = 1 - shootTurn;
        Debug.Log("Switch to " + shootTurn);
        if (shootTurn == ShootTurn.PLAYERTURN)
        {
            curTurnText = "YOUR TURN";
            canShoot = true;
        }
        else
        {
            curTurnText = "OPPONENT'S TURN";
            canShoot = false;
        }
    }

    internal void SendShoot(int x, int y)
    {
        Debug.Log("can shoot " + canShoot);
        if (canShoot)
        {
            gameStateManager.SendPlayerMove(x, y);
            //Diable click
            oppoGridLayout.Grid[x - 1][y - 1].GetComponent<BoxCollider2D>().enabled = false;
        }
        canShoot = false;
    }

    internal void ReceiveShootResult(int score, int x = -1, int y = -1, bool endGame = false,  int orientation =-1, bool destroyed = false, int shipId = -1, Vector2 shipCenter = new Vector2())
    {
        Debug.Log("Receiveeeeeeeeeeeeeeeeeee");
        Debug.Log("score " + score);

        gridAnimPos = new Vector3(x, y, (float) shootTurn);

        if (score > 0)
        {
            shootStatus = ShootStatus.TARGETHIT;
            if (destroyed)
            {
                destroyShipState = DestroyShipState.DESTROYONE;
                destroyedShipId = shipId;
                destroyedShipOrient = orientation;
                destroyedShipPos = shipCenter;
            }
        }
        else shootStatus = ShootStatus.TARGETMISS;

        if (!endGame)
        {
            PostReceiveDamage();
            SwitchTurn();
        }
    }

    internal void ReceiveEndGameSignal(int winningState)
    {
        this.winningState = winningState;
    }

    void PostReceiveDamage()
    {
        gameStateManager.StartReceivingDamage();
    }

    internal void ReceiveDamage(int x, int y, int score, bool endGame)
    {
        if (score > 0)
            shootStatus = ShootStatus.TARGETHIT;
        else shootStatus = ShootStatus.TARGETMISS;
        gridAnimPos = new Vector3(x, y, (float) shootTurn);

        if (!endGame)
        {
            SwitchTurn();
        }
    }

    internal void gameCrashed()
    {
        Debug.Log("Crashed !!!");
        this.winningState = 3;
        uiState = UIState.GAMEEND;
    }

    // Update is called once per frame
    void Update () {

        //Update for UI and Start the TURN for SHOOTING
        if (uiState == UIState.GAMESTART)
        {
            canvas.SetActive(false);
            opponentField.SetActive(true);
            uiState = UIState.UIDONE;
            for (int i = 0; i < 5; i++)
                arrOppoShip[i] = Instantiate(arrShip[i], arrShip[i].transform.position, Quaternion.identity) as GameObject;

            //START SHOOTING HERE
            canvasTurn.SetActive(true); 
            if (shootTurn == ShootTurn.PLAYERTURN)
            {
                Debug.Log("update turn ne");
                curTurnText = "YOUR TURN";
                canShoot = true;
            }
            else
            {
                Debug.Log("update turn ne");
                curTurnText = "OPPONENT'S TURN";
                canShoot = false;
                PostReceiveDamage();
            }
        }
        else if (uiState == UIState.UIDONE)     //Game is already starting
        {
            //Update Turn Status
            textTurn.text = curTurnText;

            //Get animation position
            Vector3 animPos = new Vector3();
            if (shootStatus != ShootStatus.WAITFORSHOOT)
            {
                if (gridAnimPos.z == (float) ShootTurn.PLAYERTURN)
                    animPos = oppoGridLayout.Grid[(int) gridAnimPos.x][(int) gridAnimPos.y].transform.position;
                else
                    animPos = gridLayout.Grid[(int)gridAnimPos.x][(int)gridAnimPos.y].transform.position;

                Debug.Log("Animation Pos " + animPos);
                Debug.Log("Animation Grid Pos " + gridAnimPos);

                //Animation for shoot
                GameObject animObject = null;
                if (shootStatus == ShootStatus.TARGETHIT)
                {
                    animObject = Instantiate(goExplosion, animPos, Quaternion.identity) as GameObject;
                    shootStatus = ShootStatus.WAITFORSHOOT;
                    queue[idQueue].Enqueue(animObject);
                }
                else if (shootStatus == ShootStatus.TARGETMISS)
                {
                    animObject = Instantiate(goWater, animPos, Quaternion.identity) as GameObject;
                    shootStatus = ShootStatus.WAITFORSHOOT;
                    queue[idQueue].Enqueue(animObject);
                }
            }
            
            //Animation Playing Check
            while (queue[idQueue].Count > 0)
            {
                GameObject animObject = queue[idQueue].Dequeue();
                if (animObject != null)
                {
                    if (animObject.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).tagHash == Animator.StringToHash("EndFire"))
                    {
                        GameObject g = Instantiate(goCross, animObject.transform.position, Quaternion.identity) as GameObject;
                        GameObject h = Instantiate(goSmoke, animObject.transform.position, Quaternion.identity) as GameObject;
                        h.transform.localEulerAngles = new Vector3(0, 0, -45);
                        Destroy(animObject);
                        animObject = null;
                    }
                    else if (animObject.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).tagHash == Animator.StringToHash("EndWater"))
                    {
                        GameObject g = Instantiate(goBlueDot, animObject.transform.position, Quaternion.identity) as GameObject;
                        Destroy(animObject);
                        animObject = null;
                    }
                    if (animObject != null) queue[1 - idQueue].Enqueue(animObject);
                    else
                        if (winningState!=-1) uiState = UIState.GAMEEND;
                }
            }
            idQueue = 1 - idQueue;


            //Display ship if it is destroyed
            if (destroyShipState == DestroyShipState.DESTROYONE)
            {
                //Change rotation
                if (destroyedShipOrient == 1)
                    arrOppoShip[destroyedShipId].transform.localEulerAngles = new Vector3(0, 0, -90);
                else arrOppoShip[destroyedShipId].transform.localEulerAngles = new Vector3(0, 0, 0);

                arrOppoShip[destroyedShipId].transform.localScale = arrShip[destroyedShipId].transform.localScale;

                //Transform ship to the position
                GameObject shipCell = oppoGridLayout.Grid[(int)destroyedShipPos.x][(int)destroyedShipPos.y];
                Vector2 cellPos = shipCell.transform.position;
                Debug.Log("cell Pos " + cellPos);
                Vector2 offsetShip = new Vector2((1 - destroyedShipOrient) * (arrOppoShip[destroyedShipId].GetComponent<SpriteRenderer>().bounds.size.x / 2 - shipCell.GetComponent<SpriteRenderer>().bounds.size.x / 2),
                                                    destroyedShipOrient * (-arrOppoShip[destroyedShipId].GetComponent<SpriteRenderer>().bounds.size.y / 2 + shipCell.GetComponent<SpriteRenderer>().bounds.size.y / 2));
                arrOppoShip[destroyedShipId].transform.position = cellPos + offsetShip;
                arrOppoShip[destroyedShipId].transform.SetParent(arrShip[destroyedShipId].transform.parent.transform, true);
                destroyShipState = DestroyShipState.NOTYET;
            }

        }
        else if (uiState == UIState.GAMEEND)
        {
            if (winningState == 1)
            {
                textResult.text = "YOU WIN";
                textResultStatus.text = "You destroyed all opponent's ships";
            }
            else if (winningState == 0)
            {
                textResult.text = "YOU LOSE";
                textResultStatus.text = "Opponent destroyed all your ships";
            }
            else
            {
                textResult.text = "YOU WIN";
                textResultStatus.text = "Opponent has surrendered";
            }
            canvasResult.SetActive(true);
            canvas.SetActive(false);
            uiState = UIState.UIFINAL;
        }
    }

    void Logger(Transform t)
    {
        Debug.Log("-------------------------");
        Debug.Log("Position: " + t.position);
        Debug.Log("Rotation: " + t.rotation);
        Debug.Log("Scale: " + t.localScale);
    }
}
