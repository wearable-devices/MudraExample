
#if UNITY_ANDROID && MUDRA_ENABLED
using Mudra.Unity;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;

namespace Mudra.Unity
{
    public class MudraUnityAndroidPlugin : PluginPlatform
    {

        #region Android variables
        static AndroidJavaClass _mudraClass;
        static AndroidJavaObject _mudraDevices;
        static AndroidJavaObject _mudraInstance;
      
        static AndroidJavaObject devicesArrayJO;
        static List<AndroidJavaObject> devicesJO = new List<AndroidJavaObject>();

        string path = "Assets/Plugins/MudraSettings.asset";


        public static string name = "not initialized";
        public bool _isConnected = false;

        #endregion
        public override void SwitchToDpad()
        {
            for (int i = 0; i < devicesJO.Count; i++)
            {
                //  devicesJO[i].Call("SwitchToDpadMode");
            }
        }
        public override void Init(string calibrationFile = "")
        {
            //find the main mudra class
            _mudraClass = new AndroidJavaClass("mudraAndroidSDK.model.Mudra");
            Debug.Log("Get Mudra Class");

            //Get application context
            AndroidJavaClass jcu = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject jo = jcu.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject context = jo.Call<AndroidJavaObject>("getApplicationContext");

            //Get mudra singleon instance
            _mudraInstance = _mudraClass.CallStatic<AndroidJavaObject>("getInstance");
            Debug.Log("Get Mudra Instance");

            String LICENSE = "LicenseType::Main";
            _mudraInstance.Call("setLicense", 0, LICENSE);
            _mudraInstance.Call("requestAccessPermissions", jo);
            AndroidJavaObject topValue = new AndroidJavaClass("mudraAndroidSDK.enums.LoggingSeverity").GetStatic<AndroidJavaObject>("Warning");

            _mudraInstance.Call("setCoreLoggingSeverity", topValue);
            Debug.Log("Set Mudra License");


            //Get all mudra devices, we get an AndroidJavaObject which is a Java ArrayList
            devicesArrayJO = _mudraInstance.Call<AndroidJavaObject>("getBondedDevices", context);
            int size = devicesArrayJO.Call<int>("size");
            Debug.Log("Found:" + size + " devices");


            //we can then connect each device and create a new MudraDevice for each one
            for (int i = 0; i < size; i++)
            {
                AndroidJavaObject currDevice = devicesArrayJO.Call<AndroidJavaObject>("get", i);
                devicesJO.Add(currDevice);
                currDevice.Call("connectDevice", context);

                //TODO: move to plugin platfrom under an add device function
                DeviceIdentifier identifier;
                identifier.id = i;

                MudraDevice newDevice = (MudraDevice)InputSystem.AddDevice(new InputDeviceDescription
                {
                    interfaceName = "Mudra",
                    product = "Sample Mudra"
                });
                newDevice.identifier = identifier;
                devices.Add(newDevice);

                OnConnected(i);


            }

        }

        public override void SwitchToAirmouse(bool state)
        {
            for (int i = 0; i < devicesJO.Count; i++)
            {
                devicesJO[i].Call("setAirMouseActive", state);
            }
        }
        public override void ChangeScale(int Scale)
        {
            for (int i = 0; i < devicesJO.Count; i++)
            {


                switch (Scale)
                {
                    case 0:
                        devicesJO[i].Call("sendGeneralCommand", MudraConstants.PRESSURE_SCALE_LOW, null);
                        break;
                    case 1:
                        devicesJO[i].Call("sendGeneralCommand", MudraConstants.PRESSURE_SCALE_MID, null);
                        break;
                    case 2:
                        devicesJO[i].Call("sendGeneralCommand", MudraConstants.PRESSURE_SCALE_HIGH, null);
                        break;

                }
            }
        }
        public void OnConnected(int id)
        {
          

            UpdateOnGestureReadyCallback(id);
            UpdateOnFingerTipPressureCallback(id);
            //UpdateOnQuaternionReadyCallback(id);
            // UpdateOnSNCReady(id);
            // UpdateOnImuRawCallback(id);
           // SwitchToAirmouse(true);
           // sbyte[] test = new sbyte[] { 0x07, 0x07, 0x01 };
            SendFirmwareCommand(MudraConstants.AIRMOUSE_ON);
            SetAirMouseSpeed(5);
            

           // InvokeInitFinished();

        }

        class OnGestureReady : AndroidJavaProxy
        {
            int deviceId;
            public OnGestureReady(int id) : base("mudraAndroidSDK.interfaces.callback.OnGestureReady")
            {
                this.deviceId = id;
                Debug.Log("setGestureCallback");
            }

            void run(AndroidJavaObject retObj)
            {
                devices[deviceId].OnGesture((GestureType)retObj.Call<int>("ordinal"));

            }
        }

