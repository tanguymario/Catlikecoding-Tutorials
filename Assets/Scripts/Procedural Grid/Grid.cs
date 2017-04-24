using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Grid : MonoBehaviour 
{
    #region Properties

    [Range(0, 100)]
    public int width;

    [Range(0, 100)]
    public int height;

    public bool generateStepByStep = false;

    private Vector3[] vertices;

    private Mesh mesh;

    private const float delayCoroutine = 0.01f;

    #endregion
	
    #region Unity Callbacks

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            StopAllCoroutines();
            StartCoroutine(Generate());
        }
    }
	
    #endregion

    #region Methods

    private void OnDrawGizmos()
    {
        if (vertices == null)
            return;

        Gizmos.color = Color.black;

        for (int i = 0; i < vertices.Length; i++)
            Gizmos.DrawSphere(vertices[i], 0.1f);
    }

    #endregion

    #region Coroutines

    private IEnumerator Generate()
    {
        WaitForSeconds delay = new WaitForSeconds(delayCoroutine);

        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";

        vertices = new Vector3[(width + 1) * (height + 1)];

        Vector2[] uv = new Vector2[vertices.Length];

        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);

        for (int i = 0, y = 0; y <= height; y++) 
        {
            for (int x = 0; x <= width; x++, i++) 
            {
                vertices[i] = new Vector3(x, y);

                uv[i] = new Vector2((float)x / width, (float)y / height);

                tangents[i] = tangent;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.tangents = tangents;

        int[] triangles = new int[width * height * 6];

        for (int ti = 0, vi = 0, y = 0; y < height; y++, vi++)
        {
            for (int x = 0; x < width; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 1] = triangles[ti + 4] = vi + width + 1;
                triangles[ti + 2] = triangles[ti + 3] = vi + 1;
                triangles[ti + 5] = vi + width + 2; 

                mesh.triangles = triangles;

                if (generateStepByStep)
                    yield return delay;

            }
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        yield return null;
    }

    #endregion
}
