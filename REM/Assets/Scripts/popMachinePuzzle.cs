using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PopMachinePuzzle : MonoBehaviour {

    public GameObject[] machines;
    public GameObject spawnParent;
    public List<Transform> spawnLocations;
    public int count = 0;
    public GameObject popCan;
    public GameObject key;
    public AudioClip insertCoinSound;
    public AudioClip getPopSound;

    void Awake() {

        foreach (Transform spawn in spawnParent.transform) {
            spawnLocations.Add(spawn);
        }

        spawnLocations = spawnLocations.OrderBy(i => Random.value).ToList();

        for (int i = 0; i < machines.Length; i++) {
            machines[i].transform.position = spawnLocations[i].position;
            machines[i].transform.rotation = spawnLocations[i].rotation;
        }

        Destroy(spawnParent);

    }

    public void grabPopMachine(GameObject currentMachine) {
        if (count == 0 && currentMachine == machines[0]) {
            count++;
            correctMachine();
        } else if (count == 1 && currentMachine == machines[1]) {
            count++;
            correctMachine();
        } else if (count == 2 && currentMachine == machines[2]) {
            count++;
            winPopPuzzle(currentMachine);
        } else {
            count = 0;
            giveSoda(currentMachine);
        }
    }
    void correctMachine() {
        //Play Correct Sound Noise
    }
    void giveSoda(GameObject currentMachine) {
        //Spawn soda at the correct location
        Instantiate(popCan, currentMachine.transform.GetChild(1));
    }
    void winPopPuzzle(GameObject currentMachine) {
        correctMachine();
        //Drop the key for the player
        key.SetActive(true);
        key.transform.position = currentMachine.transform.GetChild(1).transform.position;
        key.transform.Rotate(Vector3.forward);
        key.GetComponent<Rigidbody>().isKinematic = false;
        key.GetComponent<Rigidbody>().useGravity = true;
    }
}
