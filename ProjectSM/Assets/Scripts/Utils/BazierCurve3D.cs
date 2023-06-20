using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BazierCurve3D
{
    [System.Serializable]
    public class BazierPoint
    {
        public Vector3 point;
        public Vector3 postTangent;
        public Vector3 preTangent;

        public BazierPoint(Vector3 point, Vector3 postTangent, Vector3 preTangent)
        {
            this.point = point;
            this.postTangent = postTangent;
            this.preTangent = preTangent;
        }

        public BazierPoint() { }
    }
    [System.Serializable]
    public class Curve
    {
        public BazierPoint[] points;

        public Curve Clone()
        {
            Curve curve = new Curve();
            curve.points = points;
            return curve;
        }
    }

    public static Vector3 GetCurves(float t, Curve curve)
    {
        if (curve.points.Length <= 0)
            return Vector3.zero;
        else
        {
            float _t = t * (float)(curve.points.Length - 1);
            BazierPoint postPoint = curve.points[(int)_t];
            BazierPoint prePoint = curve.points[(int)(_t + 1f)];

            Vector3 p1 = Vector3.Lerp(postPoint.point, postPoint.preTangent, _t - Mathf.Floor(_t));
            Vector3 p2 = Vector3.Lerp(postPoint.preTangent, prePoint.postTangent, _t - Mathf.Floor(_t));
            Vector3 p3 = Vector3.Lerp(prePoint.postTangent, prePoint.point, _t - Mathf.Floor(_t));
            Vector3 p4 = Vector3.Lerp(p1, p2, _t - Mathf.Floor(_t));
            Vector3 p5 = Vector3.Lerp(p2, p3, _t - Mathf.Floor(_t));

            return Vector3.Lerp(p4, p5, _t - Mathf.Floor(_t));
        }
    }
}
