using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HammerPuzzle : MonoBehaviour
{
    public GameObject spawnParent;
    public List<Transform> spawnLocations;
    public GameObject key;

    void Awake() {

        foreach (Transform spawn in spawnParent.transform) {
            spawnLocations.Add(spawn);
        }


        int x = Random.Range(0, spawnLocations.Count - 1);
        Debug.Log(spawnLocations.Count);
        Debug.Log(x);
        key.transform.localPosition = spawnLocations[x].transform.localPosition;
        key.transform.localRotation = spawnLocations[x].transform.localRotation;

        Destroy(spawnParent);

    }
}
