#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ScreenshotTaker : ScriptableWizard {

    [MenuItem("Custom/Take Screenshot")]
    static void TakeScreenshot()
    {
        string savePath = Path.Combine(Application.persistentDataPath, "screenshot.png");
        ScreenCapture.CaptureScreenshot(savePath);
        Debug.Log("ScreenshotTaker - Saved screenshot to " + savePath);
    }
}

#endif