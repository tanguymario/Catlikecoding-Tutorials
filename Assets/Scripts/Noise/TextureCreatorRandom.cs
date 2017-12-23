using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureCreatorRandom : TextureCreator
{
    #region [Properties]

    [SerializeField]
    protected bool m_chooseSeed = true;

    [SerializeField]
    [Range(1, 100)]
    protected int m_randomSeed = 42;

    #endregion

    #region [Unity Callbacks]

    #endregion

    #region [Methods]

    protected override void BeforeFillTexture()
    {
        if (m_chooseSeed)
        {
            Random.seed = m_randomSeed;
        }
    }

    protected override void AfterFillTexture()
    {

    }

    protected override Color GetTextureColorAt(Vector3 point)
    {
        return Color.white * Random.value;
    } 

    #endregion
}
