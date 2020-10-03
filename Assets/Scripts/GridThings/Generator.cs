using UnityEngine;
using System.Collections;

public class Generator : Tile, IGoalItem {

    public float timeBetweenHits;
    public Vector3Int entryDirection;
    public Vector3Int entryTile;

    public float timeRetainBall;

    public Vector3Int exitTile;
    public Vector3Int exitVelocity;

    private float timeSinceLastHit;

    private Ball ball;

    public float rotationalSpeed;
    public GameObject rotatedObject;

    public void Start() {
        timeSinceLastHit = 0;
    }

    public void Update() {
        timeSinceLastHit += Time.deltaTime;

        if(timeSinceLastHit < timeBetweenHits && rotatedObject != null) {
            rotatedObject.transform.Rotate(new Vector3(0, 0, 1), rotationalSpeed * Time.deltaTime);
        }
    }

    public void FixedUpdate() {
        if(ball != null && timeSinceLastHit > timeRetainBall) {
            ball.SetPositionVelocity(exitTile + TileBasePos(), exitVelocity);
            ball.currentTile = null;
            ball = null;
        }
    }

    public override bool AcceptBall(Vector3Int position, Ball ball, Vector3 hitNormal) {
        if(position == new Vector3Int(x, y, z) + entryTile && (ReduceToDirection(hitNormal) == entryDirection || entryDirection.magnitude < 0.01)) {
            this.ball = ball;
            ball.SetPositionVelocity(new Vector3Int(x, y, z), new Vector3Int(0, 0, 0));
            timeSinceLastHit = 0;
            return true;
        } else {
            return false;
        }
    }

    public bool IsSatisfied() {
        return timeSinceLastHit < timeBetweenHits;
    }

}
