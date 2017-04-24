using UnityEngine;
using System.Collections.Generic;

public enum MazeDirection
{
    North,
    East,
    South, 
    West
}

static public class MazeDirections 
{
    #region Properties

    static public readonly int Count = System.Enum.GetValues(typeof(MazeDirection)).Length;

    static Dictionary<MazeDirection, MazeVector2> vectors = new Dictionary<MazeDirection, MazeVector2>()
    {
        {MazeDirection.North, new MazeVector2(0, 1)},
        {MazeDirection.East, new MazeVector2(1, 0)},
        {MazeDirection.South, new MazeVector2(0, -1)},
        {MazeDirection.West, new MazeVector2(-1, 0)}
    };

    static Dictionary<MazeDirection, MazeDirection> opposites = new Dictionary<MazeDirection, MazeDirection>()
    {
        {MazeDirection.North, MazeDirection.South},
        {MazeDirection.East, MazeDirection.West},
        {MazeDirection.South, MazeDirection.North},
        {MazeDirection.West, MazeDirection.East}
    };

    static Dictionary<MazeDirection, Quaternion> rotations = new Dictionary<MazeDirection, Quaternion>()
    {
        {MazeDirection.North, Quaternion.identity},
        {MazeDirection.East, Quaternion.Euler(0f, 90f, 0f)},
        {MazeDirection.South, Quaternion.Euler(0f, 180f, 0f)},
        {MazeDirection.West, Quaternion.Euler(0f, 270f, 0f)}
    };

    #endregion

    #region Methods

    static public MazeDirection RandomValue 
    {
        get { return (MazeDirection)Random.Range(0, System.Enum.GetValues(typeof(MazeDirection)).Length); }
    }

    static public MazeVector2 ToMazeVector2 (this MazeDirection mazeDirection)
    {
        return vectors[mazeDirection];
    }

    static public MazeDirection GetOpposite(this MazeDirection direction)
    {
        return opposites[direction];
    }

    static public Quaternion ToRotation(this MazeDirection direction)
    {
        return rotations[direction];
    }

    static public MazeDirection GetNextClockWise(this MazeDirection direction)
    {
        return (MazeDirection)(((int)direction + 1) % Count);
    }

    static public MazeDirection GetNextCounterClockwise(this MazeDirection direction)
    {
        return (MazeDirection)(((int)direction + Count - 1) % Count);
    }

    #endregion
}