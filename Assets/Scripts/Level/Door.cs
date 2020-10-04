using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

    public GameObject slider;
    public Vector3 fullSlide;
    public float slideTime;

    private float slideStatus;
    private Vector3 sliderBasePos;

    public bool open;

    public Goal goal;

    // Use this for initialization
    void Start() {
        sliderBasePos = slider.transform.position;
    }

    // Update is called once per frame
    void Update() {
        if(goal.GoalsSatisfied()) {
            open = true;
        } else {
            open = false;
        }

        if(open) {
            if(slideStatus < 1) {
                slideStatus += (1 / slideTime) * Time.deltaTime;
                slideStatus = Mathf.Min(slideStatus, 1);
            }
        } else {
            if(slideStatus > 0) {
                slideStatus -= (1 / slideTime) * Time.deltaTime;
                slideStatus = Mathf.Max(slideStatus, 0);
            }
        }

        slider.transform.position = sliderBasePos + fullSlide * slideStatus;
    }
}
