using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RogueLikeWall : MonoBehaviour 
{
    #region Properties

    public Sprite dmgSprite;
    public int hp = 4;

    public AudioClip chop1;
    public AudioClip chop2;

    private SpriteRenderer spriteRenderer;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    #endregion

    #region Methods

    public void DamageWall(int loss)
    {
        spriteRenderer.sprite = dmgSprite;
        hp -= loss;

        RogueLikeSoundManager.instance.RandomizeSfx(chop1, chop2);

        if (hp <= 0)
            gameObject.SetActive(false);
    }

    #endregion
}
