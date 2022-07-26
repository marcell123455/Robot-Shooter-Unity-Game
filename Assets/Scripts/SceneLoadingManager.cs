using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadingManager : MonoBehaviour
{
    bool startedLoading;
    int sceneToLoad;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public void LoadScene(int sceneIndex)
    {
        sceneToLoad = sceneIndex;
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void FixedUpdate()
    {
        if(SceneManager.GetSceneByBuildIndex(1) == SceneManager.GetActiveScene())
        {
            if(!startedLoading)
            StartCoroutine(LoadSceneAsync(sceneToLoad));
        }
    }

    IEnumerator LoadSceneAsync(int sceneIndex)
    {
        startedLoading = true;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);

        if (asyncLoad.isDone)
            startedLoading = false;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
