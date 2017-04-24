using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScenesManager : MonoBehaviour 
{
	#region Properties

    static private List<int> Scenes = new List<int>();

    static private int nbTotalScenes;

	static public readonly string sceneManagersTag = "Current Scenes Manager";

	#endregion

	#region Unity Callbacks

	void Start () 
	{
        nbTotalScenes = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;

        Scenes.Add(0);
	}
	
	void Awake () 
	{
		if (GameObject.FindGameObjectWithTag(sceneManagersTag))
		{
			Destroy(gameObject);
			Destroy(this);
		}
		else 
		{
			if (!CompareTag(sceneManagersTag))
				gameObject.tag = sceneManagersTag;

			DontDestroyOnLoad(this);
		}
	}

	#endregion

	#region Methods

	static public void updateScenes(int index)
	{
        if (index < nbTotalScenes)
        {
            if (!checkSceneExistence(index))
                Scenes.Add(index);
            else 
                removeScenesAfter(index);

            loadScene(index);
        }
        else
        {
            Debug.LogWarning("Index incorrect\nResetting...");

            resetScenes();
        }
	}

    static public void loadPreviousScene()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex.Equals(0))
        {
            Application.Quit();
        }
        else if (Scenes.Count >= 2)
        {
            int previousScene = Scenes[Scenes.Count - 2];

            // Remove current scene and previous scene
            for (int i = 0; i < 2; i++)
                Scenes.RemoveAt(Scenes.Count - 1);
           
            updateScenes(previousScene);
        }
        else
        {
            Debug.LogWarning("Scenes List incorrect");

            resetScenes();
        }
    }

    static private bool checkSceneExistence(int sceneIndex)
    {
        if (Scenes.Contains(sceneIndex))
            return true;
        else
            return false;
    }

    static private void removeScenesAfter(int sceneIndex)
    {
        int indexInList = -1;
        for (int i = 0; i < Scenes.Count; i++)
        {
            if (sceneIndex.Equals(Scenes[i]))
            {
                indexInList = i;
                break;
            }
        }

        if (indexInList != -1)
            Scenes.RemoveRange(indexInList + 1, Scenes.Count - indexInList - 1);
        else
            Debug.LogWarning("Wrong scene index");
    }

	static private void resetScenes()
	{
		Scenes.Clear();

        updateScenes(0);
	}

	static private void loadScene(int index)
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(index);
	}

	#endregion
}
