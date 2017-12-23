using UnityEngine;

public class CameraTransformation : Transformation
{
    #region Properties

    public bool ortographic;

    public bool perspective;

    public float focalLength = 1f;

    #endregion

    #region Methods

    public override Matrix4x4 Matrix
    {
        get
        {
            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetRow(0, new Vector4(focalLength, 0f, 0f, 0f));
            matrix.SetRow(1, new Vector4(0f, focalLength, 0f, 0f));

            if (!ortographic)
                matrix.SetRow(2, new Vector4(0f, 0f, 1f, 0f));
            else 
                matrix.SetRow(2, new Vector4(0f, 0f, 0f, 0f));

            if (!perspective)
                matrix.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
            else
                matrix.SetRow(3, new Vector4(0f, 0f, 1f, 0f));

            return matrix;
        }
    }

    #endregion
}
