using UnityEngine;
using UnityEngine.UI;

public class TouchHandler : MonoBehaviour {
    public GameObject UI;
    public Text GreetingText;
    public Text NameText;

    void Update() {
        if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began)) {
            Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit raycastHit;

            if (Physics.Raycast(raycast, out raycastHit, maxDistance: 1000f)) {
                if (raycastHit.collider.CompareTag("TestCapsule")) {
                    UI.SetActive(true);
                }
            }
        }
    }

    public void EnteredName(InputField name) {
        GreetingText.gameObject.SetActive(true);

        NameText.text = name.text;
        NameText.gameObject.SetActive(true);
    }
}
