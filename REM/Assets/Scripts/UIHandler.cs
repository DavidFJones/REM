using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public Text raycastMessage;

    public GameObject gameplayHUD;
    public Image crossHair;

    public Sprite crossHairImage;
    public Sprite handCrossHair;
    public Sprite lockCrossHair;


    public bool changeDoorMessage = true;
    private IEnumerator doorMessageTimer;

    public void HUDMessageDoor(string message, GameObject door) {
        if(changeDoorMessage)
            raycastMessage.text = message;
            crossHair.sprite = handCrossHair;
            crossHair.rectTransform.sizeDelta = new Vector2(25, 25);

        if (message == "Door unlocked" || message == "Door is locked") {
            raycastMessage.text = message;
            crossHair.sprite = lockCrossHair;
            crossHair.rectTransform.sizeDelta = new Vector2(20, 20);
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
        if (changeDoorMessage) {
            raycastMessage.text = item.GetComponent<CollectableItem>().title.ToString();
            changeDoorMessage = true;
            crossHair.rectTransform.sizeDelta = new Vector2(25, 25);
            crossHair.sprite = handCrossHair;
        }
        
    }

    public void HUDMessageFull() {
        raycastMessage.text = "Inventory Full";
        changeDoorMessage = true;
        //Replace this with an image of a texture that convies the idea the inventory is full instead. Something like a little red x 
    }

    public void HUDMessageClear() {
        raycastMessage.text = "";
        changeDoorMessage = true;
        crossHair.sprite = crossHairImage;
        crossHair.rectTransform.sizeDelta = new Vector2(5, 5); 
    }
}
