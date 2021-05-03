// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpInstructions : MonoBehaviour
{
    public Text textInstructions;

    // Start is called before the first frame update
    void Start()
    {
        textInstructions.enabled = false;
    }

    // Update is called once per frame
    // void Update()
    // {
    //     return;
    // }

    public void OnClick_Help()
    {
        textInstructions.enabled = !textInstructions.enabled;
    }

    public void SetText(string text)
    {
        textInstructions.text = text;
    }
}
