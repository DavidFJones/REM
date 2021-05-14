using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public Text raycastMessage;

    public void HUDMessageDoor(GameObject door) {
        raycastMessage.text = "Interact";
        //This may be better to check the doors state and then either say "open" or "close"
    }

    public void HUDMessageItem(GameObject item) {
        raycastMessage.text = item.GetComponent<CollectableItem>().title.ToString();
    }

    public void HUDMessageFull() {
        raycastMessage.text = "Inventory Full";
        //Replace this with an image of a texture that convies the idea the inventory is full instead. Something like a little red x 
    }

    public void HUDMessageClear() {
        raycastMessage.text = "";
    }
}
