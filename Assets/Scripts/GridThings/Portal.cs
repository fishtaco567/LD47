using UnityEngine;
using System.Collections;

public class Portal : Tile {

    public Portal pairedPortal;

    public override bool AcceptBall(Vector3Int position, Ball ball, Vector3 hitNormal) {
        var direction = ReduceToDirection(hitNormal);
        ball.SetPositionVelocity(pairedPortal.TileBasePos() + direction, new Vector3Int());
        ball.currentTile = null;
        return true;
    }
}
