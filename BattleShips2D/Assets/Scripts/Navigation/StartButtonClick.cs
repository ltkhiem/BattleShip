using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButtonClick : MonoBehaviour {

    OpeningNavigator Navigator;
    GameStateManager gameManager; 
    Button btnStart;

    void Awake()
    {
        btnStart = gameObject.GetComponent<Button>();
        gameManager = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();

    }

	// Use this for initialization
	void Start () {
        btnStart.onClick.AddListener(OnClickStart);
        Navigator = GameObject.Find("OpeningNavigator").GetComponent<OpeningNavigator>();
	}

    private void OnClickStart()
    {
        Navigator.GameBegin();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
