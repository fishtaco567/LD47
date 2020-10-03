using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{

    public Vector3 tileSize;

    public Vector3 gridSize;

    public Tile[][][] tileGrid;

    private Tile tempTile;
    private Vector3Int[] tempCoordList;

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject tile in transform) {
            tempTile = tile.GetComponent<Tile>();
            tempCoordList = tempTile.GridPoints();
            foreach(Vector3Int gridPoint in tempCoordList) {
                if(tileGrid[gridPoint.x][gridPoint.y][gridPoint.z] == null) {
                    tileGrid[gridPoint.x][gridPoint.y][gridPoint.z] = tempTile;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
