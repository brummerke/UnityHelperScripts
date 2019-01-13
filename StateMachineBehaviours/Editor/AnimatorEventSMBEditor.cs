using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;
using BlendTree = UnityEditor.Animations.BlendTree;

[CustomEditor(typeof(AnimatorEventSMB))]
public class AnimatorEventSMBEditor : Editor {
	private List<string> eventsAvailable = new List<string>();

	private ReorderableList list_onStateEnterTransitionStart;
	private ReorderableList list_onStateEnterTransitionEnd;
	private ReorderableList list_onStateExitTransitionStart;
	private ReorderableList list_onStateExitTransitionEnd;
	private ReorderableList list_onNormalizedTimeReached;
	private ReorderableList list_onStateUpdated;

	private readonly List<AnimatorEvent> matchingAnimatorEvent = new List<AnimatorEvent>();
	private StateMachineBehaviourContext[] contexts;

	private void OnEnable() {
		// accessing serializedObject after reloading scripts can throw an exception. There is no way around it, so just ignore it
		try {
			var s = serializedObject;
		}
		catch (System.Exception) { return; }

		CreateReorderableList("On State Enter Transition Start", 20, ref list_onStateEnterTransitionStart, serializedObject.FindProperty("onStateEnterTransitionStart"),
			(rect, index, isActive, isFocused) => {
				DrawCallbackField(rect, serializedObject.FindProperty("onStateEnterTransitionStart").GetArrayElementAtIndex(index));
			});
		CreateReorderableList("On State Enter Transition End", 20, ref list_onStateEnterTransitionEnd, serializedObject.FindProperty("onStateEnterTransitionEnd"),
			(rect, index, isActive, isFocused) => {
				DrawCallbackField(rect, serializedObject.FindProperty("onStateEnterTransitionEnd").GetArrayElementAtIndex(index));
			});
		CreateReorderableList("On State Exit Transition Start", 20, ref list_onStateExitTransitionStart, serializedObject.FindProperty("onStateExitTransitionStart"),
			(rect, index, isActive, isFocused) => {
				DrawCallbackField(rect, serializedObject.FindProperty("onStateExitTransitionStart").GetArrayElementAtIndex(index));
			});
		CreateReorderableList("On State Exit Transition End", 20, ref list_onStateExitTransitionEnd, serializedObject.FindProperty("onStateExitTransitionEnd"),
			(rect, index, isActive, isFocused) => {
				DrawCallbackField(rect, serializedObject.FindProperty("onStateExitTransitionEnd").GetArrayElementAtIndex(index));
			});
		CreateReorderableList("On State Update", 20, ref list_onStateUpdated, serializedObject.FindProperty("onStateUpdated"),
			(rect, index, isActive, isFocused) => {
				DrawCallbackField(rect, serializedObject.FindProperty("onStateUpdated").GetArrayElementAtIndex(index));
			});
		CreateReorderableList("On Normalized Time Reached", 60, ref list_onNormalizedTimeReached, serializedObject.FindProperty("onNormalizedTimeReached"),
			(rect, index, isActive, isFocused) => {
				var property = serializedObject.FindProperty("onNormalizedTimeReached").GetArrayElementAtIndex(index);

				float timeBefore = property.FindPropertyRelative("normalizedTime").floatValue;

				DrawCallbackField(rect, property);
				bool timeEdited = EditorGUI.PropertyField(new Rect(rect.x, rect.y + 20, rect.width, 20), property.FindPropertyRelative("normalizedTime"));
				EditorGUI.PropertyField(new Rect(rect.x, rect.y + 40, rect.width / 2, 20), property.FindPropertyRelative("repeat"));
				EditorGUI.PropertyField(new Rect(rect.x + rect.width / 2, rect.y + 40, rect.width / 2, 20), property.FindPropertyRelative("executeOnExitEnds"));

				float timeNow = property.FindPropertyRelative("normalizedTime").floatValue;

				if (timeBefore != timeNow) {
					if (!AnimationMode.InAnimationMode())
						AnimationMode.StartAnimationMode();

					foreach (var context in contexts) {
						AnimatorState state = context.animatorObject as AnimatorState;
						if (null == state) continue;

						AnimationClip previewClip = GetFirstAvailableClip(state.motion);
						if (null == previewClip) continue;

						foreach (var targetObj in matchingAnimatorEvent) {
							AnimationMode.BeginSampling();
							AnimationMode.SampleAnimationClip(targetObj.gameObject, previewClip, property.FindPropertyRelative("normalizedTime").floatValue * previewClip.length);
							AnimationMode.EndSampling();
						}
					}
				}
			});
	}
	
