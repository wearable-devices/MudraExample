using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mudra.Unity;
using TMPro;
public class UiUpdate : MonoBehaviour
{
    [SerializeField] InputManager inputManager;
    [SerializeField] Image pressureBar;
    [SerializeField] TextMeshProUGUI GestureText; 
    // Update is called once per frame
    void Update()
    {
        if (PluginPlatform.HasDevices)
        {
            pressureBar.fillAmount = inputManager.pressure;
            GestureText.text = ((GestureType)inputManager.lastGesture).ToString();
        }

    }
}
