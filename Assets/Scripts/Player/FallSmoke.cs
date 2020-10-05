using UnityEngine;
using System.Collections;

public class FallSmoke : MonoBehaviour {

    public Vector3 smokeFall;
    public Vector3 scaleFall;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        transform.position += smokeFall * Time.deltaTime;
        transform.localScale += scaleFall * Time.deltaTime;
    }
}