	private AnimationClip GetFirstAvailableClip(Motion motion) {
		if (motion is AnimationClip)
			return motion as AnimationClip;

		var tree = motion as BlendTree;

		if (tree.children.Length == 0) return null;

		for (int i = 0; i < tree.children.Length; i++) {
			// Should we be worried about `cycleOffset`? https://docs.unity3d.com/ScriptReference/Animations.AnimatorState-cycleOffset.html

			var child = tree.children[i].motion;
			if (child is BlendTree) {
				var childMotion = GetFirstAvailableClip(tree.children[i].motion as BlendTree);
				if (childMotion == null)
					continue;

				return childMotion;
			}
			else {
				if (child != null)
					return child as AnimationClip;
			}
		}

		return null;
	}

	private void DrawCallbackField(Rect rect, SerializedProperty property) {
		var callbackProperty = property.FindPropertyRelative("callback");
		float buttonWidth = 100;
		if (GUI.Button(new Rect(rect.x, rect.y, buttonWidth, 18), "View events")) {
			var windowEventViewer = EditorWindow.GetWindow<EventViewerWindow>();
			windowEventViewer.SetTarget(this, callbackProperty.stringValue);
			windowEventViewer.Show();
		}
		float space = 10;
		EditorGUI.PropertyField(new Rect(rect.x + buttonWidth + space, rect.y, rect.width - buttonWidth - space, 18), callbackProperty, GUIContent.none);
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
			matchingAnimatorEvent.Clear();
			contexts = UnityEditor.Animations.AnimatorController.FindStateMachineBehaviourContext((AnimatorEventSMB) target);
			var aes = FindObjectsOfType<AnimatorEvent>();

			foreach (var c in contexts) {
				foreach (var ae in aes) {
					if (ae.GetComponent<Animator>().runtimeAnimatorController == c.animatorController) {
						matchingAnimatorEvent.Add(ae);
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

	

	public class EventViewerWindow : EditorWindow {
		AnimatorEventSMBEditor animatorEventSMBEditor;
		string callbackName;

		private void OnEnable() {
			minSize = new Vector2(320, 170);
		}

		public void SetTarget(AnimatorEventSMBEditor animatorEventSMBEditor, string callbackName) {
			this.animatorEventSMBEditor = animatorEventSMBEditor;
			this.callbackName = callbackName;
		}

		Vector2 scrollPos;

		void OnGUI() {
			GUILayout.Label("Events for callback \"" + callbackName + "\"", EditorStyles.boldLabel);

			GUILayout.BeginScrollView(scrollPos, false, false);
			foreach (var ae in animatorEventSMBEditor.matchingAnimatorEvent) {
				GUILayout.BeginVertical("box");

				GUILayout.BeginHorizontal();
				GUILayout.Label("GameObject ", EditorStyles.boldLabel);
				GUI.enabled = false;
				EditorGUILayout.ObjectField(ae.gameObject, typeof(GameObject));
				GUI.enabled = true;
				GUILayout.EndHorizontal();

				var index = GetEventIndex(ae, callbackName);
				if (index == -1) {
					GUILayout.Label("Event not found");
				}
				else {
					var so = new SerializedObject(ae);
					EditorGUILayout.PropertyField(so.FindProperty("events").GetArrayElementAtIndex(index).FindPropertyRelative("action"), true);
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndScrollView();
		}

		int GetEventIndex(AnimatorEvent ae, string eventName) {
			for (int i = 0; i < ae.events.Length; i++) {
				if (ae.events[i].name == eventName) {
					return i;
				}
			}
			return -1;
		}
	}
}
