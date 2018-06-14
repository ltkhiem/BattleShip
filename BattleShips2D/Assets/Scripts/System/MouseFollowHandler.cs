using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollowHandler : MonoBehaviour {

    public GameObject followingSprite = null;
    private Vector3 mousePos;
    private float moveSpeed = 2.0f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (followingSprite != null)
        {
            ShipInfo ship = followingSprite.GetComponent<ShipInfo>();
           
            //Check orientation change
            if (Input.GetMouseButtonDown(1))
            {
                ship.orientation = 1 - ship.orientation;
                if (ship.orientation == 1)
                    followingSprite.transform.localEulerAngles = new Vector3(0, 0, -90);
                else followingSprite.transform.localEulerAngles = new Vector3(0, 0, 0);
            }

            //Transform the ship to follow the mouse
            mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            Vector3 offsetShip = new Vector3((1 - ship.orientation) * (followingSprite.GetComponent<SpriteRenderer>().bounds.size.x / 2 - 0.2f),
                                                 ship.orientation * (-followingSprite.GetComponent<SpriteRenderer>().bounds.size.y / 2 + 0.2f), -10);
            followingSprite.transform.position = Vector2.Lerp(transform.position, mousePos + offsetShip, moveSpeed);




        }


    }

}
