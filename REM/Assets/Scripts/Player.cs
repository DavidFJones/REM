using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Camera playerCamera;//The players camera
    public PlayerInput playerInput;//Our players input action controller
    public string currentDeviceType;//What input device our player is using/last used

    //Player Look/Camera Movement Code -----------------------------
    [Header("Look Sensitivity")]
    [Range(1, 10)]
    public int horizontalSensitivity = 3;
    [Range(1, 10)]
    public int verticalSensitivity = 3;
    public float defaultCamPosY = 0;
    // -------------------------------------------------------------

    //Player Movement Code -----------------------------------------
    [Tooltip("Caps the players maximum speed. Regardless of acceleration they cannot exceed this speed")]
    public float maxSpeed = 7f;
    public Vector3 movement = Vector3.zero; // The sum of the players movement inputs
    public Rigidbody rb;
    public CapsuleCollider playerCollider;
    // -------------------------------------------------------------


    //Player Inventory Code ----------------------------------------
    [Header("Player Inventory")]
    [Tooltip("This is the list of items in the players inventory")]
    public List<GameObject> inventory;
    [HideInInspector]
    public bool inventoryFull = false;
    [Tooltip("How many items the players inventory can hold in total")]
    public int inventoryLimit = 8;
    //--------------------------------------------------------------

    //Health code --------------------------------------------------
    [Tooltip("Players current health")]
    public int currentHealth;//Players current health
    //--------------------------------------------------------------

    //Audio code ---------------------------------------------------
    public AudioSource audioSource;//Audio source for player sfx
    //--------------------------------------------------------------

    //Fixes for the bad button highlighting ------------------------
    Selectable[] selectables;
    //--------------------------------------------------------------

    //Developer Options --------------------------------------------
    [Header("Developer Options")]
    //Health code --------------------------------------------------
    [Tooltip("How much health does the player start with?")]
    public int startingHealth = 3;
    //--------------------------------------------------------------

    //Audio code ---------------------------------------------------
    [Header("Audio")]
    [Tooltip("The default sound we play for a players footstep")]
    public AudioClip defaultStepSound;//The default footstep sound
    // -------------------------------------------------------------
    //Dragging code ---------------------------------------------------
    public bool canDrag = false;//Bool used to determine if we are standing inside a dragable zone
    // -------------------------------------------------------------

   
    void Awake() {
        //This should be moved to a global game script maybe?
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Get reference to our players camera
        playerCamera = Camera.main;
        //Set's the default position/height for the camera
        defaultCamPosY = playerCamera.transform.localPosition.y;

        //Get our player input action controller
        playerInput = gameObject.GetComponent<PlayerInput>();
        currentDeviceType = playerInput.currentControlScheme;

        //Get our player's rigibody and collider on start
        rb = gameObject.GetComponent<Rigidbody>();
        playerCollider = gameObject.GetComponent<CapsuleCollider>();

        //Sets the players current health
        currentHealth = startingHealth;
    }

    //Called when we are hit by an enemy
    public void enemyHit() {
        print("we have been hit!");
        currentHealth--;
        if (currentHealth <= 0)
            die();
    }
    //called when the player is killed
    public void die() {
        print("We are dead");
    }
}
