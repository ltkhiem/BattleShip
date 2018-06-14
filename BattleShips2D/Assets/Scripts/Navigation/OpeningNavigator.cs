using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpeningNavigator : MonoBehaviour {

    GameObject goButtonStart, goButtonStatus, goButtonPlay;
    GameObject goInputField;
    GameStateManager gameStateManager;

    private bool animateInputField;
    Vector3 InputFieldEndPos;
    Text txtStatus;

    enum CheckNameState { READY, VALID, INVALID};
    CheckNameState checkNameState = CheckNameState.READY;

    bool switchScene = false;

    void Awake()
    {
        goButtonPlay = GameObject.Find("Button Play");
        goButtonStart = GameObject.Find("Button Start");
        goButtonStatus = GameObject.Find("Button Status");
        goInputField = GameObject.Find("InputField Username");
        txtStatus = goButtonStatus.GetComponentInChildren<Text>();
        gameStateManager = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();
    }

	// Use this for initialization
	void Start () {
        goButtonPlay.SetActive(false);
        goButtonStatus.SetActive(false);
        goInputField.SetActive(false);
	}
	
    public void GameBegin()
    {
        goButtonStart.SetActive(false);
        goButtonPlay.SetActive(true);
        goInputField.SetActive(true);

        animateInputField = true;
        InputFieldEndPos = goInputField.transform.position + new Vector3(0, 1.9f, 0);
    }

    public void DisplayCheckNameProgress()
    {
        goButtonPlay.SetActive(false);
        txtStatus.text = "Checking ...";
        goButtonStatus.SetActive(true);
    }

    public void DisplayCheckNameResult(bool isValid)
    {
        if (isValid)
            checkNameState = CheckNameState.VALID;
        else
            checkNameState = CheckNameState.INVALID;
    }

    internal void DisplayMatchingResult()
    {
        switchScene = true;
    }

	// Update is called once per frame
	void Update () {
        if (animateInputField)
            goInputField.transform.position = Vector2.Lerp(goInputField.transform.position, InputFieldEndPos, 0.05f);

        //User type in input field
        if (goInputField.GetComponent<InputField>().isFocused)
        {
            goButtonStatus.SetActive(false);
            goButtonPlay.SetActive(true);
        }

        //Display checking name result and progress matching step
        if (checkNameState.Equals(CheckNameState.VALID))
        {
            txtStatus.text = "Matching ...";
            checkNameState = CheckNameState.READY;
            gameStateManager.SendMatching();
            //disable focus input field

        }
        else if (checkNameState.Equals(CheckNameState.INVALID))
        {
            txtStatus.text = "Invalid Name";
            checkNameState = CheckNameState.READY;
        }

        // Jump to ship setup scene
        if (switchScene)
        {
            SceneManager.LoadScene("Main");
        }

    }
}
