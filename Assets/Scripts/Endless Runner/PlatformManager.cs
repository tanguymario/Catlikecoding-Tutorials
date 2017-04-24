using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct PlatformType
{
    public Material mat;

    public PhysicMaterial physicMat;
}

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Collider))]
public class PlatformManager : MonoBehaviour 
{
    #region Properties

    public Transform prefab;

    public int nbObjects;

    public float recycleOffset;

    public Vector3 startPosition;

    public Vector3 minSize;
    public Vector3 maxSize;

    public Vector3 minGap;
    public Vector3 maxGap;

    public float minY;
    public float maxY;

    public RunnerBooster runnerBooster;

    public PlatformType[] types;

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

    private void Update () 
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
        runnerBooster.SpawnIfAvailable(position);

        Transform t = objectQueue.Dequeue();
        t.localScale = scale;
        t.localPosition = position;

        int typeIndex = Random.Range(0, types.Length);
        t.GetComponent<Renderer>().material = types[typeIndex].mat;
        t.GetComponent<Collider>().material = types[typeIndex].physicMat;

        objectQueue.Enqueue(t);

        nextPosition += new Vector3(
            Random.Range(minGap.x, maxGap.x) + scale.x,
            Random.Range(minGap.y, maxGap.y),
            Random.Range(minGap.z, maxGap.z)
        );

        if (nextPosition.y < minY)
            nextPosition.y = minY + maxGap.y;
        else if (nextPosition.y > maxY)
            nextPosition.y = maxY - maxGap.y;
    }

    #endregion
}
