using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public Text raycastMessage;

    public bool changeDoorMessage = true;
    private IEnumerator doorMessageTimer;

    public void HUDMessageDoor(string message, GameObject door) {
        if(changeDoorMessage)
            raycastMessage.text = message;

        if(message == "Door unlocked" || message == "Door is locked") {
            raycastMessage.text = message;
            changeDoorMessage = false;
            doorMessageTimer = WaitAndPrint(5f);
            StartCoroutine(doorMessageTimer);
        }

    }

    public IEnumerator WaitAndPrint(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        changeDoorMessage = true;
    }

    public void HUDMessageItem(GameObject item) {
        raycastMessage.text = item.GetComponent<CollectableItem>().title.ToString();
        changeDoorMessage = true;
    }

    public void HUDMessageFull() {
        raycastMessage.text = "Inventory Full";
        changeDoorMessage = true;
        //Replace this with an image of a texture that convies the idea the inventory is full instead. Something like a little red x 
    }

    public void HUDMessageClear() {
        raycastMessage.text = "";
        changeDoorMessage = true;
    }
}
