using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomButtonClick : MonoBehaviour {

    GameObject[] arrShip = new GameObject[5];
    int[] arrShipLength = { 1, 3, 3, 4, 4 };
    int[] arrShipOrient = new int[5];
    Vector2[] arrShipStart = new Vector2[5];
    Vector2[] arrShipEnd = new Vector2[5];
    int[][] map;

    Button btnRandom;
    GameStateManager gameStateManager;
    CellClick cellClick = new CellClick();
    GridLayout gridLayout;

    // Use this for initialization
    void Start() {
        btnRandom = GameObject.Find("Button Random").GetComponent<Button>();
        btnRandom.onClick.AddListener(OnClickRandom);

        gameStateManager = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();

        map = new int[gameStateManager.tableSize][];
        for (int i = 0; i < gameStateManager.tableSize; i++)
            map[i] = new int[gameStateManager.tableSize];

        arrShip[0] = GameObject.Find("The Kaz");
        arrShip[1] = GameObject.Find("MV Joyita");
        arrShip[2] = GameObject.Find("Octavius");
        arrShip[3] = GameObject.Find("Mary Celeste");
        arrShip[4] = GameObject.Find("Carol Deering");

        gridLayout = GameObject.Find("Grid System").GetComponent<GridLayout>();

        InitMap();
    }

    private void InitMap()
    {
        for (int i = 0; i < gameStateManager.tableSize; i++)
            for (int j = 0; j < gameStateManager.tableSize; j++)
                map[i][j] = -1;
    }

    void SetGridData(Vector2 startPos, Vector2 endPos, bool occupyVal, GameObject refShip)
    {
        for (int i = (int)startPos.x; i <= (int)endPos.x; i++)
            for (int j = (int)startPos.y; j <= (int)endPos.y; j++)
            {
                CellInfo c = gridLayout.Grid[i][j].GetComponent<CellInfo>();
                c.hasShip = occupyVal;
                c.occupiedShip = refShip;
            }
    }

    void ResetGridData()
    {
        for (int i = 0; i < gameStateManager.tableSize; i++)
            for (int j = 0; j < gameStateManager.tableSize; j++)
            {
                CellInfo c = gridLayout.Grid[i][j].GetComponent<CellInfo>();
                c.hasShip = false;
                c.occupiedShip = null;
            }
    }

    void OnClickRandom()
    {
        InitMap();
        ResetGridData();
        GameObject.Find("Event System").GetComponent<MouseFollowHandler>().followingSprite = null;
        GenerateMap();
        for (int i=0; i<5; i++)
        {
            ShipInfo ship = arrShip[i].GetComponent<ShipInfo>();
            ship.startPos = arrShipStart[i];
            ship.endPos = arrShipEnd[i];
            ship.isDeployed = true;
            ship.orientation = arrShipOrient[i];

            if (ship.orientation == 1)
                arrShip[i].transform.localEulerAngles = new Vector3(0, 0, -90);
            else arrShip[i].transform.localEulerAngles = new Vector3(0, 0, 0);

            SetGridData(ship.startPos, ship.endPos, true, arrShip[i]);

            GameObject cell = gridLayout.Grid[(int)ship.startPos.x][(int)ship.startPos.y];
            Vector2 cellPos = cell.transform.position;
            Vector2 offsetShip = new Vector2((1 - ship.orientation) * (arrShip[i].GetComponent<SpriteRenderer>().bounds.size.x / 2 - cell.GetComponent<SpriteRenderer>().bounds.size.x / 2),
                                                ship.orientation * (-arrShip[i].GetComponent<SpriteRenderer>().bounds.size.y / 2 + cell.GetComponent<SpriteRenderer>().bounds.size.y / 2));
            arrShip[i].transform.position = cellPos + offsetShip;
            arrShip[i].GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    private void GenerateMap()
    {
        System.Random r = new System.Random();
        int countShip = 0;

        while (countShip < 5)
        {
            bool isDeployed = true;
            int x = r.Next(0, gameStateManager.tableSize);
            int y = r.Next(0, gameStateManager.tableSize);
            if (map[x][y] != -1) continue;
            int o = r.Next(0, 2);
            Vector2 shipEnd = new Vector2(x + o * (arrShipLength[countShip] - 1), y + (1 - o) * (arrShipLength[countShip] - 1));
            if (0 <= shipEnd.x && shipEnd.x < gameStateManager.tableSize && 0 <= shipEnd.y && shipEnd.y < gameStateManager.tableSize)
            {
                for (int i = x; i <= shipEnd.x; i++)
                {
                    for (int j = y; j <= shipEnd.y; j++)
                        if (map[i][j] != -1)
                        {
                            isDeployed = false;
                            break;
                        }
                    if (!isDeployed) break;
                }

                if (isDeployed)
                {
                    for (int i = x; i <= shipEnd.x; i++)
                        for (int j = y; j <= shipEnd.y; j++)
                            map[i][j] = countShip;
                    arrShipStart[countShip] = new Vector2(x, y);
                    arrShipEnd[countShip] = shipEnd;
                    arrShipOrient[countShip] = o;
                    countShip++;
                }
            }
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}

