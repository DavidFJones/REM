using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    
    public static bool showDebugLinesGlobal;
    public bool showDebugLines = false;

    void Awake() {
        showDebugLinesGlobal = showDebugLines;
    }

    public static InteractionType returnInteractionType(GameObject currentItem) {
        if(currentItem.GetComponent<CollectableItem>()){
            return currentItem.GetComponent<CollectableItem>().type;
        }

        if(currentItem.transform.root.GetComponent<Door>()) {
            return InteractionType.Door;
        }


        return InteractionType.Null;
    }

}
