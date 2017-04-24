using UnityEngine;

public class MazePlayer : MonoBehaviour 
{
    #region Properties

    private MazeCell currCell;

    private MazeDirection currDirection;

    #endregion
	
    #region Unity Callbacks

	private void Update () 
    {
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            Move(currDirection);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(currDirection.GetNextClockWise());
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            Move(currDirection.GetOpposite());
        }
        else if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(currDirection.GetNextCounterClockwise());
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Look(currDirection.GetNextCounterClockwise());
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Look(currDirection.GetNextClockWise());
        }
	}

    #endregion

    #region Methods

    public void SetLocation(MazeCell cell)
    {
        if (currCell != null)
            currCell.OnPlayerExited();

        currCell = cell;
        transform.localPosition = cell.transform.localPosition;

        currCell.OnPlayerEntered();
    }

    private void Move(MazeDirection direction)
    {
        MazeCellEdge edge = currCell.GetEdge(direction);
        if (edge is MazePassage)
            SetLocation(edge.otherCell);
    }

    private void Look(MazeDirection direction)
    {
        transform.localRotation = direction.ToRotation();
        currDirection = direction;
    }

    #endregion
}
