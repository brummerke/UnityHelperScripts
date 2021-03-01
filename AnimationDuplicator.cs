#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;

public class AnimationDuplicator : EditorWindow
{
    [MenuItem("Assets/Duplicate out Animation")]
    private static void DoDuplicateAnim()
    {
        DoAnimDuplOnFbx(Selection.activeGameObject, true);
    }

    // Note that we pass the same path, and also pass "true" to the second argument.
    [MenuItem("Assets/Duplicate out Animation", true)]
    private static bool DoDuplicateAnimValidaton()
    {
        Object main = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(Selection.activeObject));
        string mainPath = AssetDatabase.GetAssetPath(main);
        if (mainPath.Contains(".fbx"))
            return true;
        else
            return false;
    }

    [MenuItem("Assets/Debug animation info")]
    private static void DoAnimDebug()
    {
        DoAnimDebug((AnimationClip)Selection.activeObject);
    }
    // Note that we pass the same path, and also pass "true" to the second argument.
    [MenuItem("Assets/Debug animation info", true)]
    private static bool DoAnimDebugValidation()
    {
        Object main = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(Selection.activeObject));
        string mainPath = AssetDatabase.GetAssetPath(main);
        if (mainPath.Contains(".anim"))
            return true;
        else
            return false;
    }
   public static void DoAnimDebug(AnimationClip animObj)
    {
        Debug.Log(animObj.legacy);
    }

    public static void DoAnimDuplOnFbx(Object fbxObject, bool doLegacyAnimMode)
    {
        if (AssetDatabase.IsMainAsset(fbxObject))
        {
            Object main = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(fbxObject));
            string mainPath = AssetDatabase.GetAssetPath(main);
            if (mainPath.Contains(".fbx"))
            {
                string mainDir = Path.GetDirectoryName(mainPath);
                string mainFileName = Path.GetFileNameWithoutExtension(mainPath);

                AnimationClip clip = AssetDatabase.LoadAssetAtPath(mainPath, typeof(AnimationClip)) as AnimationClip;
                /*
                Debug.Log(mainPath);
                Debug.Log(mainDir);
                Debug.Log(mainFileName);
                Debug.Log(clip.name);
                */
                AnimationClip new_clip = UnityEngine.Object.Instantiate(clip);

                if (doLegacyAnimMode)
                    new_clip.legacy = true;
             

                new_clip.name = mainFileName;
                string newFilePath = Path.Combine(mainDir, mainFileName);
                string p = Path.Combine(mainDir, mainFileName) + ".anim";
                if (File.Exists(p))
                {
                    Debug.Log("existing animation detected, renaming");
                    newFilePath += "new";
                }
                newFilePath += ".anim";

                AssetDatabase.CreateAsset(new_clip, newFilePath);

                EditorUtility.FocusProjectWindow();
                Selection.activeObject = AssetDatabase.LoadAssetAtPath(newFilePath, typeof(Object));
                EditorGUIUtility.PingObject(Selection.activeObject);
            }
        }
    }
}

#endif