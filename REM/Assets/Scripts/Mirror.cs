using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    Camera mirrorCam;
    Camera playerCam;
    public Matrix4x4 m;
    
    void Awake() {
        mirrorCam = transform.GetChild(0).gameObject.GetComponent<Camera>();
        playerCam = SceneManager.Instance.player.GetComponent<Camera>();
    }

    void Update() {
       
    }
}
