using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    //public PlayerController playerController;
    [HideInInspector]
    public PlayerInventory playerInventory;
    [HideInInspector]
    public UIHandler uIManager;
    [HideInInspector]
    public AudioManager audioManager;

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
        uIManager = FindObjectOfType<UIHandler>();
        playerInventory = FindObjectOfType<PlayerInventory>();

    }

    //Returns the type of object the player interacted with
    public static InteractionType returnInteractionType(GameObject currentItem) {
        if (currentItem.GetComponent<CollectableItem>()) {
            return currentItem.GetComponent<CollectableItem>().type;
        }

        if (currentItem.transform.root.GetComponent<Door>()) {
            return InteractionType.Door;
        }

        return InteractionType.Null;
    }

}
