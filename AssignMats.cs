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
        if (GUILayout.Button("Replace Materials"))
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
            for (int j = 0; j < renderers[i].materials.Length; j++)
            renderers[i].materials[j] = g;
        }

        SkinnedMeshRenderer[] renderersSkin = Selection.activeGameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

        for (int i = 0; i < renderersSkin.Length; i++)
        {
            for (int j = 0; j < renderersSkin[i].materials.Length; j++)
                renderersSkin[i].materials[j] = g;
        }

        EditorSceneManager.MarkAllScenesDirty();
    }
}

#endif