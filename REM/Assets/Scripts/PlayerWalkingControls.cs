using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerWalkingControls : MonoBehaviour
{
    Camera playerCamera;//The players camera
    public PlayerInput playerInput;//Our players input action controller

    //Player Look/Camera Movement Code -----------------------------
    private Vector2 lookDirection = Vector2.zero;
    private float rotationY = 0f;
    // -------------------------------------------------------------

    //Player Movement Code -----------------------------------------
    private Vector3 origin; //Start point for our grounding spherecast
    private float relativeStepHeight; //What the current step hight of the collided object is

    private float currentHitDistance; //How for our hit location is for our spherecast

    private float currentSlopeAngle; //Angle of the slope we are currently standing on
    // -------------------------------------------------------------

    // ViewBobbing Code --------------------------------------------
    float viewBobTimer = 0;
    // -------------------------------------------------------------

    //Viewport Raycast Code ----------------------------------------
    private float cameraAngle;//The x (up/down) angle of the players camera
    private float maxDistance;//How for our raycast will travel

    bool canTouch = false;//Used to determine if the player is allowed to grab the item their raycast is hitting (This is based on the items tag)
    Vector3 hitPoint;//Point in world space where the players viewport raycast hit
    // --------------------------------------------------------------

    //Player Movement Code -----------------------------------------
    [Header("Movement")]
    [SerializeField]
    [Tooltip("How fast the player speeds up (Needs to be a high value to work)")]
    private float accelertaion = 5500f;
    
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
    // ViewBobbing Code --------------------------------------------
    [Header("View Bobbing")]
    [SerializeField]
    [Tooltip("How long it takes for us to complete a full view bob")]
    private float walkingBobbingSpeed = 14f;
    [SerializeField]
    [Tooltip("How much the camera bobs up and down")]
    private float bobbingAmount = 0.05f;
    // -------------------------------------------------------------

    //Audio code ---------------------------------------------------
    public Transform footstepContainer;// Where the footsteps sounds come from
    bool footRight = true;//If the footstep is on our right or left
    bool playingFootSound = false;//Checks if we are currently playing our footstep sfx
    // -------------------------------------------------------------

    public float tempCameraRotationY = 0;
    public float tempCameraRotationX = 0;


    void Awake() {
        //Get reference to our players camera
        playerCamera = Camera.main;
        //Get our player input action controller
        playerInput = gameObject.GetComponent<PlayerInput>();
    }

    void OnEnable() {
        //Set our input map
        playerInput.SwitchCurrentActionMap("Walking");
        SceneManager.Instance.player.rb.velocity = new Vector3(0, 0, 0);
        //Get the current camera rotation and change the player body to match that
        transform.eulerAngles = new Vector3(0, playerCamera.transform.eulerAngles.y, 0);
        playerCamera.transform.eulerAngles = new Vector3(playerCamera.transform.eulerAngles.x, 0,0);
    }
    void Update() {
        //Rotate our capsule collider/player horizontally based on player input
        gameObject.transform.Rotate(0, lookDirection.x * SceneManager.Instance.player.horizontalSensitivity * 0.1f, 0);

        //Clamps our vertical to prevent view flipping
        rotationY += lookDirection.y * SceneManager.Instance.player.verticalSensitivity * 0.1f;
        rotationY = Mathf.Clamp(rotationY, -90f, 90f);

        SceneManager.Instance.player.playerCamera.transform.localEulerAngles = new Vector3(-rotationY, gameObject.transform.rotation.x, 0);
    }
    void FixedUpdate() {
        //This is the angle of our camera. Without needing to convert 90deg up or down is equal to .7
        cameraAngle = Mathf.Abs(SceneManager.Instance.player.playerCamera.transform.localRotation.x);

        maxDistance = raycastDistance + cameraAngle * 1.4f;//Sets the actual distance of our raycast based on the editor value and the angle of the camera

        //Determines what we are looking at ---------------------------------
        RaycastHit hit;

        //If we have enabled debug lines, show the raycast
        if (SceneManager.Instance.showDebugLines)
            Debug.DrawRay(SceneManager.Instance.player.playerCamera.transform.position, SceneManager.Instance.player.playerCamera.transform.forward * maxDistance, Color.red);

        //Checks to see if our player is looking at something within the max distance
        if (Physics.Raycast(SceneManager.Instance.player.playerCamera.transform.position, SceneManager.Instance.player.playerCamera.transform.forward, out hit, maxDistance)) {
            hitObject = hit.transform.gameObject;//Sets the stored object we just hit

            //If we are looking at an item we can interact with
            if (hitObject.tag == "Interactable") {
                canTouch = true;
                hitPoint = hit.transform.InverseTransformPoint(hit.point);
                InteractionType currentType = hitObject.GetComponent<interactionType>().type;
                switch (currentType) {
                    case InteractionType.Key:
                    case InteractionType.Item:
                        if (!SceneManager.Instance.player.inventoryFull) {
                            SceneManager.Instance.playerUI.HUDMessageItem(hitObject);
                        } else {
                            SceneManager.Instance.playerUI.HUDMessageFull();
                        }
                        break;
                    case InteractionType.Door:
                    case InteractionType.Pop:
                    case InteractionType.Mirror_lock:
                        SceneManager.Instance.playerUI.HUDMessageInteract();
                        break;
                    case InteractionType.Drag:
                        if (SceneManager.Instance.player.canDrag) {
                            SceneManager.Instance.playerUI.HUDMessageInteract();
                        }
                        break;
                    default:
                        Debug.LogError("Player looked at an interactive object without a proper interaction type " + hitObject, hitObject);
                        break;
                }

            } else {
                hitObject = null;
                canTouch = false;
                SceneManager.Instance.playerUI.HUDMessageClear();
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
        origin = SceneManager.Instance.player.playerCollider.bounds.center; //Sets the start position for our spherecast
        if (Physics.SphereCast(origin, SceneManager.Instance.player.playerCollider.radius + .2f, -transform.up, out hitFloor, SceneManager.Instance.player.playerCollider.height * 0.25f)) {
            //Disable the gravity and fix our drag/mass by default
            SceneManager.Instance.player.rb.useGravity = false;
            SceneManager.Instance.player.rb.drag = 5;
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
            relativeStepHeight = Mathf.Abs(SceneManager.Instance.player.playerCollider.bounds.min.y - hitFloor.point.y);//Checks how tall the object we wish to step on is

            //Verifies that the step we are climbing a step that doesn't exceed the max height, or a slope
            if (relativeStepHeight <= maxStepHeight && currentSlopeAngle <= maxSlope) {
                Vector3 newPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y - 1f * currentHitDistance + 0.3f, gameObject.transform.position.z);
                //Snaps the player to the ground
                SceneManager.Instance.player.playerCollider.transform.position = newPosition;

                //If the current angle exeeds our max slope value, turn on gravity to start pulling the player down
            } else if (currentSlopeAngle > maxSlope) {
                SceneManager.Instance.player.rb.useGravity = true;
                SceneManager.Instance.player.rb.drag = 0;
            }
        } else {
            //If we are not touching a ground surface, enable gravity to the player can fall
            SceneManager.Instance.player.rb.useGravity = true;
            SceneManager.Instance.player.rb.drag = 0;
            currentHitDistance = SceneManager.Instance.player.playerCollider.height * 0.25f;
            currentSlopeAngle = 0;
        }

        //Pushes our player relative to the movement direction
        SceneManager.Instance.player.rb.AddRelativeForce(SceneManager.Instance.player.movement * accelertaion * Time.deltaTime);

        //This code clamps our x/z speed while maintaining our y
        //Saves our current y velocity
        float tempY = SceneManager.Instance.player.rb.velocity.y;

        if (SceneManager.Instance.player.rb.velocity.magnitude > SceneManager.Instance.player.maxSpeed) {
            SceneManager.Instance.player.rb.velocity = Vector3.ClampMagnitude(SceneManager.Instance.player.rb.velocity, SceneManager.Instance.player.maxSpeed);
        }
        SceneManager.Instance.player.rb.velocity = new Vector3(SceneManager.Instance.player.rb.velocity.x, tempY, SceneManager.Instance.player.rb.velocity.z);

        //---------------------------------------------------------------

        // ViewBobbing Code  & footseps audio code ----------------------

        //Checks to see if we are moving x or z
        if (Mathf.Abs(SceneManager.Instance.player.rb.velocity.x) > 0.1f || Mathf.Abs(SceneManager.Instance.player.rb.velocity.z) > 0.1f) {
            viewBobTimer += Time.deltaTime * (walkingBobbingSpeed + SceneManager.Instance.player.maxSpeed);
            //Checks to see if we have view bobbing enabled in the settings/scene manager
            if (SceneManager.Instance.viewBob) {
                SceneManager.Instance.player.playerCamera.transform.localPosition = new Vector3(SceneManager.Instance.player.playerCamera.transform.localPosition.x, SceneManager.Instance.player.defaultCamPosY + Mathf.Sin(viewBobTimer) * bobbingAmount, SceneManager.Instance.player.playerCamera.transform.localPosition.z);
            }
            //Code to control audio
            if (Mathf.Sin(viewBobTimer) <= -0.3f) {
                //checks to see if we are currently playing our footstep sounds
                //This has a .25 second delay to prevent footstep sounds from overlapping
                if (!playingFootSound) {
                    StartCoroutine(FootstepSoundPlayer());
                }
            }
        } else { // We are not moving
            viewBobTimer = 0;
            if (SceneManager.Instance.viewBob) {
                SceneManager.Instance.player.playerCamera.transform.localPosition = new Vector3(SceneManager.Instance.player.playerCamera.transform.localPosition.x, Mathf.Lerp(SceneManager.Instance.player.playerCamera.transform.localPosition.y, SceneManager.Instance.player.defaultCamPosY, Time.deltaTime * (walkingBobbingSpeed + SceneManager.Instance.player.maxSpeed)), SceneManager.Instance.player.playerCamera.transform.localPosition.z);
            }
        }

        // -------------------------------------------------------------
    }

    //Called when the player presses the interaction button
    public void Interact(InputAction.CallbackContext context) {
        //If we pressed the interact button
        if (context.started) {
            //And we are looking at an interactive item, do the thing
            if (canTouch) {
                InteractionType currentType = hitObject.GetComponent<interactionType>().type;
                switch (currentType) {
                    case InteractionType.Key:
                    case InteractionType.Item:
                        if (!SceneManager.Instance.player.inventoryFull) {
                            grabItem(hitObject);
                            SceneManager.Instance.playerUI.HUDMessageClear();
                        } else {
                            //Our inventory is full
                            //Display this message to the player
                        }
                        break;
                    case InteractionType.Door:
                        interactDoor(hitObject, hitPoint, transform.position);
                        break;
                    case InteractionType.Pop:
                        SceneManager.Instance.popPuzzleManager.grabPopMachine(hitObject);
                        break;
                    case InteractionType.Mirror_lock:
                        hitObject.GetComponent<MirrorLock>().openMirror();
                        break;
                    case InteractionType.Drag:
                        if (SceneManager.Instance.player.canDrag) {
                            changeDragControls();
                        }
                        break;
                    default:
                        Debug.LogError("Player interacted with an object without a proper interaction type " + hitObject, hitObject);
                        break;
                }
            }
        }
    }
    //Plays our footstep sounds and prevents it from playing overitself
    IEnumerator FootstepSoundPlayer() {
        //Set bool to prevent this coroutine from playing before the sound is done
        playingFootSound = true;

        SceneManager.Instance.player.audioSource.pitch = Random.Range(0.75f, 1.25f);

        //Sets which side the sound should be played on
        if (footRight) {
            footRight = false;
            footstepContainer.localPosition = new Vector3(footstepContainer.localPosition.x * -1, footstepContainer.localPosition.y, footstepContainer.localPosition.z);
            SceneManager.Instance.player.audioSource.Play();
        } else {
            footRight = true;
            footstepContainer.localPosition = new Vector3(footstepContainer.localPosition.x * -1, footstepContainer.localPosition.y, footstepContainer.localPosition.z);
            SceneManager.Instance.player.audioSource.Play();
        }

        //Wait for 0.25 seconds
        yield return new WaitForSeconds(0.25f);

        //Set bool to false to allow this coroutine to run again
        playingFootSound = false;

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
        SceneManager.Instance.player.movement = new Vector3(move.x, 0, move.y);
    }
    //Pause the game
    public void Pause(InputAction.CallbackContext context) {
        Debug.Log("pause");
        if (context.started) {
            SceneManager.Instance.playerUI.PauseGame();
        }

    }
    //Changes the player's control scheme to drag an object
    void changeDragControls() {
        SceneManager.Instance.player.GetComponent<PlayerDragControls>().enabled = true;
        this.enabled = false;
    }
    //Called when we press the back button while in the pause menu
    //THIS CURRENTLY DOESN'T WORK
    public void Back(InputAction.CallbackContext context) {
        print("here");
        if (context.started) {
            switch (SceneManager.Instance.playerUI.state) {
                case (PauseState.Pause):
                    SceneManager.Instance.playerUI.PauseGame();
                    break;
                case (PauseState.Options):
                    SceneManager.Instance.playerUI.CloseOptions();
                    break;
                case (PauseState.Quit):
                    SceneManager.Instance.playerUI.CloseQuit();
                    break;
                default:
                    break;
            }

        }
    }
    //Grab the selected object and attempt to put it in our inventory
    public void grabItem(GameObject item) {
        if (SceneManager.Instance.player.inventory.Count < SceneManager.Instance.player.inventoryLimit) {
            SceneManager.Instance.player.inventory.Add(item);
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
