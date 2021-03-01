#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EventCopier : EditorWindow
{
    private Object sourceObject;
    private AnimationClip sourceClip;
    // private AnimationClip sourceObject;
    private AnimationClip targetClip;
    private Object lastSelected;
    private float normTime;

    private TimeDisplayType currTimeDisplayType = TimeDisplayType.Frames;

    [MenuItem("Window/brumTech/EventCopier")]
    private static void Init()
    {
        GetWindow(typeof(EventCopier));
    }

    /*
    protected void OnEnable()
    {
        // Here we retrieve the data if it exists or we save the default field initialisers we set above
        var data = EditorPrefs.GetString("EventCopier", JsonUtility.ToJson(this, false));
        // Then we apply them to this window
        JsonUtility.FromJsonOverwrite(data, this);
    }

    protected void OnDisable()
    {
        // We get the Json data
        var data = JsonUtility.ToJson(this, false);
        // And we save it
        EditorPrefs.SetString("EventCopier", data);

        // Et voilà !
    }
    */

    private void OnGUI()
    {
        float w = position.size.x - 30f - 90f - 45f - 26f - 60f - 100f;
        EditorStyles.helpBox.wordWrap = false;
        if (lastSelected != Selection.activeObject)
            if (Selection.activeObject != null)
                targetClip = GetClip(Selection.activeObject);

        EditorGUILayout.BeginHorizontal();
        //   sourceObject = EditorGUILayout.ObjectField("Source", sourceObject, typeof(AnimationClip), true) as AnimationClip;
        GUILayout.Label("Source clip: " + (sourceClip == null ? "NONE" : sourceClip.name));
        if (GUILayout.Button("Lock selection as source clip"))
        {
            sourceClip = GetClip(Selection.activeObject);
            if (sourceClip != null)
            {
                sourceObject = Selection.activeObject;
            }
            else
                Debug.Log("No animation clip selected");
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Label("Target clip: " + (targetClip == null ? "NONE" : targetClip.name));

        EditorGUILayout.BeginHorizontal();

        //if selection has changed

        GUILayout.EndHorizontal();

        if (sourceClip != null && targetClip != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Targe Time: " + CalcTimeDisplay(normTime), GUILayout.ExpandWidth(false));
            normTime = GUILayout.HorizontalSlider(normTime, 0f, 1f);
            currTimeDisplayType = (TimeDisplayType)EditorGUILayout.EnumPopup(currTimeDisplayType, GUILayout.Width(70f));
            GUILayout.EndHorizontal();

            for (int i = 0; i < sourceClip.events.Length; i++)
            {
                AnimationEvent currEvent = sourceClip.events[i];
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Del", GUILayout.Width(30f)))
                    DeleteEvent(i);

                if (GUILayout.Button("Copy Event " + i, GUILayout.Width(90f)))
                    CopyEvent(currEvent);

                EditorGUILayout.LabelField("{" + CalcTimeDisplay(currEvent.time) + "}", EditorStyles.helpBox, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(45f));

                EditorGUILayout.LabelField(currEvent.functionName, EditorStyles.helpBox, GUILayout.MaxWidth(w));
                EditorGUILayout.LabelField("s:" + currEvent.stringParameter.Substring(0, (currEvent.stringParameter.Length > 10 ? 10 : currEvent.stringParameter.Length)), EditorStyles.helpBox, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(100f));
                EditorGUILayout.LabelField("f:" + currEvent.floatParameter, EditorStyles.helpBox, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(60f));
                EditorGUILayout.LabelField("i:" + currEvent.intParameter, EditorStyles.helpBox, GUILayout.MaxWidth(26f));

                GUILayout.EndHorizontal();
            }
        }
        else
            GUILayout.Label("Select a clip in project and click [Lock selection]");

        GUILayout.Space(16f);

        if (targetClip != null)
        {
            if (targetClip.events == null && targetClip.events.Length == 0)
                GUILayout.Label("Clip has no events");
            else
                GUILayout.Label("Target clip events");

            for (int i = 0; i < targetClip.events.Length; i++)
            {
                AnimationEvent currEvent = targetClip.events[i];
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("{" + CalcTimeDisplay(currEvent.time) + "}", EditorStyles.helpBox, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(45f));

                EditorGUILayout.LabelField(currEvent.functionName, EditorStyles.helpBox, GUILayout.MaxWidth(w));
                EditorGUILayout.LabelField("s:" + currEvent.stringParameter.Substring(0, (currEvent.stringParameter.Length > 10 ? 10 : currEvent.stringParameter.Length)), EditorStyles.helpBox, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(100f));
                EditorGUILayout.LabelField("f:" + currEvent.floatParameter, EditorStyles.helpBox, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(60f));
                EditorGUILayout.LabelField("i:" + currEvent.intParameter, EditorStyles.helpBox, GUILayout.MaxWidth(26f));

                GUILayout.EndHorizontal();
            }
        }
        else
        {
            GUILayout.Label("Select a clip in project ");
        }

        lastSelected = Selection.activeObject;
    }

    private void CopyEvent(AnimationEvent copiedEvent)
    {
        List<AnimationEvent> targetList = new List<AnimationEvent>(AnimationUtility.GetAnimationEvents(targetClip));
        copiedEvent.time = normTime;
        targetList.Add(copiedEvent);
        AnimationUtility.SetAnimationEvents(targetClip, targetList.ToArray());

        GetWindow(typeof(EventCopier)).Repaint();
        GetWindow(typeof(Animation)).Repaint();
    }

    private AnimationClip GetClip(Object checkObject)
    {
        if (checkObject.GetType() == typeof(AnimationClip))
        {
            return (AnimationClip)checkObject as AnimationClip;
        }
        else
            return null;
    }

    private void DeleteEvent(int id)
    {
        List<AnimationEvent> origList = new List<AnimationEvent>(AnimationUtility.GetAnimationEvents(sourceClip));
        origList.RemoveAt(id);
        AnimationUtility.SetAnimationEvents(targetClip, origList.ToArray());

        GetWindow(typeof(EventCopier)).Repaint();
        GetWindow(typeof(Animation)).Repaint();
    }

    private string CalcTimeDisplay(float eventNormTime)
    {
        switch (currTimeDisplayType)
        {
            case TimeDisplayType.Normalized:
                return eventNormTime.ToString("0.0");

            case TimeDisplayType.Frames:
                return ((int)(eventNormTime * (targetClip.length * targetClip.frameRate))).ToString();

            case TimeDisplayType.Seconds:
                return Mathf.Lerp(0f, targetClip.length, eventNormTime).ToString("0.00");
        }
        return "ERR";
    }

    public enum TimeDisplayType
    {
        Normalized,
        Frames,
        Seconds
    }
}

#endif