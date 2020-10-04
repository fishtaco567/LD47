using UnityEngine;
using System.Collections;

public class Portal : Tile {

    public Portal pairedPortal;

    public float blorpTime;

    private float timeSinceHit;

    private Vector3 baseScale;

    public float retainTime;

    public Ball retainedBall;
    public Vector3Int direction;

    private float currentRetainedTime;

    public bool isDestination;

    public void Start() {
        baseScale = transform.localScale;
        timeSinceHit = 0;
        currentRetainedTime = 0;
    }

    public void Update() {
        timeSinceHit += Time.deltaTime;

        if(timeSinceHit < blorpTime) {
            transform.localScale = baseScale * (0.1f * (1 - (2f / blorpTime) * (2f / blorpTime) * ((timeSinceHit - blorpTime / 2) * (timeSinceHit - blorpTime / 2))) + 1);
        } else {
            transform.localScale = baseScale;
        }

        if(retainedBall != null) {
            currentRetainedTime += Time.deltaTime;
            if(!isDestination && currentRetainedTime > retainTime) {
                retainedBall.SetPositionVelocity(new Vector3Int(pairedPortal.x, pairedPortal.y, pairedPortal.z), Vector3Int.zero, true);
                pairedPortal.currentRetainedTime = 0;
                pairedPortal.retainedBall = retainedBall;
                retainedBall.currentTile = pairedPortal;
                retainedBall = null;
                pairedPortal.direction = direction;
                pairedPortal.isDestination = true;
            }

            if(isDestination && currentRetainedTime > retainTime) {
                retainedBall.SetPositionVelocity(new Vector3Int(x, y, z) - direction, new Vector3Int(0, -1, 0), false);
                retainedBall.currentTile = null;
                retainedBall = null;
            }
        }
    }

    public override bool AcceptBall(Vector3Int position, Ball ball, Vector3 hitNormal) {
        direction = ReduceToDirection(hitNormal);
        ball.SetPositionVelocity(new Vector3Int(x, y, z), new Vector3Int(), false);
        retainedBall = ball;
        retainedBall.currentTile = this;
        currentRetainedTime = 0;
        timeSinceHit = 0;
        pairedPortal.timeSinceHit = 0;
        return true;
    }

    public override void PostAccept(Vector3Int position, Ball ball, Vector3 hitNormal) {
        //ball.currentTile = null;
    }
}
