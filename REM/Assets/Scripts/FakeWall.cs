using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeWall : MonoBehaviour
{
    [HideInInspector]
    public bool wallVisible = false;

    public GameObject reflectedPlayer;

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Renderer>().IsVisibleFrom(Camera.main)) {
            wallVisible = true;
            this.GetComponent<Renderer>().enabled = true;
        } else {
            wallVisible = false;
            this.GetComponent<Renderer>().enabled = false;
        }
    }

    void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            Destroy(reflectedPlayer.GetComponent<ReflectedPlayer>());
            this.GetComponent<Renderer>().enabled = false;
            Destroy(this);
        }
    }
}
