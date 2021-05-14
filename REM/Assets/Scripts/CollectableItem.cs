using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectableItem : MonoBehaviour
{
    [Tooltip("The title of the given object. Will show up on the inventory ui")]
    public string title;
    [Tooltip("The description of the given object. Will show up on the inventory ui")]
    [TextArea]
    public string description;
    [Tooltip("The icon for this image on the hud and in the inventory")]
    public Texture2D image;
    [Tooltip("Determines item type. 'Key' is used for one use keys to open doors. 'Item' is for an equipable tool, such as a pocket mirror")]
    public InteractionType type;
    Rigidbody rb;
    private float previousVelocity;//Checks our velocity for the previous frame
    [SerializeField]
    [Tooltip("Used to modify the size of our trigger for the object")]
    private Vector3 interactionScale;//A vector 3 used to alter the scale of our trigger box
    [SerializeField]
    BoxCollider interactionBox;//The trigger box used when looking at the object
    
    void Awake() {
        rb = gameObject.GetComponent<Rigidbody>();

        interactionBox = gameObject.transform.GetChild(0).gameObject.GetComponent<BoxCollider>();
        interactionBox.size = gameObject.transform.GetComponent<Collider>().bounds.size * .5f + interactionScale;

        if (image == null) {
            Debug.LogError("No Image selected for - " + this);   
            image = UnityEditor.AssetPreview.GetAssetPreview(this);
        }

        if (title == "") {
            Debug.LogError("No Title given for - " + this);
            title = "#ERROR - NO TITLE SET";
        }

        if (description == "") {
            Debug.LogError("No description given for - " + this);
            description = "#ERROR - NO DESCRIPTION SET";
        }

        switch (type) {
            case InteractionType.Key:
                break;
            case InteractionType.Item:
                break;
            default:
                Debug.LogError("Something has gone very wrong with this item " + this);
                break;
        }
    }

    void Start()
    {
        //Ensures the item will move for atleast one frame before turning the rb off
        rb.isKinematic = false;
        previousVelocity = -1f;        
    }

    void FixedUpdate()
    {

        //I don't think this works yet
        //if the item has stopped moving, turn off the rigidbody
        if (rb.velocity.magnitude < 0.01 && previousVelocity != -1f) {
        rb.isKinematic = true;
        }
            

        //Stores our current velocity for the next frame
        previousVelocity = rb.velocity.magnitude;
    }
    void OnDrawGizmos() {
        //shows our interaction collision box
        if (GlobalVariables.showDebugLinesGlobal) {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(this.transform.position, interactionBox.size);
        }    
    }

    void OnValidate() {
        interactionBox.size = gameObject.transform.GetComponent<Collider>().bounds.size * .5f + interactionScale;
    }
}
