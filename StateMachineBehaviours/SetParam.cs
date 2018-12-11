﻿using UnityEngine;

public class SetParam : StateMachineBehaviourExtended {
	public enum Type { Bool, Trigger, Int, Float }
	public enum When { OnStateEnterTransitionStarts, OnStateEnterTransitionEnds, OnStateExitTransitionStarts, OnStateExitTransitionEnds }

	public When when = When.OnStateEnterTransitionStarts;
	public Type what = Type.Bool;
	public string paramName;
	public bool wantedBool;
	public int wantedInt;
	public float wantedFloat;

	public override void StateEnter_TransitionStarts(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		TryExecute(animator, When.OnStateEnterTransitionStarts);
	}
	public override void StateEnter_TransitionEnds(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		TryExecute(animator, When.OnStateEnterTransitionEnds);
	}
	public override void StateExit_TransitionStarts(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		TryExecute(animator, When.OnStateExitTransitionStarts);
	}
	public override void StateExit_TransitionEnds(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		TryExecute(animator, When.OnStateExitTransitionEnds);
	}

	private void TryExecute(Animator animator, When origin) {
		if (origin != when) return;
		switch (what) {
			case Type.Bool: animator.SetBool(paramName, wantedBool); break;
			case Type.Trigger: if (wantedBool) animator.SetTrigger(paramName); else animator.ResetTrigger(paramName); break;
			case Type.Int: animator.SetInteger(paramName, wantedInt); break;
			case Type.Float: animator.SetFloat(paramName, wantedFloat); break;
		}
	}
}
