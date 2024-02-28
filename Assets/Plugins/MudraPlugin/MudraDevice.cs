#if MUDRA_ENABLED
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Mudra.Unity
{
    public struct MudraDeviceState : IInputStateTypeInfo
    {
        public FourCC format => new FourCC('M', 'U', 'D', 'R');

        [InputControl(layout = "Axis")]
        public float pressure;

        //[InputControl(layout = "Quaternion")]
        //public Quaternion quaternion;

        [InputControl(layout = "Integer")]
        public int lastGesture;

        //[InputControl(layout = "Vector3")]
        //public Vector3 AccRaw;

        //[InputControl(layout = "Vector3")]
        //public Vector3 SncRaw;
    }




#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [InputControlLayout(displayName = "Mudra Device", stateType = typeof(MudraDeviceState))]
    public class MudraDevice : InputDevice, IInputUpdateCallbackReceiver
    {
        public DeviceIdentifier identifier;
        public DeviceData deviceData;

        public AxisControl pressure { get; private set; }
       // public QuaternionControl quaternion { get; private set; }
        public IntegerControl lastGesture { get; private set; }
       // public Vector3Control accRaw { get; private set; }
       // public Vector3Control sncRaw { get; private set; }

        public MudraDevice(DeviceIdentifier identifier)
        {
            this.identifier = identifier;
            Init();
        }

        public MudraDevice()
        {
            Init();
            Debug.Log("CreatedMudraDevice");
        }

        protected bool _isGestureEnabled = true;
        public bool IsGestureEnabled
        {
            get { return _isGestureEnabled; }
            set
            {
                if (_isGestureEnabled != value)
                {
                    _isGestureEnabled = value;
                    MudraUnityManager.plugin.UpdateOnGestureReadyCallback(identifier.id);
                }
            }
        }

        protected bool _isFingerTipPressureEnabled = true;
        public bool IsFingerTipPressureEnabled
        {
            get { return _isFingerTipPressureEnabled; }
            set
            {
                if (_isFingerTipPressureEnabled != value)
                {
                    _isFingerTipPressureEnabled = value;
                    MudraUnityManager.plugin.UpdateOnFingerTipPressureCallback(identifier.id);
                }
            }
        }

        protected bool _isImuQuaternionEnabled = true;
        public bool IsImuQuaternionEnabled
        {
            get { return _isImuQuaternionEnabled; }
            set
            {
                if (_isImuQuaternionEnabled != value)
                {
                    _isImuQuaternionEnabled = value;
                    MudraUnityManager.plugin.UpdateOnQuaternionReadyCallback(identifier.id);
                }
            }
        }

        protected bool _isImuAccRawEnabled = true;

        public bool IsAccRawEnabled
        {
            get { return _isImuAccRawEnabled; }
            set
            {
                if (_isImuAccRawEnabled != value)
                {
                    _isImuAccRawEnabled = value;
                    MudraUnityManager.plugin.UpdateOnImuRawCallback(identifier.id);
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            InputSystem.RegisterLayout<MudraDevice>(
                matches: new InputDeviceMatcher()
                    .WithInterface("Mudra"));
        }




#if UNITY_EDITOR

        //[MenuItem("Tools/MudraDevice")]
#endif
        static MudraDevice()
        {
            InputSystem.RegisterLayout<MudraDevice>();
            Debug.Log("Registered MudraDevice Layout");
        }

        protected override void FinishSetup()
        {
            base.FinishSetup();

            pressure = GetChildControl<AxisControl>("pressure");
           // quaternion = GetChildControl<QuaternionControl>("quaternion");
            lastGesture = GetChildControl<IntegerControl>("lastGesture");
           // accRaw = GetChildControl<Vector3Control>("accRaw");
           // sncRaw = GetChildControl<Vector3Control>("sncRaw");

            Debug.Log("----------------------------------Finished Setup---------------------------------------");
        }

        public void OnGesture(GestureType gesture)
        {
            deviceData.lastGesture = gesture;
        }

        public void OnPressure(float pressure)
        {
            deviceData.fingerTipPressure = pressure;
        }
        public void OnAccRaw(float[] acc)
        {
            deviceData.accRaw = new Vector3(acc[0], acc[1], acc[2]);
        }

        public void OnSNCRaw(float[] snc)
        {
            //deviceData.sncRaw = new Vector3(snc[0], snc[1], snc[2]);
            //Debug.Log("AAAAAAAAAAAA"+deviceData.sncRaw);
        }


        public void OnQuaternion(Quaternion quaternion)
        {
            float angle = 2 * (Mathf.Acos(quaternion[0]));

            float x = quaternion[1] / Mathf.Sin(angle / 2);
            float y = quaternion[2] / Mathf.Sin(angle / 2);
            float z = quaternion[3] / Mathf.Sin(angle / 2);

            Quaternion newQuat = rightCoordToUnityCord(Quaternion.AngleAxis(Mathf.Rad2Deg * angle, new Vector3(x, y, z)));

            newQuat *= Quaternion.Euler(new Vector3(-90, 180, 0));

            deviceData.quaternion = newQuat;

        }

        private Quaternion rightCoordToUnityCord(Quaternion q)
        {
            return new Quaternion(q.x, q.y * -1, q.z * -1, q.w);
        }

        public void OnUpdate()
        {
            MudraDeviceState state = new MudraDeviceState();

            if (deviceData.fingerTipPressure != null)
                state.pressure = (float)deviceData.fingerTipPressure;

            // if (deviceData.quaternion != null)
            //state.quaternion = (Quaternion)deviceData.quaternion;

            if (deviceData.lastGesture != null)
            {
                state.lastGesture = (int)deviceData.lastGesture;
                Debug.Log("AAAAAAAAAA:" + state.lastGesture);
            }

            if (deviceData.accRaw != null)
               // state.AccRaw = (Vector3)deviceData.accRaw;

            if (deviceData.sncRaw != null)
            {
               // state.SncRaw = (Vector3)deviceData.sncRaw;
                //Debug.Log((Vector3)deviceData.sncRaw);
            }
            InputSystem.QueueDeltaStateEvent(this, state);
        }

        [RuntimeInitializeOnLoadMethod]
        private static void InitializeInPlayer() { }
    }

}
#endif