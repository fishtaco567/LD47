using UnityEngine;
using System.Collections;

public abstract class Tile : MonoBehaviour {

    public int x;
    public int y;
    public int z;

    public int sizeX;
    public int sizeY;
    public int sizeZ;

    public bool bounce;

    public abstract bool AcceptBall(Vector3Int position, Ball ball, Vector3 hitNormal);

    public Vector3Int[] GridPoints() {
        var list = new Vector3Int[sizeX * sizeY * sizeZ];

        var index = 0;

        for(int i = 0; i < sizeX; i++) {
            for(int j = 0; j < sizeY; j++) {
                for(int k = 0; k < sizeZ; k++) {
                    list[index] = new Vector3Int(i + x, j + y, k + z);
                }
            }
        }

        return list;
    }

    private void OnValidate() {
        var grid = GetComponentInParent<Grid>();
        transform.position = new Vector3(x * grid.tileSize.x, y * grid.tileSize.y, z * grid.tileSize.z);
    }

    protected Vector3Int ReduceToDirection(Vector3 normal) {
        var direction = new Vector3Int();
        if(Mathf.Abs(normal.x) > Mathf.Abs(normal.y) && Mathf.Abs(normal.x) > Mathf.Abs(normal.z)) {
            direction.x = (int)Mathf.Sign(normal.x);
        } else if(Mathf.Abs(normal.y) > Mathf.Abs(normal.x) && Mathf.Abs(normal.y) > Mathf.Abs(normal.z)) {
            direction.y = (int)Mathf.Sign(normal.y);
        } else {
            direction.z = (int)Mathf.Sign(normal.z);
        }

        return direction;
    }

    protected Vector3Int TileBasePos() {
        return new Vector3Int(x, y, z);
    }

}
