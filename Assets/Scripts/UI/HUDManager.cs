using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using Rewired;

public class HUDManager : Singleton<HUDManager> {

    public TextMeshProUGUI hudText;
    public float enterExitScreenTime;
    public float displayTime;

    private Vector3 hudTextBasePos;
    private Vector3 hudTextOffPos;

    private float currentTime;
    private bool displayingHudText;

    public GameObject inGameMenu;

    private bool inGameMenuOn;

    public void DisplayText(string text) {
        currentTime = 0;
        hudText.text = text;
        displayingHudText = true;
        inGameMenuOn = false;
    }

    // Use this for initialization
    void Start() {
        hudTextBasePos = hudText.rectTransform.position;
        hudTextOffPos = hudText.rectTransform.position - new Vector3(0, 150, 0);
        currentTime = displayTime + 2 * enterExitScreenTime;
        displayingHudText = true;
        hudText.rectTransform.position = hudTextOffPos;
    }

    // Update is called once per frame
    void Update() {
        if(displayingHudText) {
            currentTime += Time.deltaTime;
            if(currentTime < enterExitScreenTime) {
                var lerp = Mathf.Clamp01(currentTime / enterExitScreenTime);
                hudText.rectTransform.position = Vector3.Lerp(hudTextOffPos, hudTextBasePos, lerp);
            } else if(currentTime > enterExitScreenTime + displayTime) {
                var lerp = 1 - Mathf.Clamp01((currentTime - enterExitScreenTime - displayTime) / enterExitScreenTime);
                hudText.rectTransform.position = Vector3.Lerp(hudTextOffPos, hudTextBasePos, lerp);
            }
            
            if(currentTime > displayTime + 2 * enterExitScreenTime) {
                displayingHudText = false;
            }
        }

        inGameMenu.SetActive(inGameMenuOn);
        if(inGameMenuOn) {
            Time.timeScale = 0;
        } else {
            Time.timeScale = 1;
        }

        var menuPressed = ReInput.players.GetPlayer(0).GetButtonDown("Menu");
        if(menuPressed) {
            inGameMenuOn = !inGameMenuOn;
        }
    }

    public void OnClickQuit() {
        inGameMenuOn = false;
        SceneManager.LoadScene(0);
    }

    public void OnClickResume() {
        inGameMenuOn = false;
    }

}
