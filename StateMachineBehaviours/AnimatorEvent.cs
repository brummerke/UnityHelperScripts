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

	void Awake() {
		for (int i = 0; i < queuedEvents.Length; i++) {
			queuedEvents[i] = new Queue<string>();
		}
		foreach (var elem in events) {
			elementsDict.Add(elem.name, elem);
		}
	}

	public void Event(string name, EventType eventType) {
		//Debug.Log("Event " + eventType + ": " + name);

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
