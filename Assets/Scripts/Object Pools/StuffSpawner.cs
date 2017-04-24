using UnityEngine;

public class StuffSpawner : MonoBehaviour 
{
    #region Properties

    [System.Serializable]
    public struct FloatRange
    {
        public float min;
        public float max;

        public float RandomInRange
        {
            get { return Random.Range(min, max); }
        }
    }

    public FloatRange timeBetweenSpawns;

    public FloatRange scale;

    public FloatRange randomVelocity;

    public FloatRange randomAngularVelocity;

    public float velocity;

    public bool decreaseTimeBetweenSpawns;

    public Stuff[] stuffPrefabs;

    public Material stuffMaterial;

    float currentSpawnDelay;

    float timeSinceLastSpawn;

    #endregion

    #region Unity Callbacks

    void FixedUpdate () 
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= currentSpawnDelay)
        {
            timeSinceLastSpawn -= currentSpawnDelay;

            currentSpawnDelay = timeBetweenSpawns.RandomInRange;

            if (decreaseTimeBetweenSpawns)
            {
                timeBetweenSpawns.min *= 0.99f;
                timeBetweenSpawns.max *= 0.99f;
            }

            spawnStuff();
        }
    }

    #endregion

    #region Methods

    void spawnStuff()
    {
        Stuff prefab = stuffPrefabs[Random.Range(0, stuffPrefabs.Length)];
        Stuff spawn = prefab.GetPooledInstance<Stuff>();

        spawn.transform.SetParent(transform);
        spawn.transform.localPosition = transform.localPosition;
        spawn.transform.localScale = Vector3.one * scale.RandomInRange;
        spawn.transform.localRotation = Random.rotation;
        spawn.body.velocity = transform.up * velocity + Random.onUnitSphere * randomVelocity.RandomInRange;
        spawn.body.angularVelocity = Random.onUnitSphere * randomAngularVelocity.RandomInRange;
        spawn.SetMaterial(stuffMaterial);
    }

    #endregion
}
