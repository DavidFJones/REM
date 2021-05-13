using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerButtonInputs : MonoBehaviour
{
    bool paused = false;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
