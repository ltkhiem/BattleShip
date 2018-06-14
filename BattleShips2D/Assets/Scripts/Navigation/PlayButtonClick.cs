using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayButtonClick : MonoBehaviour {

    Button btnPlay;
    GameObject goInputField;
    OpeningNavigator Navigator;
    GameStateManager gameManager;

    void Awake()
    {
        goInputField = GameObject.Find("InputField Username");
        gameManager = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();
        btnPlay = gameObject.GetComponent<Button>();
        Navigator = GameObject.Find("OpeningNavigator").GetComponent<OpeningNavigator>();
    }

	// Use this for initialization
	void Start () {
        btnPlay.onClick.AddListener(OnClickPlay);
	}

    private void OnClickPlay()
    {
        Debug.Log("Clicked");
        InputField text = goInputField.GetComponent<InputField>();
        gameManager.SendPlayerName(text.text);
        Navigator.DisplayCheckNameProgress();
    }

    // Update is called once per frame
    void Update () {
		
	}

}
