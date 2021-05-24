using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAudioController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip closedSound;

    void Start() {
        Door doorInstance = gameObject.transform.parent.gameObject.GetComponent<Door>();
        openSound = doorInstance.openSound;
        closedSound = doorInstance.closedSound;
        audioSource = gameObject.transform.parent.gameObject.GetComponent<AudioSource>();
    }

    public void stopDoorOpenSound() {
        AudioManager.StopSound(audioSource);
    }

    public void playDoorClosedSound() {
        AudioManager.PlaySound(audioSource, closedSound);
    }
}
