using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text hudText;//The central text element of our hud during standard gameplay

    public GameObject gameplayHUD;//The container for our gameplay hud
    public Image crossHair;//That image/location of crosshair. This image will render one of several sprites at the center of the screen

    public Sprite defaultCrossHair;//The default crosshair image on our hud
    public Sprite handCrossHair;//The hand crosshair icon on our hud
    public Sprite lockCrossHair;//The lock crosshair icon on our hud

    private bool holdMessage = false;//Bool used to hold a specific message on the hud. This will prevent any other crosshair/ui text changes from occuring while true

    HUDMessages lastMessage;//Used to see what our last message was
    GameObject lastItem;//The last item we looked at
    
    //used as a timer to set how long holdmessage bool is true for
    IEnumerator HoldTimer(float waitTime) {
        holdMessage = true;
        yield return new WaitForSeconds(waitTime);
        holdMessage = false;
    }
    //Displays the name of the item the player is looking at
    public void HUDMessageItem(GameObject item) {
        if (!holdMessage && item != lastItem) {
            lastItem = item;
            hudText.text = item.GetComponent<CollectableItem>().title.ToString();
            crossHair.rectTransform.sizeDelta = new Vector2(25, 25);
            crossHair.sprite = handCrossHair;
            lastMessage = HUDMessages.Item;
        }
    }
    //Called when the player attempts to grab an item while their inventory is full
    public void HUDMessageFull() {
        hudText.text = "Inventory Full";
        //Replace this with an image of a texture that convies the idea the inventory is full instead. Something like a little red x 
        lastMessage = HUDMessages.InventoryFull;
        StartCoroutine(HoldTimer(2f));
    }
    //Called when we are not looking at any interactive object. This resets the hud to default
    public void HUDMessageClear() {
        if (!holdMessage && lastMessage != HUDMessages.None) {
            hudText.text = "";
            crossHair.sprite = defaultCrossHair;
            crossHair.rectTransform.sizeDelta = new Vector2(5, 5);
            lastMessage = HUDMessages.None;
        } 
    }
    //Called when the player is looking at an object they can interact with
    public void HUDMessageInteract() {
        if (!holdMessage && lastMessage != HUDMessages.Interact) {
            hudText.text = "Interact";
            crossHair.sprite = handCrossHair;
            crossHair.rectTransform.sizeDelta = new Vector2(25, 25);
            lastMessage = HUDMessages.Interact;
        }
    }
    //Called when we try to open a locked door
    public void HUDMessageLockedDoor() {
        StartCoroutine(HoldTimer(2f));
        hudText.text = "Door is locked";
        crossHair.sprite = lockCrossHair;
        crossHair.rectTransform.sizeDelta = new Vector2(20, 20);
        lastMessage = HUDMessages.LockedDoor;
    }
    //Called when we open a door
    public void HUDMessageUnlockedDoor() {
        StartCoroutine(HoldTimer(2f));
        hudText.text = "Door unlocked";
        crossHair.sprite = lockCrossHair;//This needs to be changed to an unlocking animation/icon
        crossHair.rectTransform.sizeDelta = new Vector2(20, 20);
        lastMessage = HUDMessages.UnlockDoor;
    }
    //Called when we want to force the hud to stop displaying any message
    public void HUDMessageForceClear() {
        lastMessage = HUDMessages.None;
        holdMessage = false;
    }

}
