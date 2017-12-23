using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class SwirlyPlayerAvater : MonoBehaviour 
{
    #region Properties

    public ParticleSystem shape;
    public ParticleSystem trail;
    public ParticleSystem burst;

    public float deathCountDown = -1f;

    private SwirlyPlayer player;

    private ParticleSystem.EmissionModule emissionModule;

    #endregion

    #region Unity Callbacks

	private void Awake() 
    {
        player = transform.root.GetComponent<SwirlyPlayer>();
	}
	
    private void Update()
    {
        if (deathCountDown >= 0f)
        {
            deathCountDown -= Time.deltaTime;

            if (deathCountDown <= 0f)
            {
                deathCountDown = -1f;

                emissionModule = shape.emission;
                emissionModule.enabled = true;

                emissionModule = trail.emission;
                emissionModule.enabled = true;

                player.Die();
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (deathCountDown < 0f)
        {
            emissionModule = shape.emission;
            emissionModule.enabled = false;

            emissionModule = trail.emission;
            emissionModule.enabled = false;

            burst.Emit(burst.maxParticles);

            deathCountDown = burst.startLifetime;

            player.SetDeathAcceleration();
        }
    }

    #endregion
}
