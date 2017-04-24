using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Slider))]
public class sliderTextValue : MonoBehaviour 
{
    #region Properties

    public Text text;

    private Slider slid;

    #endregion

    #region Unity Callbacks

	void Start () 
    {
        if (text == null)
        {
            Destroy(this);
        }
        else
        {
            slid = GetComponent<Slider>();

            text.text = slid.value.ToString("F2");

            slid.onValueChanged.AddListener((float value) =>
                {
                    text.text = slid.value.ToString("F2");
                });
        }

	}

    #endregion
}
