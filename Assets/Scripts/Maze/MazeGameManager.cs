using UnityEngine;
using System.Collections;

public class MazeGameManager : MonoBehaviour 
{
    #region Properties

    public Maze mazePrefab;

    public MazePlayer playerPrefab;

    private MazePlayer playerInstance;

    Maze mazeInstance;

    #endregion

    #region Unity Callbacks

	private void Start () 
    {
        StartCoroutine(BeginGame());
	}
	
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            RestartGame();
    }

    #endregion

    #region Methods

    private void RestartGame()
    {
        StopAllCoroutines();

        Destroy(mazeInstance.gameObject);

        if (playerInstance != null)
            Destroy(playerInstance.gameObject);

        StartCoroutine(BeginGame());
    }

    #endregion

    #region Coroutines

    private IEnumerator BeginGame()
    {
        Camera.main.clearFlags = CameraClearFlags.Skybox;
        Camera.main.rect = new Rect(0f, 0f, 1f, 1f);

        mazeInstance = Instantiate<Maze>(mazePrefab);

        yield return StartCoroutine(mazeInstance.Generate());

        playerInstance = Instantiate<MazePlayer>(playerPrefab);
        playerInstance.SetLocation(mazeInstance.GetCell(mazeInstance.RandomCoordinates));

        Camera.main.clearFlags = CameraClearFlags.Depth;
        Camera.main.rect = new Rect(0f, 0f, 0.35f, 0.35f);
    }

    #endregion
}
