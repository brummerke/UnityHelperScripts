#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class FbxImportFixer : EditorWindow
{
    //options
    [SerializeField]
    private bool doRot = true;
    [SerializeField]
    private float rotVal = 0.01f;
    [SerializeField]
    private bool doScale = true;
    [SerializeField]
    private float scaleVal = 0.01f;
    [SerializeField]
    private bool doPos = true;
    [SerializeField]
    private float posVal = 0.01f;
    [SerializeField]
    private bool doAnimType = true;
    [SerializeField]
    private bool doImportType = true;
    [SerializeField]
    private ModelImporterAnimationType currImportType = ModelImporterAnimationType.Legacy;
    [SerializeField]
    private bool duplicateOutAnim = true;
    [SerializeField]
    private bool doBones = false;
    [SerializeField]
    private int boneCount = 4;
    //only above 2018
 //   [SerializeField] 
//private ModelImporterSkinWeights currSkinWeightsType;
    [SerializeField]
    private float currMinBoneWeight = 0.001f;

    [SerializeField]
    private bool experimentalApplyImportSettFirst = false;

    public static List<Object> animDuplQueue = new List<Object>();

    [MenuItem("Window/brumTech/FbxImportFixer")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FbxImportFixer));
    }

    protected void OnEnable()
    {
        // Here we retrieve the data if it exists or we save the default field initialisers we set above
        var data = EditorPrefs.GetString("FbxImportFixer", JsonUtility.ToJson(this, false));
        // Then we apply them to this window
        JsonUtility.FromJsonOverwrite(data, this);
    }

    protected void OnDisable()
    {
        // We get the Json data
        var data = JsonUtility.ToJson(this, false);
        // And we save it
        EditorPrefs.SetString("FbxImportFixer", data);

        // Et voilà !
    }

    private void DisplaySettings()
    {
        float w = 85f;
        float w2 = 22f;
        float w3 = 40f;
        GUILayout.BeginHorizontal();
        GUILayout.Label("Set RotError", GUILayout.Width(w));
        doRot = EditorGUILayout.Toggle(doRot, GUILayout.Width(w2));
        rotVal = EditorGUILayout.FloatField(rotVal, GUILayout.Width(w3));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Set RotError", GUILayout.Width(w));
        doPos = EditorGUILayout.Toggle(doPos, GUILayout.Width(w2));
        posVal = EditorGUILayout.FloatField(posVal, GUILayout.Width(w3));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Set ScaleError", GUILayout.Width(w));
        doScale = EditorGUILayout.Toggle(doScale, GUILayout.Width(w2));
        scaleVal = EditorGUILayout.FloatField(scaleVal, GUILayout.Width(w3));
        GUILayout.EndHorizontal();
        GUILayout.Space(10f);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Set Animation Type", GUILayout.Width(w + 15f));
        doImportType = EditorGUILayout.Toggle(doImportType, GUILayout.Width(w2));
        currImportType = (ModelImporterAnimationType)EditorGUILayout.EnumPopup(currImportType, GUILayout.Width(w));
        GUILayout.EndHorizontal();
        GUILayout.Space(10f);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Set Bones", GUILayout.Width(w - 15f));
        doBones = EditorGUILayout.Toggle(doBones, GUILayout.Width(w2));
        GUILayout.Label("SkinWeight Type", GUILayout.Width(w + 25f));
     //   currSkinWeightsType = (ModelImporterSkinWeights)EditorGUILayout.EnumPopup(currSkinWeightsType, GUILayout.Width(w));
        GUILayout.EndHorizontal();
        /*
        if (currSkinWeightsType == ModelImporterSkinWeights.Custom)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Bone Count", GUILayout.Width(w));
            boneCount = EditorGUILayout.IntSlider(boneCount, 1, 32);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Bone MinWeight", GUILayout.Width(w + 15f));
            currMinBoneWeight = EditorGUILayout.Slider(currMinBoneWeight, 0.001f, 0.5f);
            GUILayout.EndHorizontal();
            GUILayout.Space(10f);
        }
        */
        GUILayout.BeginHorizontal();
        GUILayout.Label("Dupliate anim out of fbx", GUILayout.Width(w + 55f));
        duplicateOutAnim = EditorGUILayout.Toggle(duplicateOutAnim, GUILayout.Width(w2));
        GUILayout.Label("Experimental ImportAsset", GUILayout.Width(w + 66f));
        experimentalApplyImportSettFirst = EditorGUILayout.Toggle(experimentalApplyImportSettFirst, GUILayout.Width(w2));
        GUILayout.EndHorizontal();
        GUILayout.Space(10f);
    }

    private void OnGUI()
    {
        GUILayout.Label("Selected files: " + Selection.objects.Length);
        DisplaySettings();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Change import settings"))
        {
            Object[] selObjects = Selection.objects;
            for (int i = 0; i < selObjects.Length; i++)
            {
                if (ValidateFbx(selObjects[i]))
                {
                    ModifyMeta(selObjects[i]);
                }
            }
            if (experimentalApplyImportSettFirst)
                for (int i = 0; i < selObjects.Length; i++)
                {
                    if (ValidateFbx(selObjects[i]))
                    {
                        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(selObjects[i]), ImportAssetOptions.ImportRecursive);
                    }
                }
        }
        if (GUILayout.Button("ManualAnimDupl"))
        {
            Object[] selObjects = Selection.objects;
            for (int i = 0; i < selObjects.Length; i++)
            {
                if (ValidateFbx(selObjects[i]))
                    AnimationDuplicator.DoAnimDuplOnFbx(selObjects[i], doAnimType);
            }
        }

        if (GUILayout.Button("DebugMeta"))
        {
            DebugMeta(Selection.activeObject);
        }

        GUILayout.EndHorizontal();
    }

    public static bool ValidateFbx(Object obj)
    {
        string mainPath = AssetDatabase.GetAssetPath(obj);
        if (mainPath.Contains(".fbx"))
            return true;
        else
            return false;
    }

    private void DebugMeta(Object metaSourceFile)
    {
        string pathSource = AssetDatabase.GetAssetPath(metaSourceFile);
        pathSource += ".meta";
        List<string> arrSource = new List<string>(File.ReadAllLines(pathSource));

        Debug.Log("_____________________________");
        Debug.Log("object name: " + pathSource);
        for (int i = 0; i < arrSource.Count; i++)
        {
            Debug.Log(i + " " + arrSource[i]);
        }
        Debug.Log("_____________________________");
    }

    private void ModifyMeta(Object fbxObject)
    {
        ModelImporter b = (ModelImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(fbxObject));

        if (doPos)
            b.animationPositionError = posVal;
        if (doRot)
            b.animationRotationError = rotVal;
        if (doScale)
            b.animationScaleError = scaleVal;
        if (doAnimType)
            b.animationType = currImportType;
        /*
        if (doBones)
        {
            b.skinWeights = currSkinWeightsType;
            if (currSkinWeightsType == ModelImporterSkinWeights.Custom)
            {
                b.maxBonesPerVertex = boneCount;
                b.minBoneWeight = currMinBoneWeight;
            }
        }
        */

        if (duplicateOutAnim)
            animDuplQueue.Add(fbxObject);
    }
}

public class Processor : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string str in importedAssets)
        {
            Object a = AssetDatabase.LoadAssetAtPath(str, typeof(Object));
            if (FbxImportFixer.animDuplQueue.Contains(a))
            {
                ModelImporter b = (ModelImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(a));
                Debug.Log("Reimported Asset: " + str);
                Debug.Log("Procceeding with animation duplication; Reimport posError: " + b.animationPositionError + " Reimport animType: " + b.animationType.ToString());
                AnimationDuplicator.DoAnimDuplOnFbx(a, true);
                FbxImportFixer.animDuplQueue.Remove(a);
            }
        }
        /*
        foreach (string str in deletedAssets)
        {
            Debug.Log("Deleted Asset: " + str);
        }

        for (int i = 0; i < movedAssets.Length; i++)
        {
            Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
        }
        */
    }
}

#endif