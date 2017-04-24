using UnityEngine;

[System.Serializable]
public struct GraphLine
{
    public bool isOn;

    [RangeAttribute(10, 500)]
    public int resolution;

    public Color startColor;
    public Color endColor;

    [RangeAttribute(0.01f, 1f)]
    public float startSize;

    [RangeAttribute(0.01f, 1f)]
    public float endSize;

    public FunctionOption function;

    public bool isAnimated;

    public GraphLine(bool isOn, int resolution, Color startColor, Color endColor, float startSize, float endSize, FunctionOption function, bool isAnimated)
    {
        this.isOn = isOn;
        this.resolution = resolution;
        this.startColor = startColor;
        this.endColor = endColor;
        this.startSize = startSize;
        this.endSize = endSize;
        this.function = function;
        this.isAnimated = isAnimated;
    }
}

    
