using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    //Our mirror and player cameras
    Camera mirrorCam;
    Camera playerCam;

    //The mesh renderer for our mirror
    MeshRenderer mirrorFace;
    //The output texture from our camera
    RenderTexture viewTexture;

    //The starting center height of our mirror camera, relative to it's mirror parent
    float camHeight;

    void Awake() {
        
        mirrorFace = GetComponent<MeshRenderer>();
        mirrorCam = GetComponentInChildren<Camera>();

        camHeight = mirrorCam.transform.position.y;

        playerCam = Camera.main;
    }
    void FixedUpdate() {
        if (viewTexture != null) {
            viewTexture.Release();
        }
        //Set the size of the texture equal to our desired resolution (1024) multiplied by the size of our mirror
        viewTexture = new RenderTexture(1024 * (int)Mathf.Abs(transform.localScale.x), 1024 * (int)Mathf.Abs(transform.localScale.y), 0);
        //Render the view from the mirror cam to the view texture
        mirrorCam.targetTexture = viewTexture;

        //Set the mirrors texture to the render texture
        mirrorFace.material.SetTexture("_MainTex", viewTexture);
        
        //Create an offset so the camera is correctly positioned
        Vector3 offset = playerCam.transform.position - transform.position;

        //move the camera relative to the player
        mirrorCam.transform.position = transform.position - offset ;
        //Force the mirror cameras y height to stay the same (centered on the mirror)
        mirrorCam.transform.position = new Vector3(mirrorCam.transform.position.x, camHeight, mirrorCam.transform.position.z);
    }
  
}
