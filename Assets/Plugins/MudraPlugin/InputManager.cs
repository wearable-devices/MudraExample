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
    public float pressure;
    public Quaternion quaternion;
    public int lastGesture;
    public Vector3 AccRaw;
    public Vector3 SncRaw;
    public float MouseState;
    public Vector3 MousePos;

    public UnityEvent OnClick;
    public UnityEvent OnRelease;

    public void OnPressure(InputValue value)
    {
        pressure = value.Get<float>();

    }

    public void OnAccRaw(InputValue value)
    {
        AccRaw = value.Get<Vector3>();
    }

    public void OnSncRaw(InputValue value)
    {
        SncRaw = value.Get<Vector3>();
    }
    public void OnQuaternion(InputValue value)
    {
        quaternion = value.Get<Quaternion>();
    }

    public void OnGesture(InputValue value)
    {
        lastGesture = value.Get<int>();

    }

    public void OnMouseClick(InputValue value)
    {
        float clickValue = value.Get<float>();
        if (clickValue != MouseState)
        {
            if (clickValue == 1)
                OnClick.Invoke();
            else
                OnRelease.Invoke();
        }

        MouseState = value.Get<float>();
    }

    public void OnMousePosition(InputValue value)
    {
        MousePos = value.Get<Vector2>();
    }
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
#endif