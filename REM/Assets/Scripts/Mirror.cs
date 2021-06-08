using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    Camera mirrorCam;
    Camera playerCam;
    Matrix4x4 m;

    MeshRenderer mirrorFace;
    RenderTexture viewTexture;

    float m_width;
    float m_height;

    Renderer rend;

    void Awake() {
        mirrorFace = GetComponent<MeshRenderer>();
        mirrorCam = GetComponentInChildren<Camera>();
        playerCam = Camera.main;

        rend = GetComponent<Renderer>();

        print(Screen.width + " width");
        print((int)rend.bounds.size.x + "width mesh");
    }

    void Update() {
        //m = mirrorCam.transform.localToWorldMatrix * playerCam.transform.worldToLocalMatrix;
        //mirrorCam.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);
    }

    //creates our mirror texture on the surfrace of our object
    void CreateViewTexture() {
        if(viewTexture == null || viewTexture.width != Screen.width || viewTexture.height != Screen.height) {
            if(viewTexture != null) {
                viewTexture.Release();
            }
            viewTexture = new RenderTexture(Screen.width, Screen.height, 0);
            //Render the view from the portal cam to the view texture
            mirrorCam.targetTexture = viewTexture;
            //display the view texture on the screen of the linked portal
            mirrorFace.material.SetTexture("_MainTex", viewTexture);

        }
    }

    void FixedUpdate() {
        if (viewTexture == null || viewTexture.width != Screen.width || viewTexture.height != Screen.height) {
            if (viewTexture != null) {
                viewTexture.Release();
            }
            
            viewTexture = new RenderTexture(Screen.width * (int)rend.bounds.size.x, Screen.height * (int)rend.bounds.size.y, 0);
            //Render the view from the portal cam to the view texture
            mirrorCam.targetTexture = viewTexture;

            //display the view texture on the screen of the linked portal
            mirrorFace.material.SetTexture("_MainTex", viewTexture);
        }
        m = transform.localToWorldMatrix * playerCam.transform.worldToLocalMatrix * playerCam.transform.localToWorldMatrix;
        Vector3 offset = playerCam.transform.position - mirrorCam.transform.position;
        
        //mirrorCam.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation * Quaternion.Euler(-1,1,-1));
    }
  
}
