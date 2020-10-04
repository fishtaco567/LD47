using UnityEngine;
using System.Collections;

public class CameraFocus : MonoBehaviour {

    public Vector3 offset;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public Vector3 GetFocus() {
        return transform.position + offset;
    }

    public void OnDrawGizmosSelected() {
        Gizmos.DrawSphere(GetFocus(), 1);
    }

}
