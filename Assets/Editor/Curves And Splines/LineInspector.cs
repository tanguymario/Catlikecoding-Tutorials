using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Line))]
public class LineInspector : Editor 
{
    #region Properties

    private Line line;
    private Transform handleTransform;
    private Quaternion handleRotation;

    #endregion

    #region Unity Callbacks

    private void OnSceneGUI()
    {
        line = target as Line;
        handleTransform = line.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

        Vector3 p0 = ShowPoint(0);
        Vector3 p1 = ShowPoint(1);

        Handles.color = Color.white;
        Handles.DrawLine(p0, p1);


        EditorGUI.BeginChangeCheck();
        p0 = Handles.DoPositionHandle(p0, handleRotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(line, "Move Point");
            EditorUtility.SetDirty(line);
            line.p0 = handleTransform.InverseTransformPoint(p0);
        }

        EditorGUI.BeginChangeCheck();
        p1 = Handles.DoPositionHandle(p1, handleRotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(line, "Move Point");
            EditorUtility.SetDirty(line);
            line.p1 = handleTransform.InverseTransformPoint(p1);
        }
    }

    #endregion

    #region Methods

    private Vector3 ShowPoint(int index)
    {
        Vector3 linePoint;
        if (index == 0)
            linePoint = line.p0;
        else
            linePoint = line.p1;

        Vector3 point = handleTransform.TransformPoint(linePoint);

        EditorGUI.BeginChangeCheck();
        point = Handles.DoPositionHandle(point, handleRotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(line, "Move Point");
            EditorUtility.SetDirty(line);
            linePoint = handleTransform.InverseTransformPoint(point);
        }

        return point;
    }

    #endregion
}
