using UnityEngine;
using UnityEngine.UI;

public class EndlessRunnerUIManager : MonoBehaviour 
{
    #region Properties

    public Text txtTitle;

    public Text txtGameOver;

    public Text txtInstructions;

    public Text txtBoots;

    public Text txtDistance;

    private static EndlessRunnerUIManager instance;

    #endregion

    #region Unity Callbacks

	private void Start () 
    {
        instance = this;

        EndlessRunnerGameManager.GameStart += GameStart;
        EndlessRunnerGameManager.GameOver += GameOver;

        txtGameOver.enabled = false;
	}

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
            EndlessRunnerGameManager.TriggerGameStart();
    }

    #endregion

    #region Methods

    static public void SetBoosts(int boosts)
    {
        instance.txtBoots.text = boosts.ToString();
    }

    static public void SetDistance(float distance)
    {
        instance.txtDistance.text = distance.ToString("f0");
    }

    private void GameStart()
    {
        txtGameOver.enabled = false;
        txtInstructions.enabled = false;
        txtTitle.enabled = false;

        enabled = false;
    }

    private void GameOver()
    {
        txtGameOver.enabled = true;
        txtInstructions.enabled = true;

        enabled = true;
    }

    #endregion
}
