#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CopyPasteMetafile : EditorWindow
{
    private Object fromFile;
    string feedbackString;

    [MenuItem("Window/brumTech/CopyPasteMeta")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CopyPasteMetafile));
    }

    private void OnGUI()
    {
        fromFile = EditorGUILayout.ObjectField("Copy metadata from", fromFile, typeof(Object), true);
        EditorGUILayout.LabelField("Current meta file: " + (fromFile != null ? fromFile.name : "NONE") + "  Selected objects: " + Selection.objects.Length);

        if (fromFile != null && Selection.activeObject != null)
        {
            if (GUILayout.Button("Copy meta to selected object(s)"))
            {
                ReplaceMeta();
            }
        }
        if (fromFile != null)
        {
            if (GUILayout.Button("Debug"))
            {
                DebugMetas();
            }
        }
        EditorGUILayout.LabelField(feedbackString);
    }


    private void ReplaceMeta()
    {
        //get source meta file
        string pathSource = AssetDatabase.GetAssetPath(fromFile);
        pathSource += ".meta";
        string[] arrSource = File.ReadAllLines(pathSource);

        //get all selected objects
        Object[] selectionObjects = Selection.objects;

        //store their meta file paths
        List<string> pathTargets = new List<string>();
        for (int i = 0; i < selectionObjects.Length; i++)
            pathTargets.Add(AssetDatabase.GetAssetPath(selectionObjects[i]) + ".meta");

        //read all the meta contents into a list of strings
        List<string[]> pathTargetContents = new List<string[]>();
        for (int i = 0; i < pathTargets.Count; i++)
            pathTargetContents.Add(File.ReadAllLines(pathTargets[i]));

        for (int i = 0; i < pathTargetContents.Count; i++)
        {
            string originalGuidTarget = pathTargetContents[i][1];
            pathTargetContents[i] = arrSource;
            pathTargetContents[i][1] = originalGuidTarget;
            File.WriteAllLines(pathTargets[i], pathTargetContents[i]);
            Debug.Log("done copying to " + pathTargets[i]);
        }
        feedbackString = "refresh (ctrl r) / reimport files for .meta changes to take effect";
        Debug.Log("Success");
    }

    private void DebugMetas()
    {
        string pathSource = AssetDatabase.GetAssetPath(fromFile);
        pathSource += ".meta";
        string[] arrSource = File.ReadAllLines(pathSource);

        Debug.Log("____________SOURCE______________");
        Debug.Log("object name: " + pathSource);
        for (int i = 0; i < arrSource.Length; i++)
        {
            Debug.Log(i + " " + arrSource[i]);
        }

        Debug.Log("  ");

        if (Selection.activeObject != null)
        {
            string pathSel = AssetDatabase.GetAssetPath(Selection.activeObject);
            pathSel += ".meta";
            string[] arrSel = File.ReadAllLines(pathSel);
            Debug.Log("___________ACTIVESELECTION____________");
            Debug.Log("object name: " + pathSel);
            for (int i = 0; i < arrSel.Length; i++)
            {
                Debug.Log(i + " " + arrSel[i]);
            }

            Debug.Log("  ");
        }
    }
}

#endif