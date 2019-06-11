using UnityEngine;
using UnityEngine.Animations;

public abstract class StateMachineBehaviourExtended : StateMachineBehaviour {
	public virtual void StateEnter_TransitionStarts(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
	public virtual void StateEnter_TransitionEnds(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
	public virtual void StateExit_TransitionStarts(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
	public virtual void StateExit_TransitionEnds(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
	public virtual void StateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

	// Removed: transitions MUST go through the states Enter or Exit inside the SubSM for this to work: e.g. AnyState breaks it
	// DO NOT USE, so removed to prevent future problems
	public sealed override void OnStateMachineEnter(Animator animator, int stateMachinePathHash) { }
	public sealed override void OnStateMachineEnter(Animator animator, int stateMachinePathHash, AnimatorControllerPlayable controller) { }
	public sealed override void OnStateMachineExit(Animator animator, int stateMachinePathHash) { }
	public sealed override void OnStateMachineExit(Animator animator, int stateMachinePathHash, AnimatorControllerPlayable controller) { }

	private bool onEnterEnded, onExitStarted;
	private int transitionHash;

	public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		StateEnter_TransitionStarts(animator, stateInfo, layerIndex);
		onExitStarted = false;
		onEnterEnded = !animator.IsInTransition(layerIndex);
		if (onEnterEnded) {
			StateEnter_TransitionEnds(animator, stateInfo, layerIndex);
		}
		else {
			transitionHash = animator.GetAnimatorTransitionInfo(layerIndex).fullPathHash;
		}

		// First frame calls StateEnter, next ones call Update. Call Update AFTER Start too, on the same frame
		StateUpdate(animator, stateInfo, layerIndex);
	}

	public sealed override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (!onEnterEnded) {
			StateEnter_TransitionEnds(animator, stateInfo, layerIndex);
		}

		if (!onExitStarted) {
			StateExit_TransitionStarts(animator, stateInfo, layerIndex);
		}

		StateExit_TransitionEnds(animator, stateInfo, layerIndex);
	}

	public sealed override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (!onEnterEnded && (
			!animator.IsInTransition(layerIndex) ||
			animator.GetAnimatorTransitionInfo(layerIndex).fullPathHash != transitionHash)) {
			StateEnter_TransitionEnds(animator, stateInfo, layerIndex);
			onEnterEnded = true;
		}

		StateUpdate(animator, stateInfo, layerIndex);

		if (onEnterEnded && !onExitStarted && animator.IsInTransition(layerIndex)) {
			StateExit_TransitionStarts(animator, stateInfo, layerIndex);
			onExitStarted = true;
		}
	}
}
