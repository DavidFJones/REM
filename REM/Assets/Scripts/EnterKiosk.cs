using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterKiosk : MonoBehaviour
{
    void OnTriggerEnter(Collider col) {
        if (col.tag == "Player") {
            SceneManager.Instance.player.canDrag = true;
        }
    }

    void OnTriggerExit(Collider col) {
        if (col.tag == "Player") {
            SceneManager.Instance.player.canDrag = false;
        }
    }
}
