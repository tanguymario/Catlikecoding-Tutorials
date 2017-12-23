using UnityEngine;

public class RollABallCollectible : MonoBehaviour 
{
    #region Properties

    [SerializeField]
    private Vector3 rotateSpeed = new Vector3(15, 30, 45);

    #endregion

    #region Unity Callbacks

	private void Update() 
    {
        transform.Rotate(rotateSpeed * Time.deltaTime);
	}

    #endregion
}
