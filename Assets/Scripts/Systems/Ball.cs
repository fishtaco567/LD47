using UnityEngine;
using System.Collections;
using System;

public class Ball : MonoBehaviour {

    private int[] otherCoordPairs = { 2, 0, 0, 1, 2, 1 };

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

    private Vector3Int lastFramePosition;

    public BallGrid grid;

    public bool held;

    private float timeSinceLastTick;

    private Vector3Int lastMoveDirection;

    // Use this for initialization
    void Start() {
        timeSinceLastTick = 0;
        grid.tick += Tick;
        grid.balls.Add(this);
    }

    private void Update() {
        timeSinceLastTick += Time.deltaTime;
        transform.position = Vector3.Lerp(lastFramePosition, currentPosition, timeSinceLastTick / grid.tickTime) + grid.transform.position;
    }

    // Update is called once per frame
    void Tick() {
        timeSinceLastTick = 0;
        lastFramePosition = currentPosition;

        if(currentTile == null && !held) {
            if(grid.tileGrid[currentPosition.x, currentPosition.y, currentPosition.z] != null) {
                if(grid.tileGrid[currentPosition.x, currentPosition.y, currentPosition.z].AcceptBall(currentPosition, this, lastMoveDirection * -1)) {
                    currentTile = grid.tileGrid[currentPosition.x, currentPosition.y, currentPosition.z];
                    grid.tileGrid[currentPosition.x, currentPosition.y, currentPosition.z].PostAccept(currentPosition, this, lastMoveDirection * -1);
                    if(currentTile != null) {
                        return;
                    }
                }
            }

            currentVelocity += currentAcceleration;

            if(currentVelocity.x != 0) {
                var toCheck = currentPosition + new Vector3Int((int)Mathf.Sign(currentVelocity.x), 0, 0);
                if(toCheck.x < 0 || toCheck.x >= grid.gridSize.x) {
                    currentVelocity.x = 0;
                }
            }

            if(currentVelocity.y != 0) {
                var toCheck = currentPosition + new Vector3Int(0, (int)Mathf.Sign(currentVelocity.y), 0);
                if(toCheck.y < 0 || toCheck.y >= grid.gridSize.y) {
                    currentVelocity.y = 0;
                }
            }

            if(currentVelocity.z != 0) {
                var toCheck = currentPosition + new Vector3Int(0, 0, (int)Mathf.Sign(currentVelocity.z));
                if(toCheck.z < 0 || toCheck.z >= grid.gridSize.z) {
                    currentVelocity.z = 0;
                }
            }

            var newPosition = currentPosition + currentVelocity;
            Move(newPosition);
        }
    }

    public bool SetPosition(Vector3Int newPosition, bool teleport) {
        if(newPosition.x >= 0 && newPosition.y >= 0 && newPosition.z >= 0 && newPosition.x < grid.gridSize.x && newPosition.y < grid.gridSize.y && newPosition.z < grid.gridSize.z) {
            lastFramePosition = currentPosition;
            currentPosition = newPosition;
            if(teleport) {
                lastFramePosition = currentPosition;
            } else if((currentPosition - lastFramePosition).magnitude != 0) {
                lastMoveDirection = currentPosition - lastFramePosition;
            }
            return true;
        }
        return false;
    }

