using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HMWeek1 : MonoBehaviour
{
    string Aries = "Aries";
    string Taurus = "Taurus";
    string Gemini = "Gemini";
    string Cancer = "Cancer";
    string Leo = "Leo";
    public Button buttonClick;
    public InputField inputText;
    public Text showText;    

    // Start is called before the first frame update
    void Start()
    {
        buttonClick.onClick.AddListener(GetInputOnClickButton);        
    }    

    public void GetInputOnClickButton()
    {        
        if (Aries == inputText.text)
        {            
            showText.text = "[ " + inputText.text + " ]" + " is Found.";            
            print("[ " + inputText.text + " ]" + " is Found.");
        }

        else if (Taurus == inputText.text)
        {
            showText.text = "[ " + inputText.text + " ]" + " is Found.";
            print("[ " + inputText.text + " ]" + " is Found.");
        }

        else if (Gemini == inputText.text)
        {
            showText.text = "[ " + inputText.text + " ]" + " is Found.";
            print("[ " + inputText.text + " ]" + " is Found.");
        }

        else if (Cancer == inputText.text)
        {
            showText.text = "[ " + inputText.text + " ]" + " is Found.";
            print("[ " + inputText.text + " ]" + " is Found.");
        }

        else if (Leo == inputText.text)
        {
            showText.text = "[ " + inputText.text + " ]" + " is Found.";
            print("[ " + inputText.text + " ]" + " is Found.");
        }

        else 
        {
            showText.text = "[ " + inputText.text + " ]" + " is not Found.";
            print("[ " + inputText.text + " ]" + " is not Found.");
        }       
    }    
}
