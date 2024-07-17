
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
            AndroidJNI.AttachCurrentThread();
            //find the main mudra class

            Debug.Log("Get Mudra Class");
            _mudraClass = new AndroidJavaClass("mudraAndroidSDK.model.Mudra");

            //Get application context
            AndroidJavaClass jcu = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject jo = jcu.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject context = jo.Call<AndroidJavaObject>("getApplicationContext");

            //Get mudra singleon instance
            _mudraInstance = _mudraClass.CallStatic<AndroidJavaObject>("getInstance");
            Debug.Log("Get Mudra Instance");

            String LICENSE = "LicenseType::Main";
            _mudraInstance.Call("setLicenseInternalUseOnly");
            _mudraInstance.Call("requestAccessPermissions", jo);
            AndroidJavaObject topValue = new AndroidJavaClass("mudraAndroidSDK.enums.LoggingSeverity").GetStatic<AndroidJavaObject>("Info");

            _mudraInstance.Call("setCoreLoggingSeverity", topValue);
            Debug.Log("Set Mudra License");

            _mudraInstance.Call("setMudraDelegate", new MudraDelegate(this));

            // Get all mudra devices, we get an AndroidJavaObject which is a Java ArrayList
            devicesArrayJO = _mudraInstance.Call<AndroidJavaObject>("getBondedDevices", context);
            int size = devicesArrayJO.Call<int>("size");
            Debug.Log("Found:" + size + " devices");


            // we can then connect each device and create a new MudraDevice for each one
            for (int i = 0; i < size; i++)
            {
                AndroidJavaObject currDevice = devicesArrayJO.Call<AndroidJavaObject>("get", i);
                //devicesJO.Add(currDevice);
                currDevice.Call("connectDevice", context);
                //TODO: move to plugin platfrom under an add device function



            }

        }


        #region UpdateCallbacks

        override public void UpdateOnGestureReadyCallback(int index)
        {

            Debug.Log("Gesture Set To " + devices[index].IsGestureEnabled);
            if (devices[index].IsGestureEnabled)
            {


                devicesJO[index].Call("setOnGestureReady", new OnGestureReady(index));

            }
            else
            {

                devicesJO[index].Call("setOnGestureReady", null);
            }

        }
        override public void UpdateOnQuaternionReadyCallback(int index)
        {
            Debug.Log("Quaternion Set To " + devices[index].IsImuQuaternionEnabled);

            if (devices[index].IsImuQuaternionEnabled)
            {
                devicesJO[index].Call("setOnImuQuaternionReady", new OnImuQuaternionReady(index));
            }
            else
            {
                devicesJO[index].Call("setOnImuQuaternionReady", null);
            }

        }


        override public void UpdateOnFingerTipPressureCallback(int index)
        {
            Debug.Log("Pressure Set To " + devices[index].IsFingerTipPressureEnabled);

            if (devices[index].IsFingerTipPressureEnabled)
            {

                devicesJO[index].Call("setOnPressureReady", new OnFingertipPressureReady(index));

            }
            else
            {

                devicesJO[index].Call("setOnPressureReady", null);

            }

        }
        public override void UpdateNavigationCallback(int index)
        {
            Debug.Log("Navigation Set To " + devices[index].isNavigationEnabled);

            if (devices[index].isNavigationEnabled)
            {

                devicesJO[index].Call("setOnNavigationPointerReady", new OnNavigationPointerReady(index));
                setNavigationActive(true);
                devicesJO[index].Call("setOnButtonChanged", new OnButtonChanged(index));



            }
            else
            {

                devicesJO[index].Call("setOnNavigationPointerReady", null);
                setNavigationActive(false);
                devicesJO[index].Call("setOnButtonChanged", null);


            }
        }
        #endregion


        #region Callback Interfaces
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

                devices[id].OnPressure(pressure /*/ 100.0f*/);


            }
        }
        class OnMessageReceived : AndroidJavaProxy
        {
            int id;
            public OnMessageReceived(int id) : base("mudraAndroidSDK.interfaces.callback.OnMessageReceived")
            {
                this.id = id;
                Debug.Log("Set Up On Message Received");
            }
            void run(byte[] data)
            {
                byte header = data[0];

                if (header == 0x60)
                {
                    //Pressure;
                    // Debug.Log("received pressure" + (float)data[1]);
                    //devices[id].deviceData.fingerTipPressure = (float)data[1] / 100.0f;
                }
                if (header == 0x61)
                {
                    //Pressure;
                    // Debug.Log("received gesture" + (NewGestureType)((int)data[1]));

                    devices[id].deviceData.lastGesture = (GestureType)((int)data[1]);
                }
            }

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
        class OnNavigationPointerReady : AndroidJavaProxy
        {
            int id;
            public OnNavigationPointerReady(int id) : base("mudraAndroidSDK.interfaces.callback.OnNavigationPointerReady")
            {
                this.id = id;
                Debug.Log("Set Up OnNavigationPointerReady");
            }
            void run(int delta_x, int delta_y)
            {
                devices[id].OnMouseDelta(delta_x, delta_y);

            }

        }
        class MudraDelegate : AndroidJavaProxy
        {
            MudraUnityAndroidPlugin plugin;
            public MudraDelegate(MudraUnityAndroidPlugin plugin) : base("mudraAndroidSDK.interfaces.MudraDelegate")
            {
                this.plugin = plugin;
                Debug.Log("Created Mudra Delegate");
            }


            void onDeviceDiscovered(AndroidJavaObject mudraDevice)
            {
                Debug.Log("Discovered Devices");
                AndroidJavaClass jcu = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject jo = jcu.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject context = jo.Call<AndroidJavaObject>("getApplicationContext");

                Debug.Log(mudraDevice.Get<string>("deviceName"));
                mudraDevice.Call("connectDevice", context);
            }

            void onDeviceConnected(AndroidJavaObject mudraDevice)
            {
                devicesJO.Add(mudraDevice);

                Debug.Log("Add Devices To Queue");
                Debug.Log(mudraDevice.Get<string>("deviceName"));

                DeviceIdentifier identifier;
                identifier.id = devices.Count;


                PluginPlatform.deviceCreationQueue.Add(identifier);
                plugin.OnDeviceConnected.Invoke();

            }

            void onDeviceDisconnected(AndroidJavaObject mudraDevice)
            {
                Debug.Log("DeviceDisconnected");

                devices.RemoveAt(devices.Count - 1);
                devicesJO.RemoveAt(devicesJO.Count - 1);
                plugin.OnDeviceDisconnected.Invoke();

            }
        }
        class OnButtonChanged : AndroidJavaProxy
        {
            int id;
            public OnButtonChanged(int id) : base("mudraAndroidSDK.interfaces.callback.OnButtonChanged")
            {
                this.id = id;
                Debug.Log("Set Up OnButtonChanged");
            }
            void run(AndroidJavaObject value)
            {
                devices[id].OnButtonChanged((NavigationButtons)value.Call<int>("ordinal"));
            }

        }
        #endregion
        public override void Update()
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }



        public override void SetupDevice(MudraDevice device)
        {
            AndroidJavaObject Model = new AndroidJavaClass("mudraAndroidSDK.enums.ModelType").GetStatic<AndroidJavaObject>("Embedded");

            devicesJO[device.identifier.id].Call<bool>("setModelType", Model);

            AndroidJavaObject firmwareMode = new AndroidJavaClass("mudraAndroidSDK.enums.DeviceMode").GetStatic<AndroidJavaObject>("ANDROID");

            devicesJO[device.identifier.id].Call("setDeviceMode", firmwareMode);

            AndroidJavaObject NavigationType = new AndroidJavaClass("mudraAndroidSDK.enums.NavigationType").GetStatic<AndroidJavaObject>("POINTER_MODE");

            devicesJO[device.identifier.id].Call("setNavigationType", NavigationType);

            UpdateOnGestureReadyCallback(device.identifier.id);
            UpdateOnFingerTipPressureCallback(device.identifier.id);
            UpdateOnQuaternionReadyCallback(device.identifier.id);
            UpdateNavigationCallback(device.identifier.id);
        }

        #region General Use Commands
        public override void SwitchToAirmouse(bool state)
        {
            for (int i = 0; i < devicesJO.Count; i++)
            {
                devicesJO[i].Call("setAirMouseActive", state);
            }
        }

        public override void SetMainHand(int hand, int index)
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


            //devicesJO[index].Call("setHand", topValue);


        }

        public override void ClearQueues()
        {

        }

        public override string getFirmwareVersion(int id)
        {
            return devicesJO[id].Call<string>("getFirmwareVersion");
        }

        public override void SetNavigationSpeed(int speed, int index)
        {

            byte[] command = MudraConstants.NAVIGATION_SPEED;
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
                    command[2] = 0x07;
                    command[4] = 0x07;
                    break;

                case 3:
                    command[2] = 0x0F;
                    command[4] = 0x0F;
                    break;
                case 4:
                    command[2] = 0x15;
                    command[4] = 0x15;
                    break;
                case 5:
                    command[2] = 0x1E;
                    command[4] = 0x1E;
                    break;
                case 6:
                    command[2] = 0x25;
                    command[4] = 0x25;
                    break;
                case 7:
                    command[2] = 0x2D;
                    command[4] = 0x2D;
                    break;
                case 8:
                    command[2] = 0x3A;
                    command[4] = 0x3A;
                    break;
                case 9:
                    command[2] = 0x33;
                    command[4] = 0x33;
                    break;
                case 10:
                    command[2] = 0x3C;
                    command[4] = 0x3C;
                    break;

            }

            devicesJO[index].Call("sendGeneralCommand", command, null);

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
        //public override void setAirMousePointerActive(bool state)
        //{
        //    for (int i = 0; i < devicesJO.Count; i++)
        //    {
        //        devicesJO[i].Call("setAirMousePointerActive", state);
        //    }
        //}
        //public override void setAirMousePressReleaseActive(bool state)
        //{
        //    for (int i = 0; i < devicesJO.Count; i++)
        //    {
        //        devicesJO[i].Call("setAirMousePressReleaseActive", state);
        //    }
        //}
        public override void setGestureActive(bool state)
        {
            for (int i = 0; i < devicesJO.Count; i++)
            {
                devicesJO[i].Call("setGestureActive", state);
            }
        }
        public override void setPressureActive(bool state)
        {
            for (int i = 0; i < devicesJO.Count; i++)
            {
                devicesJO[i].Call("setPressureActive", state);
            }
        }
        public override void setAirTouchActive(bool state)
        {
            for (int i = 0; i < devicesJO.Count; i++)
            {
                devicesJO[i].Call("setAirTouchActive", state);
            }
        }
        public override void sendHIDTO(bool appState, bool HIDState, int index)
        {

            devicesJO[index].Call("sendHIDTO", appState, HIDState);

        }
        public override void setNavigationActive(bool state)
        {
            for (int i = 0; i < devicesJO.Count; i++)
            {
                devicesJO[i].Call("setNavigationActive", state);
            }
        }



        #endregion

        #region NoLongerSupported



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

                // devices[id].OnAccRaw(dataVector);

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
        public override void UpdateOnSNCReady(int index)
        {

            devicesJO[index].Call("setOnSncReady", new OnSNCReady(index));
            Debug.Log("sncSet");
        }

        public override void ResetQuaternion(int index)
        {
            int[] dimentions = { Screen.width, Screen.height };
            devicesJO[index].Call("resetAirMouse", dimentions);
        }

        //public override void ChangeScale(int Scale, int index)
        //{

        //    switch (Scale)
        //    {
        //        case 0:
        //            devicesJO[index].Call("sendGeneralCommand", MudraConstants.PRESSURE_SCALE_LOW, null);
        //            break;
        //        case 1:
        //            devicesJO[index].Call("sendGeneralCommand", MudraConstants.PRESSURE_SCALE_MID, null);
        //            break;
        //        case 2:
        //            devicesJO[index].Call("sendGeneralCommand", MudraConstants.PRESSURE_SCALE_HIGH, null);
        //            break;

        //    }

        //}

        //public override void SetPressureSensitivity(int sens, int index)
        //{
        //    byte[] command = { };



        //    switch (sens)
        //    {
        //        case 0:
        //            command = MudraConstants.PRESSURE_SENS_LOW;
        //            break;
        //        case 1:
        //            command = MudraConstants.PRESSURE_SENS_MIDLOW;
        //            break;
        //        case 2:
        //            command = MudraConstants.PRESSURE_SENS_MID;

        //            break;
        //        case 3:
        //            command = MudraConstants.PRESSURE_SENS_MIDHIGH;

        //            break;
        //        case 4:
        //            command = MudraConstants.PRESSURE_SENS_HIGH;

        //            break;
        //    }
        //    devicesJO[index].Call("sendGeneralCommand", command, null);


        //}

        #endregion
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