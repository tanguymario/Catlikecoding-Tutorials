using UnityEngine;

public class RunnerBooster : MonoBehaviour 
{
    #region Properties

    public Vector3 offset;
    public Vector3 rotationVelocity;

    public float recycleOffset;
    public float spawnChance;

    #endregion

    #region Unity Callbacks

	private void Start () 
    {
        EndlessRunnerGameManager.GameOver += GameOver;

        gameObject.SetActive(false);
	}
	
    private void Update()
    {
        if (transform.localPosition.x + recycleOffset < Runner.distanceTravelled)
        {
            gameObject.SetActive(false);
            return;
        }

        transform.Rotate(rotationVelocity * Time.deltaTime);
    }

    private void OnTriggerEnter()
    {
        Runner.AddBoost();
        gameObject.SetActive(false);
    }

    #endregion

    #region Methods

    public void SpawnIfAvailable(Vector3 position)
    {
        if (gameObject.activeSelf || spawnChance <= Random.Range(0f, 100f))
            return;

        transform.localPosition = position + offset;
        gameObject.SetActive(true);
    }

    private void GameOver()
    {
        gameObject.SetActive(false);
    }

    #endregion
}
