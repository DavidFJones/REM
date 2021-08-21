using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopCan : MonoBehaviour
{
    float lifeTime = 20;

    void Update() {
        lifeTime -= lifeTime * Time.deltaTime;
        if (lifeTime <= 0) {
            removeCan();
        }
    }

    void removeCan() {
        //Maybe we'll add some fx to this in the future...
        Destroy(this);
    }
}
