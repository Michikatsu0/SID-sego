using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum AppScene { LOGIN, GAME }

public class SceneManagement : MonoBehaviour
{
    public static SceneManagement Instance;
    public float delayTransition;

    private void Awake()
    {
        Instance = this;
    }

    public void ChangeScene(int sceneIndex) => StartCoroutine(LoadNextScene(sceneIndex, delayTransition));
    public void ChangeScene(int sceneIndex, float delayTransition) => StartCoroutine(LoadNextScene(sceneIndex, delayTransition));

    private IEnumerator LoadNextScene(int sceneIndex, float delayTransition)
    {
        //Transition

        yield return new WaitForSeconds(delayTransition);
        SceneManager.LoadScene(sceneIndex);
    }
}
