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

    Transform playerPos;

    Vector3 scaleMod;

    float camHeight;

    void Awake() {
        
        mirrorFace = GetComponent<MeshRenderer>();
        mirrorCam = GetComponentInChildren<Camera>();

        camHeight = mirrorCam.transform.position.y;
        print(camHeight);

        playerCam = Camera.main;

        rend = GetComponent<Renderer>();

        scaleMod = new Vector3(Mathf.Abs(transform.localScale.x), 0, Mathf.Abs(transform.localScale.y));
    }
    void Start() {
        playerPos = SceneManager.Instance.player.transform;
    }

    void Update() {
        //m = mirrorCam.transform.localToWorldMatrix * playerCam.transform.worldToLocalMatrix;
        //mirrorCam.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);
    }

    //creates our mirror texture on the surfrace of our object
    void CreateViewTexture() {
        if(viewTexture == null) {
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
            if (viewTexture != null) {
                viewTexture.Release();
            }
        //viewTexture = new RenderTexture(Screen.width * (int)rend.bounds.size.x, Screen.height * (int)rend.bounds.size.y, 0);
        viewTexture = new RenderTexture(1024 * (int)Mathf.Abs(transform.localScale.x), 1024 * (int)Mathf.Abs(transform.localScale.y), 0);
        //viewTexture = new RenderTexture(Screen.width * (int)Mathf.Abs(transform.localScale.x), Screen.height * (int)Mathf.Abs(transform.localScale.y), 0);
        //Render the view from the portal cam to the view texture
        mirrorCam.targetTexture = viewTexture;

            //display the view texture on the screen of the linked portal
            mirrorFace.material.SetTexture("_MainTex", viewTexture);
        
        //m = transform.localToWorldMatrix * playerCam.transform.localToWorldMatrix;
        

        Vector3 newEulerAngles = transform.eulerAngles;
        newEulerAngles.y += 180f;
        newEulerAngles.z += 180f;

        //mirrorCam.transform.SetPositionAndRotation(m.GetColumn(3) + new Vector4(0, -1.5f, 0, 0), Quaternion.Euler(newEulerAngles));
        //mirrorCam.transform.SetPositionAndRotation(m.GetColumn(3) + new Vector4(0, -1.5f, 0, 0), m.rotation * Quaternion.Euler(new Vector3(0,0,180)));
        /*
        Vector3 offset = playerCam.transform.position - transform.position;
        m = transform.localToWorldMatrix * transform.worldToLocalMatrix * playerCam.transform.localToWorldMatrix;
        mirrorCam.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);*/

        Vector3 offset = playerCam.transform.position - transform.position;

        mirrorCam.transform.position = transform.position - offset ;
        mirrorCam.transform.position = new Vector3(mirrorCam.transform.position.x, camHeight, mirrorCam.transform.position.z);
        
       // mirrorCam.transform.LookAt(playerCam.transform);
    }
  
}
