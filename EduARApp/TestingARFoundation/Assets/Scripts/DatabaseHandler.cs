using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DatabaseHandler : MonoBehaviour {
    [SerializeField]
    private InputField nameInputField;
    [SerializeField]
    private InputField pinCodeInputField;
    [SerializeField]
    private Text LogInErrorBuffer;

    private PanelHandler panelHandler;

    private void Awake() {
        panelHandler = GameObject.Find("MainCanvas").GetComponent<PanelHandler>();
    }

    /// <summary>
    /// Called when the log in button is pressed
    /// </summary>
    public void StudentLogIn() {
        string name = nameInputField.text;
        int pinCode = int.Parse(pinCodeInputField.text);

        DBConnector.GetUserData((callback) => {
            if (callback == null)   // Incorrect login detected
                PrintError("Incorrecte informatie ingevoerd", Color.red);
            else {
                foreach(object student in callback) {
                    // Correct login detected, set current student
                    Student.currentStudent = (Student)student;
                    nameInputField.text = "";
                    pinCodeInputField.text = "";
                    PrintError("", Color.black);
                    panelHandler.SwitchPanel(Panel.Level.ToString());
                }
            }
        }, isTeacher: false, studentName: name, studentPin: pinCode);
    }

    /// <summary>
    /// Print to the ErrorBuffer text object in the LogIn panel (max 24 chars)
    /// </summary>
    /// <param name="text">The text to print</param>
    /// <param name="color">Color of the text</param>
    private void PrintError(string text, Color color) {
        if (text.Length <= 24) {
            LogInErrorBuffer.color = color;
            LogInErrorBuffer.text = text;
        } else {
            throw new System.ArgumentException("Maximum of 24 characters");
        }
    }
}
