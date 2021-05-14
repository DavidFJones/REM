using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerButtonInputs : MonoBehaviour
{
    Camera playerCamera;//The players camera
    private float cameraAngle;//The x (up/down) angle of the players camera
    private float maxDistance;//How for our raycast will travel
    [SerializeField]
    [Tooltip("This changes the distance the raycast(red line from center of view) will travel. The raycast is also affected by the angle of the players camera (higher/lower angle means longer raycast)")]
    private float raycastDistance;//An editor value to modify the distance of the raycast
    [SerializeField]
    [Tooltip("This is what object we have currently hit with our raycast")]
    GameObject hitObject;//object we have currently hit with our raycast
    bool canTouch = false;//Used to determine if the player is allowed to grab the item their raycast is hitting (This is based on the items tag)

    Vector3 hitPoint;

    bool paused = false;//Bool to determine if the game state is paused
    PlayerInventory playerInventory;
    UIHandler playerUI;
    void Start()
    {
        //This should be moved to a global game script maybe?
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerCamera = Camera.main;
        playerInventory = gameObject.GetComponent<PlayerInventory>();
        playerUI = gameObject.GetComponent<UIHandler>();
    }

    void FixedUpdate() {
        //This is the angle of our camera. Without needing to convert 90deg up or down is equal to .7
        cameraAngle = Mathf.Abs( playerCamera.transform.localRotation.x);

        maxDistance = raycastDistance + cameraAngle * 1.4f;//Sets the actual distance of our raycast based on the editor value and the angle of the camera

        RaycastHit hit;

        //If we have enabled debug lines, show the raycast
        if (GlobalVariables.showDebugLinesGlobal)
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * maxDistance, Color.red);

        //Checks to see if our player is looking at something within the max distance
        if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, maxDistance)) {
            hitObject = hit.transform.gameObject;//Sets the stored object we just hit

            //If we are looking at an item we can interact with
            if (hitObject.tag == "Interactable") {
                canTouch = true;
                hitPoint = hit.normal;
                InteractionType currentType = GlobalVariables.returnInteractionType(hitObject);
                switch (currentType) {
                    case InteractionType.Key:
                    case InteractionType.Item:
                        if (!playerInventory.inventoryFull) {
                            playerUI.HUDMessageItem(hitObject);
                        } else {
                            playerUI.HUDMessageFull();
                        }
                        break;
                    case InteractionType.Door:
                        playerUI.HUDMessageDoor("Interact", hitObject.transform.root.gameObject);
                        break;
                    default:
                        Debug.LogError("Player looked at an interactive object without a proper interaction type " +hitObject,hitObject);
                        break;
                }
                
            }
                

        } else {
            hitObject = null;
            canTouch = false;
            playerUI.HUDMessageClear();
        }
    }

    public void Interact(InputAction.CallbackContext context) {
        //If we pressed the interact button
        if (context.started) {
            //And we are looking at an interactive item, do the thing
            if (canTouch) {
                InteractionType currentType = GlobalVariables.returnInteractionType(hitObject);
                switch (currentType) {
                    case InteractionType.Key:
                    case InteractionType.Item:
                        if (!playerInventory.inventoryFull) {
                            playerInventory.grabItem(hitObject);
                            playerUI.HUDMessageClear();
                        }
                        else
                            displayInventoryFullMessage(hitObject);
                        break;
                    case InteractionType.Door:
                        interactDoor(hitObject, hitPoint);
                        break;
                    default:
                        Debug.LogError("Player interacted with an object without a proper interaction type " + hitObject, hitObject);
                        break;
                }
            }
        }
    }

    public void displayInventoryFullMessage(GameObject currentObject) {
        print("Inventory full!");
    }

    public void interactDoor(GameObject currentDoor,Vector3 hitPoint) {
        Door door = currentDoor.transform.root.GetComponent<Door>();
        door.openCloseDoor(hitPoint);
        
    }

    public void Pause(InputAction.CallbackContext context) {
        if (paused) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            paused = false;
        } else {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            paused = true;
        }
    }
}
