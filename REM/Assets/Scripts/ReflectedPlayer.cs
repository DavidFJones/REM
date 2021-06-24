using UnityEngine;

public class ReflectedPlayer : MonoBehaviour {

	public GameObject fakeWall;

    bool isVisible = false;

    Renderer wallRenderer;
    BoxCollider wallCollision;
    FakeWall wallScript;
    Color defaultWallColor;


    void Awake() {
        wallRenderer = fakeWall.GetComponent<Renderer>();
        wallCollision = fakeWall.GetComponent<BoxCollider>();
        wallScript = fakeWall.GetComponent<FakeWall>();
        defaultWallColor = wallRenderer.material.color;
    }
    void Update() {
		if (GetComponent<Renderer>().IsVisibleFrom(Camera.main)) {
            if (isVisible)
                return;

                isVisible = true;
                wallCollision.isTrigger = true;
               
        } else {
            if (!isVisible)
                return;

            isVisible = false;
            wallCollision.isTrigger = false;
        }
	}
}