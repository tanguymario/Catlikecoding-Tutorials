using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Stuff : PooledObject 
{
    #region Properties

    [System.NonSerialized]
    ObjectPool poolInstanceForPrefab;

    public Rigidbody body { get; private set; }

    MeshRenderer[] meshRenderers;

    const string killZoneTag = "Kill Zone";

    #endregion

    #region Unity Callbacks

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
    }

    void OnTriggerEnter(Collider enteredCollider)
    {
        if (enteredCollider.CompareTag(killZoneTag))
            ReturnToPool();
    }

    #endregion

    #region Methods

    public void SetMaterial(Material m)
    {
        foreach (MeshRenderer meshRend in meshRenderers)
            meshRend.material = m;
    }

    public T GetPooledInstance<T> () where T : PooledObject 
    {
        if (!poolInstanceForPrefab)
            poolInstanceForPrefab = ObjectPool.GetPool(this);

        return (T)poolInstanceForPrefab.GetObject();
    }

    #endregion
}
