using UnityEngine;
using System.Collections;
using System;

public class Ball : MonoBehaviour {

    public enum HitClass {
        Accepted,
        Hit,
        Bounce,
        NoHit,
        OutOfBounds
    }

    public Tile currentTile;

    public Vector3Int currentPosition;
    public Vector3Int currentVelocity;
    public Vector3Int currentAcceleration;

    public BallGrid grid;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void FixedUpdate() {
        if(currentTile == null) {
            currentVelocity += currentAcceleration;
            var newPosition = currentPosition + currentVelocity;
            Debug.Log("Try move to " + newPosition);
            Move(newPosition);
        }
    }

    public void SetPosition(Vector3Int newPosition) {
        currentPosition = newPosition;
        transform.position = currentPosition + grid.transform.position;
    }

    public void SetPositionVelocity(Vector3Int newPosition, Vector3Int newVelocity) {
        currentPosition = newPosition;
        currentVelocity = newVelocity;
        transform.position = currentPosition + grid.transform.position;
    }

    public void SetAcceleration(Vector3Int newAccel) {
        currentAcceleration = newAccel;
    }

    private void Move(Vector3Int newPosition) {
        var hit = newPosition;
        var hitNormal = new Vector3();
        var hitType = CheckLine(currentPosition.x, currentPosition.y, currentPosition.z, newPosition.x, newPosition.y, newPosition.z, out hit, out hitNormal);

        switch(hitType) {
            case HitClass.Accepted:
                break;
            case HitClass.Bounce:
                SetPosition(hit);
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
                        SetPosition(newPosition);
                        break;
                    case HitClass.Accepted:
                        break;
                    case HitClass.Bounce:
                    case HitClass.Hit:
                        SetPosition(hit);
                        if(Mathf.Abs(hitNormal.x) > Mathf.Abs(hitNormal.y) && Mathf.Abs(hitNormal.x) > Mathf.Abs(hitNormal.z)) {
                            currentVelocity.x = 0;
                        }
                        //Hit on Y
                        else if(Mathf.Abs(hitNormal.y) > Mathf.Abs(hitNormal.x) && Mathf.Abs(hitNormal.y) > Mathf.Abs(hitNormal.z)) {
                            currentVelocity.y = 0;
                        }
                        //Hit on Z
                        else {
                            currentVelocity.z = 0;
                        }
                        break;
                    case HitClass.OutOfBounds:
                        Destroy(this.gameObject);
                        break;
                }
                break;
            case HitClass.Hit:
                SetPosition(hit);
                //Hit on X
                if(Mathf.Abs(hitNormal.x) > Mathf.Abs(hitNormal.y) && Mathf.Abs(hitNormal.x) > Mathf.Abs(hitNormal.z)) {
                    currentVelocity.x = 0;
                }
                //Hit on Y
                else if(Mathf.Abs(hitNormal.y) > Mathf.Abs(hitNormal.x) && Mathf.Abs(hitNormal.y) > Mathf.Abs(hitNormal.z)) {
                    currentVelocity.y = 0;
                }
                //Hit on Z
                else {
                    currentVelocity.z = 0;
                }
                break;
            case HitClass.NoHit:
                SetPosition(newPosition);
                break;
            case HitClass.OutOfBounds:
                Destroy(this.gameObject);
                break;
        }
    }

    private HitClass CheckLine(int x0, int y0, int z0, int x1, int y1, int z1, out Vector3Int hit, out Vector3 hitNormal) {
        var start = new Vector3Int(x0, y0, z0);
        var end = new Vector3Int(x1, y1, z1);

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
            Debug.Log(xCopy + ", " + yCopy + ", " + zCopy);
            
            if(xCopy > grid.gridSize.x || yCopy > grid.gridSize.y || zCopy > grid.gridSize.z) {
                hit = new Vector3Int(xCopy, yCopy, zCopy);
                return HitClass.OutOfBounds;
            }

            var tileTest = grid.tileGrid[xCopy, yCopy, zCopy];
            if(tileTest != null) {
                var direction = end -  start;
                RaycastHit rayHit;
                Debug.DrawRay(start + grid.transform.position + new Vector3(.5f, .5f, .5f), direction);
                if(Physics.Raycast(start + grid.transform.position + new Vector3(.5f, .5f, .5f), direction, out rayHit, direction.magnitude)) {
                    hitNormal = rayHit.normal;
                }

                if(tileTest.AcceptBall(new Vector3Int(xCopy, yCopy, zCopy), this, hitNormal)) {
                    this.currentTile = tileTest;
                    hit = new Vector3Int(xCopy, yCopy, zCopy);
                    tileTest.PostAccept(new Vector3Int(xCopy, yCopy, zCopy), this, hitNormal);
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

    private void OnValidate() {
        transform.position = currentPosition + grid.transform.position;
    }

}
