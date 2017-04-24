using UnityEngine;

public class MazeCellEdge : MonoBehaviour 
{
    #region Properties

    public MazeCell cell;
    public MazeCell otherCell;

    public MazeDirection direction;

    #endregion

    #region Methods

    public virtual void Initialize(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        this.cell = cell;
        this.otherCell = otherCell;
        this.direction = direction;

        cell.SetEdge(direction, this);
        transform.parent = cell.transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = direction.ToRotation();
    }

    public virtual void OnPlayerEntered() { }

    public virtual void OnPlayerExited() { }

    #endregion
}
