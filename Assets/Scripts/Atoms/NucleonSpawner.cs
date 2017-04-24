using UnityEngine;
using System.Collections;

public class NucleonSpawner : MonoBehaviour 
{
    #region Properties

    public float timeBetweenSpawns;

    public float spawnDistance;

    public bool decreaseTimeBetweenSpawns;

    public Nucleon[] nucleonPrefabs;

    float timeSinceLastSpawn;

    #endregion

    #region Unity Callbcaks

	private void FixedUpdate () 
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= timeBetweenSpawns)
        {
            timeSinceLastSpawn -= timeBetweenSpawns;

            if (decreaseTimeBetweenSpawns)
                timeBetweenSpawns *= 0.99f;

            spawnNucleons();
        }
	}

    #endregion

    #region Methods

    void spawnNucleons()
    {
        Nucleon prefab = nucleonPrefabs[Random.Range(0, nucleonPrefabs.Length)];
        Nucleon spawn = Instantiate<Nucleon>(prefab);

        spawn.transform.SetParent(transform);
        spawn.transform.localPosition = Random.onUnitSphere * spawnDistance;
    }

    #endregion
}
