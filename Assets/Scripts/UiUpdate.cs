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
    [SerializeField] MudraUnityManager unityManager;
    [SerializeField] TextMeshProUGUI GestureText;

    bool hidState;
    bool appState;
    // Update is called once per frame
    void Update()
    {
        if (PluginPlatform.HasDevices)
        {
            pressureBar.fillAmount = inputManager.pressure;
            GestureText.text = ((GestureType)inputManager.lastGesture).ToString();
        }

    }

    public void SetPressure(bool state)
    {
        unityManager.SetPressureState(state, 0);
    }
    public void SetNavigation(bool state)
    {
        unityManager.SetNavigationState(state, 0);
    }
    public void SetGesture(bool state)
    {
        unityManager.SetGestureState(state, 0);
    }
    public void SetQuaternion(bool state)
    {
        unityManager.SetQuaternionState(state, 0);
    }
    public void SetSendHID(bool state)
    {
        unityManager.SetFirmwareTarget(FirmwareTarget.NAVIGATION_TO_HID, state, 0);
        hidState = state;
    }
    public void SetSendAPP(bool state)
    {

        unityManager.SetFirmwareTarget(FirmwareTarget.NAVIGATION_TO_APP, state, 0);
        appState = state;
    }
}
