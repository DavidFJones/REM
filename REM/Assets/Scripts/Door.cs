using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Door : MonoBehaviour {

    [HideInInspector]
    public InteractionType type = InteractionType.Door;
    [HideInInspector]
    GameObject doorObject;//The actual 3d model object of our door
    Transform doorContainer;//This is the parent for our 3d door object
    [Tooltip("Toggles whether the door is opened or closed")]
    public bool open = false;

    [Tooltip("Toggles whether the door requires a key to open it. Checking this will show you more options")]
    public bool hasLock = false;
    [Tooltip("Drag a Key to act as the key for this door. The player will need to possess this key to unlock the door")]
    [ConditionalField("hasLock")]
    public GameObject keyObject;
    [SerializeField]
    [ConditionalField("hasLock")]
    [Tooltip("By default a locked door is locked. Us this option to manually unlock a door, regardless of if the player has the relevant key (This is only useful for testing purposes)")]
    public bool unlocked = false;

    [Header("Sound Effects")]
    [Tooltip("Sound that plays while the door swings open")]
    public AudioClip openSound;
    [Tooltip("Sound that plays when the door clicks into the closed position")]
    public AudioClip closedSound;

    [HideInInspector]
    DoorState currentState = DoorState.Closed;//What state the door is in

    BoxCollider collision;//The auto generate collider for the door model

    Animator animator;
    [HideInInspector]
    public AudioSource doorAudio;

    private float openDirection;//A float the determines if we touched the front or back of an object

    void Awake() {

        //Sets our animator
        animator = transform.GetChild(0).gameObject.GetComponent<Animator>();

        //Find our door model within our parent
        for (int i = 0; i < this.transform.childCount; i++) {
            if (this.transform.GetChild(i).name == "Door Container") {
                doorContainer = this.transform.GetChild(i);
                doorObject = doorContainer.GetChild(0).gameObject;
                break;
            }
        }
        //If the user sets the door to have a lock, but forgets to assign a key throw an error and set the door to unlocked
        if (hasLock && keyObject == null) {
            Debug.LogError("Door was set to locked but key was never assigned - " + gameObject, gameObject);
        }

        //If we don't have a lock, make the door unlocked
        if (!hasLock)
            unlocked = true;

        //If we set the door to open, change the state to open
        if (open)
            currentState = DoorState.OpenFront;

        //assign our door model the interactable tag, proper physics tag and give it a box collider
        doorObject.tag = "Interactable";
        doorObject.layer = 7;
        collision = doorObject.AddComponent<BoxCollider>() as BoxCollider;

        //Sets which animation state our door should have
        animator.SetInteger("Door State", (int)currentState);

        //Sets our doors audio source/controller
        doorAudio = gameObject.GetComponent<AudioSource>();
    }

    //Called whenever the player goes to interact with a door object
    public void openCloseDoor(Vector3 openLocation, Vector3 playerPos) {
        openDirection = Vector3.Dot(openLocation.normalized, Vector3.forward);
        //If our door is unlocked, or does not have a lock, allow us to open the door
        if (!hasLock || unlocked) {

            //Toggles the open/closed state
            open = !open;
            SceneManager.Instance.playerUI.HUDMessageForceClear();//Forces whatever message is on the hud to be removed

            if (!open) {
                //play swing closing sound
                AudioManager.PlaySound(doorAudio, openSound);

                //close the door
                currentState = DoorState.Closed;
            } else {

                //play swing open sound
                AudioManager.PlaySound(doorAudio, openSound);

                //Checks which side of the door we are interacting from, and set the target position/rotation appropriatly 
                if ((openDirection) < 0) {
                    //open front
                    currentState = DoorState.OpenFront;
                } else {
                    //open back
                    currentState = DoorState.OpenBack;
                }

            }
        } else { // The door is locked and we do not have the key
            foreach (GameObject item in SceneManager.Instance.player.inventory) {
                if (GameObject.ReferenceEquals(item, keyObject)) {
                    SceneManager.Instance.playerUI.HUDMessageUnlockedDoor();
                    unlocked = true;
                    break;
                }
            }
            if (!unlocked)
                SceneManager.Instance.playerUI.HUDMessageLockedDoor();
        }

        //Sets our door animator state to match our enum state
        animator.SetInteger("Door State", (int)currentState);

    }

   

}
