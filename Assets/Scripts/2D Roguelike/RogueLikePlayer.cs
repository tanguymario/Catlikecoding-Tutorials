using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RogueLikePlayer : RogueLikeMovingObjects 
{
    #region Properties

    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public Text foodText;

    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    private Animator animator;
    private int food;

    #endregion

    #region Unity Callbacks

    protected override void Start()
    {
        animator = GetComponent<Animator>();

        food = RoguelikeGameManager.instance.playerFoodPoints;

        foodText.text = string.Concat("Food: ", food.ToString());

        base.Start();
    }

    private void Update()
    {
        if (!RoguelikeGameManager.instance.playersTurn)
            return;

        int horizontal = 0; 
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
            vertical = 0;

        if (horizontal != 0 || vertical != 0)
        {
            AttemptMove<RogueLikeWall>(horizontal, vertical);
        }
    }

    private void OnDisable()
    {
        RoguelikeGameManager.instance.playerFoodPoints = food;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if (collider.tag == "Food")
        {
            food += pointsPerFood;

            foodText.text = string.Concat("Food: ", food.ToString(), " + ", pointsPerFood.ToString());

            RogueLikeSoundManager.instance.RandomizeSfx(eatSound1, eatSound2);

            collider.gameObject.SetActive(false);
        }
        else if (collider.tag == "Soda")
        {
            food += pointsPerSoda;

            foodText.text = string.Concat("Food: ", food.ToString(), " + ", pointsPerSoda.ToString());

            RogueLikeSoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);

            collider.gameObject.SetActive(false);
        }
    }
   
    #endregion

    #region Methods

    public void LooseFood(int loss)
    {
        animator.SetTrigger("playerHit");

        food -= loss;

        foodText.text = string.Concat("Food: ", food.ToString(), " - ", loss.ToString());

        CheckIfGameOver();
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        food--;

        foodText.text = string.Concat("Food: ", food.ToString());

        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;
        if (Move(xDir, yDir, out hit))
        {
            RogueLikeSoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

        CheckIfGameOver();
        
        RoguelikeGameManager.instance.playersTurn = false;
    }
    
    protected override void OnCantMove<T>(T component)
    {
        RogueLikeWall hitWall = component as RogueLikeWall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void CheckIfGameOver()
    {
        if (food <= 0)
        {
            RogueLikeSoundManager.instance.PlaySingle(gameOverSound);
            RogueLikeSoundManager.instance.musicSource.Stop();

            RoguelikeGameManager.instance.GameOver();
        }
    }

    #endregion
}
