using UnityEngine;

public class PipeItem : MonoBehaviour 
{
    #region Properties

    private Transform rotater;

    #endregion

    #region Unity Callbacks

	private void Awake() 
    {
        rotater = transform.GetChild(0);
	}
	
    #endregion

    #region Methods

    public void Position(Pipe p, float curveRotation, float ringRotation)
    {
        transform.SetParent(p.transform, false);
        transform.localRotation = Quaternion.Euler(0f, 0f, -curveRotation);

        rotater.localPosition = new Vector3(0f, p.CurveRadius);
        rotater.localRotation = Quaternion.Euler(ringRotation, 0f, 0f);
    }

    #endregion
}
