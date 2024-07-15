using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;

public class MudraSettings : EditorWindow
{
    string path = "Assets/Plugins/MudraSettings.asset";
    static SerializedObject obj;
    static AddRequest Request;

    public delegate void FixFunc();
    public delegate bool CheckFunc();

    [MenuItem("Mudra/Mudra Setup")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MudraSettings));
    }

    public void FixButton(FixFunc func, string message)
    {
        if (GUILayout.Button(message))
        {
            func();
        }
    }
    void OnGUI()
    {

#if !ENABLE_INPUT_SYSTEM

        EditorGUILayout.HelpBox("The mudra plugin is based on the new input system, Please install the New Input System", MessageType.Error);

        if (Request == null)
        {

            if (GUILayout.Button("Install"))
            {
                Request = Client.Add("com.unity.inputsystem");
            }
        }
        else
        {
            GUILayout.Label("Installing:" + Request.Status);
        }

#elif !MUDRA_ENABLED
        if (GUILayout.Button("Enable Mudra"))
        {
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Android, "MUDRA_ENABLED");
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Standalone, "MUDRA_ENABLED");
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.iOS, "MUDRA_ENABLED");
            EditorGUILayout.HelpBox("Please wait for scripts to recompile", MessageType.Warning);

        }
#endif

#if !UNITY_ANDROID && !UNITY_IOS

        if (EditorUserBuildSettings.selectedBuildTargetGroup != BuildTargetGroup.Android && EditorUserBuildSettings.selectedBuildTargetGroup != BuildTargetGroup.iOS)
        {
            EditorGUILayout.HelpBox("Build target should be Android or iOS", MessageType.Error);
            GUILayout.BeginHorizontal();

            FixButton(() => EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android), "Android");

            FixButton(() => EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS), "iOS");

            GUILayout.EndHorizontal();
        }
#endif
#if UNITY_ANDROID

        ScriptingImplementation backend = PlayerSettings.GetScriptingBackend(NamedBuildTarget.Android);

        if (backend != ScriptingImplementation.IL2CPP)
        {
            EditorGUILayout.HelpBox("Scripting Backend should be IL2CPP", MessageType.Error);
            FixButton(() => PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP), "FIX");
        }

        AndroidArchitecture targetArchitecture = PlayerSettings.Android.targetArchitectures;
        if (targetArchitecture != AndroidArchitecture.ARM64)
        {
            EditorGUILayout.HelpBox("Target architecture should be only ARM64", MessageType.Error);
            FixButton(() => PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64, "FIX");
        }

        if ((int)PlayerSettings.Android.minSdkVersion < 26)
        {
            EditorGUILayout.HelpBox("Mudra does not support sdk versions lower than 26", MessageType.Error);
            FixButton(() => PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26, "FIX");

        }

#endif
    }
}
