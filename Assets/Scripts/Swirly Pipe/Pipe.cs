﻿using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Pipe : MonoBehaviour 
{
    #region Properties

    #region Pipe

    [Range(0, 100)]
    public float minCurveRadius, maxCurveRadius;
    
    [Range(0, 100)]
    public int minCurveSegmentCout, maxCurveSegmentCount;

    [Range(0, 100)]
    public float pipeRadius;

    [Range(0, 100)]
    public int pipeSegmentCount;

    [Range(0f, 1f)]
    public float ringDistance;

    public PipeItemGenerator[] generators;

    private float curveRadius;
    public float CurveRadius
    {
        get { return curveRadius; }
    }

    private int curveSegmentCount;
    public int CurveSegmentCount
    {
        get { return curveSegmentCount; }
    }

    private float curveAngle;
    public float CurveAngle
    {
        get { return curveAngle; }
    }

    private float curveSegmentStep
    {
        get { return (2f * Mathf.PI) / curveSegmentCount; }
    }

    private float pipeSegmentStep
    {
        get { return (2f * Mathf.PI) / pipeSegmentCount; }
    }

    private float relativeRotation;
    public float RelativeRotation
    {
        get { return relativeRotation; }
    }

    #endregion

    #region Mesh

    private Mesh mesh;

    private Vector3[] vertices;

    private int[] triangles;

    private Vector2[] uv;

    #endregion

    #endregion

    #region Unity Callbacks
	
    private void Awake() 
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Pipe";
	}

    #endregion

    #region Methods

    public void Generate(bool withItems = true)
    {
        curveRadius = Random.Range(minCurveRadius, maxCurveRadius);
        curveSegmentCount = Random.Range(minCurveSegmentCout, maxCurveSegmentCount + 1);
        
        mesh.Clear();
        
        SetVertices();

        SetUV();

        SetTriangles();
        
        mesh.RecalculateNormals();

        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);

        if (withItems)
        {
            if (generators.Length > 0)
                generators[Random.Range(0, generators.Length)].GenerateItems(this);
        }
    }

    public void AlignWith(Pipe p)
    {
        relativeRotation = Random.Range(0, curveSegmentCount) *  360f / pipeSegmentCount;

        transform.SetParent(p.transform, false);

        transform.localPosition = Vector3.zero;

        transform.localRotation = Quaternion.Euler(0f, 0f, -p.curveAngle);

        transform.Translate(0f, p.curveRadius, 0f);

        transform.Rotate(relativeRotation, 0f, 0f);

        transform.Translate(0f, -curveRadius, 0f);

        transform.SetParent(p.transform.parent);

        transform.localScale = Vector3.one;
    }

    private void SetVertices()
    {
        vertices = new Vector3[pipeSegmentCount * curveSegmentCount * 4];

        float uStep = ringDistance / curveRadius;

        curveAngle = uStep * curveSegmentCount * (360f / (2f * Mathf.PI));

        CreateFirstQuadRing(uStep);

        int iDelta = pipeSegmentCount * 4;
        for (int u = 2, i = iDelta; u <= curveSegmentCount; u++, i += iDelta)
        {
            CreateQuadRing(u * uStep, i);
        }

        mesh.vertices = vertices;
    }

    private void SetUV()
    {
        uv = new Vector2[vertices.Length];

        for (int i = 0; i < vertices.Length; i += 4)
        {
            uv[i] = Vector2.zero;
            uv[i + 1] = Vector2.right;
            uv[i + 2] = Vector2.up;
            uv[i + 3] = Vector2.one;
        }

        mesh.uv = uv;
    }

    private void SetTriangles()
    {
        triangles = new int[pipeSegmentCount * curveSegmentCount * 6];

        for (int t = 0, i = 0; t < triangles.Length; t += 6, i += 4)
        {
            triangles[t] = i;
            triangles[t + 1] = triangles[t + 4] = i + 2;
            triangles[t + 2] = triangles[t + 3] = i + 1;
            triangles[t + 5] = i + 3;
        }

        mesh.triangles = triangles;
    }

    private void CreateFirstQuadRing(float u)
    {
        float vStep = pipeSegmentStep;

        Vector3 vertexA = GetPointOnTorus(0f, 0f);
        Vector3 vertexB = GetPointOnTorus(u, 0f);

        for (int v = 1, i = 0; v <= pipeSegmentCount; v++, i += 4)
        {
            vertices[i] = vertexA;
            vertices[i + 1] = vertexA = GetPointOnTorus(0f, v * vStep);
            vertices[i + 2] = vertexB;
            vertices[i + 3] = vertexB = GetPointOnTorus(u, v * vStep);
        }
    }

    private void CreateQuadRing(float u, int i)
    {
        float vStep = pipeSegmentStep;

        int ringOffset = pipeSegmentCount * 4;

        Vector3 vertex = GetPointOnTorus(u, 0f);
        for (int v = 1; v <= pipeSegmentCount; v++, i += 4)
        {
            vertices[i] = vertices[i - ringOffset + 2];
            vertices[i + 1] = vertices[i - ringOffset + 3];
            vertices[i + 2] = vertex;
            vertices[i + 3] = vertex = GetPointOnTorus(u, v * vStep);
        }
    }

    private Vector3 GetPointOnTorus(float u, float v)
    {
        Vector3 p;

        float r = curveRadius + pipeRadius * Mathf.Cos(v);

        p.x = r * Mathf.Sin(u);
        p.y = r * Mathf.Cos(u);
        p.z = pipeRadius * Mathf.Sin(v);

        return p;
    }

    #endregion
}
