using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip doorOpenSound;
    AudioSource playerSounds;

    void Awake() {
        playerSounds = gameObject.GetComponent<AudioSource>();
        playerSounds.clip = doorOpenSound;
    }
}
