#if MUDRA_ENABLED
using System.Collections.Generic;
using UnityEngine;
using Mudra.Unity;
namespace Mudra.Unity
{
    public delegate void OnLoggingMessageCallBack(string message);

    abstract public class PluginPlatform
    {
        const int NUM_OF_SNCS = 3;

        public static List<MudraDevice> devices = new List<MudraDevice>();
        public static Vector2 mousePos =new Vector2(Screen.width/2,Screen.height/2);
        abstract public void Init(string calibrationFile = "");
        abstract public void Update();
        abstract public void Close();
        abstract public void ResetQuaternion(int index);
        abstract public void SwitchToDpad();
        abstract public void SwitchToAirmouse(bool state);
        abstract public void SetAirMouseSpeed(int speed);
        abstract public void SetMainHand(int hand);
        abstract public void ChangeScale(int Scale);
        abstract public void SetPressureSensitivity(int sens);
        abstract public void ClearQueues();
        abstract public void SendFirmwareCommand(byte[] command);
        public virtual void MousePos() { }
        public static List<DeviceIdentifier> deviceCreationQueue = new List<DeviceIdentifier>();

        public delegate void onInitFunc();
        public event onInitFunc onInit;

#region OnGestureReady

        abstract public void UpdateOnGestureReadyCallback(int index);
#endregion

#region OnFingerTipPressureReady
       
        abstract public void UpdateOnFingerTipPressureCallback(int index);
        abstract public void UpdateOnImuRawCallback(int index);

        #endregion

        #region OnAirMousePositionChanged
        protected bool _isAirMouseEnabled = true;
        public bool IsAirMouseEnabled
        {
            get { return _isAirMouseEnabled; }
            set
            {
                if (_isAirMouseEnabled != value)
                {
                    _isAirMouseEnabled = value;
                    UpdateAirMousePositionChangedCallback();
                }
            }
        }
        public void InvokeInitFinished()
        {
            onInit?.Invoke();
            Debug.Log("Invoke Init");
        }
        abstract protected void UpdateAirMousePositionChangedCallback();
        #endregion
        public abstract string getFirmwareVersion(int id);
        public abstract void SetupDevice(MudraDevice device);  
        abstract public void UpdateOnQuaternionReadyCallback(int index);
        abstract public void UpdateOnSNCReady(int index);

        public void Clear()
        {
            foreach (var device in devices)
            {
                device.deviceData.lastGesture = null;
                device.deviceData.quaternion = null;
                device.deviceData.fingerTipPressure = null;
            }
        }
    }
}
#endif