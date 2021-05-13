using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerLook : MonoBehaviour
{
    [Range(1,10)]
    public int horizontalSensitivity = 3;
    [Range(1, 10)]
    public int verticalSensitivity = 3;

    Camera playerCamera;
    private Vector2 lookDirection = Vector2.zero;
    private float rotationY = 0f;

    void Start() {
        playerCamera = Camera.main;
    }

    //Called when the player moves their camera
    public void Look(InputAction.CallbackContext context) {
        Vector2 look = context.ReadValue<Vector2>();
        lookDirection = new Vector2(look.x, look.y);
    }

    void Update() {
        //Rotate our capsule collider/player horizontally based on player input
        gameObject.transform.Rotate(0,lookDirection.x * horizontalSensitivity * 0.1f, 0);


        //Clamps our vertical to prevent view flipping
        rotationY += lookDirection.y * verticalSensitivity * 0.1f;
        rotationY = Mathf.Clamp(rotationY, -90f, 90f);

        playerCamera.transform.localEulerAngles = new Vector3(-rotationY, gameObject.transform.rotation.x, 0);
    }
}
