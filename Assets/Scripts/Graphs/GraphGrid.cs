using UnityEngine;

[System.Serializable]
public struct GraphGrid
{
    public bool isOn;

    public bool useYAxisInFormulas;

    [RangeAttribute(10, 250)]
    public int resolution;

    public bool useFullColors;

    public Color startColorX;
    public Color startColorY;

    public Color endColorX;
    public Color endColorY;

    [RangeAttribute(0.01f, 1f)]
    public float startSizeX;

    [RangeAttribute(0.01f, 1f)]
    public float startSizeY;

    [RangeAttribute(0.01f, 1f)]
    public float endSizeX;

    [RangeAttribute(0.01f, 1f)]
    public float endSizeY;

    public FunctionOption function;

    public bool isAnimated;

    public GraphGrid(bool isOn, bool useYAxisInFormulas, bool useFullColors, int resolution, Color startColorX, Color startColorY, Color endColorX, Color endColorY, float startSizeX, float startSizeY, float endSizeX, float endSizeY, FunctionOption function, bool isAnimated)
    {
        this.isOn = isOn;
        this.useYAxisInFormulas = useYAxisInFormulas;
        this.useFullColors = useFullColors;
        this.resolution = resolution;
        this.startColorX = startColorX;
        this.startColorY = startColorY;
        this.endColorX = endColorX;
        this.endColorY = endColorY;
        this.startSizeX = startSizeX;
        this.startSizeY = startSizeY;
        this.endSizeX = endSizeX;
        this.endSizeY = endSizeY;
        this.function = function;
        this.isAnimated = isAnimated;
    }
}


