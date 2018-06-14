using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridLayout : MonoBehaviour
{

    //grid specifics
    [SerializeField]
    public int rows;
    [SerializeField]
    public int cols;
    [SerializeField]
    public Vector2 gridSize;
    [SerializeField]
    private Vector2 gridOffset;

    //about cells
    [SerializeField]
    private Sprite cellSprite;
    private Vector2 cellSize;
    private Vector2 cellScale;

    public GameObject[][] Grid;
    GameStateManager gameStateManager;

    void Awake()
    {
        gameStateManager = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();
    }

    void Start()
    {
        rows = cols = gameStateManager.tableSize;
        InitCells(); //Initialize all cells
    }

    void InitCells()
    {
        GameObject cellObject = new GameObject();
        Grid = new GameObject[rows][];
        for (int i = 0; i < rows; i++)
            Grid[i] = new GameObject[cols];

        //creates an empty object and adds a sprite renderer component -> set the sprite to cellSprite
        cellObject.AddComponent<SpriteRenderer>().sprite = cellSprite;

        //catch the size of the sprite
        cellSize = cellSprite.bounds.size;

        //get the new cell size -> adjust the size of the cells to fit the size of the grid
        Vector2 newCellSize = new Vector2(gridSize.x / (float)cols, gridSize.y / (float)rows);

        //Get the scales so you can scale the cells and change their size to fit the grid
        cellScale.x = newCellSize.x / cellSize.x;
        cellScale.y = newCellSize.y / cellSize.y;

        cellSize = newCellSize; //the size will be replaced by the new computed size, we just used cellSize for computing the scale

        cellObject.transform.localScale = new Vector2(cellScale.x, cellScale.y);

        //fix the cells to the grid by getting the half of the grid and cells add and minus experiment
        gridOffset.x = -(gridSize.x / 2) + cellSize.x / 2;
        gridOffset.y = -(gridSize.y / 2) + cellSize.y / 2;

        //fill the grid with cells by using Instantiate
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                //add the cell size so that no two cells will have the same x and y position
                Vector3 pos = new Vector3(col * cellSize.x + gridOffset.x + transform.position.x, row * cellSize.y + gridOffset.y + transform.position.y, 20);

                //instantiate the game object, at position pos, with rotation set to identity
                Grid[rows - row - 1][col] = Instantiate(cellObject, pos, Quaternion.identity) as GameObject;

                //set the parent of the cell to GRID so you can move the cells together with the grid;
                Grid[rows - row - 1][col].transform.parent = transform;

                Grid[rows - row - 1][col].AddComponent<CellClick>();
                Grid[rows - row - 1][col].AddComponent<BoxCollider2D>();
                Grid[rows - row - 1][col].AddComponent<CellInfo>();

                CellInfo ic = Grid[rows - row - 1][col].GetComponent<CellInfo>();
                ic.rowId = rows-row-1;
                ic.colId = col;

            }
        }

        //destroy the object used to instantiate the cells
        Destroy(cellObject);
    }

    //so you can see the width and height of the grid on editor
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, gridSize);
    }
}