        public override void SetPressureSensitivity(int sens)
        {
            byte[] command = { };

            for (int i = 0; i < devicesJO.Count; i++)
            {

                switch (sens)
                {
                    case 0:
                        command = MudraConstants.PRESSURE_SENS_LOW;
                        break;
                    case 1:
                        command = MudraConstants.PRESSURE_SENS_MIDLOW;
                        break;
                    case 2:
                        command = MudraConstants.PRESSURE_SENS_MID;

                        break;
                    case 3:
                        command = MudraConstants.PRESSURE_SENS_MIDHIGH;

                        break;
                    case 4:
                        command = MudraConstants.PRESSURE_SENS_HIGH;

                        break;
                }
                devicesJO[i].Call("sendGeneralCommand", command, null);

            }
        }
        public override void SetMainHand(int hand)
        {
            AndroidJavaObject topValue;
            if (hand == 0)
            {
                topValue = new AndroidJavaClass("mudraAndroidSDK.enums.HandType").GetStatic<AndroidJavaObject>("LEFT");
            }
            else
            {
                topValue = new AndroidJavaClass("mudraAndroidSDK.enums.HandType").GetStatic<AndroidJavaObject>("RIGHT");
            }

            for (int i = 0; i < devicesJO.Count; i++)
            {
                devicesJO[i].Call("setHand", topValue);
            }

        }

        class OnDeviceStatusChanged : AndroidJavaProxy
        {
            MudraUnityAndroidPlugin plugin;
            int id;
            public OnDeviceStatusChanged(int id, MudraUnityAndroidPlugin plugin) : base("mudraAndroidSDK.interfaces.callback.OnDeviceStatusChanged")
            {
                this.plugin = plugin;
                this.id = id;

            }

            void run(bool isConnected)
            {
                Debug.Log("ddevice id: " + id + " ConnectStatus: " + isConnected);

                if (isConnected)
                {
                    plugin.OnConnected(id);
                }
            }
        }

        override public void UpdateOnGestureReadyCallback(int index)
        {
            if (devices[index].IsGestureEnabled)
            {

                devicesJO[index].Call("setOnGestureReady", new OnGestureReady(index));

            }
            else
            {

                devicesJO[index].Call("setOnGestureReady", null);
            }

        }

        class OnFingertipPressureReady : AndroidJavaProxy
        {
            MudraUnityAndroidPlugin _unityPlugin;
            int id;

            public OnFingertipPressureReady(int id) : base("mudraAndroidSDK.interfaces.callback.OnPressureReady")
            {
                this.id = id;
                Debug.Log("setPressureCallback");
            }

            void run(float pressure)
            {
                //Debug.Log("Test");

                devices[id].OnPressure(pressure);


            }
        }

        override public void UpdateOnFingerTipPressureCallback(int index)
        {
            if (devices[index].IsFingerTipPressureEnabled)
            {

                devicesJO[index].Call("setOnPressureReady", new OnFingertipPressureReady(index));

            }
            else
            {

                devicesJO[index].Call("setOnPressureReady", null);

            }

        }

        class OnAirMousePositionChanged : AndroidJavaProxy
        {
            MudraUnityAndroidPlugin _unityPlugin;

            public OnAirMousePositionChanged(MudraUnityAndroidPlugin unityplugin) : base("MudraAndroidSDK.Mudra$OnAirMousePositionChanged") { _unityPlugin = unityplugin; }

            void run(float[] positionChanged)
            {
                // _unityPlugin.SetLastAirMousePositionChange(positionChanged);
            }
        }

        override protected void UpdateAirMousePositionChangedCallback()
        {
            if (_isAirMouseEnabled)
                _mudraDevices.Call("setOnAirMousePositionChanged", new OnAirMousePositionChanged(this));
            else
                _mudraDevices.Call("setOnAirMousePositionChanged", null);
        }

        class OnImuQuaternionReady : AndroidJavaProxy
        {
            MudraUnityAndroidPlugin _unityPlugin;
            int id;

            public OnImuQuaternionReady(int id) : base("mudraAndroidSDK.interfaces.callback.OnImuQuaternionReady")
            {
                this.id = id;
            }

            void run(long ts, float[] q)
            {
                Quaternion quaternion = new Quaternion(q[0], q[1], q[2], q[3]);
                devices[id].OnQuaternion(quaternion);
            }
        }
        class OnImuAccRawReady : AndroidJavaProxy
        {
            int id;
            public OnImuAccRawReady(int id) : base("mudraAndroidSDK.interfaces.callback.OnImuAccRawReady")
            {
                this.id = id;
            }

