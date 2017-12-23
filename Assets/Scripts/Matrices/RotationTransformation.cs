﻿using UnityEngine;

public class RotationTransformation : Transformation 
{
    #region Properties

    public Vector3 rotation;

    #endregion

    #region Methods

    public override Matrix4x4 Matrix
    {
        get
        {
            Matrix4x4 matrix = new Matrix4x4();

            float radX = rotation.x * Mathf.Deg2Rad;
            float radY = rotation.y * Mathf.Deg2Rad;
            float radZ = rotation.z * Mathf.Deg2Rad;

            float cosX = Mathf.Cos(radX);
            float sinX = Mathf.Sin(radX);

            float cosY = Mathf.Cos(radY);
            float sinY = Mathf.Sin(radY);

            float cosZ = Mathf.Cos(radZ);
            float sinZ = Mathf.Sin(radZ);

            matrix.SetColumn(0, new Vector4(
                    cosY * cosZ,
                    cosX * sinZ + sinX * sinY * cosZ,
                    sinX * sinZ - cosX * sinY * cosZ,
                    0f
                ));

            matrix.SetColumn(1, new Vector4(
                    -cosY * sinZ,
                    cosX * cosZ - sinX * sinY * sinZ,
                    sinX * cosZ + cosX * sinY * sinZ, 
                    0f
                ));

            matrix.SetColumn(2, new Vector4(
                    sinY,
                    -sinX * cosY,
                    cosX * cosY,
                    0f
                ));

            matrix.SetColumn(3, new Vector4(0f, 0f, 0f, 1f));

            return matrix;
        }
    }

    #endregion
}
