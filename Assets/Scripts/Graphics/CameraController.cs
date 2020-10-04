using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public Vector3 offset;

    bool isLocked;
    CameraFocus lockObject;

    public float easingFactor;

    // Use this for initialization
    void Start() {
        isLocked = false;
    }

    // Update is called once per frame
    void LateUpdate() {
        var cameraGoal = lockObject.GetFocus() + offset;
        transform.position = Vector3.Lerp(transform.position, cameraGoal, easingFactor * Time.deltaTime);
    }

    public void LockToRoom(CameraFocus room) {
        isLocked = true;
        lockObject = room;
    }

    public void LockToPlayer(CameraFocus player) {
        isLocked = false;
        lockObject = player;
    }

}
