using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public static class ExtensionMethods
{
    #region Vector3

    public static bool IsNAN(this Vector3 vec)
    {
        return float.IsNaN(vec.x) || float.IsNaN(vec.y) || float.IsNaN(vec.z);
    }

    public static Vector3 xy(this Vector3 vec)
    {
        return new Vector3(vec.x, vec.y, 0);
    }
    public static Vector3 xz(this Vector3 vec)
    {
        return new Vector3(vec.x, 0, vec.z);
    }
    public static Vector3 yz(this Vector3 vec)
    {
        return new Vector3(0, vec.y, vec.z);
    }
    public static Vector3 x(this Vector3 vec)
    {
        return new Vector3(vec.x, 0, 0);
    }
    public static Vector3 y(this Vector3 vec)
    {
        return new Vector3(0, vec.y, 0);
    }
    public static Vector3 z(this Vector3 vec)
    {
        return new Vector3(0, 0, vec.z);
    }

    public static float Maximum(this Vector3 vec)
    {
        return Mathf.Max(vec.x, Mathf.Max(vec.y, vec.z));
    }

    public static float Minimum(this Vector3 vec)
    {
        return Mathf.Min(vec.x, Mathf.Min(vec.y, vec.z));
    }

    public static Vector3 Absolute(this Vector3 vec)
    {
        return new Vector3(Mathf.Abs(vec.x), Mathf.Abs(vec.y), Mathf.Abs(vec.z));
    }

    #endregion

    public static T Random<T>(this IList<T> list)
    {
        return list.Count == 0 ? default(T) : list[UnityEngine.Random.Range(0, list.Count)];
    }

    public static K Random<T, K>(this Dictionary<T, K> dictionary)
    {
        int value = UnityEngine.Random.Range(0, dictionary.Count);
        return dictionary.ElementAt(value).Value;
    }

    public static bool Contains(this Enum keys, Enum flag)
    {
        if (keys.GetType() != flag.GetType())
            throw new ArgumentException("Type Mismatch");
        return (Convert.ToUInt64(keys) & Convert.ToUInt64(flag)) != 0;
    }

    /// <summary>
    /// Applies the transform values of target to self.
    /// </summary>
    public static void CopyTransformations(this Transform transform, Transform target, bool ignoreScale = true)
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
        if (!ignoreScale) transform.localScale = target.localScale;
    }

    /// <summary>
    /// Zeros the transform's position and rotation
    /// </summary>
    public static void ResetTransformations(this Transform transform, bool ignoreScale = true)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        if (!ignoreScale) transform.localScale = Vector3.one;
    }

    public static T Choose<T>(this IList<T> list)
    {
        return list.Count == 0 ? default(T) : list[UnityEngine.Random.Range(0, list.Count)];
    }

    public static Vector3 Average(this List<Vector3> list)
    {
        Vector3 total = Vector3.zero;
        foreach (var v in list) total += v;
        return total / list.Count;
    }

    public static Quaternion Rotation(this Matrix4x4 matrix)
    {
        Vector3 forward;
        forward.x = matrix.m02;
        forward.y = matrix.m12;
        forward.z = matrix.m22;

        Vector3 upwards;
        upwards.x = matrix.m01;
        upwards.y = matrix.m11;
        upwards.z = matrix.m21;

        return Quaternion.LookRotation(forward, upwards);
    }

    public static Vector3 Position(this Matrix4x4 matrix)
    {
        Vector3 position;
        position.x = matrix.m03;
        position.y = matrix.m13;
        position.z = matrix.m23;
        return position;
    }

    public static Vector3 Scale(this Matrix4x4 matrix)
    {
        Vector3 scale;
        scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
        scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
        scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
        return scale;
    }

    public static TValue ElementAtOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
        return dictionary.ContainsKey(key) ? dictionary[key] : default(TValue);
    }
}
