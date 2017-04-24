using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class RoundedCube : MonoBehaviour 
{
    #region Properties

    [Range(0, 100)]
    public int width;

    [Range(0, 100)]
    public int height;

    [Range(0, 100)]
    public int depth;

    [Range(0, 30)]
    public float roundness;

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

    private void OnDrawGizmos () 
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
        Vector3 inner = vertices[i] = new Vector3(x, y, z);

        if (x < roundness)
        {
            inner.x = roundness;
        }
        else if (x > width - roundness)
        {
            inner.x = width - roundness;
        }

        if (y < roundness)
        {
            inner.y = roundness;
        }
        else if (y > height - roundness)
        {
            inner.y = height - roundness;
        }

        if (z < roundness)
        {
            inner.z = roundness;
        }
        else if (z > depth - roundness)
        {
            inner.z = depth - roundness;
        }

        normals[i] = (vertices[i] - inner).normalized;

        vertices[i] = inner + normals[i] * roundness;

        cubeUV[i] = new Color32((byte)x, (byte)y, (byte)z, 0);
    }

    private void SetTriangles(int[] trianglesZ, int[] trianglesX, int[] trianglesY)
    {
        mesh.subMeshCount = 3;

        mesh.SetTriangles(trianglesZ, 0);
        mesh.SetTriangles(trianglesX, 1);
        mesh.SetTriangles(trianglesY, 2);
    }

    private void AddBoxCollider(float x, float y, float z)
    {
        BoxCollider c = gameObject.AddComponent<BoxCollider>();
        c.size = new Vector3(x, y, z);
    }

    private void AddCapsuleCollider (int direction, float x, float y, float z) 
    {
        CapsuleCollider c = gameObject.AddComponent<CapsuleCollider>();

        c.center = new Vector3(x, y, z);
        c.direction = direction;
        c.radius = roundness;
        c.height = c.center[direction] * 2f;
    }

    private void RemoveAllColliders()
    {
        BoxCollider[] boxColliders = GetComponents<BoxCollider>();
        CapsuleCollider[] capsuleColliders = GetComponents<CapsuleCollider>();

        foreach (BoxCollider box in boxColliders)
            Destroy(box);

        foreach (CapsuleCollider capsule in capsuleColliders)
            Destroy(capsule);
    }

    #endregion

    #region Coroutines

    private IEnumerator Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();

        mesh.name = "Procedural Cube";

        delay = new WaitForSeconds(delayCoroutine);

        yield return (StartCoroutine(CreateVertices()));

        yield return (StartCoroutine(CreateTriangles()));

        yield return (StartCoroutine(CreateColliders()));
    }

    private IEnumerator CreateVertices() 
    {
        int cornerVertices = 8;

        int edgeVertices = (width + height + depth - 3) * 4;

        int faceVertices = 
            (
                (width - 1) * (height - 1) +
                (width - 1) * (depth - 1) +
                (height - 1) * (depth - 1)
            ) * 2;

        vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];

        normals = new Vector3[vertices.Length];

        cubeUV = new Color32[vertices.Length];

        int v = 0;

        for (int y = 0; y <= height; y++)
        {
            for (int x = 0; x <= width; x++)
            {
                SetVertex(v++, x, y, 0);

                if (generateStepByStep)
                    yield return delay;
            }

            for (int z = 1; z <= depth; z++)
            {
                SetVertex(v++, width, y, z);

                if (generateStepByStep)
                    yield return delay;
            }

            for (int x = width - 1; x >= 0; x--)
            {
                SetVertex(v++, x, y, depth);

                if (generateStepByStep)
                    yield return delay;
            }

            for (int z = depth - 1; z > 0; z--)
            {
                SetVertex(v++, 0, y, z);

                if (generateStepByStep)
                    yield return delay;
            }
        }

        for (int z = 1; z < depth; z++) 
        {
            for (int x = 1; x < width; x++) 
            {
                SetVertex(v++, x, height, z);

                if (generateStepByStep)
                    yield return delay;
            }
        }

        for (int z = 1; z < depth; z++) 
        {
            for (int x = 1; x < width; x++) 
            {
                SetVertex(v++, x, 0, z);

                if (generateStepByStep)
                    yield return delay;
            }
        }

        mesh.vertices = vertices;

        mesh.normals = normals;

        mesh.colors32 = cubeUV;

        if (roundness == 0f)
            mesh.RecalculateNormals();

        yield return null;
    }

    private IEnumerator CreateTriangles()
    {
        int[] trianglesZ = new int[(width * height) * 12];
        int[] trianglesX = new int[(height * depth) * 12];
        int[] trianglesY = new int[(width * depth) * 12];

        int ring = (width + depth) * 2;
        int tZ = 0, tX = 0, tY = 0, v = 0;

        // Rings around cube
        for (int y = 0; y < height; y++, v++)
        {
            for (int q = 0; q < width; q++, v++)
            {
                tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);

                if (generateStepByStep)
                {
                    SetTriangles(trianglesZ, trianglesX, trianglesY);
                    yield return delay;
                }
            }

            for (int q = 0; q < depth; q++, v++) 
            {
                tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);

                if (generateStepByStep)
                {
                    SetTriangles(trianglesZ, trianglesX, trianglesY);
                    yield return delay;
                }
            }

            for (int q = 0; q < width; q++, v++) 
            {
                tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);

                if (generateStepByStep)
                {
                    SetTriangles(trianglesZ, trianglesX, trianglesY);
                    yield return delay;
                }
            }

            for (int q = 0; q < depth - 1; q++, v++) 
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
        v = ring * height;
        for (int x = 0; x < width - 1; x++, v++) 
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

        int vMin = ring * (height + 1) - 1;
        int vMid = vMin + 1;
        int vMax = v + 2;

        for (int z = 1; z < depth - 1; z++, vMin--, vMid++, vMax++) 
        {
            tY = SetQuad(trianglesY, tY, vMin, vMid, vMin - 1, vMid + width - 1);

            if (generateStepByStep)
            {
                SetTriangles(trianglesZ, trianglesX, trianglesY);
                yield return delay;
            }

            for (int x = 1; x < width - 1; x++, vMid++) 
            {
                tY = SetQuad(trianglesY, tY, vMid, vMid + 1, vMid + width - 1, vMid + width);

                if (generateStepByStep)
                {
                    SetTriangles(trianglesZ, trianglesX, trianglesY);
                    yield return delay;
                }
            }

            tY = SetQuad(trianglesY, tY, vMid, vMax, vMid + width - 1, vMax + 1);

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

        for (int x = 1; x < width - 1; x++, vTop--, vMid++) 
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
        vMid = vertices.Length - (width - 1) * (depth - 1);

        tY = SetQuad(trianglesY, tY, ring - 1, vMid, 0, 1);

        if (generateStepByStep)
        {
            SetTriangles(trianglesZ, trianglesX, trianglesY);
            yield return delay;
        }

        for (int x = 1; x < width - 1; x++, v++, vMid++) 
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
        vMid -= width - 2;
        vMax = v + 2;

        for (int z = 1; z < depth - 1; z++, vMin--, vMid++, vMax++) 
        {
            tY = SetQuad(trianglesY, tY, vMin, vMid + width - 1, vMin + 1, vMid);

            if (generateStepByStep)
            {
                SetTriangles(trianglesZ, trianglesX, trianglesY);
                yield return delay;
            }

            for (int x = 1; x < width - 1; x++, vMid++) 
            {
                tY = SetQuad(trianglesY, tY, vMid + width - 1, vMid + width, vMid, vMid + 1);

                if (generateStepByStep)
                {
                    SetTriangles(trianglesZ, trianglesX, trianglesY);
                    yield return delay;
                }
            }

            tY = SetQuad(trianglesY, tY, vMid + width - 1, vMax + 1, vMid, vMax);

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

        for (int x = 1; x < width - 1; x++, vTop--, vMid++) 
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
        RemoveAllColliders();

        AddBoxCollider(width, height - roundness * 2, depth - roundness * 2);
        AddBoxCollider(width - roundness * 2, height, depth - roundness * 2);
        AddBoxCollider(width - roundness * 2, height - roundness * 2, depth);

        Vector3 min = Vector3.one * roundness;
        Vector3 half = new Vector3(width, height, depth) * 0.5f; 
        Vector3 max = new Vector3(width, height, depth) - min;

        AddCapsuleCollider(0, half.x, min.y, min.z);
        AddCapsuleCollider(0, half.x, min.y, max.z);
        AddCapsuleCollider(0, half.x, max.y, min.z);
        AddCapsuleCollider(0, half.x, max.y, max.z);

        AddCapsuleCollider(1, min.x, half.y, min.z);
        AddCapsuleCollider(1, min.x, half.y, max.z);
        AddCapsuleCollider(1, max.x, half.y, min.z);
        AddCapsuleCollider(1, max.x, half.y, max.z);

        AddCapsuleCollider(2, min.x, min.y, half.z);
        AddCapsuleCollider(2, min.x, max.y, half.z);
        AddCapsuleCollider(2, max.x, min.y, half.z);
        AddCapsuleCollider(2, max.x, max.y, half.z);

        yield return null;
    }

    /*
    private IEnumerator CreateTriangles()
    {
        int quads = (width * height + width * depth + height * depth) * 2;

        int[] triangles = new int[quads * 6];

        int ring = (width + depth) * 2;
        int t = 0, v = 0;

        // Rings around cube
        for (int y = 0; y < height; y++, v++)
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
        v = ring * height;
        for (int x = 0; x < width - 1; x++, v++) 
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

        int vMin = ring * (height + 1) - 1;
        int vMid = vMin + 1;
        int vMax = v + 2;

        for (int z = 1; z < depth - 1; z++, vMin--, vMid++, vMax++) 
        {
            t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + width - 1);

            if (generateStepByStep)
            {
                mesh.triangles = triangles;
                yield return delay;
            }

            for (int x = 1; x < width - 1; x++, vMid++) 
            {
                t = SetQuad(triangles, t, vMid, vMid + 1, vMid + width - 1, vMid + width);

                if (generateStepByStep)
                {
                    mesh.triangles = triangles;
                    yield return delay;
                }
            }

            t = SetQuad(triangles, t, vMid, vMax, vMid + width - 1, vMax + 1);

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

        for (int x = 1; x < width - 1; x++, vTop--, vMid++) 
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
        vMid = vertices.Length - (width - 1) * (depth - 1);

        t = SetQuad(triangles, t, ring - 1, vMid, 0, 1);

        if (generateStepByStep)
        {
            mesh.triangles = triangles;
            yield return delay;
        }

        for (int x = 1; x < width - 1; x++, v++, vMid++) 
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
        vMid -= width - 2;
        vMax = v + 2;

        for (int z = 1; z < depth - 1; z++, vMin--, vMid++, vMax++) 
        {
            t = SetQuad(triangles, t, vMin, vMid + width - 1, vMin + 1, vMid);

            if (generateStepByStep)
            {
                mesh.triangles = triangles;
                yield return delay;
            }

            for (int x = 1; x < width - 1; x++, vMid++) 
            {
                t = SetQuad(triangles, t, vMid + width - 1, vMid + width, vMid, vMid + 1);

                if (generateStepByStep)
                {
                    mesh.triangles = triangles;
                    yield return delay;
                }
            }

            t = SetQuad(triangles, t, vMid + width - 1, vMax + 1, vMid, vMax);

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

        for (int x = 1; x < width - 1; x++, vTop--, vMid++) 
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
