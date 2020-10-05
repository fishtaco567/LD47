using UnityEngine;
using System.Collections;

public class GravityVelocityEffector : Tile {

    public Vector3Int newGravity;
    public Vector3Int newVelocity;

    bool acceptedBallLastTick;
    bool acceptedBallThisTick;

    public override void Start() {
        base.Start();
    }

    public void Update() {
        acceptedBallThisTick = false;
    }

    public override void Tick() {
        acceptedBallLastTick = false;
        acceptedBallLastTick = acceptedBallThisTick;
        acceptedBallThisTick = false;
    }

    public override bool AcceptBall(Vector3Int position, Ball ball, Vector3 hitNormal) {
        if(!acceptedBallThisTick && !acceptedBallLastTick) {
            if(newGravity.magnitude != 0) {
                ball.currentAcceleration = newGravity;
            }

            if(newVelocity.magnitude != 0) {
                ball.currentVelocity += newVelocity;
            }
        }

        acceptedBallThisTick = true;

        return true;
    }

    public override void PostAccept(Vector3Int position, Ball ball, Vector3 hitNormal) {
        ball.currentTile = null;
    }

}
