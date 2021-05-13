using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    /*
     viewBob **
     viewBobToggle *
     */
    [Header("Development Settings")]
    [SerializeField]
    [Tooltip("How fast the player speeds up (Needs to be a high value to work)")]
    private float accelertaion = 5500f;
    [SerializeField]
    [Tooltip("Caps the players maximum speed. Regardless of acceleration they cannot exceed this speed")]
    private float maxSpeed = 7f;
  

    private float currentSpeedMagnitute = 0;
    private Vector3 movement = Vector3.zero; // The sum of the players movement inputs

    private float sphereDistance; //How far our grounding spherecast travels
    private Vector3 origin; //Start point for our grounding spherecast
    private float sphereRadius; //Radius for our spherecast
    private float relativeStepHeight; //What the current step hight of the collided object is
    [SerializeField]
    [Tooltip("The highest step the player can walk up")]
    private float maxStepHeight = .2f;
    private float currentHitDistance; //How for our hit location is for our spherecast
    [SerializeField]
    [Tooltip("The maximum angle or 'slope' our character can walk up before being pulled against by gravity")]
    private float maxSlope = 40;
    [SerializeField]
    private float currentSlopeAngle; //Angle of the slope we are currently standing on
    [SerializeField]

    Rigidbody rb;
    CapsuleCollider playerCollider;

    void Start() {
        //Get our player's rigibody on start
        rb = gameObject.GetComponent<Rigidbody>();
        playerCollider = gameObject.GetComponent<CapsuleCollider>();
        //Sets the default radius and distance for our spherecast
        sphereRadius = playerCollider.radius + .2f;
        sphereDistance = playerCollider.height * 0.25f;
    }


    //Called when the player presses any movement direction
    public void Move(InputAction.CallbackContext context) {
        Vector2 move = context.ReadValue<Vector2>();
        //Sets the movement vector to equal the direction the player pushes
        movement = new Vector3(move.x, 0, move.y);
    }

    void FixedUpdate() {
        //Calculates if we are moving up a step
        RaycastHit hit;
        origin = playerCollider.bounds.center; //Sets the start position for our spherecast
        if (Physics.SphereCast(origin, sphereRadius, -transform.up, out hit, sphereDistance)) {
            //Disable the gravity and fix our drag/mass by default
            rb.useGravity = false;
            rb.drag = 5;
            rb.mass = 1;
            //checks the x and z rotation of the hit object
            float rotX = Mathf.Abs(hit.transform.rotation.x);
            float rotZ = Mathf.Abs(hit.transform.rotation.z);
            //if The x/z rotation exceeds... Than set the slope angle to the 
            //This is bad but i'm not actually sure the real number, roughly 10ish degrees?
            if(rotX > 0.1 || rotZ > 0.1) {
                currentSlopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            } else {
                currentSlopeAngle = 0;
            }

            currentHitDistance = hit.distance;
            relativeStepHeight = Mathf.Abs(playerCollider.bounds.min.y - hit.point.y);
            Debug.DrawLine(hit.point, hit.point + new Vector3(0, 2, 0), Color.black);
            if(relativeStepHeight <= maxStepHeight && currentSlopeAngle <= maxSlope) {
                Vector3 newPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y - 1f * currentHitDistance + 0.3f, gameObject.transform.position.z);
                playerCollider.transform.position = newPosition;
            } else if (currentSlopeAngle > maxSlope) {
                rb.useGravity = true;
                rb.drag = 0;
                rb.mass = 5;
            }
        } else {
            rb.useGravity = true;
            rb.drag = 0;
            rb.mass = 10;
            currentHitDistance = sphereDistance;
            currentSlopeAngle = 0;
        }
        
        

        rb.AddRelativeForce(movement * accelertaion * Time.deltaTime);

        //Clamp our max velocity to our max speed value
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);

        //check our current velocity
        currentSpeedMagnitute = rb.velocity.magnitude;

        //If the player has stopped moving, but the engine is still applying tiny math to the rb, force the rb velocity to 0
        if(currentSpeedMagnitute < 0.5f) {
            rb.velocity = new Vector3(0,rb.velocity.y,0);
            currentSpeedMagnitute = 0;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origin + -transform.up * currentHitDistance, sphereRadius);
    }
}
