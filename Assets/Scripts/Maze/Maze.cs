using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Maze : MonoBehaviour 
{
    #region Properties

    public MazeVector2 size;

    public MazeCell cellPrefab;

    public float generationStepDelay;

    public MazePassage passagePrefab;

    public MazeWall[] wallPrefabs;

    public MazeDoor doorPrefab;

    [Range(0f, 1f)]
    public float doorProbability;

    public MazeRoomSettings[] roomSettings;

    public MazeVector2 RandomCoordinates 
    {
        get { return new MazeVector2(Random.Range(0, size.x), Random.Range(0, size.z)); }
    }

    List<MazeRoom> rooms = new List<MazeRoom>();

    MazeCell[,] cells;

    #endregion

    #region Methods

    MazeCell CreateCell(MazeVector2 coordinates)
    {
        MazeCell newCell = Instantiate<MazeCell>(cellPrefab);
        newCell.name = string.Concat("Maze Cell ", coordinates.x.ToString(), ", ", coordinates.z.ToString());
        newCell.coordinates = coordinates;
        newCell.transform.SetParent(transform);
        newCell.transform.localPosition = new Vector3(coordinates.x - size.x * 0.5f + 0.5f, 0, coordinates.z - size.z * 0.5f + 0.5f);

        cells[coordinates.x, coordinates.z] = newCell;

        return newCell;
    }

    MazeRoom CreateRoom(int indexToExclude)
    {
        MazeRoom newRoom = ScriptableObject.CreateInstance<MazeRoom>();
        newRoom.settingsIndex = Random.Range(0, roomSettings.Length);
        if (newRoom.settingsIndex == indexToExclude)
            newRoom.settingsIndex = (newRoom.settingsIndex + 1) % roomSettings.Length;

        newRoom.settings = roomSettings[newRoom.settingsIndex];

        rooms.Add(newRoom);

        return newRoom;
    }

    public bool ContainsCoordinates (MazeVector2 coordinate) 
    {
        return coordinate.x >= 0 && coordinate.x < size.x && coordinate.z >= 0 && coordinate.z < size.z;
    }

    public MazeCell GetCell(MazeVector2 coordinates)
    {
        return cells[coordinates.x, coordinates.z];
    }

    void DoFirstGenerationStep (List<MazeCell> activeCells) 
    {
        MazeCell newCell = CreateCell(RandomCoordinates);
        newCell.Initialize(CreateRoom(-1));

        activeCells.Add(newCell);
    }

    void DoNextGenerationStep (List<MazeCell> activeCells) 
    {
        int currIndex = activeCells.Count - 1;

        MazeCell currCell = activeCells[currIndex];

        if (currCell.isFullyInitialized)
        {
            activeCells.RemoveAt(currIndex);
            return;
        }

        MazeDirection direction = currCell.RandomUnitializedDirection;

        MazeVector2 coordinates = currCell.coordinates + direction.ToMazeVector2();

        if (ContainsCoordinates(coordinates))
        {
            MazeCell neighbor = GetCell(coordinates);
            if (neighbor == null)
            {
                neighbor = CreateCell(coordinates);
                CreatePassage(currCell, neighbor, direction);

                activeCells.Add(neighbor);
            }
            else if (currCell.room.settingsIndex == neighbor.room.settingsIndex) 
            {
                CreatePassageInSameRoom(currCell, neighbor, direction);
            }
            else
            {
                CreateWall(currCell, neighbor, direction);
            }
        }
        else
        {
            CreateWall(currCell, null, direction);
        }
    }

    void CreatePassageInSameRoom (MazeCell cell, MazeCell otherCell, MazeDirection direction) 
    {
        MazePassage passage = Instantiate<MazePassage>(passagePrefab);
        passage.Initialize(cell, otherCell, direction);

        passage = Instantiate(passagePrefab) as MazePassage;
        passage.Initialize(otherCell, cell, direction.GetOpposite());

        if (cell.room != otherCell.room) 
        {
            MazeRoom roomToAssimilate = otherCell.room;
            cell.room.Assimilate(roomToAssimilate);
            rooms.Remove(roomToAssimilate);
            Destroy(roomToAssimilate);
        }
    }

    void CreatePassage (MazeCell cell, MazeCell otherCell, MazeDirection direction) 
    {
        MazePassage prefab = Random.value < doorProbability ? doorPrefab : passagePrefab;

        MazePassage passage = Instantiate<MazePassage>(prefab);
        passage.Initialize(cell, otherCell, direction);

        passage = Instantiate<MazePassage>(prefab);

        if (passage is MazeDoor)
            otherCell.Initialize(CreateRoom(cell.room.settingsIndex));
        else
            otherCell.Initialize(cell.room);

        passage.Initialize(otherCell, cell, direction.GetOpposite());
    }

    void CreateWall (MazeCell cell, MazeCell otherCell, MazeDirection direction) 
    {
        MazeWall wall = Instantiate<MazeWall>(wallPrefabs[Random.Range(0, wallPrefabs.Length)]);
        wall.Initialize(cell, otherCell, direction);

        if (otherCell != null) 
        {
            wall = Instantiate<MazeWall>(wallPrefabs[Random.Range(0, wallPrefabs.Length)]) as MazeWall;
            wall.Initialize(otherCell, cell, direction.GetOpposite());
        }
    }

    #endregion

    #region Coroutines

    public IEnumerator Generate()
    {
        WaitForSeconds delay = new WaitForSeconds(generationStepDelay);

        cells = new MazeCell[size.x, size.z];

        List<MazeCell> activeCells = new List<MazeCell>();
        DoFirstGenerationStep(activeCells);

        while (activeCells.Count > 0)
        {
            yield return delay;

            DoNextGenerationStep(activeCells);
        }

        for (int i = 0; i < rooms.Count; i++)
            rooms[i].hide();

        yield return null;
    }

    #endregion
}
