using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Runner : MonoBehaviour 
{
    #region Properties

    static public float distanceTravelled;

    public float acceleration;

    public Vector3 jumpVelocity;

    public Vector3 boostVelocity;

    public float gameOverY;

    private Rigidbody body;

    private Vector3 startPosition;

    private bool isTouchingPlatform;

    static private int boosts;

    #endregion

    #region Unity Callbacks

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        body.isKinematic = true;

        EndlessRunnerGameManager.GameStart += GameStart;
        EndlessRunnerGameManager.GameOver += GameOver;

        startPosition = transform.localPosition;

        GetComponent<Renderer>().enabled = false;

        enabled = false;
    }

    private void Update() 
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (isTouchingPlatform)
            {
                body.AddForce(jumpVelocity, ForceMode.VelocityChange);
                isTouchingPlatform = false;
            }
            else if (boosts > 0)
            {
                body.AddForce(boostVelocity, ForceMode.VelocityChange);
                boosts--;

                EndlessRunnerUIManager.SetBoosts(boosts);
            }
        }

        distanceTravelled = transform.localPosition.x;

        EndlessRunnerUIManager.SetDistance(distanceTravelled);

        if (transform.localPosition.y < gameOverY)
            EndlessRunnerGameManager.TriggerGameOver();
	}

    private void FixedUpdate()
    {
        if (isTouchingPlatform)
            body.AddForce(acceleration, 0f, 0f, ForceMode.Acceleration);
    }

    private void OnCollisionEnter()
    {
        isTouchingPlatform = true;
    }

    private void OnCollisionExit()
    {
        isTouchingPlatform = false;
    }

    #endregion

    #region Methods

    static public void AddBoost()
    {
        boosts++;
        EndlessRunnerUIManager.SetBoosts(boosts);
    }

    private void GameStart()
    {
        boosts = 0;

        EndlessRunnerUIManager.SetBoosts(boosts);

        distanceTravelled = 0f;

        EndlessRunnerUIManager.SetDistance(distanceTravelled);

        transform.localPosition = startPosition;
        GetComponent<Renderer>().enabled = true;
        body.isKinematic = false;

        enabled = true;
    }

    private void GameOver()
    {
        GetComponent<Renderer>().enabled = false;
        body.isKinematic = true;

        enabled = false;
    }

    #endregion
}
