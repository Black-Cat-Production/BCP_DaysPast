using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.MinigameSystem.ConnectTheDots
{
    public static class IntersectionUtility
    {

        public static bool IntersectAny(ConnectTheDotsGame.LineSegment _newLine, List<ConnectTheDotsGame.LineSegment> _lineSegments)
        {
            return _lineSegments.Any(_line => DoLinesIntersect(_newLine.start, _newLine.end, _line.start, _line.end) && !IsSharedEndpoint(_newLine.start, _newLine.end, _line.start, _line.end));
        }

        static bool IsSharedEndpoint(Vector2 _p1, Vector2 _p2, Vector2 _p3, Vector2 _p4)
        {
            return NearlyEqual(_p1, _p3) || NearlyEqual(_p1, _p4) || NearlyEqual(_p2, _p3) || NearlyEqual(_p2, _p4);
        }

        static bool NearlyEqual(Vector2 _a, Vector2 _b, float _tolerance = 0.001f)
        {
            return Vector2.Distance(_a , _b) < _tolerance;
        }

        static bool DoLinesIntersect(Vector2 _p1, Vector2 _p2, Vector2 _p3, Vector2 _p4)
        {
            var a = _p2 - _p1;
            var b = _p4 - _p3;
            var c = _p3 - _p1;
            
            float denominator = Cross(a,b);
            if (denominator == 0) return false;

            float numerator1 = Cross(c,b);
            float numerator2 = Cross(c,a);

            float t1 = numerator1 / denominator;
            float t2 = numerator2 / denominator;

            return t1 is >= 0 and <= 1 && t2 is >= 0 and <= 1;
        }

        static float Cross(Vector2 _a, Vector2 _b)
        {
            return _a.x * _b.y - _a.y * _b.x;
        }
    }
}