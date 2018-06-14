using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellClick : MonoBehaviour {

    GameObject EventSystem, GridSystem;
    GridLayout gridLayout;
	// Use this for initialization
	void Start () {
        EventSystem = GameObject.Find("Event System");
        GridSystem = GameObject.Find("Grid System");
        gridLayout = GridSystem.GetComponent<GridLayout>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    bool valid(int x, int y)
    {
        if (0 <= x && x < gridLayout.rows && 0 <= y && y < gridLayout.cols) //check inside
        {
            CellInfo cell = gridLayout.Grid[x][y].GetComponent<CellInfo>();
            if (!cell.hasShip) return true;
        }
        return false;
    }

    public void SetGridData(Vector2 startPos, Vector2 endPos, bool occupyVal, GameObject refShip)
    {
        for (int i = (int)startPos.x; i <= (int)endPos.x; i++)
            for (int j = (int)startPos.y; j <= (int)endPos.y; j++)
            {
                CellInfo c = gridLayout.Grid[i][j].GetComponent<CellInfo>();
                c.hasShip = occupyVal;
                c.occupiedShip = refShip;
            }
    }

    void OnMouseDown()
    {
        bool occupy = true;
        //Check condition to put ship in
        MouseFollowHandler mfhandler = EventSystem.GetComponent<MouseFollowHandler>();
        GameObject shipSprite = mfhandler.followingSprite;
        if (shipSprite != null)
        {
            //Checking before deploying ship into the selected cell
            
            ShipInfo ship = shipSprite.GetComponent<ShipInfo>();
            CellInfo cell = gameObject.GetComponent<CellInfo>();
            Vector2 endPos = new Vector2(cell.rowId + ship.orientation * (ship.length-1), cell.colId + (1 - ship.orientation) * (ship.length-1));
            for (int i = cell.rowId; i <= endPos.x; i++)
            {
                for (int j = cell.colId; j <= endPos.y; j++)
                    if (!valid(i, j))
                    {
                        occupy = false;
                        break;
                    }
                if (!occupy) break;
            }

            if (occupy)
            {
                //Update grid data and deploy the ship
                Debug.Log("Ship deployed !!!");
                ship.startPos = new Vector2(cell.rowId, cell.colId);
                ship.endPos = endPos;
                SetGridData(ship.startPos, ship.endPos, true, shipSprite);
                ship.isDeployed = true;

                //Release mouse following and restore clickable feature
                Vector2 cellPos = this.gameObject.transform.position;
                Vector2 offsetShip = new Vector2((1 - ship.orientation) * (shipSprite.GetComponent<SpriteRenderer>().bounds.size.x / 2 - GetComponent<SpriteRenderer>().bounds.size.x / 2),
                                                    ship.orientation * (-shipSprite.GetComponent<SpriteRenderer>().bounds.size.y / 2 + GetComponent<SpriteRenderer>().bounds.size.y / 2));
                shipSprite.transform.position = cellPos + offsetShip;
                shipSprite.GetComponent<BoxCollider2D>().enabled = true;
                mfhandler.followingSprite = null;
            }
            else
            {
                Debug.Log("Nooo, can't put the ship thereee!!!");
            }
        }

    }
}
