using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class M_Ghost_FunHouse : MonoBehaviour
{
    [Tooltip("How long the ghost must wait before they can begin moving after a successful attack on the player (in seconds)")]
    public float attackDelayTimer = 5f;

    public float currentTimer; // What our current timer is at
    public bool hitCountDown = false;// checks if we are counting down after an attack

    public mGhostState state = mGhostState.Idle; // What our ghosts current state is

    CapsuleCollider capsuleCollider;// Our ghosts collider
    private NavMeshAgent navAgent;//Our ghosts navMesh Agent

    void Start() {
        currentTimer = attackDelayTimer;

        capsuleCollider = GetComponent<CapsuleCollider>();
        capsuleCollider.enabled = false;

        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = SceneManager.Instance.player.maxSpeed * 1.5f;
    }

    void Update() {
        //Always look at the player
        transform.LookAt(SceneManager.Instance.player.transform);

        //Determines our behaviour based on our ghosts current state
        switch (state) {
            case mGhostState.Hunting:
                //Move towards the player character
                navAgent.destination = SceneManager.Instance.player.transform.position;
                capsuleCollider.enabled = true;
                break;
            case mGhostState.Cooldown:
                if (!hitCountDown)
                    return;// If we are not waiting for the hit timer to count, return
                else {
                    currentTimer -= Time.deltaTime;
                    //When our timer hits 0
                    if (currentTimer < 0) {
                        hitCountDown = false;
                        //change the state of our ghost to hunt
                        state = mGhostState.Hunting;

                        // Set the speed of our ghost relative to our players max speed
                        navAgent.speed = SceneManager.Instance.player.maxSpeed * 1.5f;

                        //Make sure all our ghost timers are turned off
                        hitCountDown = false;
                        //Allow our ghost to move
                        navAgent.isStopped = false;
                    }
                }
                break;
            case mGhostState.Idle:  //starting state, don't do anything
                break;
        }
    }

    void OnCollisionEnter(Collision collision) {
        print(collision);
        //If we touch/collide with the player, attack them
        if (collision.gameObject.tag == "Player") {
            ghostAttack();
        }
    }

    void ghostAttack() {
        //Put the ghost on a brief cooldown after landing an attack
        state = mGhostState.Cooldown;

        navAgent.isStopped = true;//Stop the ghost from moving
        capsuleCollider.enabled = false;//disable the ghosts collision

        //If we are not currently counting down already, reset our timer
        if (!hitCountDown) {
            currentTimer = attackDelayTimer;
        }
        hitCountDown = true;

        //Cause the player character to take a hit
        SceneManager.Instance.player.enemyHit();
    }
}
