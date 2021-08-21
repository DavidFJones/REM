using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorLock : MonoBehaviour
{
    public GameObject key;
 
    bool unlocked = false;
    public void openMirror() {
        foreach (GameObject item in SceneManager.Instance.player.inventory) {
            if (GameObject.ReferenceEquals(item, key)) {
                //This message needs to change to better reflect the mirror/lock relationship
                SceneManager.Instance.playerUI.HUDMessageUnlockedDoor();
                this.transform.gameObject.tag = "Untagged";
                unlocked = true;
                break;
            }
        }
        if (!unlocked) {
            //This message needs to change to better reflect the mirror/lock relationship
            SceneManager.Instance.playerUI.HUDMessageLockedDoor();
        }
            
    }

}
