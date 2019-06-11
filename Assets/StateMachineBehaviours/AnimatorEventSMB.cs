using System;
using UnityEngine;

public class AnimatorEventSMB : StateMachineBehaviourExtended {
	// Choose string from a dropdown
	public TimedEvent[] onStateEnterTransitionStart;
	public TimedEvent[] onStateEnterTransitionEnd;
	public TimedEvent[] onStateExitTransitionStart;
	public TimedEvent[] onStateExitTransitionEnd;

	// Show also in a timeline!
	[UnityEngine.Serialization.FormerlySerializedAs("onStateUpdate")]
	public TimedEvent[] onNormalizedTimeReached;
	public TimedEvent[] onStateUpdated; // onStateUpdated instead of onStateUpdate because of a naming error. In the future it can be changed to onStateUpdate

	private AnimatorEvent animatorEvent; // Initialized by AnimatorEvent.Awake

	[Serializable]
	public struct TimedEvent {
		[Range(0, 1)]
		public float normalizedTime;
		public string callback;
		public bool repeat;
		[Tooltip("Execute as OnStateExitTransitionEnd if this hasn't been executed yet")]
		public bool executeOnExitEnds;
		[NonSerialized] public int nextNormalizedTime;
	}

	public override void StateEnter_TransitionStarts(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//Debug.Log("[" + state.fullPathHash + "] (" + stateInfo.fullPathHash + ") OnStateEnter_TransitionStarts");
		FireTimedEvents(onStateEnterTransitionStart, AnimatorEvent.EventType.OnEnterTransitionStart);
	}

	public override void StateEnter_TransitionEnds(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//Debug.Log("[" + state.fullPathHash + "] (" + stateInfo.fullPathHash + ") OnStateEnter_TransitionEnds");
		FireTimedEvents(onStateEnterTransitionEnd, AnimatorEvent.EventType.OnEnterTransitionEnd);
	}

	public override void StateExit_TransitionStarts(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//Debug.Log("[" + state.fullPathHash + "] (" + stateInfo.fullPathHash + " => " + animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash + ") OnStateExit_TransitionStarts");
		FireTimedEvents(onStateExitTransitionStart, AnimatorEvent.EventType.OnExitTransitionStart);
	}

	public override void StateExit_TransitionEnds(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//Debug.Log("[" + state.fullPathHash + "] (" + stateInfo.fullPathHash + " => " + animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash + ") OnStateExit_TransitionEnds");
		FireTimedEvents(onStateExitTransitionEnd, AnimatorEvent.EventType.OnExitTransitionEnd);

		// Finalize events that want to execute on end, and reset them for later
		for (int i = 0; i < onNormalizedTimeReached.Length; i++) {
			if (onNormalizedTimeReached[i].executeOnExitEnds && onNormalizedTimeReached[i].nextNormalizedTime == 0) {
				animatorEvent.Event(onNormalizedTimeReached[i].callback, AnimatorEvent.EventType.OnExitTransitionEnd);
			}
			onNormalizedTimeReached[i].nextNormalizedTime = 0;
		}
	}

	public override void StateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		for (int i = 0; i < onStateUpdated.Length; i++) {
			animatorEvent.Event(onStateUpdated[i].callback, AnimatorEvent.EventType.OnUpdate);
		}
		for (int i = 0; i < onNormalizedTimeReached.Length; i++) {
			if (stateInfo.normalizedTime >= onNormalizedTimeReached[i].normalizedTime + onNormalizedTimeReached[i].nextNormalizedTime) {
				animatorEvent.Event(onNormalizedTimeReached[i].callback, AnimatorEvent.EventType.OnUpdate);
				if (onNormalizedTimeReached[i].repeat)
					onNormalizedTimeReached[i].nextNormalizedTime++;
				else
					onNormalizedTimeReached[i].nextNormalizedTime = int.MaxValue;
			}
		}
	}

	public void SetAnimatorEvent(AnimatorEvent animatorEvent) {
		this.animatorEvent = animatorEvent;
	}

	private void FireTimedEvents(TimedEvent[] events, AnimatorEvent.EventType eventType) {
		for (int i = 0; i < events.Length; i++)
			animatorEvent.Event(events[i].callback, eventType);
	}
}
