[System.Serializable]
public struct MazeVector2
{
    #region Properties

    public int x;
    public int z;

    #endregion

    #region Constructors

    public MazeVector2(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    #endregion

    #region Other

    static public MazeVector2 operator + (MazeVector2 a, MazeVector2 b)
    {
        a.x += b.x;
        a.z += b.z;

        return a;
    }

    #endregion
}