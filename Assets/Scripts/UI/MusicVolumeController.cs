using UnityEngine;
using System.Collections;

public class MusicVolumeController : MonoBehaviour {

    public AudioSource source;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        source.volume = Volume.Instance.musicVolume;
    }
}
