using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Door : MonoBehaviour
{

    [HideInInspector]
    public InteractionType type = InteractionType.Door;

    [SerializeField]
    [Tooltip("The 3d model for our door. If this is empty just change a value of the door (Such as open). If it is still empty make sure you have a 3d model inside of the 'Door Container' object")]
    GameObject doorObject;
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
    [SerializeField]
    [Tooltip("How long it takes for the door to open/close")]
    private float doorSpeed = 1.5f;

    [Header("Closed Position")]
    [Tooltip("The closed rotation of our door. Typically this should be left at 0, 0, 0")]
    private Vector3 closedRotation;
    [Tooltip("The closed position of our door. Typically this should be left at 0, 0, 0")]
    private Vector3 closedPosition;
    [Header("Open Position")]
    [Tooltip("The open rotation of our door")]
    public Vector3 openRotation;
    [Tooltip("The open rotation of our door")]
    public Vector3 openPosition;

    Vector3 targetRotation;//The targeted rotation for the door on state change
    Vector3 targetPosition;//The targeted position for the door on state change

    float startTime;//Used to determine when we interacted with the door

    BoxCollider collision;//The auto generate collider for the door model

    //This should probably be change to a singleton for our player***
    GameObject player;//A reference to our player character
    PlayerInventory playerInventory;//A reference to our players inventory
    UIHandler playerUI;//A reference to our players ui controller

    void Awake() {
        //THIS NEEDS TO BE FIXED
        closedPosition = gameObject.transform.position;
        closedRotation = gameObject.transform.localRotation.eulerAngles;
        //FIX THIS

        //Finds the player in the scene and assigns the ui and inventory references
        GameObject player = GameObject.Find("Player");
        playerUI = player.GetComponent<UIHandler>();
        playerInventory = player.GetComponent<PlayerInventory>();

        //Find our door model within our parent
        for (int i = 0; i < this.transform.childCount; i++) {
            if(this.transform.GetChild(i).name == "Door Container") {
                doorContainer = this.transform.GetChild(i);
                doorObject = doorContainer.GetChild(0).gameObject;
                break;
            }
        }

        //Sets the targeted rotation and position to our current rotation and position
        targetPosition = transform.localPosition;
        targetRotation = transform.localRotation.eulerAngles;

        //If the user sets the door to have a lock, but forgets to assign a key throw an error and set the door to unlocked
        if(hasLock && keyObject == null) {
            Debug.LogError("Door was set to locked but key was never assigned - " + gameObject, gameObject);
        }

        //If we don't have a lock, make the door unlocked
        if (!hasLock)
            unlocked = true;

        //assign our door model the interactable tag, proper physics tag and give it a box collider
        doorObject.tag = "Interactable";
        doorObject.layer = 7;
        collision = doorObject.AddComponent<BoxCollider>() as BoxCollider;
    }

    public void Update() {

        //Checks if our current rotation is not the same as our target rotation. Lerps between the two positions overtime if true
        if (Quaternion.Angle(Quaternion.Euler(transform.rotation.eulerAngles), Quaternion.Euler(targetRotation)) > .05f) {
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(transform.rotation.eulerAngles), Quaternion.Euler(targetRotation), doorSpeed * 0.01f * (Time.time - startTime));
        }

        //Checks if our current position is not the same as our target position. Lerps between the two positions overtime if true
        if (Vector3.Angle(transform.localPosition, targetPosition) > 0.05f) {
            transform.position = Vector3.Lerp(transform.localPosition, targetPosition, doorSpeed * 0.01f * (Time.time - startTime));
        }
    }

    //Called whenever the player goes to interact with a door object
    public void openCloseDoor(Vector3 openLocation) {
        //If our door is unlocked, or does not have a lock, allow us to open the door
        if (!hasLock || unlocked) {
            //Toggles the open/closed state
            open = !open;

            //Sets the time which we pressed the interact button. Used to smoothly slerp our lerp between values
            startTime = Time.time;

            if (!open) {
                //If the door is not open, set the target position and rotation to the closed  position
                targetPosition = closedPosition;
                targetRotation = closedRotation;
            } else {
                //Checks which side of the door we are interacting from, and set the target position/rotation appropriatly 
                if((openLocation.x + openLocation.y + openLocation.z) < 0) {
                    targetPosition = openPosition;
                    targetRotation = openRotation;
                } else {
                    targetPosition = openPosition;
                    targetRotation = openRotation * -1; // Inverts the open rotation if opening the door from behind
                }
                
            }
        } else { // The door is locked and we do not have the key
            foreach (GameObject item in playerInventory.inventory) {
                if (GameObject.ReferenceEquals(item, keyObject)) {
                    playerUI.HUDMessageDoor("Door unlocked", gameObject);
                    unlocked = true;
                    openCloseDoor(openLocation);
                    break;
                }
            }
            if(!unlocked)
            playerUI.HUDMessageDoor("Door is locked", gameObject);
        }
        
    }

    /*
    void OnValidate() {
        if (!open) {
            //gameObject.transform.localPosition = closedPosition;
            //gameObject.transform.localRotation = Quaternion.Euler(closedRotation);
        } else {
            //gameObject.transform.localPosition = openPosition;
            //gameObject.transform.localRotation = Quaternion.Euler(openRotation);
        }
    }*/

}