    public bool SetPositionVelocity(Vector3Int newPosition, Vector3Int newVelocity, bool teleport) {
        if(newPosition.x >= 0 && newPosition.y >= 0 && newPosition.z >= 0 && newPosition.x < grid.gridSize.x && newPosition.y < grid.gridSize.y && newPosition.z < grid.gridSize.z) {
            lastFramePosition = currentPosition;
            currentPosition = newPosition;
            currentVelocity = newVelocity;
            if(teleport) {
                lastFramePosition = currentPosition;
            } else if((currentPosition - lastFramePosition).magnitude != 0) {
                lastMoveDirection = currentPosition - lastFramePosition;
            }
            return true;
        }
        return false;
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
                SetPosition(hit, false);
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
                        SetPosition(newPosition, false);
                        break;
                    case HitClass.Accepted:
                        break;
                    case HitClass.Bounce:
                    case HitClass.Hit:
                        SetPosition(hit, false);
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
                        break;
                }
                break;
            case HitClass.Hit:
                SetPosition(hit, false);
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
                SetPosition(newPosition, false);
                break;
            case HitClass.OutOfBounds:
                break;
        }

        if((currentPosition - lastFramePosition).magnitude != 0) {
            lastMoveDirection = currentPosition - lastFramePosition;
        }
    }

    public HitClass CheckLine(int x0, int y0, int z0, int x1, int y1, int z1, out Vector3Int hit, out Vector3 hitNormal) {
        var start = new Vector3Int(x0, y0, z0);
        var end = new Vector3Int(x1, y1, z1);
        hit = new Vector3Int();
        hitNormal = new Vector3();

        var initialPosition = new int[3];
        var finalPosition = new int[3];

        initialPosition[0] = x0;
        initialPosition[1] = y0;
        initialPosition[2] = z0;

        finalPosition[0] = x1;
        finalPosition[1] = y1;
        finalPosition[2] = z1;

        int[] direction = {
            0, 0, 0
        };

        int maxGradDir = 0;
        for(int i = 0; i < 3; i++) {
            direction[i] = finalPosition[i] - initialPosition[i];
            if(Mathf.Abs(direction[i]) > Mathf.Abs(direction[maxGradDir])) {
                maxGradDir = i;
            }
        }

        if(direction[maxGradDir] == 0) {
            return HitClass.OutOfBounds;
        }

        int secondCoord = otherCoordPairs[maxGradDir];
        int thirdCoord = otherCoordPairs[maxGradDir + 3];
        int stepFirstCoord;

        if(direction[maxGradDir] > 0) {
            stepFirstCoord = 1;
        } else {
            stepFirstCoord = -1;
        }

        float secondCoordGradient = (float)direction[secondCoord] / (float)direction[maxGradDir];
        float thirdCoordGradient = (float)direction[thirdCoord] / (float)direction[maxGradDir];

        int[] curPosInLine = {
            initialPosition[0], initialPosition[1], initialPosition[2]
        };

        int endCoord = direction[maxGradDir] + stepFirstCoord;

        int[] lastPos = new int[3];

        for(int firstCoord = 0; firstCoord != endCoord; firstCoord += stepFirstCoord) {
            lastPos[0] = curPosInLine[0];
            lastPos[1] = curPosInLine[1];
            lastPos[2] = curPosInLine[2];
            curPosInLine[maxGradDir] = Mathf.FloorToInt((float)(initialPosition[maxGradDir] + firstCoord) + 0.5f);
            curPosInLine[secondCoord] = Mathf.FloorToInt((float)initialPosition[secondCoord] + (float)firstCoord * secondCoordGradient + 0.5f);
            curPosInLine[thirdCoord] = Mathf.FloorToInt((float)initialPosition[thirdCoord] + (float)firstCoord * thirdCoordGradient + 0.5f);

            if(curPosInLine[0] >= grid.gridSize.x || curPosInLine[1] >= grid.gridSize.y || curPosInLine[2] >= grid.gridSize.z || curPosInLine[0] < 0 || curPosInLine[1] < 0 || curPosInLine[2] < 0) {
                hit = new Vector3Int(lastPos[0], lastPos[1], lastPos[2]);
                return HitClass.Hit;
            }

            var tileTest = grid.tileGrid[curPosInLine[0], curPosInLine[1], curPosInLine[2]];
            if(tileTest != null) {
                var rayDirection = end - start;
                RaycastHit rayHit;
                if(Physics.Raycast(start + grid.transform.position + new Vector3(.5f, .5f, .5f), rayDirection, out rayHit, rayDirection.magnitude)) {
                    hitNormal = rayHit.normal;
                }

                if(tileTest.AcceptBall(new Vector3Int(curPosInLine[0], curPosInLine[1], curPosInLine[2]), this, hitNormal)) {
                    this.currentTile = tileTest;
                    hit = new Vector3Int(curPosInLine[0], curPosInLine[1], curPosInLine[2]);
                    tileTest.PostAccept(new Vector3Int(curPosInLine[0], curPosInLine[1], curPosInLine[2]), this, hitNormal);
                    Debug.Log(currentTile);
                    if(currentTile != null) {
                        return HitClass.Accepted;
                    }
                } else {
                    if(tileTest.bounce) {
                        hit = new Vector3Int(lastPos[0], lastPos[1], lastPos[2]);
                        return HitClass.Bounce;
                    } else {
                        hit = new Vector3Int(lastPos[0], lastPos[1], lastPos[2]);
                        return HitClass.Hit;
                    }
                }
            }
        }

        hit = new Vector3Int(curPosInLine[0], curPosInLine[1], curPosInLine[2]);
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

    private void OnDestroy() {
        grid.tick -= Tick;
    }

}
