using UnityEngine;
using System.Collections;
using System;

public class clockManager : MonoBehaviour 
{
    #region Properties

    public Transform trHours;
    public Transform trMinutes;
    public Transform trSeconds;

    [SerializeField]
    private bool isAnalog;

    private const float hoursToDegrees = 360f / 12f;
    private const float minutesToDegrees = 360f / 60f;
    private const float secondsToDegrees = 360f / 60f;

    #endregion

    #region Unity Callbacks

	private void Start () 
    {
        if (trHours == null || trMinutes == null || trSeconds == null)
            Destroy(this);
	}

    private void Update()
    {
        if (isAnalog) 
        {
            TimeSpan timespan = DateTime.Now.TimeOfDay;

            trHours.localRotation = Quaternion.Euler(0f, 0f, (float)timespan.TotalHours * hoursToDegrees);
            trMinutes.localRotation = Quaternion.Euler(0f, 0f, (float)timespan.TotalMinutes * minutesToDegrees);
            trSeconds.localRotation = Quaternion.Euler(0f, 0f, (float)timespan.TotalSeconds * secondsToDegrees);
        }
        else 
        {
            DateTime time = DateTime.Now;

            trHours.localRotation = Quaternion.Euler(0f, 0f, time.Hour * hoursToDegrees);
            trMinutes.localRotation = Quaternion.Euler(0f, 0f, time.Minute * minutesToDegrees);
            trSeconds.localRotation = Quaternion.Euler(0f, 0f, time.Second * secondsToDegrees);
        }
    }

    #endregion

    #region Methods

    public void setAnalog(bool analogValue)
    {
        isAnalog = analogValue;
    }

    #endregion
}
