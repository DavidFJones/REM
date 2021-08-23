using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerDragControls : MonoBehaviour
{
    Camera playerCamera;//The players camera
    public PlayerInput playerInput;//Our players input action controller
    public GameObject kiosk;//The kiosk we are dragging
    Rigidbody rb;

    //Player Look/Camera Movement Code -----------------------------
    [Tooltip("How fast the player speeds up (Needs to be a high value to work)")]
    public float accelertaion = 100;
    private Vector2 lookDirection = Vector2.zero;
    private float rotationY = 0f;
    private float rotationX = 0f;
    Vector3 movement;
    public float maxSpeed = .5f;
    // -------------------------------------------------------------

    public Transform footstepContainer;// Where the footsteps sounds come from
    float defaultFootPos;//Where the footsteps right side position is
    bool footRight = true;//If the footstep is on our right or left
    bool playingFootSound = false;//Checks if we are currently playing our footstep sfx
    //--------------------------------------------------------------

   
    [Tooltip("The default sound we play for a players footstep")]
    public AudioClip defaultStepSound;//The default footstep sound
    // -------------------------------------------------------------

    public float tempCameraRotationY = 0;
    public float tempCameraRotationX = 0;

    void Awake() {
        //Get reference to our players camera
        playerCamera = Camera.main;
        //Get our player input action controller
        playerInput = gameObject.GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable() {
        //Clear the hud message
        SceneManager.Instance.playerUI.HUDMessageClear();
        //Set the players input map
        playerInput.SwitchCurrentActionMap("Dragging");
        //Forces the players rb to come to a complete stop
        SceneManager.Instance.player.rb.velocity = new Vector3(0,0,0);
        //Get the players camera position and stores it
        rotationX = transform.eulerAngles.y - 90f;
        rotationY = -playerCamera.transform.eulerAngles.x;
        //Snap the players rotation to face the object
        transform.eulerAngles = new Vector3(0,90,0);
        //Send the offset to the kiosk object
        Vector3 offset = transform.position - kiosk.transform.position;
        DragKiosk currentKiosk = kiosk.GetComponent<DragKiosk>();
        currentKiosk.enabled = true;
        currentKiosk.offset = offset;
        //disable collisions with the kiosk
        Physics.IgnoreCollision(kiosk.GetComponent<Collider>(), GetComponent<Collider>());
    }

    void OnDisable() {
        Physics.IgnoreCollision(kiosk.GetComponent<Collider>(), GetComponent<Collider>(), false);
        kiosk.GetComponent<DragKiosk>().enabled = false;
    }

    void Update() {
        //Clamps our vertical to prevent view flipping
        rotationY += lookDirection.y * SceneManager.Instance.player.verticalSensitivity * 0.1f;
        rotationY = Mathf.Clamp(rotationY, -90f, 90f);

        //Clamps our horizontal view
        rotationX += lookDirection.x * SceneManager.Instance.player.horizontalSensitivity * 0.1f;
        rotationX = Mathf.Clamp(rotationX, -70f, 70f);

        playerCamera.transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
    }

    void FixedUpdate() {
        //Pushes our player relative to the movement direction
        rb.AddRelativeForce(movement * accelertaion * Time.deltaTime);

        //This code clamps our x/z speed while maintaining our y
        //Saves our current y velocity
        float tempY = rb.velocity.y;

        if (rb.velocity.magnitude > maxSpeed) {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        }
        rb.velocity = new Vector3(rb.velocity.x, tempY, rb.velocity.z);
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
    public void Move(InputAction.CallbackContext context) {
        Vector2 move = context.ReadValue<Vector2>();
        //Sets the movement vector to equal the direction the player pushes
        movement = new Vector3(move.x, 0, move.y);
    }
    //Pause the game
    public void Pause(InputAction.CallbackContext context) {
        if (context.started) {
            SceneManager.Instance.playerUI.PauseGame();
        }
    }
    //Change the player's control scheme to normal walking
    void changeWalkControls() {
        SceneManager.Instance.player.GetComponent<PlayerWalkingControls>().enabled = true;
        this.enabled = false;
    }
    //Called when the player moves their camera
    public void Look(InputAction.CallbackContext context) {
        Vector2 look = context.ReadValue<Vector2>();
        lookDirection = new Vector2(look.x, look.y);
    }
    //Press the interact button
    public void Interact(InputAction.CallbackContext context) {
        if (context.started) {
            changeWalkControls();
        }
    }


}
