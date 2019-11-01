using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buttontester : MonoBehaviour {
    int buttonPressed;
    public void Test(Text text) {
        buttonPressed++;
        text.text = "Button pressed " + buttonPressed + " times";
    }
}
