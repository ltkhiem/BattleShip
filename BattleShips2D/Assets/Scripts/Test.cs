using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    GameObject[] arrShip = new GameObject[5];
    GameObject destroyedShip;
    int cnt = 1;

    // Use this for initialization
    void Start () {
        arrShip[0] = GameObject.Find("Mary Celeste");
        arrShip[1] = GameObject.Find("MV Joyita");
        arrShip[2] = GameObject.Find("The Kaz");
        arrShip[3] = GameObject.Find("Octavius");
        arrShip[4] = GameObject.Find("Carol Deering");

        //destroyedShip.transform.localEulerAngles = new Vector3(0, 0, -90);
    }
	
	// Update is called once per frame
	void Update () {
        if (cnt == 0)
            destroyedShip.transform.localEulerAngles = new Vector3(0, 0, -90);
        if (cnt==1)
        {
            destroyedShip = Instantiate(arrShip[0], Vector3.zero, Quaternion.identity) as GameObject;
            cnt--;
        }
    }
}
