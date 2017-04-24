using UnityEngine;
using System.Collections;

public class FPSCounter : MonoBehaviour 
{
    #region Properties

    public int frameRange = 60;

    public int averageFPS { get; private set; }

    public int highestFPS { get; private set; }
    public int lowestFPS { get; private set; }

    int[] fpsBuffer;
    int fpsBufferIndex;

    #endregion

    #region Unity Callbacks

    void Update()
    {
        if (fpsBuffer == null || fpsBuffer.Length != frameRange)
            InitializeBuffer();

        UpdateBuffer();

        CalculateFPS();
    }

    #endregion

    #region Methods

    void InitializeBuffer()
    {
        if (frameRange <= 0)
            frameRange = 1;

        fpsBuffer = new int[frameRange];
        fpsBufferIndex = 0;
    }

    void UpdateBuffer()
    {
        fpsBuffer[fpsBufferIndex++] = (int)(1f / Time.unscaledDeltaTime);
        if (fpsBufferIndex >= frameRange)
            fpsBufferIndex = 0;
    }

    void CalculateFPS()
    {
        int sum = 0;
        int highest = 0;
        int lowest = int.MaxValue;

        for (int i = 0; i < frameRange; i++)
        {
            int fps = fpsBuffer[i];
            sum += fps;

            if (fps > highest)
                highest = fps;

            if (fps < lowest)
                lowest = fps;
        }

        averageFPS = sum / frameRange;
        highestFPS = highest;
        lowestFPS = lowest;
    }

    #endregion
}
