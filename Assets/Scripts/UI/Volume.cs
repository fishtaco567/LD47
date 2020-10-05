using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Volume : Singleton<Volume> {

    public float musicVolume = 1;
    public float sfxVolume = 1;

    public Slider sfxBar;
    public Slider musicBar;

    // Use this for initialization
    void Start() {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update() {
        var others = GameObject.FindObjectsOfType<Volume>();
        if(others.Length != 1) {
            if(sfxBar == null) {
                var newVol = others[0];
                if(newVol == this) {
                    newVol = others[1];
                }

                newVol.sfxVolume = sfxVolume;
                newVol.sfxBar.value = sfxVolume;
                newVol.musicVolume = musicVolume;
                newVol.musicBar.value = musicVolume;
                Destroy(this.gameObject);
            }
        }
    }

    public void ChangeMusicVolume() {
        if(musicBar != null) {
            musicVolume = musicBar.value;
        }
    }

    public void ChangeSFXVolume() {
        if(sfxBar != null) {
            sfxVolume = sfxBar.value;
        }
    }

}
