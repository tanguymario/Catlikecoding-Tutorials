using UnityEngine;
using System.Collections;

public enum FunctionOption
{
    Linear,
    Exponantial,
    Parabola,
    Sine,
    Cosine,
    Ripple
}

static public class FunctionOptions 
{
    static public float t
    {
        get { return Time.timeSinceLevelLoad; }
    }

    static public float GetFuncValue(this FunctionOption func, Vector3 p, bool isAnimated, bool useYAxis, bool useAllAxis)
    {
        switch (func)
        {
            case FunctionOption.Linear:
                return Linear(p, isAnimated, useAllAxis);
            case FunctionOption.Exponantial:
                return Exponantial(p, isAnimated, useAllAxis);
            case FunctionOption.Parabola:
                return Parabola(p, isAnimated, useYAxis, useAllAxis);
            case FunctionOption.Sine:
                return Sine(p, isAnimated, useYAxis, useAllAxis);
            case FunctionOption.Cosine:
                return Cosine(p, isAnimated, useYAxis, useAllAxis);
            case FunctionOption.Ripple:
                return Ripple(p, isAnimated, useAllAxis);
            default:
                return 0f;
        }
    }

    static private float Linear(Vector3 p, bool isAnimated, bool useAllAxis)
    {
        if (!useAllAxis)
        {
            return p.x;
        }
        else
        {
            if (!isAnimated)
                return 1f - p.x - p.y - p.z;
            else
                return 1f - p.x - p.y - p.z + 0.5f * Mathf.Sin(t);
        }
    }

    static private float Exponantial(Vector3 p, bool isAnimated, bool useAllAxis)
    {
        if (!useAllAxis)
        {
            return p.x * p.x;
        }
        else
        {
            if (!isAnimated)
                return 1f - p.x * p.x - p.y * p.y - p.z * p.z;
            else 
                return 1f - p.x * p.x - p.y * p.y - p.z * p.z + 0.5f * Mathf.Sin(t);
        }
    }

    static private float Parabola(Vector3 p, bool isAnimated, bool useYAxis, bool useAllAxis)
    {
        if (!useYAxis)
        {
            if (!useAllAxis)
            {
                p.x = 2f * p.x - 1f;
                return p.x * p.x;
            }
            else
            {
                if (!isAnimated)
                {
                    p.x += p.x - 1f;
                    p.y += p.y - 1f;

                    return 1f - p.x * p.x - p.y * p.y;
                }
                else
                {
                    p.x += p.x - 1f;
                    p.y += p.y - 1f;

                    return 1f - p.x * p.x - p.y * p.y + 0.5f * Mathf.Sin(t);
                }
            }
        }
        else
        {
            p.x += p.x - 1f;
            p.y += p.y - 1f;

            return 1f - p.x * p.x * p.y * p.y;
        }
    }

    static private float Sine(Vector3 p, bool isAnimated, bool useYAxis, bool useAllAxis)
    {
        if (!useYAxis)
        {
            if (!useAllAxis)
            {
                if (!isAnimated)
                    return 0.5f + 0.5f * Mathf.Sin(2 * Mathf.PI * p.x);
                else
                    return 0.5f + 0.5f * Mathf.Sin(2 * Mathf.PI * p.x + Time.timeSinceLevelLoad); 
            }
            else
            {
                if (!isAnimated)
                {
                    float x = Mathf.Sin(2 * Mathf.PI * p.x);
                    float y = Mathf.Sin(2 * Mathf.PI * p.y);
                    float z = Mathf.Sin(2 * Mathf.PI * p.z);

                    return x * x * y * y * z * z;
                }
                else
                {
                    float x = Mathf.Sin(2 * Mathf.PI * p.x);
                    float y = Mathf.Sin(2 * Mathf.PI * p.y);
                    float z = Mathf.Sin(2 * Mathf.PI * p.z + (p.y > 0.5f ? t : -t));

                    return x * x * y * y * z * z;
                }
            }
        }
        else
        {
            if (!isAnimated)
            {
                return 0.50f +
                    0.25f * Mathf.Sin(4f * Mathf.PI * p.x) * Mathf.Sin(2f * Mathf.PI * p.y) +
                    0.10f * Mathf.Cos(3f * Mathf.PI * p.x) * Mathf.Cos(5f * Mathf.PI * p.y) +
                    0.15f * Mathf.Sin(Mathf.PI * p.x);
            }
            else
            {
                return 0.50f +
                    0.25f * Mathf.Sin(4f * Mathf.PI * p.x + 4f * t) * Mathf.Sin(2f * Mathf.PI * p.y + t) +
                    0.10f * Mathf.Cos(3f * Mathf.PI * p.x + 5f * t) * Mathf.Cos(5f * Mathf.PI * p.y + 3f * t) +
                    0.15f * Mathf.Sin(Mathf.PI * p.x + 0.6f * t);
            }
        }
    }

