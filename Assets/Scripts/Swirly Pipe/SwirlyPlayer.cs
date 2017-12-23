using UnityEngine;

public class SwirlyPlayer : MonoBehaviour 
{
    #region Properties

    public PipeSystem pipeSystem;

    public SwirlyMainMenu mainMenu;

    public SwirlyHUD hud;

    [Range(0, 100)]
    public float startVelocity;

    [Range(0, 360)]
    public float rotationVelocity;

    public float[] accelerations;

    private float acceleration;

    private float velocity;

    private Pipe currPipe;

    private float distanceTravelled;

    private float deltaToRotation;

    private float systemRotation;

    private Transform world, rotater;

    private float worldRotation, avatarRotation;

    private const float deathAcceleration = -5f;

    #endregion

    #region Unity Callbacks

	private void Awake() 
    {
        world = pipeSystem.transform.parent;

        rotater = transform.GetChild(0);

        gameObject.SetActive(false);
	}

    private void Update()
    {
        velocity += acceleration * Time.deltaTime;

        if (velocity < 0f)
            velocity = 0f;    

        float delta = velocity * Time.deltaTime;
        distanceTravelled += delta;

        systemRotation += delta * deltaToRotation;

        if (systemRotation >= currPipe.CurveAngle)
        {
            delta = (systemRotation - currPipe.CurveAngle) / deltaToRotation;

            currPipe = pipeSystem.SetupNextPipe();

            SetupCurrentPipe();

            systemRotation = delta * deltaToRotation;
        }

        pipeSystem.transform.localRotation = Quaternion.Euler(0f, 0f, systemRotation);

        UpdateAvatarRotation();

        hud.SetValues(distanceTravelled, velocity);
    }
	
    #endregion

    #region Methods

    public void StartGame(int accelerationMode)
    {
        distanceTravelled = 0f;

        avatarRotation = 0f;
        systemRotation = 0f;
        worldRotation = 0f;

        velocity = startVelocity;
        acceleration = accelerations[accelerationMode];

        currPipe = pipeSystem.SetupFirstPipe();

        rotater.localRotation = Quaternion.identity;

        SetupCurrentPipe();

        gameObject.SetActive(true);

        hud.SetValues(distanceTravelled, velocity);
    }

    public void Die()
    {
        mainMenu.EndGame(distanceTravelled);
        gameObject.SetActive(false);
    }

    public void SetDeathAcceleration()
    {
        acceleration = deathAcceleration;
    }

    private void UpdateAvatarRotation()
    {
        float rotationInput = 0f;
        if (Application.isMobilePlatform)
        {
            if (Input.touchCount == 1)
            {
                if (Input.GetTouch(0).position.x < Screen.width * 0.5f)
                    rotationInput = -1f;
                else
                    rotationInput = 1f;
            }
        }
        else
        {
            rotationInput = Input.GetAxis("Horizontal");
        }

        avatarRotation += rotationVelocity * Time.deltaTime * rotationInput;

        if (avatarRotation < 0f)
            avatarRotation += 360f;
        else if (avatarRotation >= 360f)
            avatarRotation -= 360f;

        if (avatarRotation != 0f)
            rotater.localRotation = Quaternion.Euler(avatarRotation, 0f, 0f);
    }

    private void SetupCurrentPipe()
    {
        deltaToRotation = 360f / (2f * Mathf.PI * currPipe.CurveRadius);

        worldRotation += currPipe.RelativeRotation;

        if (worldRotation < 0f)
            worldRotation += 360f;
        else if (worldRotation >= 360f)
            worldRotation -= 360f;

        world.localRotation = Quaternion.Euler(worldRotation, 0f, 0f);
    }

    #endregion
}
