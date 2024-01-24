//using UnityEngine;

//namespace Mudra.Unity
//{
//    public class Plugin
//    {
//        public PluginPlatform _pluginPlatform;

//        private static Plugin instance = null;
//        public static Plugin Instance
//        {
//            get
//            {
//                if (instance == null)
//                {
//                    instance = new Plugin();
//                }
//                return instance;
//            }
//        }

//        private Plugin()
//        {

//#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN) || NETFX_CORE || UNITY_WSA || UNITY_WSA_10_0
//            //  _pluginPlatform = new WindowsPlugin();
//#elif (UNITY_ANDROID)
//            _pluginPlatform = new MudraUnityAndroidPlugin();
//#elif (UNITY_EDITOR_OSX || UNITY_IOS)
//            Logger.Print("MudraUnityPlugin new iOS_PlugIn()");
//            //_pluginPlatform = new iOS_PlugIn();
//#endif

//        }

//        public void Init(string calibrationFile)
//        {
//            _pluginPlatform.Init(calibrationFile);
//        }
      
//        [RuntimeInitializeOnLoadMethod]
//        public void Init()
//        {
//            Debug.Log("Wee");

//            _pluginPlatform.Init();
//        }

//        public void Update()
//        {
//            _pluginPlatform.Update();
//        }

//        public void ClearFrame()
//        {
//            _pluginPlatform.Clear();
//        }

//        public void Close()
//        {
//            _pluginPlatform.Close();
//        }


//        public bool IsGestureEnabled
//        {
//            get
//            {
//                return _pluginPlatform.IsGestureEnabled;
//            }
//            set
//            {
//                _pluginPlatform.IsGestureEnabled = value;
//            }
//        }

//        public bool IsFingerTipPressureEnabled
//        {
//            get { return _pluginPlatform.IsFingerTipPressureEnabled; }
//            set
//            {
//                _pluginPlatform.IsFingerTipPressureEnabled = value;
//            }
//        }

//        public bool IsAirMouseEnabled
//        {
//            get { return _pluginPlatform.IsAirMouseEnabled; }
//            set
//            {
//                _pluginPlatform.IsAirMouseEnabled = value;
//            }
//        }

//        public bool IsImuQuaternionEnabled
//        {
//            get { return _pluginPlatform.IsImuQuaternionEnabled; }
//            set
//            {
//                _pluginPlatform.IsImuQuaternionEnabled = value;
//            }
//        }


//    }
//}


