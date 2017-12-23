using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TextureCreator))]
public class TextureCreatorInspector : Editor 
{
    #region [Properties]

    private TextureCreator m_textureCreator;

    #endregion

    #region [Unity Callbacks]

    private void OnEnable()
    {
        m_textureCreator = target as TextureCreator;
        Undo.undoRedoPerformed += RefreshCreator;
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= RefreshCreator;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();

        if (EditorGUI.EndChangeCheck() && Application.isPlaying)
        {
            m_textureCreator.FillTexture();
        }
    }

    #endregion

    #region [Methods]

    private void RefreshCreator()
    {
        if (Application.isPlaying)
        {
            m_textureCreator.FillTexture();
        }
    }

    #endregion
}
