using UnityEngine;
using System.Collections.Generic;

public class SkylineManager : MonoBehaviour 
{
    #region Properties

    public Transform prefab;

    public int nbObjects;

    public float recycleOffset;

    public Vector3 startPosition;

    public Vector3 minSize;
    public Vector3 maxSize;

    private Vector3 nextPosition;

    private Queue<Transform> objectQueue;

    #endregion

    #region Unity Callbacks

	private void Start () 
    {
        EndlessRunnerGameManager.GameStart += GameStart;
        EndlessRunnerGameManager.GameOver += GameOver;

        objectQueue = new Queue<Transform>(nbObjects);
        for (int i = 0; i < nbObjects; i++)
            objectQueue.Enqueue(Instantiate<Transform>(prefab));

        enabled = false;
	}
	
	void Update () 
    {
        if (objectQueue.Peek().localPosition.x + recycleOffset < Runner.distanceTravelled)
            Recycle();
	}

    #endregion

    #region Methods

    private void GameStart()
    {
        nextPosition = startPosition;
        for (int i = 0; i < nbObjects; i++)
            Recycle();

        enabled = true;
    }

    private void GameOver()
    {
        enabled = false;
    }

    private void Recycle()
    {
        Vector3 scale = new Vector3(
            Random.Range(minSize.x, maxSize.x),
            Random.Range(minSize.y, maxSize.y),
            Random.Range(minSize.z, maxSize.z)
        );

        Vector3 position = nextPosition;
        position.x += scale.x * 0.5f;
        position.y += scale.y * 0.5f;

        Transform t = objectQueue.Dequeue();
        t.localScale = scale;
        t.localPosition = position;
        nextPosition.x += scale.x;
        objectQueue.Enqueue(t);
    }

    #endregion
}
