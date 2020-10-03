using UnityEngine;
using System.Collections;
using System;

public class Ball : MonoBehaviour {

    public enum HitClass {
        Accepted,
        Hit,
        Bounce,
        NoHit
    }

    public Tile currentTile;

    public Vector3Int currentPosition;
    public Vector3Int currentVelocity;
    public Vector3Int currentAcceleration;

    public Grid grid;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void FixedUpdate() {
        if(currentTile == null) {
            currentVelocity += currentAcceleration;
            var newPosition = currentPosition + currentVelocity;
            Move(currentPosition, newPosition);
        }
    }

    private void Move(Vector3Int initialPosition, Vector3Int newPosition) {
        var hit = newPosition;
        var hitNormal = new Vector3();
        var hitType = CheckLine(currentPosition.x, currentPosition.y, currentPosition.z, newPosition.x, newPosition.y, newPosition.z, out hit, out hitNormal);

        switch(hitType) {
            case HitClass.Accepted:
                break;
            case HitClass.Bounce:
                currentPosition = hit;
                //Hit on X
                var remainingMovement = newPosition - hit;
                if(hitNormal.x > hitNormal.y && hitNormal.x > hitNormal.z) {
                    currentVelocity.x = -currentVelocity.x;
                    remainingMovement.x = -remainingMovement.x;
                }
                //Hit on Y
                else if(hitNormal.y > hitNormal.x && hitNormal.y > hitNormal.z) {
                    currentVelocity.y = -currentVelocity.y;
                    remainingMovement.y = -remainingMovement.y;
                }
                //Hit on Z
                else {
                    currentVelocity.z = -currentVelocity.z;
                    remainingMovement.z = -remainingMovement.z;
                }

                newPosition = currentPosition + remainingMovement;

                var hitType2 = CheckLine(currentPosition.x, currentPosition.y, currentPosition.z, newPosition.x, newPosition.y, newPosition.z, out hit, out hitNormal);
                switch(hitType2) {
                    case HitClass.NoHit:
                        currentPosition = newPosition;
                        break;
                    case HitClass.Accepted:
                        break;
                    case HitClass.Bounce:
                    case HitClass.Hit:
                        currentPosition = hit;
                        if(hitNormal.x > hitNormal.y && hitNormal.x > hitNormal.z) {
                            currentVelocity.x = 0;
                        }
                        //Hit on Y
                        else if(hitNormal.y > hitNormal.x && hitNormal.y > hitNormal.z) {
                            currentVelocity.y = 0;
                        }
                        //Hit on Z
                        else {
                            currentVelocity.z = 0;
                        }
                        break;
                }
                currentPosition = hit;
                break;
            case HitClass.Hit:
                currentPosition = hit;
                //Hit on X
                if(hitNormal.x > hitNormal.y && hitNormal.x > hitNormal.z) {
                    currentVelocity.x = 0;
                }
                //Hit on Y
                else if(hitNormal.y > hitNormal.x && hitNormal.y > hitNormal.z) {
                    currentVelocity.y = 0;
                }
                //Hit on Z
                else {
                    currentVelocity.z = 0;
                }
                break;
            case HitClass.NoHit:
                currentPosition = newPosition;
                break;
        }
    }

    private HitClass CheckLine(int x0, int y0, int z0, int x1, int y1, int z1, out Vector3Int hit, out Vector3 hitNormal) {
        hitNormal = new Vector3(1, 0, 0);

        bool steepXY = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
        if(steepXY) { Swap(ref x0, ref y0); Swap(ref x1, ref y1); }

        bool steepXZ = Math.Abs(z1 - z0) > Math.Abs(x1 - x0);
        if(steepXZ) { Swap(ref x0, ref z0); Swap(ref x1, ref z1); }

        int deltaX = Math.Abs(x1 - x0);
        int deltaY = Math.Abs(y1 - y0);
        int deltaZ = Math.Abs(z1 - z0);

        int errorXY = deltaX / 2, errorXZ = deltaX / 2;

        int stepX = (x0 > x1) ? -1 : 1;
        int stepY = (y0 > y1) ? -1 : 1;
        int stepZ = (z0 > z1) ? -1 : 1;

        int y = y0, z = z0;
        int lastY = y, lastZ = z;

        // Check if the end of the line hasn't been reached.
        for(int x = x0; x != x1; x += stepX) {
            int xCopy = x, yCopy = y, zCopy = z;

            if(steepXZ) Swap(ref xCopy, ref zCopy);
            if(steepXY) Swap(ref xCopy, ref yCopy);

            var tileTest = grid.tileGrid[x][y][z];
            if(tileTest != null) {
                var direction = new Vector3(x1 - x0, y1 - y0, z1 - z0);
                RaycastHit rayHit;
                if(Physics.Raycast(new Vector3(x0, y0, z0), direction, out rayHit, direction.magnitude)) {
                    hitNormal = rayHit.normal;
                }

                if(tileTest.AcceptBall(new Vector3Int(x, y, z), this, hitNormal)) {
                    this.currentTile = tileTest;
                    hit = new Vector3Int(x, y, z);
                    return HitClass.Accepted;
                } else {
                    if(tileTest.bounce) {
                        hit = new Vector3Int(x - stepX, lastY, lastZ);
                        return HitClass.Bounce;
                    } else {
                        hit = new Vector3Int(x - stepX, lastY, lastZ);
                        return HitClass.Hit;
                    }
                }
            }

            errorXY -= deltaY;
            errorXZ -= deltaZ;

            lastY = y;
            lastZ = z;

            if(errorXY < 0) {
                y += stepY;
                errorXY += deltaX;
            }

            if(errorXZ < 0) {
                z += stepZ;
                errorXZ += deltaX;
            }
        }
        hit = new Vector3Int(x1, y, z);
        return HitClass.NoHit;
    }

    public static void Swap<T>(ref T x, ref T y) {
        T tmp = y;
        y = x;
        x = tmp;
    }

}
