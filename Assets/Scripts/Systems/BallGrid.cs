using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallGrid : MonoBehaviour
{

    public Vector3 tileSize;

    public Vector3Int gridSize;

    public Tile[,,] tileGrid;

    private Tile tempTile;
    private Vector3Int[] tempCoordList;

    public float tickTime = 0.5f;

    private float currentTime;

    public Action tick;

    // Start is called before the first frame update
    void Start()
    {
        currentTime = 0;

        tileGrid = new Tile[gridSize.x, gridSize.y, gridSize.z];

        for(int i = 0; i < transform.childCount; i++) {
            var tile = transform.GetChild(i);
            tempTile = tile.GetComponent<Tile>();
            tempCoordList = tempTile.GridPoints();
            tick += tempTile.Tick;
            foreach(Vector3Int gridPoint in tempCoordList) {
                if(tileGrid[gridPoint.x, gridPoint.y, gridPoint.z] == null) {
                    tileGrid[gridPoint.x, gridPoint.y, gridPoint.z] = tempTile;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        if(currentTime > tickTime) {
            currentTime -= tickTime;
            tick();
        }
    }

    public Tile GetTileAtWorldPos(Vector3 worldPos) {
        var intPos = Vector3Int.FloorToInt(worldPos);
        var basePos = Vector3Int.FloorToInt(transform.position);
        var offsetPos = intPos - basePos;

        if(offsetPos.x < gridSize.x && offsetPos.y < gridSize.y && offsetPos.z < gridSize.z && offsetPos.x >= 0 && offsetPos.y >= 0 && offsetPos.z >= 0) {
            return tileGrid[offsetPos.x, offsetPos.y, offsetPos.z];
        } else {
            return null;
        }
    }

    public Tile GetTileAtWorldPosInt(Vector3Int worldPosInt) {
        var basePos = Vector3Int.FloorToInt(transform.position);
        Debug.Log(basePos);
        var offsetPos = worldPosInt - basePos;

        if(offsetPos.x < gridSize.x && offsetPos.y < gridSize.y && offsetPos.z < gridSize.z && offsetPos.x >= 0 && offsetPos.y >= 0 && offsetPos.z >= 0) {
            return tileGrid[offsetPos.x, offsetPos.y, offsetPos.z];
        } else {
            return null;
        }
    }

    private void OnTriggerEnter(Collider other) {
        var player = other.GetComponent<PlayerController>();

        if(player != null) {
            player.grid = this;
            player.OnEnterRoom(this.gameObject);
        }
    }

    private void OnTriggerExit(Collider other) {
        var player = other.GetComponent<PlayerController>();

        if(player != null) {
            player.grid = null;
            player.OnExitRoom(this.gameObject);
        }
    }

}
