using UnityEngine;
using System.Collections;
using Rewired;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {

    public enum PlayerState {
        Standard,
        HoldBlock
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

    public PlayerState playerState;

    public float timeBetweenPushes;
    private float timeSinceLastPush;
    private Tile currentTile;

    // Use this for initialization
    void Start() {
        timeSinceLastPush = 0;
        playerState = PlayerState.Standard;
        rewiredPlayer = ReInput.players.GetPlayer(0);
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update() {
        timeSinceLastPush += Time.deltaTime;
        horizInput = rewiredPlayer.GetAxis("Horizontal");
        vertInput = rewiredPlayer.GetAxis("Vertical");
        upInput = rewiredPlayer.GetButton("Up");
        downInput = rewiredPlayer.GetButton("Down");
        pickUpPressed = rewiredPlayer.GetButtonDown("PickUp");
    }

    private void FixedUpdate() {
        switch(playerState) {
            case PlayerState.Standard:
                Vector3 moveVector = new Vector3(horizInput, 0, vertInput).normalized * moveSpeed;

                if(moveVector.magnitude > 0.01) {
                    transform.rotation = Quaternion.LookRotation(moveVector, transform.up);
                }

                moveVector.y = upInput ? verticalSpeed : (downInput ? -verticalSpeed : 0);
                controller.Move(moveVector * Time.fixedDeltaTime);

                var forward = transform.position + transform.forward;
                var forwardInt = Vector3Int.FloorToInt(forward);
                selector.transform.position = forwardInt;

                if(pickUpPressed && grid != null) {
                    var tile = grid.GetTileAtWorldPosInt(forwardInt + Vector3Int.down);
                    Debug.Log(tile);
                    if(tile != null) {
                        playerState = PlayerState.HoldBlock;
                        currentTile = tile;
                    }
                }

                break;
            case PlayerState.HoldBlock:
                if(pickUpPressed) {
                    currentTile = null;
                    playerState = PlayerState.Standard;
                    break;
                }

                if(timeSinceLastPush < timeBetweenPushes) {
                    break;
                }

                if(vertInput != 0 || horizInput != 0) {
                    timeSinceLastPush = 0;
                    if(Mathf.Abs(horizInput) > Mathf.Abs(vertInput)) {
                        var sign = Mathf.Sign(horizInput);
                        if(currentTile.Move(new Vector3Int((int)sign, 0, 0), grid)) {
                            transform.position += new Vector3Int((int)sign, 0, 0);
                        }
                    } else {
                        var sign = Mathf.Sign(vertInput);
                        if(currentTile.Move(new Vector3Int(0, 0, (int)sign), grid)) {
                            transform.position += new Vector3Int(0, 0, (int)sign);
                        }

                    }
                } else if(upInput) {
                    timeSinceLastPush = 0;
                    if(currentTile.Move(new Vector3Int(0, 1, 0), grid)) {
                        transform.position += new Vector3Int(0, 1, 0);
                    }
                } else if(downInput) {
                    timeSinceLastPush = 0;
                    if(currentTile.Move(new Vector3Int(0, -1, 0), grid)) {
                        transform.position += new Vector3Int(0, -1, 0);
                    }
                }

                var forward2 = transform.position + transform.forward;
                var forwardInt2 = Vector3Int.FloorToInt(forward2);
                selector.transform.position = forwardInt2;

                break;
        }
    }

}
