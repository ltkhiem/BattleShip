using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipInfo : MonoBehaviour {
    public string name;
    public int length;

    public int orientation = 0;
    public Vector2 startPos, endPos;
    public Vector2 lastTransformPos;
    public bool isDeployed = false;

    GameObject GridSystem;
    GridLayout gridLayout;
    
	// Use this for initialization
	void Start () {
        //Init Variables
        GridSystem = GameObject.Find("Grid System");
        gridLayout = GridSystem.GetComponent<GridLayout>();

        //Transform size to fit cell size
        Vector2 shipScale = new Vector2();
        shipScale.x = (length * (gridLayout.gridSize.x/gridLayout.rows)) / gameObject.GetComponent<SpriteRenderer>().bounds.size.x;
        shipScale.y = (gridLayout.gridSize.y/gridLayout.cols) / gameObject.GetComponent<SpriteRenderer>().bounds.size.y;
        gameObject.transform.localScale = shipScale;

        //Fit collider to ship object
        Vector2 S = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
        gameObject.GetComponent<BoxCollider2D>().size = S;
    }
	
	// Update is called once per frame
	void Update () {

		
	}


}
