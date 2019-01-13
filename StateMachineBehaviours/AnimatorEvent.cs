using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class AnimatorEvent : MonoBehaviour {
	public enum EventType {
		OnExitTransitionStart,
		OnExitTransitionEnd,
		OnEnterTransitionStart,
		OnEnterTransitionEnd,
		OnUpdate
	}

	public EventElement[] events;

	private Dictionary<string, EventElement> elementsDict = new Dictionary<string, EventElement>();

	// Instead of executing events inmediately, we wait for the animator to play the animation for this frame
	// This will prevent be one frame ahead, with the character's old pose from the previous animation
	private Queue<string>[] queuedEvents = new Queue<string>[5];

	public bool debug = false;

	void Awake() {
		for (int i = 0; i < queuedEvents.Length; i++) {
			queuedEvents[i] = new Queue<string>();
		}
		foreach (var elem in events) {
			elementsDict.Add(elem.name, elem);
		}

		// Initialize references in SMBs
		foreach (var smb in GetComponent<Animator>().GetBehaviours<AnimatorEventSMB>()) {
			smb.SetAnimatorEvent(this);
		}
	}

	public void Event(string name, EventType eventType) {
#if UNITY_EDITOR
		if (debug) {
			Debug.Log("Event [" + eventType + "]: " + name);
		}
#endif

		EventElement e;
		if (elementsDict.TryGetValue(name, out e)) {
			queuedEvents[(int) eventType].Enqueue(name);
		}
	}

	private void LateUpdate() {
		for (int i = 0; i < queuedEvents.Length; i++) {
			while (queuedEvents[i].Count > 0) {
				elementsDict[queuedEvents[i].Dequeue()].action.Invoke();
			}
		}
	}

	[Serializable]
	public class EventElement {
		public string name;
		public UnityEvent action;
	}
}
