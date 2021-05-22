using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorAudioController : MonoBehaviour
{
    Door parentDoor;

    void Awake() {
        parentDoor = gameObject.transform.root.transform.gameObject.GetComponent<Door>();
    }
    void playClosedSound() {
        parentDoor.playClosedSound();
    }

}
