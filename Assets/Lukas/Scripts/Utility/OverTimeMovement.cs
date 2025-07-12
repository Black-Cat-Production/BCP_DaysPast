using System;
using System.Collections;
using UnityEngine;

namespace Scripts.Utility
{
    public static class OverTimeMovement
    {
        public static IEnumerator MoveOverTime<T>(T _from, T _to, float _duration, Action<T> _onUpdate)
        {
            float elapsed = 0f;

            if (typeof(T) == typeof(Quaternion))
            {
                var fromQ = (Quaternion)(object)_from;
                var toQ = (Quaternion)(object)_to;

                float fromZ = fromQ.eulerAngles.z;
                float toZ = toQ.eulerAngles.z;

                if (toZ > fromZ)
                    toZ -= 360f;

                while (elapsed < _duration)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / _duration);
                    float z = Mathf.Lerp(fromZ, toZ, t);

                    var rotation = Quaternion.Euler(0f, fromQ.eulerAngles.y, z);
                    _onUpdate((T)(object)rotation);
                    yield return null;
                }

                yield break;
            }

            while (elapsed < _duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / _duration);

                if (typeof(T) == typeof(Vector3))
                    _onUpdate((T)(object)Vector3.Lerp((Vector3)(object)_from, (Vector3)(object)_to, t));

                yield return null;
            }
        }
    }
}