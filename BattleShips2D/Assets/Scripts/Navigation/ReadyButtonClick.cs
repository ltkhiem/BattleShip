using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyButtonClick : MonoBehaviour {

    Button btnReady;
    GameStateManager gameStateManager;
    SetupNavigator setupNavigator;
    GameObject[] arrShip = new GameObject[5];

    void Awake()
    {
        btnReady = GetComponent<Button>();
        gameStateManager = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();
        setupNavigator = GameObject.Find("SetupNavigator").GetComponent<SetupNavigator>();

        arrShip[0] = GameObject.Find("Mary Celeste");
        arrShip[1] = GameObject.Find("MV Joyita");
        arrShip[2] = GameObject.Find("The Kaz");
        arrShip[3] = GameObject.Find("Octavius");
        arrShip[4] = GameObject.Find("Carol Deering");
    }

	// Use this for initialization
	void Start () {
        btnReady.interactable = false;
        btnReady.onClick.AddListener(OnClickReady);
	}

    private void OnClickReady()
    {
        setupNavigator.PostShipSetup();
        gameObject.GetComponentInChildren<Text>().text = "Waiting ...";
        gameObject.GetComponent<Button>().interactable = false;   
    }

    // Update is called once per frame
    void Update () {
        int cnt = 0;
        for (int i=0; i<5; i++)
            if (arrShip[i].GetComponent<ShipInfo>().isDeployed) cnt++;
        if (cnt == 5) btnReady.interactable = true;
        else btnReady.interactable = false;
    }
}
