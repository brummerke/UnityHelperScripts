#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;

public class AssignMats : EditorWindow
{

    Material g;
    [MenuItem("Window/brumTech/Assign Mats")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AssignMats));
    }

    private void OnGUI()
    {
        g = (Material)EditorGUILayout.ObjectField(g, typeof(Material), true);
        if (GUILayout.Button("Fix Sources"))
        {
            if (g != null)
                UpdateButtons();
        }
    }

    private void UpdateButtons()
    {
        MeshRenderer[] renderers = Selection.activeGameObject.GetComponentsInChildren<MeshRenderer>();
       
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = g;
        }

        EditorSceneManager.MarkAllScenesDirty();
    }
}

#endif