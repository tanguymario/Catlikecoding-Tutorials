using UnityEngine;

public class RollABallCameraController : MonoBehaviour 
{
    #region Properties

    public RollABallPlayerControls player;

    private Vector3 offset;

    #endregion

    #region Unity Callbacks

	private void Start () 
    {
        offset = transform.position - player.transform.position;
	}
	
	private void Update () 
    {
        transform.position = player.transform.position + offset;
	}

    #endregion
}
