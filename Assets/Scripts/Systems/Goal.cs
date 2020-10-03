using UnityEngine;
using System.Collections.Generic;

public class Goal : MonoBehaviour {

    List<IGoalItem> goals;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public bool GoalsSatisfied() {
        foreach(IGoalItem goal in goals) {
            if(!goal.IsSatisfied()) {
                return false;
            }
        }

        return true;
    }

}
