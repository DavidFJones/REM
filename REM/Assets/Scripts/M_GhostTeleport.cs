using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class M_GhostTeleport : MonoBehaviour
{
    Transform point;

    void Awake() {
        point = transform.GetChild(0).transform;
    }
    void OnTriggerEnter(Collider other) {
        if(other.tag== "Player" && SceneManager.Instance.lastTeleport != this) {
            teleportToPoint();
        }
    }

    void teleportToPoint() {
        M_Ghost_FunHouse currentGhost = SceneManager.Instance.mirrorFunGhost.GetComponent<M_Ghost_FunHouse>();
        if (currentGhost.state == mGhostState.Idle) {
            currentGhost.state = mGhostState.Hunting;
        }

        SceneManager.Instance.lastTeleport = this;
        SceneManager.Instance.mirrorFunGhost.GetComponent<NavMeshAgent>().Warp(point.position);
    }
}
