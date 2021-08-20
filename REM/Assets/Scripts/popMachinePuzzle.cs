using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class popMachinePuzzle : MonoBehaviour
{
    public GameObject[] machines;
    public GameObject spawnParent;
    List<Transform> spawnLocations;

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
}