    static private float Cosine(Vector3 p, bool isAnimated, bool useYAxis, bool useAllAxis)
    {
        if (!useYAxis)
        {
            if (!useAllAxis)
            {
                if (!isAnimated)
                    return 0.5f + 0.5f * Mathf.Cos(2 * Mathf.PI * p.x);
                else
                    return 0.5f + 0.5f * Mathf.Cos(2 * Mathf.PI * p.x + Time.timeSinceLevelLoad);
            }
            else
            {
                if (!isAnimated)
                {
                    float x = Mathf.Cos(2 * Mathf.PI * p.x);
                    float y = Mathf.Cos(2 * Mathf.PI * p.y);
                    float z = Mathf.Cos(2 * Mathf.PI * p.z);

                    return x * x * y * y * z * z;
                }
                else
                {
                    float x = Mathf.Cos(2 * Mathf.PI * p.x);
                    float y = Mathf.Cos(2 * Mathf.PI * p.y);
                    float z = Mathf.Cos(2 * Mathf.PI * p.z + (p.y > 0.5f ? t : -t));

                    return x * x * y * y * z * z;
                }
            }
        }
        else
        {
            if (!isAnimated)
            {
                return 0.50f +
                    0.25f * Mathf.Cos(4f * Mathf.PI * p.x) * Mathf.Sin(2f * Mathf.PI * p.y) +
                    0.10f * Mathf.Sin(3f * Mathf.PI * p.x) * Mathf.Cos(5f * Mathf.PI * p.y) +
                    0.15f * Mathf.Cos(Mathf.PI * p.x);
            }
            else
            {
                return 0.50f +
                    0.25f * Mathf.Cos(4f * Mathf.PI * p.x + 4f * t) * Mathf.Sin(2f * Mathf.PI * p.y + t) +
                    0.10f * Mathf.Sin(3f * Mathf.PI * p.x + 5f * t) * Mathf.Cos(5f * Mathf.PI * p.y + 3f * t) +
                    0.15f * Mathf.Cos(Mathf.PI * p.x + 0.6f * t);
            }
        }
    }

    static private float Ripple(Vector3 p, bool isAnimated, bool useAllAxis)
    {
        if (!useAllAxis)
        {
            if (!isAnimated)
            {
                p.x -= 0.5f;
                p.y -= 0.5f;

                float squareRadius = p.x * p.x + p.y * p.y;

                return 0.5f + Mathf.Sin(15f * Mathf.PI * squareRadius) / (2f + 100f * squareRadius);
            }
            else
            {
                p.x -= 0.5f;
                p.y -= 0.5f;

                float squareRadius = p.x * p.x + p.y * p.y;

                return 0.5f + Mathf.Sin(15f * Mathf.PI * squareRadius - 2f * t) / (2f + 100f * squareRadius);
            }
        }
        else
        {
            if (!isAnimated)
            {
                p.x -= 0.5f;
                p.y -= 0.5f;
                p.z -= 0.5f;

                float squareRadius = p.x * p.x + p.y * p.y + p.z * p.z;

                return Mathf.Sin(4f * Mathf.PI * squareRadius);
            }
            else
            {
                p.x -= 0.5f;
                p.y -= 0.5f;
                p.z -= 0.5f;

                float squareRadius = p.x * p.x + p.y * p.y + p.z * p.z;

                return Mathf.Sin(4f * Mathf.PI * squareRadius - 2f * t);
            }
        }
    }
}
