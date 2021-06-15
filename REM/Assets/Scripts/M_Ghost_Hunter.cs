using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class M_Ghost_Hunter : MonoBehaviour
{
    [Tooltip("How long the ghost will wait before attack the player from a new mirror (in seconds)")]
    public float huntTimer = 5f;
    [Tooltip("How long the ghost must wait before they can begin moving after a successful attack on the player (in seconds)")]
    public float attackDelayTimer = 2f;

    public float currentTimer; // What our current timer is at
    public bool waitCountDown = false; // Checks if we are counting down while waiting or not
    public bool hitCountDown = false;// checks if we are counting down after an attack

    [SerializeField]//Serialize this field for the sake of debugging
    mGhostState state = mGhostState.Idle; // What our ghosts current state is

    CapsuleCollider capsuleCollider;// Our ghosts collider
    private NavMeshAgent navAgent;//Our ghosts navMesh Agent


    void Awake() {
        currentTimer = huntTimer;

        capsuleCollider = GetComponent<CapsuleCollider>();
        capsuleCollider.enabled = false;

        navAgent = GetComponent<NavMeshAgent>();  
    }

    void Update()
    {
        //Always look at the player
        transform.LookAt(SceneManager.Instance.player.transform);

        //Determines our behaviour based on our ghosts current state
        switch (state) {
            case mGhostState.Waiting:
                if (!waitCountDown)
                    return; // If we are not waiting for the wait timer to count, return
                else {
                    currentTimer -= Time.deltaTime;
                    //When our timer hits 0
                    if (currentTimer < 0) {
                        waitCountDown = false;
                        transform.position += transform.forward * (Vector3.Distance(SceneManager.Instance.player.transform.position, transform.position) * 0.5f);// Teleport the ghost halfway between themselves and the player
                        ghostStateChange(mGhostState.Hunting);//Change their state to hunting
                    }       
                }
                break;
            case mGhostState.Hunting:
                //Move towards the player character
                navAgent.destination = SceneManager.Instance.player.transform.position;
                break;
            case mGhostState.Cooldown:
                if (!hitCountDown)
                    return;// If we are not waiting for the hit timer to count, return
                else {
                    currentTimer -= Time.deltaTime;
                    //When our timer hits 0
                    if (currentTimer < 0) {
                        hitCountDown = false;
                        ghostStateChange(mGhostState.Waiting);//Change ghost state to Waiting
                    }
                }
                break;
            case mGhostState.Idle:  //starting state, don't do anything
                break;
        }
    }

    //called when our ghost changes states
    public void ghostStateChange(mGhostState newState) {
        state = newState;
        switch (state) {
            case mGhostState.Waiting:
                navAgent.isStopped = true;//Stop the ghost from moving
                capsuleCollider.enabled = false;//disable the ghosts collision

                //If we are not currently counting down already, reset our timer
                if (!waitCountDown) {
                    currentTimer = huntTimer;
                }
                waitCountDown = true;
                break;
            case mGhostState.Hunting:
                capsuleCollider.enabled = true;//Enable collision for our ghost

                //If we have already set an active mirror (which is not the case when first playing the level), set the active mirrors isActive bool to false
                if (SceneManager.Instance.activeMirror != null) {
                    SceneManager.Instance.activeMirror.GetComponent<MirrorHuntZone>().isActive = false;
                }  
                //Set the mirror we are currently standing in to the active mirror
                SceneManager.Instance.activeMirror = SceneManager.Instance.currentMirror;
                //Flag this mirror as active
                SceneManager.Instance.activeMirror.GetComponent<MirrorHuntZone>().isActive = true;

                // Set the speed of our ghost relative to our players max speed
                navAgent.speed = SceneManager.Instance.player.maxSpeed * 1.5f; 

                //Make sure all our ghost timers are turned off
                hitCountDown = waitCountDown = false;
                //Allow our ghost to move
                navAgent.isStopped = false;
                break;
            case mGhostState.Cooldown:
                navAgent.isStopped = true;//Stop the ghost from moving
                capsuleCollider.enabled = false;//disable the ghosts collision

                //If we are not currently counting down already, reset our timer
                if (!hitCountDown) {
                    currentTimer = attackDelayTimer;
                }
                hitCountDown = true;
                break;
            case mGhostState.Idle:
                navAgent.isStopped = true;//Stop the ghost from moving
                capsuleCollider.enabled = false;//disable the ghosts collision
                break;
        }
    }

    void OnCollisionEnter(Collision collision) {
        //If we touch/collide with the player, attack them
        if (collision.gameObject.tag == "Player") {
            ghostAttack();
        }
    }

    void ghostAttack() {
        //Put the ghost on a brief cooldown after landing an attack
        ghostStateChange(mGhostState.Cooldown);

        //Cause the player character to take a hit
        SceneManager.Instance.player.enemyHit();
    }
}
