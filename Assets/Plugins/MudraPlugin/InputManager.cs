#if ENABLE_INPUT_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using TMPro;
using Mudra.Unity;
public class InputManager : MonoBehaviour
{
    //public static float temp;
    public float pressure;
    public int lastGesture;
    public NavigationButtons click;
    public Quaternion quaternion;

    public UnityEvent OnClickEvent;
    public UnityEvent OnReleaseEvent;

    public static Vector3 mousePos;
    [SerializeField] float mousespeed;


    public void OnPressure(InputValue value)
    {
        pressure = value.Get<float>();
    }

    public void OnMouseDelta(InputValue value)
    {
        mousePos.x += value.Get<Vector2>().x * mousespeed;
        mousePos.y -= value.Get<Vector2>().y * mousespeed;

        mousePos.x = Mathf.Clamp(mousePos.x, 0, Screen.width);
        mousePos.y = Mathf.Clamp(mousePos.y, 0, Screen.width);

    }
    public void OnQuaternion(InputValue value)
    {
        quaternion = value.Get<Quaternion>();
    }

    public void OnGesture(InputValue value)
    {
        lastGesture = value.Get<int>();
        //Debug.Log("Gesture");

    }

    public void OnClick(InputValue value)
    {
        NavigationButtons clickValue = (NavigationButtons)value.Get<float>();
        Debug.Log(clickValue);

        if (clickValue == NavigationButtons.Press)
            OnClickEvent.Invoke();
        else
            OnReleaseEvent.Invoke();


        click = clickValue;
    }
    public void OnMousePC(InputValue value)
    {
        if (!PluginPlatform.devices[0].deviceData.sendToHID)
            return;
        Debug.Log(value.Get<float>());
        NavigationButtons clickValue = (NavigationButtons)value.Get<float>();
        Debug.Log(clickValue);

        if (clickValue == NavigationButtons.Press)
            OnClickEvent.Invoke();
        else
            OnReleaseEvent.Invoke();


        click = clickValue;
    }
    public void OnMousePCPos(InputValue value)
    {
        if (!PluginPlatform.devices[0].deviceData.sendToHID)
            return;
        mousePos = value.Get<Vector2>();
    }

    //public void OnMousePosition(InputValue value)
    //{
    //    MousePos = value.Get<Vector2>();
    //}
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        ResetMousePos();
    }
   
    public void ResetMousePos()
    {
        mousePos = new Vector2(Screen.width / 2, Screen.height / 2);
    }
}
#endif