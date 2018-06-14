using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OppoCellClick : MonoBehaviour {

    // Use this for initialization
    SetupNavigator setupNavigator;
    OppoGridLayout gridLayout;

    void Awake()
    {
        gridLayout = GameObject.Find("Grid System Opponent").GetComponent<OppoGridLayout>();
        setupNavigator = GameObject.Find("SetupNavigator").GetComponent<SetupNavigator>();
    }

	void Start () {
        
    }


    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown()
    {
        CellInfo cell = GetComponent<CellInfo>();
        setupNavigator.SendShoot(cell.rowId+1, cell.colId+1);
        Debug.Log("True anim pos: " + gridLayout.Grid[cell.rowId][cell.colId].transform.position);

        
    }
}