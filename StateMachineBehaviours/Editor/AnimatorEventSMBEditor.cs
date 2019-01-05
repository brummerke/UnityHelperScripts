using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(AnimatorEventSMB))]
public class AnimatorEventSMBEditor : Editor {
	private List<string> eventsAvailable = new List<string>();

	private ReorderableList list_onStateEnterTransitionStart;
	private ReorderableList list_onStateEnterTransitionEnd;
	private ReorderableList list_onStateExitTransitionStart;
	private ReorderableList list_onStateExitTransitionEnd;
	private ReorderableList list_onNormalizedTimeReached;
	private ReorderableList list_onStateUpdated;

	private readonly List<GameObject> gameObjectsWithAnimator = new List<GameObject>();
	private StateMachineBehaviourContext[] contexts;

	private void OnEnable() {
		CreateReorderableList("On State Enter Transition Start", 20, ref list_onStateEnterTransitionStart, serializedObject.FindProperty("onStateEnterTransitionStart"),
			(rect, index, isActive, isFocused) => {
				EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, 18), serializedObject.FindProperty("onStateEnterTransitionStart").GetArrayElementAtIndex(index).FindPropertyRelative("callback"), GUIContent.none);
			});
		CreateReorderableList("On State Enter Transition End", 20, ref list_onStateEnterTransitionEnd, serializedObject.FindProperty("onStateEnterTransitionEnd"),
			(rect, index, isActive, isFocused) => {
				EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, 18), serializedObject.FindProperty("onStateEnterTransitionEnd").GetArrayElementAtIndex(index).FindPropertyRelative("callback"), GUIContent.none);
			});
		CreateReorderableList("On State Exit Transition Start", 20, ref list_onStateExitTransitionStart, serializedObject.FindProperty("onStateExitTransitionStart"),
			(rect, index, isActive, isFocused) => {
				EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, 18), serializedObject.FindProperty("onStateExitTransitionStart").GetArrayElementAtIndex(index).FindPropertyRelative("callback"), GUIContent.none);
			});
		CreateReorderableList("On State Exit Transition End", 20, ref list_onStateExitTransitionEnd, serializedObject.FindProperty("onStateExitTransitionEnd"),
			(rect, index, isActive, isFocused) => {
				EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, 18), serializedObject.FindProperty("onStateExitTransitionEnd").GetArrayElementAtIndex(index).FindPropertyRelative("callback"), GUIContent.none);
			});
		CreateReorderableList("On State Update", 20, ref list_onStateUpdated, serializedObject.FindProperty("onStateUpdated"),
			(rect, index, isActive, isFocused) => {
				EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, 18), serializedObject.FindProperty("onStateUpdated").GetArrayElementAtIndex(index).FindPropertyRelative("callback"), GUIContent.none);
			});
		CreateReorderableList("On Normalized Time Reached", 60, ref list_onNormalizedTimeReached, serializedObject.FindProperty("onNormalizedTimeReached"),
			(rect, index, isActive, isFocused) => {
				var property = serializedObject.FindProperty("onNormalizedTimeReached").GetArrayElementAtIndex(index);

				float timeBefore = property.FindPropertyRelative("normalizedTime").floatValue;

				EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, 18), property.FindPropertyRelative("callback"));
				bool timeEdited = EditorGUI.PropertyField(new Rect(rect.x, rect.y + 20, rect.width, 20), property.FindPropertyRelative("normalizedTime"));
				EditorGUI.PropertyField(new Rect(rect.x, rect.y + 40, rect.width, 20), property.FindPropertyRelative("repeat"));

				float timeNow = property.FindPropertyRelative("normalizedTime").floatValue;

				if (timeBefore != timeNow) {
					if (!AnimationMode.InAnimationMode())
						AnimationMode.StartAnimationMode();

					foreach (var context in contexts) {
						AnimatorState state = context.animatorObject as AnimatorState;
						var previewClip = state.motion as AnimationClip;
						if (null == previewClip) continue;
						foreach (var targetObj in gameObjectsWithAnimator) {
							AnimationMode.BeginSampling();
							AnimationMode.SampleAnimationClip(targetObj, previewClip, property.FindPropertyRelative("normalizedTime").floatValue * previewClip.length);
							AnimationMode.EndSampling();
						}
					}
				}
			});
	}

	private void CreateReorderableList(string title, int height, ref ReorderableList reorderableList, SerializedProperty soList, ReorderableList.ElementCallbackDelegate drawCallback) {
		reorderableList = new ReorderableList(serializedObject, soList, true, false, true, true);
		reorderableList.elementHeight = height;
		reorderableList.drawHeaderCallback = (rect) => {
			GUI.Label(rect, title);
		};
		reorderableList.drawElementCallback = drawCallback;
		reorderableList.onAddDropdownCallback = (buttonRect, list) => {
			var menu = new GenericMenu();
			for (int i = 0; i < eventsAvailable.Count; i++) {
				menu.AddItem(new GUIContent(eventsAvailable[i]),
				false, (data) => {
					serializedObject.Update();
					soList.InsertArrayElementAtIndex(soList.arraySize);
					soList.GetArrayElementAtIndex(soList.arraySize - 1).FindPropertyRelative("callback").stringValue = data as string;
					serializedObject.ApplyModifiedProperties();
				}, eventsAvailable[i]);
			}
			menu.ShowAsContext();
		};
	}

	public override void OnInspectorGUI() {
		//serializedObject.ShowScriptInput();
		//DrawDefaultInspector();

		if (eventsAvailable.Count == 0) {
			gameObjectsWithAnimator.Clear();
			contexts = UnityEditor.Animations.AnimatorController.FindStateMachineBehaviourContext((AnimatorEventSMB) target);
			var aes = GameObject.FindObjectsOfType<AnimatorEvent>();

			foreach (var c in contexts) {
				foreach (var ae in aes) {
					if (ae.GetComponent<Animator>().runtimeAnimatorController == c.animatorController) {
						gameObjectsWithAnimator.Add(ae.gameObject);
						foreach (var ev in ae.events) {
							if (!eventsAvailable.Contains(ev.name)) {
								eventsAvailable.Add(ev.name);
							}
						}
					}
				}
			}

			eventsAvailable.Sort();
			eventsAvailable.Add("New event");
		}

		serializedObject.Update();

		list_onStateEnterTransitionStart.DoLayoutList();
		list_onStateEnterTransitionEnd.DoLayoutList();
		list_onStateExitTransitionStart.DoLayoutList();
		list_onStateExitTransitionEnd.DoLayoutList();
		list_onStateUpdated.DoLayoutList();
		list_onNormalizedTimeReached.DoLayoutList();

		serializedObject.ApplyModifiedProperties();
	}

	private void OnDestroy() {
		if (AnimationMode.InAnimationMode())
			AnimationMode.StopAnimationMode();
	}
}
