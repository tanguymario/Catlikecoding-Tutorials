using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureCreatorNoise : TextureCreator
{
    #region [Properties]

    [SerializeField]
    [Range(0.1f, 500f)]
    protected float m_frequency = 10f;

    [SerializeField]
    [Range(1, 3)]
    protected int m_dimensions = 3;

    [SerializeField]
    protected NoiseMethodType m_type;

    #endregion

    #region [Unity Callbacks]

    #endregion

    #region [Methods]

    protected override void BeforeFillTexture()
    {
        
    }

    protected override void AfterFillTexture()
    {

    }

    protected override Color GetTextureColorAt(Vector3 point)
    {
        NoiseMethod method = Noise.noiseMethods[(int)m_type][m_dimensions - 1];
        
        float sample = method(point, m_frequency);
        if (m_type != NoiseMethodType.Value) 
        {
            sample = sample * 0.5f + 0.5f;
        }

        return Color.white * sample;
    } 

    #endregion
}
