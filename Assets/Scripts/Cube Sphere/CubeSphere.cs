using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CubeSphere : MonoBehaviour 
{
    #region Properties

    [Range(2, 50)]
    public int gridSize = 10;

    [Range(1,50)]
    public float radius = 10;

    public bool alternativeMapping;

    public bool showGizmos = false;

    public bool generateStepByStep = false;

    [Range(0.01f, 1f)]
    public float delayCoroutine = 0.1f;

    private Vector3[] vertices;

    private Vector3[] normals;

    private Mesh mesh;

    private Color32[] cubeUV;

    private WaitForSeconds delay;

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
        if (showGizmos)
        {
            if (vertices == null)
                return;

            Gizmos.color = Color.black;
            for (int i = 0; i < vertices.Length; i++)
                Gizmos.DrawSphere(vertices[i], 0.1f);

            Gizmos.color = Color.yellow;
            for (int i = 0; i < vertices.Length; i++)
                Gizmos.DrawRay(vertices[i], normals[i]);
        }
    }

    private static int SetQuad (int[] triangles, int i, int v00, int v10, int v01, int v11) 
    {
        triangles[i] = v00;
        triangles[i + 1] = triangles[i + 4] = v01;
        triangles[i + 2] = triangles[i + 3] = v10;
        triangles[i + 5] = v11;
        return i + 6;
    }

    private void SetVertex(int i, int x, int y, int z)
    {
        Vector3 v = new Vector3(x, y, z) * 2f / gridSize - Vector3.one;

        if (alternativeMapping)
        {
            float x2 = v.x * v.x;
            float y2 = v.y * v.y;
            float z2 = v.z * v.z;
            Vector3 s;
            s.x = v.x * Mathf.Sqrt(1f - y2 / 2f - z2 / 2f + y2 * z2 / 3f);
            s.y = v.y * Mathf.Sqrt(1f - x2 / 2f - z2 / 2f + x2 * z2 / 3f);
            s.z = v.z * Mathf.Sqrt(1f - x2 / 2f - y2 / 2f + x2 * y2 / 3f);

            normals[i] = s;
        }
        else
        {
            normals[i] = v.normalized;
        }

        vertices[i] = normals[i] * radius;

        cubeUV[i] = new Color32((byte)x, (byte)y, (byte)z, 0);
    }

    private void SetTriangles(int[] trianglesZ, int[] trianglesX, int[] trianglesY)
    {
        mesh.subMeshCount = 3;

        mesh.SetTriangles(trianglesZ, 0);
        mesh.SetTriangles(trianglesX, 1);
        mesh.SetTriangles(trianglesY, 2);
    }

    private void RemoveAllColliders()
    {
        SphereCollider[] spheres = GetComponents<SphereCollider>();

        foreach (SphereCollider sphere in spheres)
            Destroy(sphere);
    }

    #endregion

    #region Coroutines

    private IEnumerator Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();

        mesh.name = "Procedural Cube";

        delay = new WaitForSeconds(delayCoroutine);

        RemoveAllColliders();

        yield return (StartCoroutine(CreateVertices()));

        yield return (StartCoroutine(CreateTriangles()));

        yield return (StartCoroutine(CreateColliders()));

        if (gameObject.GetComponent<MeshDeformer>() != null)
            Destroy(gameObject.GetComponent<MeshDeformer>());
        
        if (gameObject.GetComponent<MeshDeformerInput>() != null)
            Destroy(gameObject.GetComponent<MeshDeformerInput>());

        gameObject.AddComponent<MeshDeformer>();
        gameObject.AddComponent<MeshDeformerInput>();
    }

    private IEnumerator CreateVertices() 
    {
        int cornerVertices = 8;

        int edgeVertices = (gridSize + gridSize + gridSize - 3) * 4;

        int faceVertices = 
            (
                (gridSize - 1) * (gridSize - 1) +
                (gridSize - 1) * (gridSize - 1) +
                (gridSize - 1) * (gridSize - 1)
            ) * 2;

        vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];

        normals = new Vector3[vertices.Length];

        cubeUV = new Color32[vertices.Length];

        int v = 0;

        for (int y = 0; y <= gridSize; y++)
        {
            for (int x = 0; x <= gridSize; x++)
            {
                SetVertex(v++, x, y, 0);

                if (generateStepByStep)
                    yield return delay;
            }

            for (int z = 1; z <= gridSize; z++)
            {
                SetVertex(v++, gridSize, y, z);

                if (generateStepByStep)
                    yield return delay;
            }

            for (int x = gridSize - 1; x >= 0; x--)
            {
                SetVertex(v++, x, y, gridSize);

                if (generateStepByStep)
                    yield return delay;
            }

            for (int z = gridSize - 1; z > 0; z--)
            {
                SetVertex(v++, 0, y, z);

                if (generateStepByStep)
                    yield return delay;
            }
        }

        for (int z = 1; z < gridSize; z++) 
        {
            for (int x = 1; x < gridSize; x++) 
            {
                SetVertex(v++, x, gridSize, z);

                if (generateStepByStep)
                    yield return delay;
            }
        }

        for (int z = 1; z < gridSize; z++) 
        {
            for (int x = 1; x < gridSize; x++) 
            {
                SetVertex(v++, x, 0, z);

                if (generateStepByStep)
                    yield return delay;
            }
        }

        mesh.vertices = vertices;

        mesh.normals = normals;

        mesh.colors32 = cubeUV;

        yield return null;
    }

    private IEnumerator CreateTriangles()
    {
        int[] trianglesZ = new int[(gridSize * gridSize) * 12];
        int[] trianglesX = new int[(gridSize * gridSize) * 12];
        int[] trianglesY = new int[(gridSize * gridSize) * 12];

        int ring = (gridSize + gridSize) * 2;
        int tZ = 0, tX = 0, tY = 0, v = 0;

        // Rings around cube
        for (int y = 0; y < gridSize; y++, v++)
        {
            for (int q = 0; q < gridSize; q++, v++)
            {
                tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);

                if (generateStepByStep)
                {
                    SetTriangles(trianglesZ, trianglesX, trianglesY);
                    yield return delay;
                }
            }

            for (int q = 0; q < gridSize; q++, v++) 
            {
                tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);

                if (generateStepByStep)
                {
                    SetTriangles(trianglesZ, trianglesX, trianglesY);
                    yield return delay;
                }
            }

            for (int q = 0; q < gridSize; q++, v++) 
            {
                tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);

                if (generateStepByStep)
                {
                    SetTriangles(trianglesZ, trianglesX, trianglesY);
                    yield return delay;
                }
            }

            for (int q = 0; q < gridSize - 1; q++, v++) 
            {
                tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);

                if (generateStepByStep)
                {
                    SetTriangles(trianglesZ, trianglesX, trianglesY);
                    yield return delay;
                }
            }

            tX = SetQuad(trianglesX, tX, v, v - ring + 1, v + ring, v + 1);

            if (generateStepByStep)
            {
                SetTriangles(trianglesZ, trianglesX, trianglesY);
                yield return delay;
            }
        }

        // Top of the cube
        v = ring * gridSize;
        for (int x = 0; x < gridSize - 1; x++, v++) 
        {
            tY = SetQuad(trianglesY, tY, v, v + 1, v + ring - 1, v + ring);

            if (generateStepByStep)
            {
                SetTriangles(trianglesZ, trianglesX, trianglesY);
                yield return delay;
            }
        }

        tY = SetQuad(trianglesY, tY, v, v + 1, v + ring - 1, v + 2);

        if (generateStepByStep)
        {
            SetTriangles(trianglesZ, trianglesX, trianglesY);
            yield return delay;
        }

        int vMin = ring * (gridSize + 1) - 1;
        int vMid = vMin + 1;
        int vMax = v + 2;

        for (int z = 1; z < gridSize - 1; z++, vMin--, vMid++, vMax++) 
        {
            tY = SetQuad(trianglesY, tY, vMin, vMid, vMin - 1, vMid + gridSize - 1);

            if (generateStepByStep)
            {
                SetTriangles(trianglesZ, trianglesX, trianglesY);
                yield return delay;
            }

            for (int x = 1; x < gridSize - 1; x++, vMid++) 
            {
                tY = SetQuad(trianglesY, tY, vMid, vMid + 1, vMid + gridSize - 1, vMid + gridSize);

                if (generateStepByStep)
                {
                    SetTriangles(trianglesZ, trianglesX, trianglesY);
                    yield return delay;
                }
            }

            tY = SetQuad(trianglesY, tY, vMid, vMax, vMid + gridSize - 1, vMax + 1);

            if (generateStepByStep)
            {
                SetTriangles(trianglesZ, trianglesX, trianglesY);
                yield return delay;
            }
        }

        int vTop = vMin - 2;

        tY = SetQuad(trianglesY, tY, vMin, vMid, vMin - 1, vMin - 2);

        if (generateStepByStep)
        {
            SetTriangles(trianglesZ, trianglesX, trianglesY);
            yield return delay;
        }

        for (int x = 1; x < gridSize - 1; x++, vTop--, vMid++) 
        {
            tY = SetQuad(trianglesY, tY, vMid, vMid + 1, vTop, vTop - 1);

            if (generateStepByStep)
            {
                SetTriangles(trianglesZ, trianglesX, trianglesY);
                yield return delay;
            }
        }

        tY = SetQuad(trianglesY, tY, vMid, vTop - 2, vTop, vTop - 1);

        if (generateStepByStep)
        {
            SetTriangles(trianglesZ, trianglesX, trianglesY);
            yield return delay;
        }

        // Bottom
        v = 1;
        vMid = vertices.Length - (gridSize - 1) * (gridSize - 1);

        tY = SetQuad(trianglesY, tY, ring - 1, vMid, 0, 1);

        if (generateStepByStep)
        {
            SetTriangles(trianglesZ, trianglesX, trianglesY);
            yield return delay;
        }

        for (int x = 1; x < gridSize - 1; x++, v++, vMid++) 
        {
            tY = SetQuad(trianglesY, tY, vMid, vMid + 1, v, v + 1);

            if (generateStepByStep)
            {
                SetTriangles(trianglesZ, trianglesX, trianglesY);
                yield return delay;
            }
        }

        tY = SetQuad(trianglesY, tY, vMid, v + 2, v, v + 1);

        if (generateStepByStep)
        {
            SetTriangles(trianglesZ, trianglesX, trianglesY);
            yield return delay;
        }

        vMin = ring - 2;
        vMid -= gridSize - 2;
        vMax = v + 2;

        for (int z = 1; z < gridSize - 1; z++, vMin--, vMid++, vMax++) 
        {
            tY = SetQuad(trianglesY, tY, vMin, vMid + gridSize - 1, vMin + 1, vMid);

            if (generateStepByStep)
            {
                SetTriangles(trianglesZ, trianglesX, trianglesY);
                yield return delay;
            }

            for (int x = 1; x < gridSize - 1; x++, vMid++) 
            {
                tY = SetQuad(trianglesY, tY, vMid + gridSize - 1, vMid + gridSize, vMid, vMid + 1);

                if (generateStepByStep)
                {
                    SetTriangles(trianglesZ, trianglesX, trianglesY);
                    yield return delay;
                }
            }

            tY = SetQuad(trianglesY, tY, vMid + gridSize - 1, vMax + 1, vMid, vMax);

            if (generateStepByStep)
            {
                SetTriangles(trianglesZ, trianglesX, trianglesY);
                yield return delay;
            }
        }

        vTop = vMin - 1;

        tY = SetQuad(trianglesY, tY, vTop + 1, vTop, vTop + 2, vMid);

        if (generateStepByStep)
        {
            SetTriangles(trianglesZ, trianglesX, trianglesY);
            yield return delay;
        }

        for (int x = 1; x < gridSize - 1; x++, vTop--, vMid++) 
        {
            tY = SetQuad(trianglesY, tY, vTop, vTop - 1, vMid, vMid + 1);

            if (generateStepByStep)
            {
                SetTriangles(trianglesZ, trianglesX, trianglesY);
                yield return delay;
            }
        }

        tY = SetQuad(trianglesY, tY, vTop, vTop - 1, vMid, vTop - 2);

        if (generateStepByStep)
        {
            SetTriangles(trianglesZ, trianglesX, trianglesY);
            yield return delay;
        }

        SetTriangles(trianglesZ, trianglesX, trianglesY);

        yield return null;
    }

    private IEnumerator CreateColliders()
    {
        gameObject.AddComponent<SphereCollider>();

        yield return null;
    }

    /*
    private IEnumerator CreateTriangles()
    {
        int quads = (gridSize * gridSize + gridSize * gridSize + gridSize * gridSize) * 2;

        int[] triangles = new int[quads * 6];

        int ring = (gridSize + gridSize) * 2;
        int t = 0, v = 0;

        // Rings around cube
        for (int y = 0; y < gridSize; y++, v++)
        {
            for (int q = 0; q < ring - 1; q++, v++)
            {
                t = SetQuad(triangles, t, v, v + 1, v + ring, v + ring + 1);

                if (generateStepByStep)
                {
                    mesh.triangles = triangles;
                    yield return delay;
                }
            }

            t = SetQuad(triangles, t, v, v - ring + 1, v + ring, v + 1);

            if (generateStepByStep)
            {
                mesh.triangles = triangles;
                yield return delay;
            }
        }

        // Top of the cube
        v = ring * gridSize;
        for (int x = 0; x < gridSize - 1; x++, v++) 
        {
            t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);

            if (generateStepByStep)
            {
                mesh.triangles = triangles;
                yield return delay;
            }
        }

        t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);

        if (generateStepByStep)
        {
            mesh.triangles = triangles;
            yield return delay;
        }

        int vMin = ring * (gridSize + 1) - 1;
        int vMid = vMin + 1;
        int vMax = v + 2;

        for (int z = 1; z < gridSize - 1; z++, vMin--, vMid++, vMax++) 
        {
            t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + gridSize - 1);

            if (generateStepByStep)
            {
                mesh.triangles = triangles;
                yield return delay;
            }

            for (int x = 1; x < gridSize - 1; x++, vMid++) 
            {
                t = SetQuad(triangles, t, vMid, vMid + 1, vMid + gridSize - 1, vMid + gridSize);

                if (generateStepByStep)
                {
                    mesh.triangles = triangles;
                    yield return delay;
                }
            }

            t = SetQuad(triangles, t, vMid, vMax, vMid + gridSize - 1, vMax + 1);

            if (generateStepByStep)
            {
                mesh.triangles = triangles;
                yield return delay;
            }
        }

        int vTop = vMin - 2;

        t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMin - 2);

        if (generateStepByStep)
        {
            mesh.triangles = triangles;
            yield return delay;
        }

        for (int x = 1; x < gridSize - 1; x++, vTop--, vMid++) 
        {
            t = SetQuad(triangles, t, vMid, vMid + 1, vTop, vTop - 1);

            if (generateStepByStep)
            {
                mesh.triangles = triangles;
                yield return delay;
            }
        }

        t = SetQuad(triangles, t, vMid, vTop - 2, vTop, vTop - 1);

        if (generateStepByStep)
        {
            mesh.triangles = triangles;
            yield return delay;
        }

        // Bottom
        v = 1;
        vMid = vertices.Length - (gridSize - 1) * (gridSize - 1);

        t = SetQuad(triangles, t, ring - 1, vMid, 0, 1);

        if (generateStepByStep)
        {
            mesh.triangles = triangles;
            yield return delay;
        }

        for (int x = 1; x < gridSize - 1; x++, v++, vMid++) 
        {
            t = SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);

            if (generateStepByStep)
            {
                mesh.triangles = triangles;
                yield return delay;
            }
        }

        t = SetQuad(triangles, t, vMid, v + 2, v, v + 1);

        if (generateStepByStep)
        {
            mesh.triangles = triangles;
            yield return delay;
        }

        vMin = ring - 2;
        vMid -= gridSize - 2;
        vMax = v + 2;

        for (int z = 1; z < gridSize - 1; z++, vMin--, vMid++, vMax++) 
        {
            t = SetQuad(triangles, t, vMin, vMid + gridSize - 1, vMin + 1, vMid);

            if (generateStepByStep)
            {
                mesh.triangles = triangles;
                yield return delay;
            }

            for (int x = 1; x < gridSize - 1; x++, vMid++) 
            {
                t = SetQuad(triangles, t, vMid + gridSize - 1, vMid + gridSize, vMid, vMid + 1);

                if (generateStepByStep)
                {
                    mesh.triangles = triangles;
                    yield return delay;
                }
            }

            t = SetQuad(triangles, t, vMid + gridSize - 1, vMax + 1, vMid, vMax);

            if (generateStepByStep)
            {
                mesh.triangles = triangles;
                yield return delay;
            }
        }

        vTop = vMin - 1;

        t = SetQuad(triangles, t, vTop + 1, vTop, vTop + 2, vMid);

        if (generateStepByStep)
        {
            mesh.triangles = triangles;
            yield return delay;
        }

        for (int x = 1; x < gridSize - 1; x++, vTop--, vMid++) 
        {
            t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vMid + 1);

            if (generateStepByStep)
            {
                mesh.triangles = triangles;
                yield return delay;
            }
        }

        t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vTop - 2);

        if (generateStepByStep)
        {
            mesh.triangles = triangles;
            yield return delay;
        }

        mesh.triangles = triangles;

        yield return null;
    }
    */

    #endregion
}
