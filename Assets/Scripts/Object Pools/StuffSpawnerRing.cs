using UnityEngine;

public class StuffSpawnerRing : MonoBehaviour 
{
    #region Properties

    public int numberOfSpawners;

    public float radius;
    public float tiltAngle;

    public Material[] stuffMaterials;

    public StuffSpawner spawnerPrefab;

    #endregion

    #region Unity Callbacks

    void Awake () 
    {
        for (int i = 0; i < numberOfSpawners; i++)
            CreateSpawner(i);
    }

    #endregion

    #region Methods

    void CreateSpawner(int index)
    {
        Transform rotater = new GameObject("Rotater").transform;
        rotater.SetParent(transform, false);
        rotater.localRotation = Quaternion.Euler(0f, index * 360f / numberOfSpawners, 0f);

        StuffSpawner spawner = Instantiate<StuffSpawner>(spawnerPrefab);
        spawner.transform.SetParent(rotater, false);
        spawner.transform.localPosition = new Vector3(0f, 0f, radius);
        spawner.transform.localRotation = Quaternion.Euler(tiltAngle, 0f, 0f);
        spawner.stuffMaterial = stuffMaterials[index % stuffMaterials.Length];
    }

    #endregion
}
