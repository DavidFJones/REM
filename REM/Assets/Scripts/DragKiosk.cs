using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragKiosk : MonoBehaviour
{
    public Vector3 offset;

    void Update() {
        //Move the kiosks position to match the players, less the offset
        transform.position = SceneManager.Instance.player.transform.position - offset;
    }
}
