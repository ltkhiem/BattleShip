using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipClick : MonoBehaviour {

    GameObject EventSystem;
	// Use this for initialization
	void Start () {
        EventSystem = GameObject.Find("Event System");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnMouseDown()
    {
        MouseFollowHandler mfhandler = EventSystem.GetComponent<MouseFollowHandler>();
        if (mfhandler.followingSprite == null)
        {
            gameObject.GetComponent<ShipInfo>().lastTransformPos = gameObject.transform.position;

            //Reset old occupied grid data if ship is deployed 
            ShipInfo ship = GetComponent<ShipInfo>();
            GridLayout gridLayout = GameObject.Find("Grid System").GetComponent<GridLayout>();
            if (ship.isDeployed)
            {
                for (int i = (int)ship.startPos.x; i <= (int)ship.endPos.x; i++)
                    for (int j = (int)ship.startPos.y; j <= (int)ship.endPos.y; j++)
                    {
                        CellInfo c = gridLayout.Grid[i][j].GetComponent<CellInfo>();
                        c.hasShip = false;
                        c.occupiedShip = null;
                    }
                ship.isDeployed = false;
                ship.startPos = ship.endPos = new Vector2(-1, -1);
            }

            mfhandler.followingSprite = this.gameObject;
            GetComponent<BoxCollider2D>().enabled = false;
        }
        
    }
}
