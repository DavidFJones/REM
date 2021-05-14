using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Tooltip("How many items the players inventory can hold in total")]
    private float inventorySize = 8;
    [Tooltip("This is the list of items in the players inventory")]
    public List<GameObject> inventory;
    [HideInInspector]
    public bool inventoryFull = false;
    public void grabItem(GameObject item) {
        if(inventory.Count < inventorySize) {
            inventory.Add(item);
            item.SetActive(false);
        } else {
            print("I can't pick that up right now...");
        }
        
    }
}
