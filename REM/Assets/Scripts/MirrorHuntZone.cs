using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorHuntZone : MonoBehaviour
{
    M_Ghost_Hunter ghost;

    public bool isActive = false;

    
    void Start() {
        ghost = FindObjectOfType<M_Ghost_Hunter>();
    }
    void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            SceneManager.Instance.currentMirror = this;

            if (!isActive) {
                ghost.ghostStateChange(mGhostState.Waiting);
            } else {
                ghost.ghostStateChange(mGhostState.Hunting);
            }
            
        }
    }
    void OnTriggerExit(Collider other) {
        if(other.tag == "Player") {
            ghost.ghostStateChange(mGhostState.Idle);
        }
    }
}
