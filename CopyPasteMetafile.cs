#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;

public class CopyPasteMetafile : EditorWindow
{
    private Object fromFile;
    private Object toFile;
    private bool keepGuid = true;

    [MenuItem("Window/brumTech/CopyPasteMeta")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CopyPasteMetafile));
    }

    private void OnGUI()
    {
        fromFile = EditorGUILayout.ObjectField("Copy metadata from", fromFile, typeof(Object), true);
        toFile = EditorGUILayout.ObjectField("Copy metadata to", toFile, typeof(Object), true);
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Keep original GUID (recommended)");
        keepGuid = EditorGUILayout.Toggle(keepGuid);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Replace Meta"))
        {
            UpdateButtons();
        }
        if (GUILayout.Button("Debug"))
        {
            DebugMetas();
        }

        GUILayout.EndHorizontal();
    }

    private void UpdateButtons()
    {
        string pathSource = AssetDatabase.GetAssetPath(fromFile);
        pathSource += ".meta";

        string pathTarget = AssetDatabase.GetAssetPath(toFile);
        pathTarget += ".meta";

        string[] arrSource = File.ReadAllLines(pathSource);
        string[] arrTarget = File.ReadAllLines(pathTarget);

        Debug.Log("before" + arrSource[1] + " " + arrTarget[1]);

        string originalGuidTarget = arrTarget[1];

        arrTarget = arrSource;

        if (keepGuid)
            arrTarget[1] = originalGuidTarget;

        File.WriteAllLines(pathTarget, arrTarget);

        Debug.Log("Success");
    }

    private void DebugMetas()
    {
        string pathSource = AssetDatabase.GetAssetPath(fromFile);
        pathSource += ".meta";

        string pathTarget = AssetDatabase.GetAssetPath(toFile);
        pathTarget += ".meta";

        string[] arrSource = File.ReadAllLines(pathSource);
        string[] arrTarget = File.ReadAllLines(pathTarget);

        Debug.Log("____________SOURCE______________");
        for (int i = 0; i < arrSource.Length; i++)
        {
            Debug.Log(i + " " + arrSource[i]);
        }
        Debug.Log("  ");

        Debug.Log("____________TARGET______________");
        for (int i = 0; i < arrTarget.Length; i++)
        {
            Debug.Log(i + " " + arrTarget[i]);
        }
        Debug.Log("  ");
    }
}

#endif