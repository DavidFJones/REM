using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [HideInInspector]
    public Player player;
    [HideInInspector]
    public UIManager playerUI;
    [HideInInspector]
    public AudioManager audioManager;

   
    public bool gamePaused = false;//Controls whether the games state is currently paused or not

    //Allows us to show our debug lines globally
    public bool showDebugLines = false;

    //Instance of this Scene Manager
    public static SceneManager Instance { get; private set; }

    void Awake() {
        //Make this currenet scene manager the main scenemanager instance
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        //Cache references to all glogbally accessable classes
        audioManager = FindObjectOfType<AudioManager>();
        playerUI = FindObjectOfType<UIManager>();
        player = FindObjectOfType<Player>();

    }

    //Returns the type of object the player interacted with
    public static InteractionType returnInteractionType(GameObject currentItem) {
        if (currentItem.GetComponent<CollectableItem>()) {
            return currentItem.GetComponent<CollectableItem>().type;
        }

        if (currentItem.transform.parent.transform.parent.GetComponent<Door>()) {
            return InteractionType.Door;
        }

        return InteractionType.Null;
    }

    //Pauses the game
    public bool PauseGame() {
        //If we are  paused, change the game state to unpaused
        if (gamePaused) {
            gamePaused = false;
            Time.timeScale = 1f;
            SceneManager.Instance.player.playerInput.SwitchCurrentActionMap("Player");
            return gamePaused;
        }
        //We are not paused, pause the gamestate
        else {
            gamePaused = true;
            Time.timeScale = 0f;
            SceneManager.Instance.player.playerInput.SwitchCurrentActionMap("UI");
            return gamePaused;
        }  
    }

}
