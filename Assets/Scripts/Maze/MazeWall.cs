using UnityEngine;

public class MazeWall : MazeCellEdge 
{
    #region Properties

    public Transform wall;

    #endregion

    #region Methods

    public override void Initialize (MazeCell cell, MazeCell otherCell, MazeDirection direction) 
    {
        base.Initialize(cell, otherCell, direction);

        wall.GetComponent<Renderer>().material = cell.room.settings.wallMaterial;
    }

    #endregion
}
