using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PopMachinePuzzle : MonoBehaviour {

    public GameObject[] machines;
    public GameObject spawnParent;
    List<Transform> spawnLocations;
    public int count = 0;

    void Start() {

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
            Debug.Log("correct machine 1");
        } else if (count == 1 && currentMachine == machines[1]) {
            count++;
            Debug.Log("correct machine 2");
        } else if (count == 2 && currentMachine == machines[2]) {
            count++;
            Debug.Log("correct machine 3");
        } else {
            count = 0;
            Debug.Log("Give Pop, wrong machine/finished puzzle");
        }
    }
}
