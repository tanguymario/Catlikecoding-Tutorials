using UnityEngine;

public class MazeCell : MonoBehaviour 
{
    #region Properties

    public MazeVector2 coordinates;

    public MazeRoom room;

    public bool isFullyInitialized
    {
        get { return initializedEdgesCount == MazeDirections.Count; }
    }

    public MazeDirection RandomUnitializedDirection
    {
        get 
        {
            int skips = Random.Range(0, MazeDirections.Count - initializedEdgesCount);
            for (int i = 0; i < MazeDirections.Count; i++)
            {
                if (edges[i] == null)
                {
                    if (skips == 0)
                        return (MazeDirection)i;

                    skips -= 1;
                }
            }

            throw new System.InvalidOperationException("MazeCell has no uninitialized directions left.");
        }
    }

    int initializedEdgesCount;

    MazeCellEdge[] edges = new MazeCellEdge[MazeDirections.Count];

    #endregion

    #region Methods

    public void Initialize(MazeRoom room)
    {
        room.Add(this);

        transform.GetChild(0).GetComponent<Renderer>().material = room.settings.floorMaterial;
    }

    public MazeCellEdge GetEdge (MazeDirection direction) 
    {
        return edges[(int)direction];
    }

    public void SetEdge (MazeDirection direction, MazeCellEdge edge) 
    {
        edges[(int)direction] = edge;
        initializedEdgesCount++;
    }

    public void OnPlayerEntered()
    {
        room.show();

        for (int i = 0; i < edges.Length; i++)
            edges[i].OnPlayerEntered();
    }

    public void OnPlayerExited()
    {
        room.hide();

        for (int i = 0; i < edges.Length; i++)
            edges[i].OnPlayerExited();
    }

    public void show()
    {
        gameObject.SetActive(true);
    }

    public void hide()
    {
        gameObject.SetActive(false);
    }

    #endregion
}
