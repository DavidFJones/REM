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
    public AudioClip swingOpen;
    [Tooltip("Sound that plays while the door swings shut")]
    public AudioClip swingClosed;
    [Tooltip("Sound that plays when the door clicks into the closed position")]
    public AudioClip closed;

    [HideInInspector]
    DoorState currentState = DoorState.Closed;//What state the door is in

    BoxCollider collision;//The auto generate collider for the door model

    Animator animator;
    AudioSource doorAudio;

    private bool playedClosedSound = true;//Have we already played the closed sound;
    private bool playingSound = false;//Are we playing a long lasting sound?

    //This should probably be change to a singleton for our player***
    GameObject player;//A reference to our player character
    AudioSource playerAudio;//Player audio controller (This really needs to be a global singleton

    private float openDirection;//A float the determines if we touched the front or back of an object

    void Awake() {
        //FIX THIS
        //Finds the player in the scene and assigns the ui and inventory references
        GameObject player = GameObject.Find("Player");
        playerAudio = player.GetComponent<AudioSource>();

        //-4.437 z
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

    void Update() {


        if (!playedClosedSound && animator.GetCurrentAnimatorStateInfo(0).IsName("DoorClose") && animator.GetAnimatorTransitionInfo(0).duration == 0)
            playClosedSound();

        if (playingSound && animator.GetAnimatorTransitionInfo(0).duration == 0)
            StartCoroutine(FadeAudioOut());
    }


    //Called whenever the player goes to interact with a door object
    public void openCloseDoor(Vector3 openLocation, Vector3 playerPos) {
        openDirection = Vector3.Dot(openLocation.normalized, Vector3.forward);
        //If our door is unlocked, or does not have a lock, allow us to open the door
        if (!hasLock || unlocked) {
            //plays the player opening door sound
            playerAudio.Play();


            //Toggles the open/closed state
            open = !open;

            if (!open) {
                //play swing closing sound
                playClosingSound();

                //close the door
                currentState = DoorState.Closed;
            } else {

                //play swing open sound
                playOpeningSound();

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
            foreach (GameObject item in SceneManager.Instance.playerInventory.inventory) {
                if (GameObject.ReferenceEquals(item, keyObject)) {
                    SceneManager.Instance.uIManager.HUDMessageDoor("Door unlocked", gameObject);
                    unlocked = true;
                    break;
                }
            }
            if (!unlocked)
                SceneManager.Instance.uIManager.HUDMessageDoor("Door is locked", gameObject);
        }

        //Sets our door animator state to match our enum state
        animator.SetInteger("Door State", (int)currentState);

    }

    //fades the current sfx if it is not the door closing sound. Eventually make this a global static enum
    public  IEnumerator FadeAudioOut() {

        playingSound = false;
        float startVolume = doorAudio.volume;
        

        while (doorAudio.volume > 0) {
            doorAudio.volume -= startVolume * Time.deltaTime / 0.2f;

            yield return null;
        }

        doorAudio.Stop();
        doorAudio.volume = startVolume;
    }
    //Plays our closed sound effect
    public void playClosedSound() {
        playedClosedSound = true;

        //Sets and plays the door opening sound
        doorAudio.clip = closed;
        doorAudio.Play();
    }

    //Plays our swing open sound effect
    void playOpeningSound() {
        //playingSound = true;

        //Sets and plays the door opening sound
        doorAudio.clip = swingOpen;
        doorAudio.Play();

        Invoke("playingSoundTrue", 1f);

        if (playedClosedSound)
            Invoke("setCanCloseSound", 0.2f);
    }

    //Plays our swing closed sound effect
    void playClosingSound() {
        //playingSound = true;
        //Sets and plays the door opening sound
        doorAudio.clip = swingClosed;
        doorAudio.Play();

        Invoke("playingSoundTrue", 1f);

        if (playedClosedSound)
            Invoke("setCanCloseSound", 0.2f);
        
    }

    void playingSoundTrue() {
        playingSound = true;
    }

    void setCanCloseSound() {
        playedClosedSound = false;
    }
}
