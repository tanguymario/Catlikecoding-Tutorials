using UnityEngine;

public class RollABallCollectibleSpawner : MonoBehaviour 
{
    #region Properties

    public RollABallCollectible collectible;

    [SerializeField]
    private int nbCollectibles = 5;

    [SerializeField]
    private float radius = 2.5f;

    private Transform rotater;

    #endregion

    #region Unity Callbacks

	private void Awake()
    {
        rotater = transform.GetChild(0);

        SpawnInCircle();
	}
	
    #endregion

    #region Methods

    private void SpawnInCircle()
    {
        float step = (float)(360f / (nbCollectibles));

        for (int i = 0; i < nbCollectibles; i++)
        {
            Spawn(new Vector3(0f, 0.5f, radius));
            rotater.Rotate(new Vector3(0f, step, 0f));
        }
    }

    private void Spawn(Vector3 position)
    {
        Transform t = Instantiate<RollABallCollectible>(collectible).transform;

        t.name = "Collectible";
        t.SetParent(rotater);
        t.position = position;
    }

    #endregion
}
