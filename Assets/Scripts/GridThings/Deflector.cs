using UnityEngine;
using System.Collections;

public class Deflector : Tile {

    public float retainTime;

    private float timeRetained;

    public Vector3Int entryDirection;
    public Vector3Int exitDirection;

    private bool enteredEntry;

    private Ball ball;

    public override bool AcceptBall(Vector3Int position, Ball ball, Vector3 hitNormal) {
        var direction = ReduceToDirection(hitNormal);
        if(direction == entryDirection) {
            enteredEntry = true;
            timeRetained = 0;
            this.ball = ball;
            ball.currentTile = this;
            ball.SetPosition(new Vector3Int(x, y, z), false);
            return true;
        } else if(direction == exitDirection) {
            enteredEntry = false;
            timeRetained = 0;
            this.ball = ball;
            ball.currentTile = this;
            ball.SetPosition(new Vector3Int(x, y, z), false);
            return true;
        }
        return false;
    }

    public override void Tick() {
        if(ball != null && timeRetained > retainTime) {
            if(enteredEntry) {
                if(entryDirection.x != 0) {
                    ball.currentVelocity += exitDirection * ball.currentVelocity.x * -entryDirection.x;
                    ball.SetPosition(new Vector3Int(x, y, z) + exitDirection, false);
                    ball.currentVelocity.x = 0;
                    ball.currentTile = null;
                    ball = null;
                } else if(entryDirection.y != 0) {
                    ball.currentVelocity += exitDirection * ball.currentVelocity.y * -entryDirection.y;
                    ball.SetPosition(new Vector3Int(x, y, z) + exitDirection, false);
                    ball.currentVelocity.y = 0;
                    ball.currentTile = null;
                    ball = null;
                } else if(entryDirection.z != 0) {
                    ball.currentVelocity += exitDirection * ball.currentVelocity.z * -entryDirection.z;
                    ball.SetPosition(new Vector3Int(x, y, z) + exitDirection, false);
                    ball.currentVelocity.z = 0;
                    ball.currentTile = null;
                    ball = null;
                }
            } else {
                if(exitDirection.x != 0) {
                    ball.currentVelocity += entryDirection * ball.currentVelocity.x * -exitDirection.x;
                    ball.SetPosition(new Vector3Int(x, y, z) + entryDirection, false);
                    ball.currentVelocity.x = 0;
                    ball.currentTile = null;
                    ball = null;
                } else if(exitDirection.y != 0) {
                    ball.currentVelocity += entryDirection * ball.currentVelocity.y * -exitDirection.y;
                    ball.SetPosition(new Vector3Int(x, y, z) + entryDirection, false);
                    ball.currentVelocity.y = 0;
                    ball.currentTile = null;
                    ball = null;
                } else if(exitDirection.z != 0) {
                    ball.currentVelocity += entryDirection * ball.currentVelocity.z * -exitDirection.z;
                    ball.SetPosition(new Vector3Int(x, y, z) + entryDirection, false);
                    ball.currentVelocity.z = 0;
                    ball.currentTile = null;
                    ball = null;
                }
            }
        }
    }

    // Use this for initialization
    public override void Start() {
        timeRetained = 0;
        enteredEntry = false;
        base.Start();
    }

    // Update is called once per frame
    void Update() {
        timeRetained += Time.deltaTime;
    }
}
