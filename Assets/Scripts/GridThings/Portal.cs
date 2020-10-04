using UnityEngine;
using System.Collections;

public class Portal : Tile {

    public Portal pairedPortal;

    public float blorpTime;

    private float timeSinceHit;

    private Vector3 baseScale;

    public void Start() {
        baseScale = transform.localScale;
        timeSinceHit = 0;
    }

    public void Update() {
        timeSinceHit += Time.deltaTime;

        if(timeSinceHit < blorpTime) {
            transform.localScale = baseScale * (0.1f * (1 - (2f / blorpTime) * (2f / blorpTime) * ((timeSinceHit - blorpTime / 2) * (timeSinceHit - blorpTime / 2))) + 1);
        } else {
            transform.localScale = baseScale;
        }
    }

    public override bool AcceptBall(Vector3Int position, Ball ball, Vector3 hitNormal) {
        var direction = ReduceToDirection(hitNormal);
        ball.SetPositionVelocity(pairedPortal.TileBasePos() - direction, new Vector3Int(), true);
        ball.currentTile = null;
        timeSinceHit = 0;
        pairedPortal.timeSinceHit = 0;
        return true;
    }

    public override void PostAccept(Vector3Int position, Ball ball, Vector3 hitNormal) {
        ball.currentTile = null;
    }
}
