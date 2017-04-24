using UnityEngine;
using System.Collections;

public class sceneLoader : MonoBehaviour 
{
    #region Unity Callbacks

    private void Start()
    {
        if (GameObject.FindGameObjectWithTag(ScenesManager.sceneManagersTag) == null)
        {
            Debug.LogWarning("Scenes Manager was null");

            GameObject go = new GameObject("Scenes Manager");
            go.AddComponent<ScenesManager>();
            go.transform.SetAsLastSibling();
        }
    }

	private void Update () 
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            loadPreviousScene();
	}

    #endregion

    #region Methods

    public void loadScene(int sceneIndex)
    {
        ScenesManager.updateScenes(sceneIndex);
    }

    public void loadPreviousScene()
    {
        ScenesManager.loadPreviousScene();
    }

    #endregion
}
