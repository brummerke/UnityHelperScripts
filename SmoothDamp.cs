using System;
using UnityEngine;

// Mathf.SmoothDamp is not reliable with a non-constant framerate
// Instead, Weaver (from AGDG) provided another options (used here) that does work with a varying framerate

public static class SmoothDamp {
	/// <summary>
	/// Smoothing with a speed equal or greather than this value will equal to copying the target value
	/// </summary>
	public const float MaxSpeed = 100000000;


	[Serializable]
	public struct Float {
	/*	[HideInNormalInspector]*/ public float filteredValue;

		private float pastTarget;

		public Float(Float toCopy) {
			this.filteredValue = toCopy.filteredValue;
			this.pastTarget = toCopy.pastTarget;
		}

		public void Reset(float newValue) {
			filteredValue = newValue;
			pastTarget = newValue;
		}

		public float Step(float target, float speed) {
			var deltaTime = Time.deltaTime;

			var t = deltaTime * speed;
			if (0 == t) return filteredValue;
			else if (t < MaxSpeed) {
				var v = (target - pastTarget) / t;
				var f = filteredValue - pastTarget + v;

				pastTarget = target;

				return filteredValue = target - v + f * Mathf.Exp(-t);
			}
			else {
				return filteredValue = target;
			}
		}
	}

	[Serializable]
	public struct Angle {
        /*	[HideInNormalInspector]*/
        public float filteredValue;

		private float pastTarget;

		public Angle(Angle toCopy) {
			this.filteredValue = toCopy.filteredValue;
			this.pastTarget = toCopy.pastTarget;
		}

		public void Reset(float newValue) {
			filteredValue = newValue;
			pastTarget = newValue;
		}

		public float Step(float target, float speed) {
			target = filteredValue + Mathf.DeltaAngle(filteredValue, target);

			var deltaTime = Time.deltaTime;

			var t = deltaTime * speed;
			if (0 == t) return filteredValue;
			else if (t < MaxSpeed) {
				var v = (target - pastTarget) / t;
				var f = filteredValue - pastTarget + v;

				pastTarget = target;

				return filteredValue = target - v + f * Mathf.Exp(-t);
			}
			else {
				return filteredValue = target;
			}
		}
	}

	[Serializable]
	public struct Vector3 {
        /*	[HideInNormalInspector]*/
        public UnityEngine.Vector3 filteredValue;

		private UnityEngine.Vector3 pastTarget;

		public Vector3(Vector3 toCopy) {
			this.filteredValue = toCopy.filteredValue;
			this.pastTarget = toCopy.pastTarget;
		}

		public void Reset(UnityEngine.Vector3 newValue) {
			filteredValue = newValue;
			pastTarget = newValue;
		}

		public UnityEngine.Vector3 Step(UnityEngine.Vector3 target, float speed) {
			var deltaTime = Time.deltaTime;

			var t = deltaTime * speed;
			if (0 == t) return filteredValue;
			else if (t < MaxSpeed) {
				var v = (target - pastTarget) / t;
				var f = filteredValue - pastTarget + v;

				pastTarget = target;

				return filteredValue = target - v + f * Mathf.Exp(-t);
			}
			else {
				return filteredValue = target;
			}
		}
	}

	[Serializable]
	public struct EulerAngles {
			/*	[HideInNormalInspector]*/public UnityEngine.Vector3 filteredValue;

		private UnityEngine.Vector3 pastTarget;

		public EulerAngles(EulerAngles toCopy) {
			this.filteredValue = toCopy.filteredValue;
			this.pastTarget = toCopy.pastTarget;
		}

		public void Reset(UnityEngine.Vector3 newValue) {
			filteredValue = newValue;
			pastTarget = newValue;
		}

		public UnityEngine.Vector3 Step(UnityEngine.Vector3 target, float speed) {
			target.x = filteredValue.x + Mathf.DeltaAngle(filteredValue.x, target.x);
			target.y = filteredValue.y + Mathf.DeltaAngle(filteredValue.y, target.y);
			target.z = filteredValue.z + Mathf.DeltaAngle(filteredValue.z, target.z);

			var deltaTime = Time.deltaTime;

			var t = deltaTime * speed;
			if (0 == t) return filteredValue;
			else if (t < MaxSpeed) {
				var v = (target - pastTarget) / t;
				var f = filteredValue - pastTarget + v;

				pastTarget = target;

				return filteredValue = target - v + f * Mathf.Exp(-t);
			}
			else {
				return filteredValue = target;
			}
		}
	}
}
