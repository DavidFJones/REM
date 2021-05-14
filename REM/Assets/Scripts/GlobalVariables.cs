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
        CollectableItem itemType;
        if(itemType = currentItem.GetComponent<CollectableItem>()){
            return itemType.type;
        }
        /*This will be used when doors exist
        if(itemType = currentItem.GetComponent<InteractiveDoor>()) {
            return itemType.type;
        }*/


        return InteractionType.Null;
    }

}
