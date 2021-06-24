using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectedPlayerPosition : MonoBehaviour
{
    public Transform reflectedPlayerPos;

    void FixedUpdate() {
        //The current position of our player
        Transform playerPos = SceneManager.Instance.player.transform;

        //Create an offset so the players are correctly positioned
        Vector3 offset = playerPos.position - transform.position;

        //move the reflected player relative to the player
        reflectedPlayerPos.transform.position = transform.position - offset;

        //Force the reflections height to match the players
        reflectedPlayerPos.transform.position = new Vector3(reflectedPlayerPos.transform.position.x, playerPos.position.y, reflectedPlayerPos.transform.position.z);
    }
}
