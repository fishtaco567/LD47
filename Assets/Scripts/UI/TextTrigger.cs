using UnityEngine;
using System.Collections;

public class TextTrigger : MonoBehaviour {

    public string text;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    private void OnTriggerEnter(Collider other) {
        HUDManager.Instance.DisplayText(text);
    }

}
