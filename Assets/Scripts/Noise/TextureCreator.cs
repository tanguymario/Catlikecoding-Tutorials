using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureCreator : MonoBehaviour 
{
    #region [Properties]

    [SerializeField]
    [Range(2, 512)]
    protected int m_resolution = 256;

    [SerializeField]
    private FilterMode m_filterMode;

    [SerializeField]
    private TextureWrapMode m_textureWrapMode;

    [SerializeField]
    private bool m_mipmap;

    [SerializeField]
    [Range(1, 9)]
    private int m_anisoLevel = 9;

    protected Texture2D m_texture;

    #endregion

    #region [Unity Callbacks]

    protected void OnValidate()
    {
        Generate();
    }

    protected void Update()
    {
        if (transform.hasChanged) 
        {
            transform.hasChanged = false;
            FillTexture();
        }
    }

    #endregion

    #region [Methods]

    protected void Generate()
    {
        // Mipmap produces better results when zoomed out
        m_texture = new Texture2D(
            m_resolution, m_resolution, TextureFormat.RGB24, m_mipmap);

        m_texture.name = "Procedural Texture";

        // Avoid the texture to repeat itself (default mode is repeat)
        m_texture.wrapMode = m_textureWrapMode;

        // Avoid the texture to have a smooth color transition 
        // (default is bilinear filter mode)
        m_texture.filterMode = m_filterMode;

        // If low, texture gets fuzzy quick when viewed at an angle
        m_texture.anisoLevel = m_anisoLevel;

        GetComponent<MeshRenderer>().material.mainTexture = m_texture;

        FillTexture();
    }

    public void FillTexture()
    {
        if (m_texture.width != m_resolution || m_texture.height != m_resolution) 
        {
            m_texture.Resize(m_resolution, m_resolution);
        }

        // Get the local coords of our quad
        Vector3 point00 = transform.TransformPoint(new Vector3(-0.5f,-0.5f));
        Vector3 point10 = transform.TransformPoint(new Vector3( 0.5f,-0.5f));
        Vector3 point01 = transform.TransformPoint(new Vector3(-0.5f, 0.5f));
        Vector3 point11 = transform.TransformPoint(new Vector3( 0.5f, 0.5f));

        BeforeFillTexture();

        float stepSize = 1f / m_resolution;
        for (int y = 0; y < m_resolution; y++) 
        {
            Vector3 point0 = 
                Vector3.Lerp(point00, point01, (y + 0.5f) * stepSize);
            
            Vector3 point1 = 
                Vector3.Lerp(point10, point11, (y + 0.5f) * stepSize);
            
            for (int x = 0; x < m_resolution; x++) 
            {
                Vector3 point = 
                    Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize);
                
                Color c = GetTextureColorAt(point);
                m_texture.SetPixel(x, y, c);
            }
        }

        AfterFillTexture();

        m_texture.Apply();
    }

    protected virtual void BeforeFillTexture()
    {

    }

    protected virtual void AfterFillTexture()
    {

    }

    protected virtual Color GetTextureColorAt(Vector3 point)
    {
        return new Color(point.x, point.y, point.z);
    }

    #endregion
}
