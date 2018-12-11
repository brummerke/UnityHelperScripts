using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(AnimatorEventSMB))]
public class AnimatorEventSMBEditor : Editor {
	private List<string> eventsAvailable = new List<string>();

	private ReorderableList list_onStateEnterTransitionStart;
	private ReorderableList list_onStateEnterTransitionEnd;
	private ReorderableList list_onStateExitTransitionStart;
	private ReorderableList list_onStateExitTransitionEnd;
	private ReorderableList list_onStateUpdate;

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
		CreateReorderableList("On State Update", 60, ref list_onStateUpdate, serializedObject.FindProperty("onStateUpdate"),
			(rect, index, isActive, isFocused) => {
				EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, 18), serializedObject.FindProperty("onStateUpdate").GetArrayElementAtIndex(index).FindPropertyRelative("callback"));
				EditorGUI.PropertyField(new Rect(rect.x, rect.y + 20, rect.width, 20), serializedObject.FindProperty("onStateUpdate").GetArrayElementAtIndex(index).FindPropertyRelative("normalizedTime"));
				EditorGUI.PropertyField(new Rect(rect.x, rect.y + 40, rect.width, 20), serializedObject.FindProperty("onStateUpdate").GetArrayElementAtIndex(index).FindPropertyRelative("repeat"));
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
			var contexts = UnityEditor.Animations.AnimatorController.FindStateMachineBehaviourContext((AnimatorEventSMB) target);
			var aes = GameObject.FindObjectsOfType<AnimatorEvent>();

			foreach (var c in contexts) {
				foreach (var ae in aes) {
					if (ae.GetComponent<Animator>().runtimeAnimatorController == c.animatorController) {
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
		list_onStateUpdate.DoLayoutList();

		serializedObject.ApplyModifiedProperties();
	}
}
