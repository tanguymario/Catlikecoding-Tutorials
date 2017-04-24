using UnityEngine;
using System.Collections.Generic;

public class MazeRoom : ScriptableObject 
{
    #region Properties

    public int settingsIndex;

    public MazeRoomSettings settings;

    List<MazeCell> cells = new List<MazeCell>();

    #endregion

    #region Methods

    public void Add(MazeCell cell)
    {
        cell.room = this;
        cells.Add(cell);
    }

    public void Assimilate (MazeRoom room)
    {
        for (int i = 0; i < room.cells.Count; i++)
            Add(room.cells[i]);
    }

    public void show()
    {
        for (int i = 0; i < cells.Count; i++)
            cells[i].show();
    }

    public void hide()
    {
        for (int i = 0; i < cells.Count; i++)
            cells[i].hide();
    }

    #endregion
}
