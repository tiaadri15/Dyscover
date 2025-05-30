using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ShowText : MonoBehaviour
{
    public string objectName;  // Assign the object name in the inspector

    public GameObject canvasObject; // Drag your Canvas GameObject here in the inspector
    public TMP_Text textElement1;   // Drag TMP text element 1 here
    public TMP_Text textElement2;   // Drag TMP text element 2 here
    public TMP_Text textElement3;   // Drag TMP text element 3 here

    void Start()
    {
        if (canvasObject == null)
        {
            Debug.LogError("Canvas Object not assigned in the inspector!");
        }
        if (textElement1 == null || textElement2 == null || textElement3 == null)
        {
            Debug.LogError("Missing Text Element");
        }

        // Ensure all TMP objects are initially inactive
        textElement1.gameObject.SetActive(false);
        textElement2.gameObject.SetActive(false);
        textElement3.gameObject.SetActive(false);
    }

    void OnMouseDown()
    {
        // This method is called when the object is clicked/touched
        ActivateCanvasText();
    }

    void ActivateCanvasText()
    {
        if (objectName == "Badak")
        {
            textElement1.gameObject.SetActive(true);
            textElement2.gameObject.SetActive(false);
        }
        else if (objectName == "dadu")
        {
            textElement1.gameObject.SetActive(false);
            textElement2.gameObject.SetActive(true);
        }
        else
        {
            textElement3.gameObject.SetActive(true);
        }
    }
}
