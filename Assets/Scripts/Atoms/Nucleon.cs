using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Nucleon : MonoBehaviour 
{
    #region Properties

	public float attractionForce;

	private Rigidbody body;

    #endregion

    #region Unity Callbacks

	private void Awake()
	{
        body = GetComponent<Rigidbody>();
	}

    private void FixedUpdate()
    {
        body.AddForce(transform.localPosition * -attractionForce);
    }

    #endregion

    #region Methods

    #endregion
}
