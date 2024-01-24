#if ENABLE_INPUT_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Mudra.Unity;
public class InputManager : MonoBehaviour
{
    public float pressure;
    public Quaternion quaternion;
    public int lastGesture;
    public Vector3 AccRaw;
    public Vector3 SncRaw;

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
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
#endif