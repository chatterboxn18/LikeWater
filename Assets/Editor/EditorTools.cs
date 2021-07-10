using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class EditorTools : MonoBehaviour
{
    [MenuItem("Tools/OpenPersistentDataPath")]
    static void OpenPersistent()
    {
        EditorUtility.RevealInFinder(Application.persistentDataPath);
    }

    [MenuItem("Tools/ClearPreferences")]
    static void ClearPreference()
    {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("Tools/TakeScreenshot")]
    static void Screenshot()
    {
        ScreenCapture.CaptureScreenshot(Path.Combine(Application.persistentDataPath, "screenshot.png"));
    }
}
