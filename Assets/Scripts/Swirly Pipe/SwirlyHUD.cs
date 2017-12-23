using UnityEngine;
using UnityEngine.UI;

public class SwirlyHUD : MonoBehaviour 
{
    #region Properties

    public Text distanceLabel;

    public Text velocityLabel;

    #endregion

    #region Methods

    public void SetValues(float distanceTravelled, float velocity)
    {
        distanceLabel.text = ((int)(distanceTravelled * 10f)).ToString();
        velocityLabel.text = ((int)(velocity * 10f)).ToString();
    }

    #endregion
}
