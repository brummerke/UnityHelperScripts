using System.Collections.Generic;
using UnityEngine;

public static class Mathfx
{
    //Ease in out
    public static float Hermite(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, value * value * (3.0f - 2.0f * value));
    }

    public static Vector2 Hermite(Vector2 start, Vector2 end, float value)
    {
        return new Vector2(Hermite(start.x, end.x, value), Hermite(start.y, end.y, value));
    }

    public static Vector3 Hermite(Vector3 start, Vector3 end, float value)
    {
        return new Vector3(Hermite(start.x, end.x, value), Hermite(start.y, end.y, value), Hermite(start.z, end.z, value));
    }

    //Ease out
    public static float Sinerp(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, Mathf.Sin(value * Mathf.PI * 0.5f));
    }

    public static Vector2 Sinerp(Vector2 start, Vector2 end, float value)
    {
        return new Vector2(Mathf.Lerp(start.x, end.x, Mathf.Sin(value * Mathf.PI * 0.5f)), Mathf.Lerp(start.y, end.y, Mathf.Sin(value * Mathf.PI * 0.5f)));
    }

    public static Vector3 Sinerp(Vector3 start, Vector3 end, float value)
    {
        return new Vector3(Mathf.Lerp(start.x, end.x, Mathf.Sin(value * Mathf.PI * 0.5f)), Mathf.Lerp(start.y, end.y, Mathf.Sin(value * Mathf.PI * 0.5f)), Mathf.Lerp(start.z, end.z, Mathf.Sin(value * Mathf.PI * 0.5f)));
    }

    //Ease in
    public static float Coserp(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, 1.0f - Mathf.Cos(value * Mathf.PI * 0.5f));
    }

    public static Vector2 Coserp(Vector2 start, Vector2 end, float value)
    {
        return new Vector2(Coserp(start.x, end.x, value), Coserp(start.y, end.y, value));
    }

    public static Vector3 Coserp(Vector3 start, Vector3 end, float value)
    {
        return new Vector3(Coserp(start.x, end.x, value), Coserp(start.y, end.y, value), Coserp(start.z, end.z, value));
    }

    //Boing
    public static float Berp(float start, float end, float value)
    {
        value = Mathf.Clamp01(value);
        value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
        return start + (end - start) * value;
    }

    public static Vector2 Berp(Vector2 start, Vector2 end, float value)
    {
        return new Vector2(Berp(start.x, end.x, value), Berp(start.y, end.y, value));
    }

    public static Vector3 Berp(Vector3 start, Vector3 end, float value)
    {
        return new Vector3(Berp(start.x, end.x, value), Berp(start.y, end.y, value), Berp(start.z, end.z, value));
    }

    //Like lerp with ease in ease out
    public static float SmoothStep(float x, float min, float max)
    {
        x = Mathf.Clamp(x, min, max);
        float v1 = (x - min) / (max - min);
        float v2 = (x - min) / (max - min);
        return -2 * v1 * v1 * v1 + 3 * v2 * v2;
    }

    public static Vector2 SmoothStep(Vector2 vec, float min, float max)
    {
        return new Vector2(SmoothStep(vec.x, min, max), SmoothStep(vec.y, min, max));
    }

    public static Vector3 SmoothStep(Vector3 vec, float min, float max)
    {
        return new Vector3(SmoothStep(vec.x, min, max), SmoothStep(vec.y, min, max), SmoothStep(vec.z, min, max));
    }

    public static float Lerp(float start, float end, float value)
    {
        return ((1.0f - value) * start) + (value * end);
    }

    public static Vector3 NearestPoint(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    {
        Vector3 lineDirection = Vector3.Normalize(lineEnd - lineStart);
        float closestPoint = Vector3.Dot((point - lineStart), lineDirection);
        return lineStart + (closestPoint * lineDirection);
    }

    public static Vector3 NearestPointStrict(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    {
        Vector3 fullDirection = lineEnd - lineStart;
        Vector3 lineDirection = Vector3.Normalize(fullDirection);
        float closestPoint = Vector3.Dot((point - lineStart), lineDirection);
        return lineStart + (Mathf.Clamp(closestPoint, 0.0f, Vector3.Magnitude(fullDirection)) * lineDirection);
    }

    //Bounce
    public static float Bounce(float x)
    {
        return Mathf.Abs(Mathf.Sin(6.28f * (x + 1f) * (x + 1f)) * (1f - x));
    }

    public static Vector2 Bounce(Vector2 vec)
    {
        return new Vector2(Bounce(vec.x), Bounce(vec.y));
    }

    public static Vector3 Bounce(Vector3 vec)
    {
        return new Vector3(Bounce(vec.x), Bounce(vec.y), Bounce(vec.z));
    }

    // test for value that is near specified float (due to floating point inprecision)
    // all thanks to Opless for this!
    public static bool Approx(float val, float about, float range)
    {
        return ((Mathf.Abs(val - about) < range));
    }

    // test if a Vector3 is close to another Vector3 (due to floating point inprecision)
    // compares the square of the distance to the square of the range as this
    // avoids calculating a square root which is much slower than squaring the range
    public static bool Approx(Vector3 val, Vector3 about, float range)
    {
        return ((val - about).sqrMagnitude < range * range);
    }

    /*
      * CLerp - Circular Lerp - is like lerp but handles the wraparound from 0 to 360.
      * This is useful when interpolating eulerAngles and the object
      * crosses the 0/360 boundary.  The standard Lerp function causes the object
      * to rotate in the wrong direction and looks stupid. Clerp fixes that.
      */

    public static float Clerp(float start, float end, float value)
    {
        float min = 0.0f;
        float max = 360.0f;
        float half = Mathf.Abs((max - min) / 2.0f);//half the distance between min and max
        float retval = 0.0f;
        float diff = 0.0f;

        if ((end - start) < -half)
        {
            diff = ((max - start) + end) * value;
            retval = start + diff;
        }
        else if ((end - start) > half)
        {
            diff = -((max - end) + start) * value;
            retval = start + diff;
        }
        else retval = start + (end - start) * value;

        // Debug.Log("Start: "  + start + "   End: " + end + "  Value: " + value + "  Half: " + half + "  Diff: " + diff + "  Retval: " + retval);
        return retval;
    }

    //
    // ADDON //
    //
    public static float Normalize(float rawValue, float minValue, float maxValue)
    {
        return Mathf.Clamp01(((rawValue - minValue) / (maxValue - minValue)));
    }

    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }

    public static float AverageList(List<float> avgList)
    {
        float a = 0f;
        foreach (float f in avgList)
        {
            a += f;
        }
        return a /= avgList.Count;
    }

    public static float HorizontalToVerticalFOV(float hFov, float w, float h)
    {
        hFov = hFov * Mathf.Deg2Rad;
        return 2 * Mathf.Atan(Mathf.Tan(hFov / 2) * (h / w)) * Mathf.Rad2Deg;
    }

    public static void SpreadTransform(Transform bullet, float amount, float xScale, float yScale)
    {
        //Generate some random things
        float angle = Random.Range(-180.0f, 180.0f);
        float deviation = Random.Range(-1.0f, 1.0f) * Random.Range(-1.0f, 1.0f); //float rnd2 = (Random.Range(-1.0f, 1.0f) * Random.Range(-1.0f, 1.0f)) * (inaccuracy * spreadModifier);

        //build rotations
        Quaternion rotate = Quaternion.AngleAxis(angle, Vector3.forward) * Quaternion.AngleAxis(deviation * amount, Vector3.right);

        Vector3 forward = rotate * Vector3.forward;
        rotate = Quaternion.FromToRotation(Vector3.forward, Vector3.Lerp(Vector3.ProjectOnPlane(forward, Vector3.right), forward, xScale));

        forward = rotate * Vector3.forward;
        rotate = Quaternion.FromToRotation(Vector3.forward, Vector3.Lerp(Vector3.ProjectOnPlane(forward, Vector3.up), forward, yScale));
        bullet.transform.rotation = bullet.transform.rotation * rotate;
    }

    public static Vector3 FloatToVector3(float value)
    {
        return new Vector3(value, value, value);
    }

    private static Vector3 CalculateInterceptCourse(Vector3 aTargetPos, Vector3 aTargetSpeed, Vector3 aInterceptorPos, float aInterceptorSpeed)
    {
        Vector3 targetDir = aTargetPos - aInterceptorPos;
        float iSpeed2 = aInterceptorSpeed * aInterceptorSpeed;
        float tSpeed2 = aTargetSpeed.sqrMagnitude;
        float fDot1 = Vector3.Dot(targetDir, aTargetSpeed);
        float targetDist2 = targetDir.sqrMagnitude;
        float d = (fDot1 * fDot1) - targetDist2 * (tSpeed2 - iSpeed2);
        if (d < 0.1f)  // negative == no possible course because the interceptor isn't fast enough
            return Vector3.zero;
        float sqrt = Mathf.Sqrt(d);
        float S1 = (-fDot1 - sqrt) / targetDist2;
        float S2 = (-fDot1 + sqrt) / targetDist2;
        if (S1 < 0.0001f)
        {
            if (S2 < 0.0001f)
                return Vector3.zero;
            else
                return (S2) * targetDir + aTargetSpeed;
        }
        else if (S2 < 0.0001f)
            return (S1) * targetDir + aTargetSpeed;
        else if (S1 < S2)
            return (S2) * targetDir + aTargetSpeed;
        else
            return (S1) * targetDir + aTargetSpeed;
    }

    private static float FindClosestPointOfApproach(Vector3 aPos1, Vector3 aSpeed1, Vector3 aPos2, Vector3 aSpeed2)
    {
        Vector3 PVec = aPos1 - aPos2;
        Vector3 SVec = aSpeed1 - aSpeed2;
        float d = SVec.sqrMagnitude;
        // if d is 0 then the distance between Pos1 and Pos2 is never changing
        // so there is no point of closest approach... return 0
        // 0 means the closest approach is now!
        if (d >= -0.0001f && d <= 0.0002f)
            return 0.0f;
        return (-Vector3.Dot(PVec, SVec) / d);
    }

    public static void RotateToIntercept(Transform bulletTransform, float bulletSpeed, Vector3 targetPosition, Vector3 targetVelocity, bool ignoreY)
    {
        float interceptionTime = 0.0f;
        Vector3 interceptionPoint = Vector3.zero;
        Vector3 targetVel = targetVelocity;
        if (ignoreY)
        {
            targetVel.y = 0f;
        }
        Vector3 IC = CalculateInterceptCourse(targetPosition, targetVel, bulletTransform.position, bulletSpeed);
        if (IC != Vector3.zero)
        {
            IC.Normalize();
            interceptionTime = FindClosestPointOfApproach(targetPosition, targetVel, bulletTransform.position, IC * bulletSpeed);
            interceptionPoint = targetPosition + targetVel * interceptionTime;
        }

        Vector3 aimTarget = interceptionPoint;
        bulletTransform.LookAt(aimTarget);
    }

    public static void ParentFromTo(Transform newChild, Transform newParent, bool matchScale, bool matchLayer, bool matchPosRot)
    {
        newChild.SetParent(newParent, !matchPosRot);
        if (matchPosRot)
        {
            newChild.position = newParent.position;
            newChild.rotation = newParent.rotation;
        }

        if (matchLayer)
        {
            SetLayers(newChild, newParent.gameObject.layer);
            //    Debug.Log("should set layer to " + newParent.gameObject.layer);
        }
        if (matchScale)
        {
            newChild.localScale = new Vector3(1, 1, 1);
        }
    }

    public static void SetLayers(Transform obj, int layer)
    {
        obj.gameObject.layer = layer;
        // Debug.Log("new layer " + obj.gameObject.layer);
        foreach (Renderer r in obj.GetComponentsInChildren<Renderer>())
        {
            r.gameObject.layer = layer;
        }
    }

    public static bool IsLayerPartOfMask(LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }

    public static AudioClip RndClip(AudioClip[] clipArray)
    {
        return clipArray[Random.Range(0, clipArray.Length)];
    }

    public static void AlignPartHit(Transform hitPart, RaycastHit destination, Vector3 dir)
    {
        hitPart.position = destination.point;
        hitPart.rotation = Quaternion.LookRotation((dir).normalized, destination.normal);
        hitPart.Rotate(Vector3.forward, Random.Range(-45.0f, 45.0f), Space.Self);

        // hitPart.position = destination.point;
        //  hitPart.rotation = Quaternion.LookRotation((destination.point - startPos).normalized, destination.normal);
        //  hitPart.Rotate(Vector3.forward, Random.Range(-45.0f, 45.0f), Space.Self);
        // Debug.DrawRay(hitPart.position, -hitPart.forward, Color.cyan, 1.0f);
    }

    /// <summary>Spreads the player transform based on kickAngleCheck (normalized)</summary>
    public static Ray[] GetCone(Transform origin, Vector3 forward, float spreadAngle)
    {
        Ray[] rays = new Ray[5];
        Vector3 fwDir = forward;
        rays[0].direction = forward;
        rays[1].direction = (Quaternion.AngleAxis(spreadAngle, origin.right) * fwDir).normalized;
        rays[2].direction = (Quaternion.AngleAxis(-spreadAngle, origin.right) * fwDir).normalized;
        rays[3].direction = (Quaternion.AngleAxis(spreadAngle, origin.up) * fwDir).normalized;
        rays[4].direction = (Quaternion.AngleAxis(-spreadAngle, origin.up) * fwDir).normalized;

        for (int i = 0; i < rays.Length; i++)
        {
            rays[i].origin = origin.position;
        }
        return rays;
    }

    public static Vector3[] GetConeLocal(float spreadAngle)
    {
        Vector3[] coneDirs = new Vector3[5];

        Vector3 start = Vector3.forward;
        coneDirs[0] = start;
        coneDirs[1] = Vector3.RotateTowards(start, Vector3.up, spreadAngle, 0f).normalized * 0.75f;
        coneDirs[2] = Vector3.RotateTowards(start, -Vector3.up, spreadAngle, 0f).normalized * 0.75f;
        coneDirs[3] = Vector3.RotateTowards(start, Vector3.right, spreadAngle, 0f).normalized * 0.75f;
        coneDirs[4] = Vector3.RotateTowards(start, -Vector3.right, spreadAngle, 0f).normalized * 0.75f;

        return coneDirs;
    }

    /// <summary>returns -1 or 1 (no inbetween) </summary>
    public static float RandomOneOneminus()
    {
        if (Random.Range(0, 2) == 0)
        {
            return -1;
        }
        return 1;
    }

    public static bool Contains(LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }

    public static bool FastApproximately(float a, float b, float threshold)
    {
        return ((a < b) ? (b - a) : (a - b)) <= threshold;
    }

    public static void RotateTargetPlanar(Transform rotate, Vector3 target)
    {
        Vector3 targetPostition = new Vector3(target.x,
                                     rotate.position.y,
                                      target.z);
        rotate.LookAt(targetPostition);
    }

    public static Quaternion GetPlanarRotation(Transform from, Transform to)
    {
        Vector3 dir = Vector3.ProjectOnPlane(to.position - from.position, Vector3.up).normalized;
        return Quaternion.LookRotation(dir, Vector3.up);
    }

    public static Quaternion MatchPlanarRotation(Transform to)
    {
        Vector3 dir = Vector3.ProjectOnPlane(to.forward, Vector3.up).normalized;
        return Quaternion.LookRotation(dir, Vector3.up);
    }

    public static Vector3 GetKnockbackDir(LungeDir dir, Transform origin, Transform target, bool planar)
    {
        Vector3 a = Vector3.zero;
        switch (dir)
        {
            case LungeDir.Forward:
                a = origin.forward;
                break;

            case LungeDir.Backward:
                a = -origin.forward;
                break;

            case LungeDir.Side:
                a = origin.transform.right * Mathfx.RandomOneOneminus();
                break;

            case LungeDir.Target:
                if (target)
                    a = target.position - origin.position;
                else
                    a = origin.forward;
                break;

            default:
                a = origin.forward;
                break;
        }
        if (planar)
            a = Vector3.ProjectOnPlane(a, Vector3.up);

        return a.normalized;
    }

    public static float Round(float value, float roundTo)
    {
        return Mathf.Round(value / roundTo) * roundTo;
    }

    public static Color LerpViaHSV(Color color0, Color color1, float t)
    {
        float h0, s0, v0, h1, s1, v1;
        Color.RGBToHSV(color0, out h0, out s0, out v0);
        Color.RGBToHSV(color1, out h1, out s1, out v1);
        float h = Mathf.LerpAngle(h0 * 360f, h1 * 360f, t) / 360f;
        float s = Mathf.LerpUnclamped(s0, s1, t);
        float v = Mathf.LerpUnclamped(v0, v1, t);
        return Color.HSVToRGB(h, s, v);
    }

    public static void DrawCross(Vector3 pos, Transform refAxis, Color color, bool flat, float length, float t)
    {
        //x
        Debug.DrawRay(pos, refAxis.right * length, color, t, false);
        Debug.DrawRay(pos, -refAxis.right * length, color, t, false);
        if (!flat)
        {
            //y
            Debug.DrawRay(pos, refAxis.up * length, color, t, false);
            Debug.DrawRay(pos, -refAxis.up * length, color, t, false);
        }
        //z
        Debug.DrawRay(pos, refAxis.forward * length, color, t, false);
        Debug.DrawRay(pos, -refAxis.forward * length, color, t, false);
    }

    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }

    public static float PlanarDist(Vector3 from, Vector3 to)
    {
        from.y = 0f;
        to.y = 0f;
        return Vector3.Distance(from, to);
    }

    public static float Remap(this float value, float inputMin, float inputMax, float outputMin, float outputMax, bool clamp)
    {
        float f = (outputMin + ((value - inputMin) * (outputMax - outputMin)) / (inputMax - inputMin));
        if (clamp)
            f = Mathf.Clamp(f, outputMin, outputMax);
        return f;
    }

    public static bool IsWithinRange(Vector3 a, Vector3 b, float minRange, float MaxRange)
    {
        float dist = Vector3.Distance(a, b);
        if (dist > minRange && dist < MaxRange)
            return true;
        else
            return false;
    }

    public static bool IsWithinRange(float dist, float min, float max)
    {
        if (dist > min)
        {
            if (dist < max)
                return true;
            else
                return false;
        }
        else
            return false;
    }

    public static Color ModifyChannel(int id, Color color, float newValue)
    {
        switch (id)
        {
            case 1:
                color.r = newValue;
                return color;

            case 2:
                color.g = newValue;
                return color;

            case 3:
                color.b = newValue;
                return color;

            case 4:
                color.a = newValue;
                return color;

            default:
                return color;
        }
    }

    public static bool CheckGroundedStatus(Vector3 start, float radius, float distance, float maxGroundedAngle, LayerMask groundLayers)
    {
        RaycastHit[] hits = Physics.SphereCastAll(start, radius, Vector3.down, distance, groundLayers);
        if (hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].normal != Vector3.down && hits[i].distance > 0)
                {
                    float surfaceAngleToY = Vector3.Angle(Vector3.up, hits[i].normal);

                    if (surfaceAngleToY <= maxGroundedAngle)   //point has to be on the character's feet level
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        else
            return false;
    }

    public static Vector3 SmoothApproach(Vector3 pastPosition, ref Vector3 pastTargetPosition, Vector3 targetPosition, float speed, float deltaTime)
    {
        float t = deltaTime * speed;
        if (t == 0) return pastPosition;

        Vector3 v = (targetPosition - pastTargetPosition) / t;
        Vector3 f = pastPosition - pastTargetPosition + v;
        pastTargetPosition = targetPosition;
        return targetPosition - v + f * Mathf.Exp(-t);
    }

    public static Vector2 GetPoint(Vector2 start, Vector2 end, float xSpeed, float gravity, float time)
    {
        var h_f = end.y - start.y;
        var a = gravity;
        var s = end.x - start.x;
        var t = s / xSpeed;

        // Don't overshoot
        time = Mathf.Clamp(time, 0, t);

        var v_h = h_f / t - (1 / 2f) * a * t;
        var v_s = s / t;

        var x = v_s * time;
        var y = (1 / 2f) * a * time * time + v_h * time;

        return new Vector2(x, y);
    }

    public static Vector3 GetPoint(Vector3 start, Vector3 end, float xSpeed, float gravity, float time)
    {
        Vector3 xzDir = (end - start).xz();

        var h_f = end.y - start.y;
        var a = gravity;
        var s = xzDir.magnitude;
        var t = s / xSpeed;

        // Don't overshoot
        time = Mathf.Clamp(time, 0, t);

        var v_h = h_f / t - (1 / 2f) * a * t;
        var v_s = s / t;

        var x = v_s * time;
        var y = (1 / 2f) * a * time * time + v_h * time;

        return x * xzDir.normalized + new Vector3(0, y, 0);
    }

    /// <summary>Get total distance between a list of transforms (order dependent)</summary>
    public static float GetTotalDistance(Transform[] points)
    {
        if (points.Length >= 2)
        {
            float d = 0f;
            for (int i = 0; i < points.Length - 1; i++)
            {
                d += Vector3.Distance(points[i].position, points[i + 1].position);
            }
            return d;
        }
        else
        {
            Debug.LogError("not enough points");
            return 0f;
        }
    }
}

/// <summary>Directions used for  </summary>
public enum LungeDir
{
    Forward,
    Backward,
    Side,
    Target
}