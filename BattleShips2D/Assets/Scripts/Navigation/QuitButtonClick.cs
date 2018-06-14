using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitButtonClick : MonoBehaviour {

    Button btnQuit;
    GameStateManager gameStateManger;
	// Use this for initialization
	void Start () {
        Button btnQuit = GameObject.Find("Button Quit").GetComponent<Button>();
        btnQuit.onClick.AddListener(OnClickQuit);

        gameStateManger = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();
	}

    private void OnClickQuit()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update () {
		
	}

}
