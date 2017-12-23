using UnityEngine;

public class RogueLikeEnemy : RogueLikeMovingObjects 
{
	#region Properties

	public int playerDamage;

    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

	private Animator animator;
	private Transform target;
	private bool skipMove;

	#endregion

	#region Unity Callbacks

	protected override void Start()
	{
        RoguelikeGameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
	}

	#endregion

	#region Methods

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            yDir = target.position.y > transform.position.y ? 1 : -1;
        }
        else
        {
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }

        AttemptMove<RogueLikePlayer>(xDir, yDir);
    }

	protected override void OnCantMove<T>(T component)
	{
        RogueLikePlayer hitPlayer = component as RogueLikePlayer;

        animator.SetTrigger("enemyAttack");

        RogueLikeSoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);

        hitPlayer.LooseFood(playerDamage);
	}

	protected override void AttemptMove<T>(int xDir, int yDir)
	{
		if (skipMove) 
		{
            skipMove = false;
            return;
		}

        base.AttemptMove<T>(xDir, yDir);

        skipMove = true;
	}

	#endregion
}
