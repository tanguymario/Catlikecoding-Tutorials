using UnityEngine;

public class RoguelikeLoader : MonoBehaviour 
{
    #region Properties

    public GameObject rogueLikeGameManager;

    #endregion

    #region Unity Callbacks

	private void Awake() 
    {
        if (RoguelikeGameManager.instance == null)
            Instantiate(rogueLikeGameManager);
	}
	
    #endregion
}
