using UnityEngine;

public class PooledObject : MonoBehaviour 
{
    #region Properties

    public ObjectPool Pool { get; set; }

    #endregion

    #region Methods

    public void ReturnToPool()
    {
        if (Pool)
            Pool.AddObject(this);
        else
            Destroy(gameObject);
    }

    #endregion
}
