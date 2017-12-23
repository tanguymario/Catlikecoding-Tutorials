using UnityEngine;
using UnityEngine.UI;

public class SwirlyMainMenu : MonoBehaviour 
{
    #region Properties

    public SwirlyPlayer player;

    public Text scoreLabel;

    #endregion

    #region Unity Callbacks

	private void Awake() 
    {
        SetScore(0);

        Application.targetFrameRate = 1000;
	}
	
    #endregion

    #region Methods

    public void StartGame(int mode)
    {
        player.StartGame(mode);

        Cursor.visible = false;

        gameObject.SetActive(false);
    }

    public void EndGame(float distanceTravelled)
    {
        SetScore((int)(distanceTravelled * 10f));

        Cursor.visible = true;

        gameObject.SetActive(true);
    }

    private void SetScore(int score)
    {
        scoreLabel.text = "Last Score : ";
        scoreLabel.text += score.ToString();
    }

    #endregion
}
