using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(RoguelikeBoardManager))]
public class RoguelikeGameManager : MonoBehaviour 
{
    #region Properties

    static public RoguelikeGameManager instance = null;

    public float levelStartDelay = 2f;

    public int playerFoodPoints = 100;

    public float turnDelay = 0.1f;

    [HideInInspector]
    public bool playersTurn = true;

    private Text levelText;
    private Image levelImage;

    private RoguelikeBoardManager board;

    private int level = 1;
    private List<RogueLikeEnemy> enemies;
    private bool enemiesMoving;

    private bool doingSetup;

    #endregion

    #region Unity Callbacks

	private void Awake() 
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        enemies = new List<RogueLikeEnemy>();

        board = GetComponent<RoguelikeBoardManager>();

        InitGame();
	}
	
    private void Update()
    {
        if (playersTurn || enemiesMoving || doingSetup)
            return;

        StartCoroutine(MoveEnemies());
    }

    private void OnLevelWasLoaded(int index)
    {
        level++;

        InitGame();
    }

    #endregion

    #region Methods

    public void AddEnemyToList(RogueLikeEnemy enemy)
    {
        enemies.Add(enemy);
    }

    public void GameOver()
    {
        levelText.text = string.Concat("After " + level.ToString() + " days, you starved");

        enabled = false;
    }

    private void InitGame()
    {
        doingSetup = true;

        levelImage = GameObject.Find("Image Level").GetComponent<Image>();
        levelText = GameObject.Find("Text Level").GetComponent<Text>();

        levelText.text = string.Concat("Day ", level.ToString());
        levelImage.gameObject.SetActive(true);

        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        board.SetupScene(level);
    }

    private void HideLevelImage()
    {
        levelImage.gameObject.SetActive(false);
        doingSetup = false;
    }

    #endregion

    #region Coroutines

    private IEnumerator MoveEnemies()
    {
        enemiesMoving = true;

        yield return new WaitForSeconds(turnDelay);

        if (enemies.Count == 0)
            yield return new WaitForSeconds(turnDelay);

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;

        yield return null;
    }

    #endregion
}
