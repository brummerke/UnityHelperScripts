using System;
using UnityEngine;

namespace Ashkatchap.Shared {
	public static class FastMath {
		/// <summary>
		/// Replacement for Vector3.Distance.
		/// </summary>
		public static float Distance(ref Vector3 a, ref Vector3 b) {
			return (float) Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z));
		}
		/// <summary>
		/// Replacement for Vector3.Distance. Faster with ref
		/// </summary>
		public static float Distance(Vector3 a, Vector3 b) {
			return (float) Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z));
		}

		/// <summary>
		/// Replacement for Vector2.Distance.
		/// </summary>
		public static float Distance(ref Vector2 a, ref Vector2 b) {
			return (float) Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
		}
		/// <summary>
		/// Replacement for Vector2.Distance. Faster with ref
		/// </summary>
		public static float Distance(Vector2 a, Vector2 b) {
			return (float) Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
		}

		/// <summary>
		/// Replacement for Vector3.Distance but without sqrt.
		/// </summary>
		public static float SqrDistance(ref Vector3 a, ref Vector3 b) {
			return (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z);
		}
		/// <summary>
		/// Replacement for Vector3.Distance but without sqrt. Faster with ref
		/// </summary>
		public static float SqrDistance(Vector3 a, Vector3 b) {
			return (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z);
		}

		public static void Decompose(ref Vector3 v, out Vector3 normalizedDir, out float magnitude) {
			magnitude = Magnitude(ref v);
			normalizedDir = v / magnitude;
		}

		public static float Pow(float num, uint exp) {
			float result = 1;
			while (exp > 0) {
				if (exp % 2 == 1) result *= num;
				exp >>= 1;
				num *= num;
			}

			return result;
		}

		public static Vector3 Multiply(ref Quaternion rotation, ref Vector3 point) {
			float num = rotation.x * 2f;
			float num2 = rotation.y * 2f;
			float num3 = rotation.z * 2f;
			float num4 = rotation.x * num;
			float num5 = rotation.y * num2;
			float num6 = rotation.z * num3;
			float num7 = rotation.x * num2;
			float num8 = rotation.x * num3;
			float num9 = rotation.y * num3;
			float num10 = rotation.w * num;
			float num11 = rotation.w * num2;
			float num12 = rotation.w * num3;
			Vector3 result;
			result.x = (1f - (num5 + num6)) * point.x + (num7 - num12) * point.y + (num8 + num11) * point.z;
			result.y = (num7 + num12) * point.x + (1f - (num4 + num6)) * point.y + (num9 - num10) * point.z;
			result.z = (num8 - num11) * point.x + (num9 + num10) * point.y + (1f - (num4 + num5)) * point.z;
			return result;
		}

		public static Vector3 MultiplyPoint3x4(ref Matrix4x4 m, ref Vector3 v) {
			Vector3 result;
			result.x = m.m00 * v.x + m.m01 * v.y + m.m02 * v.z + m.m03;
			result.y = m.m10 * v.x + m.m11 * v.y + m.m12 * v.z + m.m13;
			result.z = m.m20 * v.x + m.m21 * v.y + m.m22 * v.z + m.m23;
			return result;
		}

		public static float Abs(float value) {
			return value >= 0 ? value : -value;
		}

		public static Vector3 ProjectOnPlane(ref Vector3 vector, ref Vector3 planeNormal) {
			float num = planeNormal.x * planeNormal.x + planeNormal.y * planeNormal.y + planeNormal.z * planeNormal.z;
			if (num < Mathf.Epsilon) {
				return vector;
			}
			else {
				return vector - planeNormal * ((vector.x * planeNormal.x + vector.y * planeNormal.y + vector.z * planeNormal.z) / num);
			}
		}
		public static Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal) {
			return ProjectOnPlane(ref vector, ref planeNormal);
		}

		public static Vector3 GetNormalFromTrianglePoints(ref Vector3 a, ref Vector3 b, ref Vector3 c) {
			Vector3 v1 = Normalize(a - c);
			Vector3 v2 = Normalize(b - c);
			return Normalize(new Vector3(v1.y * v2.z - v1.z * v2.y, v1.z * v2.x - v1.x * v2.z, v1.x * v2.y - v1.y * v2.x));
		}
		public static Vector3 Normalize(ref Vector3 v) {
			float magnitude = (float) Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
			return magnitude > 1E-05f ? v / magnitude : new Vector3(0, 0, 0);
		}
		public static Vector3 Normalize(Vector3 v) {
			return Normalize(ref v);
		}
		public static Vector2 Normalize(ref Vector2 v) {
			float magnitude = (float) Math.Sqrt(v.x * v.x + v.y * v.y);
			return magnitude > 1E-05f ? v / magnitude : new Vector2(0, 0);
		}
		public static Vector2 Normalize(Vector2 v) {
			return Normalize(ref v);
		}
		public static float Magnitude(Vector3 v) {
			return Magnitude(ref v);
		}
		public static float Magnitude(ref Vector3 v) {
			return (float) Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
		}
		public static float Magnitude(Vector2 v) {
			return Magnitude(ref v);
		}
		public static float Magnitude(ref Vector2 v) {
			return (float) Math.Sqrt(v.x * v.x + v.y * v.y);
		}
		public static float Dot(ref Vector3 lhs, ref Vector3 rhs) {
			return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
		}
		public static Vector3 Cross(ref Vector3 lhs, ref Vector3 rhs) {
			return new Vector3(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
		}
		public static float Angle(ref Vector3 from, ref Vector3 to) {
			return (float) Math.Acos(Mathf.Clamp(Vector3.Dot(Normalize(ref from), Normalize(ref to)), -1f, 1f)) * Mathf.Rad2Deg;
		}

		public static bool Approximately(float a, float b) {
			return Math.Abs(b - a) < Math.Max(1E-06f * Math.Max(Math.Abs(a), Math.Abs(b)), Mathf.Epsilon * 8f);
		}
	}
}
