using UnityEngine;
using UnityEngine.Playables;

public class TimelineTrigger : MonoBehaviour
{
    PlayableDirector timeline;
    [SerializeField]
    private bool triggered = false;

    void Start() {
        timeline = GetComponent<PlayableDirector>();
    }

    void OnTriggerEnter(Collider col) {
        if(col.tag == "Player" && !triggered) {
            triggered = true;
            timeline.Play();
        }
        
    }
}
