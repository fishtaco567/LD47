using UnityEngine;
using System.Collections;
using Rewired;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {

    public enum PlayerState {
        Standard,
        HoldBlock,
        HoldBall
    }

    public BallGrid grid;
    public Rewired.Player rewiredPlayer;

    private float horizInput;
    private float vertInput;
    private bool upInput;
    private bool downInput;
    private bool pickUpPressed;

    public float moveSpeed;
    public float verticalSpeed;

    CharacterController controller;

    public GameObject selector;

    public GameObject lightIndicator;

    public PlayerState playerState;

    public float timeBetweenPushes;
    private float timeSinceLastPush;
    private Tile currentTile;
    private Ball heldBall;

    public float maxHeight;

    new public CameraController camera;

    public Color unselectedColor;
    public Color selectedColor;

    public AudioSource walkSound;
    public AudioSource pushFail;
    public AudioSource push;

    public GameObject smokePrefab;
    public float timeBetweenSmoke;
    private float currentSmokeTime;

    // Use this for initialization
    void Start() {
        currentSmokeTime = 0;
        timeSinceLastPush = 0;
        playerState = PlayerState.Standard;
        rewiredPlayer = ReInput.players.GetPlayer(0);
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update() {
        walkSound.volume = Volume.Instance.sfxVolume;
        pushFail.volume = Volume.Instance.sfxVolume;
        push.volume = Volume.Instance.sfxVolume;

        currentSmokeTime += Time.deltaTime;
        if(currentSmokeTime > timeBetweenSmoke) {
            currentSmokeTime = 0;
            var smoke = GameObject.Instantiate(smokePrefab);
            smoke.transform.position = this.transform.position + Vector3.down * 1.1f;
            GameObject.Destroy(smoke, 0.5f);
        }

        timeSinceLastPush += Time.deltaTime;
        horizInput = rewiredPlayer.GetAxis("Horizontal");
        vertInput = rewiredPlayer.GetAxis("Vertical");
        upInput = rewiredPlayer.GetButton("Up");
        downInput = rewiredPlayer.GetButton("Down");
        pickUpPressed = rewiredPlayer.GetButtonDown("PickUp");

        switch(playerState) {
            case PlayerState.Standard:
                if(pickUpPressed && grid != null) {
                    var forward = transform.position + transform.forward;
                    var forwardInt = Vector3Int.FloorToInt(forward);
                    var tile = grid.GetTileAtWorldPosInt(forwardInt + Vector3Int.down);
                    if(tile != null && !tile.locked) {
                        playerState = PlayerState.HoldBlock;
                        currentTile = tile;
                    } else {
                        var ball = grid.CheckBalls(forwardInt + Vector3Int.down);
                        if(ball != null) {
                            heldBall = ball;
                            heldBall.held = true;
                            playerState = PlayerState.HoldBall;
                        }
                    }
                }
                break;
            case PlayerState.HoldBlock:
                if(pickUpPressed) {
                    currentTile = null;
                    playerState = PlayerState.Standard;
                    break;
                }
                break;
            case PlayerState.HoldBall:
                if(pickUpPressed) {
                    heldBall.held = false;
                    heldBall = null;
                    playerState = PlayerState.Standard;
                }
                break;
        }
    }

    private void FixedUpdate() {
        switch(playerState) {
            case PlayerState.Standard:
                controller.enableOverlapRecovery = true;

                Vector3 moveVector = new Vector3(horizInput, 0, vertInput).normalized * moveSpeed;

                if(moveVector.magnitude > 0.01) {
                    transform.rotation = Quaternion.LookRotation(moveVector, transform.up);
                }

                moveVector.y = upInput ? verticalSpeed : (downInput ? -verticalSpeed : 0);
                
                if(transform.position.y > maxHeight) {
                    moveVector.y = Mathf.Min(moveVector.y, 0);
                }

                if(moveVector.magnitude != 0 && !walkSound.isPlaying) {
                    walkSound.Play();
                } else if(moveVector.magnitude == 0) {
                    walkSound.Stop();
                }

                controller.Move(moveVector * Time.fixedDeltaTime);

                var forwardInt = MoveSelector(false);

                break;
            case PlayerState.HoldBlock:

                if(timeSinceLastPush < timeBetweenPushes) {
                    break;
                }

                if(vertInput != 0 || horizInput != 0) {
                    timeSinceLastPush = 0;
                    if(Mathf.Abs(horizInput) > Mathf.Abs(vertInput)) {
                        var sign = Mathf.Sign(horizInput);
                        if(currentTile.Move(new Vector3Int((int)sign, 0, 0), grid)) {
                            transform.position += new Vector3Int((int)sign, 0, 0);
                            push.Play();
                        } else {
                            pushFail.Play();
                        }
                    } else {
                        var sign = Mathf.Sign(vertInput);
                        if(currentTile.Move(new Vector3Int(0, 0, (int)sign), grid)) {
                            transform.position += new Vector3Int(0, 0, (int)sign);
                            push.Play();
                        } else {
                            pushFail.Play();
                        }

                    }
                } else if(upInput) {
                    timeSinceLastPush = 0;
                    if(currentTile.Move(new Vector3Int(0, 1, 0), grid)) {
                        transform.position += new Vector3Int(0, 1, 0);
                        push.Play();
                    } else {
                        pushFail.Play();
                    }
                } else if(downInput) {
                    timeSinceLastPush = 0;
                    if(currentTile.Move(new Vector3Int(0, -1, 0), grid)) {
                        transform.position += new Vector3Int(0, -1, 0);
                        push.Play();
                    } else {
                        pushFail.Play();
                    }
                }

                MoveSelector(true);

                break;
            case PlayerState.HoldBall:
                if(timeSinceLastPush < timeBetweenPushes) {
                    break;
                }

                if(vertInput != 0 || horizInput != 0) {
                    timeSinceLastPush = 0;
                    if(Mathf.Abs(horizInput) > Mathf.Abs(vertInput)) {
                        var sign = Mathf.Sign(horizInput);
                        var newPosition = heldBall.currentPosition + new Vector3Int((int)sign, 0, 0);
                        if(heldBall.SetPosition(newPosition, true)) {
                            transform.position += new Vector3Int((int)sign, 0, 0);
                            push.Play();
                        } else {
                            pushFail.Play();
                        }
                    } else {
                        var sign = Mathf.Sign(vertInput);
                        var newPosition = heldBall.currentPosition + new Vector3Int(0, 0, (int)sign);
                        if(heldBall.SetPosition(newPosition, true)) {
                            transform.position += new Vector3Int(0, 0, (int)sign);
                            push.Play();
                        } else {
                            pushFail.Play();
                        }
                    }
                } else if(upInput) {
                    timeSinceLastPush = 0;
                    var newPosition = heldBall.currentPosition + new Vector3Int(0, 1, 0);
                    if(heldBall.SetPosition(newPosition, true)) {
                        transform.position += new Vector3Int(0, 1, 0);
                        push.Play();
                    } else {
                        pushFail.Play();
                    }
                } else if(downInput) {
                    timeSinceLastPush = 0;
                    var newPosition = heldBall.currentPosition + new Vector3Int(0, -1, 0);
                    if(heldBall.SetPosition(newPosition, true)) {
                        transform.position += new Vector3Int(0, -1, 0);
                        push.Play();
                    } else {
                        pushFail.Play();
                    }
                }
                MoveSelector(true);
                break;
        }
    }

    private Vector3Int MoveSelector(bool selected) {
        var forward = transform.position + transform.forward;
        var forwardInt = Vector3Int.FloorToInt(forward);
        selector.transform.position = forwardInt;

        selector.GetComponentInChildren<MeshRenderer>().material.SetColor("_Color", selected ? selectedColor : unselectedColor);

        if(selected) {
            lightIndicator.SetActive(true);
            lightIndicator.transform.position = forwardInt;
        } else {
            lightIndicator.SetActive(false);
        }

        return forwardInt;
    }

    public void OnEnterRoom(GameObject room) {
        camera.LockToRoom(room.GetComponent<CameraFocus>());
    }

    public void OnExitRoom(GameObject room) {
        camera.LockToPlayer(GetComponent<CameraFocus>());
    }

}