            void run(long timestamp, float[] data)
            {

                //Data is a flattened array of 8 samples at once, so the size of data is 24, in order to filter down any noise I take the averge of all samples
                float[] dataVector = new float[3];
                for (int i = 0; i < data.Length - 3; i += 3)
                {
                    dataVector[0] += data[i];
                    dataVector[1] += data[i + 1];
                    dataVector[2] += data[i + 2];


                }

                dataVector[0] /= data.Length;
                dataVector[1] /= data.Length;
                dataVector[2] /= data.Length;

                devices[id].OnAccRaw(dataVector);

            }


        }
        class OnSNCReady : AndroidJavaProxy
        {
            int id;

            public OnSNCReady(int id) : base("mudraAndroidSDK.interfaces.callback.OnSncReady")
            {
                this.id = id;
            }

            void run(long ts, float[] snc)
            {
                for (int i = 0; i < snc.Length; i++)
                {
                    Debug.Log(snc[i]);
                }
            }

        }
        class OnAirmouseButton : AndroidJavaProxy
        {
            int id;

            public OnAirmouseButton(int id) : base("mudraAndroidSDK.interfaces.callback.OnAirMouseButtonCallback")
            {
                this.id = id;
                Debug.Log("SetUpCallback");
            }

            void run(int button)
            {
                //  Debug.Log("Weee");

                // if (button == 0)
                // devices[id].onPress.Invoke();



                //  Debug.Log(quaternion);
            }
        }

        override public void UpdateOnQuaternionReadyCallback(int index)
        {
            if (devices[index].IsImuQuaternionEnabled)
            {
                devicesJO[index].Call("setOnImuQuaternionReady", new OnImuQuaternionReady(index));
            }
            else
            {
                devicesJO[index].Call("setOnImuQuaternionReady", null);
            }

        }
        public void UpdateAirmouseButtonCallback(int index)
        {

            devicesJO[index].Call("SetOnAirMouseButtons", new OnAirmouseButton(index));


        }
        public override void Update()
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }

        public override void ResetQuaternion(int index)
        {
            int[] dimentions = { Screen.width, Screen.height };
            devicesJO[index].Call("resetAirMouse", dimentions);
        }

        public override void SetupDevice(MudraDevice device)
        {
            throw new NotImplementedException();
        }

        public override void ClearQueues()
        {
            for (int i = 0; i < devices.Count; i++)
            {
                devicesJO[i].Call("Clear");
            }
        }

        public override void UpdateOnSNCReady(int index)
        {

            devicesJO[index].Call("setOnSncReady", new OnSNCReady(index));
            Debug.Log("sncSet");

        }

        public override string getFirmwareVersion(int id)
        {
            return devicesJO[id].Call<string>("getFirmwareVersion");
        }

        public override void SetAirMouseSpeed(int speed)
        {
            for (int i = 0; i < devicesJO.Count; i++)
            {
                byte[] command = MudraConstants.AIRMOUSE_SPEED;
                ;
                switch (speed)
                {
                    case 0:
                        command[2] = 0x00;
                        command[4] = 0x00;
                        break;
                    case 1:
                        command[2] = 0x01;
                        command[4] = 0x01;
                        break;
                    case 2:
                        command[2] = 0x02;
                        command[4] = 0x02;
                        break;

                    case 3:
                        command[2] = 0x03;
                        command[4] = 0x03;
                        break;
                    case 4:
                        command[2] = 0x04;
                        command[4] = 0x04;
                        break;
                    case 5:
                        command[2] = 0x05;
                        command[4] = 0x05;
                        break;
                    case 6:
                        command[2] = 0x06;
                        command[4] = 0x06;
                        break;
                    case 7:
                        command[2] = 0x07;
                        command[4] = 0x07;
                        break;
                    case 8:
                        command[2] = 0x08;
                        command[4] = 0x08;
                        break;
                    case 9:
                        command[2] = 0x09;
                        command[4] = 0x09;
                        break;
                    case 10:
                        command[2] = 0x010;
                        command[4] = 0x010;
                        break;
                }
                devicesJO[i].Call("sendGeneralCommand", command, null);
            }
        }

        public override void UpdateOnImuRawCallback(int index)
        {
            if (devices[index].IsAccRawEnabled)
            {
                Debug.Log("SetImuRawCallback");
                devicesJO[index].Call("setOnImuAccRawReady", new OnImuAccRawReady(index));
            }
            else
            {
                devicesJO[index].Call("setOnImuAccRawReady", null);
            }
        }

        public override void SendFirmwareCommand(byte[] command)
        {
            for (int i = 0; i < devicesJO.Count; i++)
            {
                devicesJO[i].Call("sendGeneralCommand", command, null);
            }
        }
    }
}


public enum LoggingSeverity
{
    Debug,
    Info,
    Warning,
    Error
}
#endif