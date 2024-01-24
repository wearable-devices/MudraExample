using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mudra.Unity;
public class UiUpdate : MonoBehaviour
{
    [SerializeField] InputManager inputManager;
    [SerializeField] Image pressureBar;
    // Update is called once per frame
    void Update()
    {
        if (PluginPlatform.HasDevices )
        {
            pressureBar.fillAmount = inputManager.pressure;
        }
    }
}
