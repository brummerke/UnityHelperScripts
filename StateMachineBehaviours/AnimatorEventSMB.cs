using System;
using UnityEngine;

public class AnimatorEventSMB : StateMachineBehaviourExtended {
	// Choose string from a dropdown
	[UnityEngine.Serialization.FormerlySerializedAs("onStateEnter")]
	public TimedEvent[] onStateEnterTransitionStart;
	public TimedEvent[] onStateEnterTransitionEnd;
	public TimedEvent[] onStateExitTransitionStart;
	[UnityEngine.Serialization.FormerlySerializedAs("onStateExit")]
	public TimedEvent[] onStateExitTransitionEnd;

	// Show also in a timeline!
	public TimedEvent[] onStateUpdate;

	private AnimatorEvent animatorEvent;

	[Serializable]
	public struct TimedEvent {
		[Range(0, 1)]
		public float normalizedTime;
		public string callback;
		public bool repeat;
		[NonSerialized] public int nextNormalizedTime;
	}

	public override void StateEnter_TransitionStarts(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//Debug.Log("[" + state.fullPathHash + "] (" + stateInfo.fullPathHash + ") OnStateEnter_TransitionStarts");
		if (animatorEvent == null) animatorEvent = animator.GetComponent<AnimatorEvent>();
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

		// Reset update events
		for (int i = 0; i < onStateUpdate.Length; i++) {
			onStateUpdate[i].nextNormalizedTime = 0;
		}
	}

	public override void StateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		for (int i = 0; i < onStateUpdate.Length; i++) {
			if (stateInfo.normalizedTime >= onStateUpdate[i].normalizedTime + onStateUpdate[i].nextNormalizedTime) {
				animatorEvent.Event(onStateUpdate[i].callback, AnimatorEvent.EventType.OnUpdate);
				if (onStateUpdate[i].repeat)
					onStateUpdate[i].nextNormalizedTime++;
				else
					onStateUpdate[i].nextNormalizedTime = int.MaxValue;
			}
		}
	}

	private void FireTimedEvents(TimedEvent[] events, AnimatorEvent.EventType eventType) {
		for (int i = 0; i < events.Length; i++)
			animatorEvent.Event(events[i].callback, eventType);
	}
}
