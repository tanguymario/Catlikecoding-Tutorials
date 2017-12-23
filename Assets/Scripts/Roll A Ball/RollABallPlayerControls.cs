using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RollABallPlayerControls : MonoBehaviour 
{
    #region Properties

    [SerializeField]
    [Range(0, 50)]
    private float speed = 5.0f;

    private Rigidbody body;

    #endregion

    #region Unity Callbacks

	private void Awake() 
    {
        body = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate() 
    {
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalMove, 0f, verticalMove);

        if (movement != Vector3.zero)
        {
            body.AddForce(movement * speed);
        }

        if (body.velocity != Vector3.zero)
            body.velocity *= 0.99f;
	}

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Collectible"))
        {
            Destroy(collider.gameObject);
        }
    }

    #endregion

    #region Methods

    #endregion
}
