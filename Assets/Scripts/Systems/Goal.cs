using UnityEngine;
using System.Collections.Generic;
using Sirenix;

public class Goal : MonoBehaviour {

    public List<Tile> goals;

    public List<Goal> previousGoals;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public bool GoalsSatisfied() {
        foreach(Tile tile in goals) {
            if(!tile.IsSatisfied()) {
                return false;
            }
        }

        foreach(Goal goal in previousGoals) {
            if(!goal.GoalsSatisfied()) {
                return false;
            }
        }

        return true;
    }

}
