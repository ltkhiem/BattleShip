using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Net.Sockets;

public class ReturnHomeButtonClick : MonoBehaviour {

    Button btnReturnHome;
    private GameObject goGameStateManger;

    // Use this for initialization
    void Start () {
        btnReturnHome = GameObject.Find("Button ReturnHome").GetComponent<Button>();
        btnReturnHome.onClick.AddListener(OnClickReturnHome);
        goGameStateManger = GameObject.Find("GameStateManager");
    }

    private void OnClickReturnHome()
    {
        Destroy(goGameStateManger);
        SceneManager.LoadScene("Opening");
    }

    // Update is called once per frame
    void Update () {
		
	}
}
