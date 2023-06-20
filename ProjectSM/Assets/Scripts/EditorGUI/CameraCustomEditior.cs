using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEditor.SceneManagement;
using Unity.VisualScripting;

[CustomEditor(typeof(CameraController))]
public class CameraCustomEditior : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CameraController controller = (CameraController)target;
        if(GUILayout.Button("X Jigger Test"))
        {
            controller.XJigger(controller.jiggerSpeed);
        }
        if (GUILayout.Button("Y Jigger Test"))
        {
            controller.YJigger(controller.jiggerSpeed);
        }
        if(GUILayout.Button("Test Curves"))
        {
            controller.CameraMove(controller.movingCurves.Length - 1, 5.0f, true);
        }
    }

    private void OnSceneGUI()
    {
        CameraController controller = (CameraController)target;

        Handles.color = Color.magenta;
        if (controller.movingCurves.Length > 0)
        {
            EditorGUI.BeginChangeCheck();
            BazierCurve3D.Curve curve = controller.movingCurves[controller.movingCurves.Length-1].curve.Clone();
            BazierCurve3D.BazierPoint zeroPoint = new BazierCurve3D.BazierPoint();
            zeroPoint.point = Handles.FreeMoveHandle(controller.transform.position, 0.2f, Vector3.zero, Handles.CylinderHandleCap);
            zeroPoint.preTangent = Handles.FreeMoveHandle(controller.transform.position, 0.1f, Vector3.zero, Handles.CylinderHandleCap);

            for(int i = 0; i < curve.points.Length; i++)
            {
                Handles.color = Color.green;
                curve.points[i].postTangent = Handles.FreeMoveHandle(curve.points[i].postTangent, .1f, Vector3.zero, Handles.CylinderHandleCap);
                if (i != curve.points.Length - 1)
                {
                    Handles.color = Color.magenta;
                    curve.points[i].point = Handles.FreeMoveHandle(curve.points[i].point, .2f, Vector3.zero, Handles.CylinderHandleCap);
                    Handles.color = Color.red;
                    curve.points[i].preTangent = Handles.FreeMoveHandle(curve.points[i].preTangent, .1f, Vector3.zero, Handles.CylinderHandleCap);
                }
                else
                {
                    if (controller.movingCurves[controller.movingCurves.Length - 1].trackingTarget)
                    {
                        Handles.color = Color.blue;
                        curve.points[i].point = Handles.FreeMoveHandle(curve.points[i].point, .2f, Vector3.zero, Handles.CylinderHandleCap);
                    }
                    else
                    {
                        Handles.color = Color.magenta;
                        curve.points[i].point = Handles.FreeMoveHandle(curve.points[i].point, .2f, Vector3.zero, Handles.CylinderHandleCap);
                    }
                    curve.points[i].preTangent = curve.points[i].point;
                }
                Handles.color = Color.white;
                Handles.DrawLine(curve.points[i].postTangent, curve.points[i].point);
                Handles.DrawLine(curve.points[i].preTangent, curve.points[i].point);
            }

            if (curve.points.Length > 0)
            {
                Handles.DrawBezier(zeroPoint.point, curve.points[0].point,zeroPoint.preTangent, curve.points[0].postTangent, Color.gray, null, 2f);
                for(int i = 0; i < curve.points.Length-1; i++) 
                {
                    Handles.DrawBezier(curve.points[i].point, curve.points[i + 1].point, curve.points[i].preTangent, curve.points[i + 1].postTangent, Color.gray, null, 2f);
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(controller, "Camera Curve Changed");
                controller.movingCurves[controller.movingCurves.Length - 1].curve = curve;
            }
        }
    }
}
