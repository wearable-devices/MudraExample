#if MUDRA_ENABLED
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mudra.Unity;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using System;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public class MudraUnityManager : MonoBehaviour
{

    public static PluginPlatform plugin;

    [SerializeField] bool AirMouseState = false;
    [SerializeField] MudraScale Scale = MudraScale.MID;
    [SerializeField] MudraSensitivity Sensitivity = MudraSensitivity.MID;
    [SerializeField] int AirMouseSpeed = 5;
    [SerializeField] HandType Hand = HandType.Left;

    public void SetAirmouseState(bool state)
    {
        if (plugin == null) return;

        plugin.SwitchToAirmouse(state);
        AirMouseState = state;
    }
    public void SetScale(int scale)
    {
        if (plugin == null) return;

        plugin.ChangeScale(scale);
        Scale = (MudraScale)scale;
    }
    public void SetPressureSensitivity(int sens)
    {
        if (plugin == null) return;

        plugin.SetPressureSensitivity(sens);
        Sensitivity = (MudraSensitivity)sens;
    }
    public void SetAirMouseSpeed(int speed)
    {
        if (plugin == null) return;

        plugin.SetAirMouseSpeed(speed);
        AirMouseSpeed = speed;
    }
    public void SetHand(int hand)
    {
        if (plugin == null) return;

        plugin.SetMainHand(hand);
        Hand = (HandType)hand;
    }
    public void SendFirmwareCommand(byte[] command)
    {
        plugin.SendFirmwareCommand(command);
    }
#if UNITY_EDITOR
    [MenuItem("Mudra/Create Mudra Manager")]
    public static void AddManager()
    {
        GameObject obj = new GameObject("MudraManager");
        obj.AddComponent<MudraUnityManager>();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
#endif



    private void Start()
    {
        Init();
        Cursor.visible = true;
    }

    private void Update()
    {
        if (plugin != null)
        {
            plugin.MousePos();
        }

#if ENABLE_INPUT_SYSTEM
        if (PluginPlatform.deviceCreationQueue.Count > 0)
        {

            for (int i = 0; i < PluginPlatform.deviceCreationQueue.Count; i++)
            {
                Debug.Log("Creating Device");
                MudraDevice newDevice = (MudraDevice)InputSystem.AddDevice(new InputDeviceDescription
                {
                    interfaceName = "Mudra",
                    product = "Sample Mudra"
                });
                plugin.SetupDevice(newDevice);
                PluginPlatform.devices.Add(newDevice);
                PluginPlatform.deviceCreationQueue.RemoveAt(i);
            }

           

        }
#endif
    }
    public void Init()
    {



        if (plugin == null)
        {
#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN) || NETFX_CORE || UNITY_WSA || UNITY_WSA_10_0
            //  _pluginPlatform = new WindowsPlugin();
#elif (UNITY_ANDROID)
Debug.Log("Create New Unity Plugin");
            plugin = new MudraUnityAndroidPlugin();
#elif (UNITY_EDITOR_OSX || UNITY_IOS)
            //Logger.Print("MudraUnityPlugin new iOS_PlugIn()");
            plugin = new  MudraUnityiOSPlugin();
#endif
        }

        if (plugin != null)
        {
            plugin.Init();
            plugin.onInit += () =>
            {
                SetAirmouseState(AirMouseState);
                SetScale((int)Scale);
                SetPressureSensitivity((int)Sensitivity);
                SetAirMouseSpeed(AirMouseSpeed);
                SetHand((int)Hand);
            };
        }
    }
    public void GetDevices()
    {
#if UNITY_IOS

        MudraUnityiOSPlugin.ConnectToDevicesExtern();
#endif
    }
    public void resetQuat()
    {
        if (plugin != null)
            plugin.ResetQuaternion(0);
    }

    public static void ClearQueues()
    {
        plugin.ClearQueues();
    }

}
#endif