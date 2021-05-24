using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    bool paused = false;//Bool to determine if the game state is paused

    Camera playerCamera;//The players camera

    //Player Look/Camera Movement Code -----------------------------
    [Header("Look Sensitivity")]
    [Range(1, 10)]
    public int horizontalSensitivity = 3;
    [Range(1, 10)]
    public int verticalSensitivity = 3;
    private Vector2 lookDirection = Vector2.zero;
    private float rotationY = 0f;
    // -------------------------------------------------------------

    //Player Movement Code -----------------------------------------
    private float currentSpeedMagnitute = 0;//What our current speed magnitute is
    private Vector3 movement = Vector3.zero; // The sum of the players movement inputs

    private float sphereDistance; //How far our grounding spherecast travels
    private Vector3 origin; //Start point for our grounding spherecast
    private float sphereRadius; //Radius for our spherecast
    private float relativeStepHeight; //What the current step hight of the collided object is

    private float currentHitDistance; //How for our hit location is for our spherecast

    private float currentSlopeAngle; //Angle of the slope we are currently standing on

    Rigidbody rb;
    CapsuleCollider playerCollider;
    // -------------------------------------------------------------

    //Viewport Raycast Code ----------------------------------------
    private float cameraAngle;//The x (up/down) angle of the players camera
    private float maxDistance;//How for our raycast will travel
    
    bool canTouch = false;//Used to determine if the player is allowed to grab the item their raycast is hitting (This is based on the items tag)
    Vector3 hitPoint;//Point in world space where the players viewport raycast hit
    // --------------------------------------------------------------

    //Player Inventory Code ----------------------------------------
    [Header("Player Inventory")]
    [Tooltip("This is the list of items in the players inventory")]
    public List<GameObject> inventory;
    [HideInInspector]
    public bool inventoryFull = false;
    //--------------------------------------------------------------

    //Developer Options

    [Header("Developer Options")]
    //Player Inventory Code ----------------------------------------
    [SerializeField]
    [Tooltip("How many items the players inventory can hold in total")]
    private int inventoryLimit = 8;
    //Player Movement Code -----------------------------------------
    [Header("Movement")]
    [SerializeField]
    [Tooltip("How fast the player speeds up (Needs to be a high value to work)")]
    private float accelertaion = 5500f;
    [SerializeField]
    [Tooltip("Caps the players maximum speed. Regardless of acceleration they cannot exceed this speed")]
    private float maxSpeed = 7f;
    [SerializeField]
    [Tooltip("The highest step the player can walk up")]
    private float maxStepHeight = .2f;
    [SerializeField]
    [Tooltip("The maximum angle or 'slope' our character can walk up before being pulled against by gravity")]
    private float maxSlope = 40;
    //Viewport Raycast Code ----------------------------------------
    [Header("Raycast")]
    [SerializeField]
    [Tooltip("This changes the distance the raycast(red line from center of view) will travel. The raycast is also affected by the angle of the players camera (higher/lower angle means longer raycast)")]
    private float raycastDistance;//An editor value to modify the distance of the raycast
    [SerializeField]
    [Tooltip("This is what object we have currently hit with our raycast")]
    GameObject hitObject;//object we have currently hit with our raycast
    // -------------------------------------------------------------


    void Awake() {
        //This should be moved to a global game script maybe?
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Get reference to our players camera
        playerCamera = Camera.main;

        //Get our player's rigibody and collider on start
        rb = gameObject.GetComponent<Rigidbody>();
        playerCollider = gameObject.GetComponent<CapsuleCollider>();
        //Sets the default radius and distance for our ground spherecast
        sphereRadius = playerCollider.radius + .2f;
        sphereDistance = playerCollider.height * 0.25f;
    }

    void Update() {
        //Rotate our capsule collider/player horizontally based on player input
        gameObject.transform.Rotate(0, lookDirection.x * horizontalSensitivity * 0.1f, 0);

        //Clamps our vertical to prevent view flipping
        rotationY += lookDirection.y * verticalSensitivity * 0.1f;
        rotationY = Mathf.Clamp(rotationY, -90f, 90f);

        playerCamera.transform.localEulerAngles = new Vector3(-rotationY, gameObject.transform.rotation.x, 0);
    }
    void FixedUpdate() {
        //This is the angle of our camera. Without needing to convert 90deg up or down is equal to .7
        cameraAngle = Mathf.Abs(playerCamera.transform.localRotation.x);

        maxDistance = raycastDistance + cameraAngle * 1.4f;//Sets the actual distance of our raycast based on the editor value and the angle of the camera

        //Determines what we are looking at ---------------------------------
        RaycastHit hit;

        //If we have enabled debug lines, show the raycast
        if (SceneManager.Instance.showDebugLines)
            Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * maxDistance, Color.red);

        //Checks to see if our player is looking at something within the max distance
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, maxDistance)) {
            hitObject = hit.transform.gameObject;//Sets the stored object we just hit

            //If we are looking at an item we can interact with
            if (hitObject.tag == "Interactable") {
                canTouch = true;
                hitPoint = hit.transform.InverseTransformPoint(hit.point);
                InteractionType currentType = SceneManager.returnInteractionType(hitObject);
                switch (currentType) {
                    case InteractionType.Key:
                    case InteractionType.Item:
                        if (!inventoryFull) {
                            SceneManager.Instance.playerUI.HUDMessageItem(hitObject);
                        } else {
                            SceneManager.Instance.playerUI.HUDMessageFull();
                        }
                        break;
                    case InteractionType.Door:
                        SceneManager.Instance.playerUI.HUDMessageDoor("Interact", hitObject.transform.parent.transform.parent.gameObject);
                        break;
                    default:
                        Debug.LogError("Player looked at an interactive object without a proper interaction type " + hitObject, hitObject);
                        break;
                }

            }


        } else {
            hitObject = null;
            canTouch = false;
            SceneManager.Instance.playerUI.HUDMessageClear();
        }
        //---------------------------------------------------------------

        // Player grounding sphere calculations -------------------------
        //Calculates if we are moving up a step
        RaycastHit hitFloor;
        origin = playerCollider.bounds.center; //Sets the start position for our spherecast
        if (Physics.SphereCast(origin, sphereRadius, -transform.up, out hitFloor, sphereDistance)) {
            //Disable the gravity and fix our drag/mass by default
            rb.useGravity = false;
            rb.drag = 5;
            rb.mass = 1;
            //checks the x and z rotation of the hit object
            float rotX = Mathf.Abs(hitFloor.transform.rotation.x);
            float rotZ = Mathf.Abs(hitFloor.transform.rotation.z);
            //if The x/z rotation exceeds... Than set the slope angle to the top of the hit normal
            //This is bad but i'm not actually sure the real number, roughly 10ish degrees?
            if (rotX > 0.1 || rotZ > 0.1) {
                currentSlopeAngle = Vector3.Angle(hitFloor.normal, Vector3.up);
            } else {
                currentSlopeAngle = 0;
            }

            currentHitDistance = hitFloor.distance;//Sets our hit distance 
            relativeStepHeight = Mathf.Abs(playerCollider.bounds.min.y - hitFloor.point.y);//Checks how tall the object we wish to step on is

            //Verifies that the step we are climbing a step that doesn't exceed the max height, or a slope
            if (relativeStepHeight <= maxStepHeight && currentSlopeAngle <= maxSlope) {
                Vector3 newPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y - 1f * currentHitDistance + 0.3f, gameObject.transform.position.z);
                //Snaps the player to the ground
                playerCollider.transform.position = newPosition;

                //If the current angle exeeds our max slope value, turn on gravity to start pulling the player down
            } else if (currentSlopeAngle > maxSlope) {
                rb.useGravity = true;
                rb.drag = 0;
                rb.mass = 5;
            }
        } else {
            //If we are not touching a ground surface, enable gravity to the player can fall
            rb.useGravity = true;
            rb.drag = 0;
            rb.mass = 10;
            currentHitDistance = sphereDistance;
            currentSlopeAngle = 0;
        }

        //Pushes our player relative to the movement direction
        rb.AddRelativeForce(movement * accelertaion * Time.deltaTime);

        //Clamp our max velocity to our max speed value
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);

        //check our current velocity
        currentSpeedMagnitute = rb.velocity.magnitude;

        //If the player has stopped moving, but the engine is still applying tiny math to the rb, force the rb velocity to 0
        if (currentSpeedMagnitute < 0.5f) {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            currentSpeedMagnitute = 0;
        }
        //---------------------------------------------------------------
    }

    //Called when the player presses the interaction button
    public void Interact(InputAction.CallbackContext context) {
        //If we pressed the interact button
        if (context.started) {
            //And we are looking at an interactive item, do the thing
            if (canTouch) {
                InteractionType currentType = SceneManager.returnInteractionType(hitObject);
                switch (currentType) {
                    case InteractionType.Key:
                    case InteractionType.Item:
                        if (!inventoryFull) {
                            grabItem(hitObject);
                            SceneManager.Instance.playerUI.HUDMessageClear();
                        } else {
                            //Our inventory is full
                            //Display this message the the player
                        }
                        break;
                    case InteractionType.Door:
                        interactDoor(hitObject, hitPoint, transform.position);
                        break;
                    default:
                        Debug.LogError("Player interacted with an object without a proper interaction type " + hitObject, hitObject);
                        break;
                }
            }
        }
    }
    //Interact with the door we are looking at and see if it should be opened/unlocked
    public void interactDoor(GameObject currentDoor, Vector3 hitPoint, Vector3 playerPos) {
        Door door = currentDoor.transform.parent.transform.parent.GetComponent<Door>();
        door.openCloseDoor(hitPoint, playerPos);

    }
    //Called when the player presses any movement direction
    public void Move(InputAction.CallbackContext context) {
        Vector2 move = context.ReadValue<Vector2>();
        //Sets the movement vector to equal the direction the player pushes
        movement = new Vector3(move.x, 0, move.y);
    }
    //Pause the game
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
    //Grab the selected object and attempt to put it in our inventory
    public void grabItem(GameObject item) {
        if (inventory.Count < inventoryLimit) {
            inventory.Add(item);
            item.SetActive(false);
        } else {
            print("I can't pick that up right now...");
        }

    }
    //Called when the player moves their camera
    public void Look(InputAction.CallbackContext context) {
        Vector2 look = context.ReadValue<Vector2>();
        lookDirection = new Vector2(look.x, look.y);
    }
}
