#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SceneToolbox : EditorWindow
{
    private List<string> sceneFiles;

    private List<List<string>> filteredScenes;

#pragma warning disable 0649
    //this garbage has to be done because unity doesnt store type list<list> or some shit
    [SerializeField]
    private List<string> l0;
    [SerializeField]
    private List<string> l1;
    [SerializeField]
    private List<string> l2;
    [SerializeField]
    private List<string> l3;
    [SerializeField]
    private List<string> l4;
    [SerializeField]
    private List<string> l5;
    [SerializeField]
    private List<string> l6;
    [SerializeField]
    private List<string> l7;
    [SerializeField]
    private List<string> l8;
    [SerializeField]
    private List<string> l9;
#pragma warning restore 0649

    private Vector2 scrollPos;

    private int currSelectedNameButton = 0;

    private bool settingsView = false;
    private GUIStyle style;

    private string newFilter;

    [SerializeField]
    private List<string> buttonNames = new List<string>() { "1, 2, 3" };


    [MenuItem("Window/brumTech/ScenePicker &s")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SceneToolbox));
    }

    protected void OnEnable()
    {
        if (filteredScenes == null || filteredScenes.Count < 10)
        {
            filteredScenes = new List<List<string>>();
            for (int i = 0; i < 10; i++)
            {
                filteredScenes.Add(new List<string> { "" });
            }
        }

        // Here we retrieve the data if it exists or we save the default field initialisers we set above
        var data = EditorPrefs.GetString("ScenePicker", JsonUtility.ToJson(this, false));
        // Then we apply them to this window
        JsonUtility.FromJsonOverwrite(data, this);

        filteredScenes[0] = l0;
        filteredScenes[1] = l1;
        filteredScenes[2] = l2;
        filteredScenes[3] = l3;
        filteredScenes[4] = l4;
        filteredScenes[5] = l5;
        filteredScenes[6] = l6;
        filteredScenes[7] = l7;
        filteredScenes[8] = l8;
        filteredScenes[9] = l9;
    }

    protected void OnDisable()
    {
        // We get the Json data
        var data = JsonUtility.ToJson(this, false);
        // And we save it
        EditorPrefs.SetString("ScenePicker", data);

        // Et voilà !
    }

    private void OnGUI()
    {
        if (buttonNames == null)
            buttonNames = new List<string>() { "1", "2", "3" };

        if (sceneFiles == null || sceneFiles.Count == 0 || sceneFiles[0].Length <= 1)
            LoadScenes();

        if (filteredScenes == null || filteredScenes.Count < 10)
        {
            filteredScenes = new List<List<string>>();
            for (int i = 0; i < 10; i++)
            {
                filteredScenes.Add(new List<string> { "" });
            }
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        if (style == null)
        {
            style = GUI.skin.button;
            style.alignment = TextAnchor.MiddleLeft;
        }
        if (!settingsView)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh Scenes"))
            {
                LoadScenes();
            }
            if (GUILayout.Button("Clear Filter"))
            {
                filteredScenes[currSelectedNameButton] = new List<string>();
                DisplayScenes();
            }
            if (GUILayout.Button("Settings"))
                settingsView = true;

            GUILayout.EndHorizontal();

            DisplayScenes();
        }
        else
            SettingsView();

        GUILayout.EndScrollView();
    }

    private void SettingsView()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("SETTINGS");
        if (GUILayout.Button("< Back to scenes"))
            settingsView = false;
        GUILayout.EndHorizontal();

        if (buttonNames != null)
        {
            for (int i = 0; i < buttonNames.Count; i++)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Del", GUILayout.Width(32f)))
                {
                    buttonNames.RemoveAt(i);
                    return;
                }
                buttonNames[i] = EditorGUILayout.TextField(buttonNames[i]);

                GUILayout.EndHorizontal();
            }
        }
        if (GUILayout.Button("Add") && buttonNames.Count < 10)
        {
            buttonNames.Add((buttonNames.Count + 1).ToString());
        }
    }

    private List<string> GetStringFilterList(string name, bool flip)
    {
        if (!flip)
        {
            List<string> stringsToAdd = new List<string>();
            foreach (string sceneString in sceneFiles)
                if (sceneString.Contains(name))
                    stringsToAdd.Add(sceneString);

            return stringsToAdd;
        }
        else
        {
            List<string> stringsToAdd = new List<string>();
            foreach (string sceneString in sceneFiles)
                if (!sceneString.Contains(name))
                    stringsToAdd.Add(sceneString);

            return stringsToAdd;
        }
    }

    private void DisplayScenes()
    {
        float w = position.size.x - (48f * 2f) - 30f;
        DisplayViewType();
        List<string> currentRemoveFilter = filteredScenes[(int)currSelectedNameButton];
        for (int i = 0; i < sceneFiles.Count; i++)
        {
            if (!currentRemoveFilter.Contains(sceneFiles[i]))
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Unpin", GUILayout.Width(48f)))
                {
                    currentRemoveFilter.Add(sceneFiles[i]);
                    DisplayScenes();
                    return;
                }

                string[] pathsplit = sceneFiles[i].Split('/');
                pathsplit[pathsplit.Length - 1] = pathsplit[pathsplit.Length - 1].Replace(".unity", "");

                if (GUILayout.Button("Select", GUILayout.Width(48f)))
                {
                    EditorUtility.FocusProjectWindow();
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(sceneFiles[i], typeof(SceneAsset)));
                }

                if (GUILayout.Button("LOAD: " + pathsplit[pathsplit.Length - 1] + " (" + pathsplit[pathsplit.Length - 2] + ") ", style, GUILayout.MaxWidth(w)))
                {
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                    EditorSceneManager.OpenScene(sceneFiles[i], OpenSceneMode.Single);
                }
                GUILayout.EndHorizontal();
            }
        }
    }

    private void DisplayViewType()
    {
        GUILayout.BeginHorizontal();

        for (int i = 0; i < buttonNames.Count; i++)
        {
            if (currSelectedNameButton == i)
            {
                GUILayout.Label(buttonNames[i]);
            }
            else
                if (GUILayout.Button(buttonNames[i], GUILayout.ExpandWidth(true)))
                currSelectedNameButton = i;
        }

        GUILayout.Label("Filter:", GUILayout.ExpandWidth(false));
        newFilter = EditorGUILayout.TextField(newFilter);
        if (GUILayout.Button("Remove") && newFilter.Length > 0)
        {
            filteredScenes[currSelectedNameButton].AddRange(GetStringFilterList(newFilter, false));
            newFilter = "";
        }
        if (GUILayout.Button("Solo") && newFilter.Length > 0)
        {
            filteredScenes[currSelectedNameButton].AddRange(GetStringFilterList(newFilter, true));
            newFilter = "";
        }

        GUILayout.EndHorizontal();
    }

    private void LoadScenes()
    {
        sceneFiles = new List<string>();
        string[] sceneGuids = AssetDatabase.FindAssets("t:SceneAsset");

        for (int i = 0; i < sceneGuids.Length; i++)
        {
            sceneFiles.Add(AssetDatabase.GUIDToAssetPath(sceneGuids[i]));
        }
    }

    public enum SceneToolboxViewType
    {
        NamedButton,
        Filter
    }
}

#endif