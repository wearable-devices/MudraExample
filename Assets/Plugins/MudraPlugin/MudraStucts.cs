

using System.Runtime.InteropServices;
using UnityEngine;

namespace Mudra.Unity
{
    public struct DeviceData
    {

        public Vector2? airMousePos;
        public float? fingerTipPressure;
        public GestureType? lastGesture;
        public Quaternion? quaternion;
        public Vector3? accRaw;
        public Vector3? sncRaw;

    }

    [StructLayout(LayoutKind.Sequential,CharSet = CharSet.Ansi)]
    public struct DeviceIdentifier
    {
        //[MarshalAs(UnmanagedType.LPStr)]
        //public string name;

        //[MarshalAs(UnmanagedType.LPStr)]
        //public string uuid;

        public int id;
    }
}