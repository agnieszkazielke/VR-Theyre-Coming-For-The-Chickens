using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneChanger : MonoBehaviour
{
    private Scene scene;
    private int sceneIndex;
    

    // Start is called before the first frame update

    public void MenuUILoadSceneOne()
    {

        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void MenuUILoadSceneTwo()
    {

        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }


    public void MenuUILoadSceneThree()
    {

        SceneManager.LoadScene(3, LoadSceneMode.Single);
    }


    public void LoadMenuScene()
    {

        SceneManager.LoadScene(0);
    }



    public void LoadNextScene()
    {
        scene = SceneManager.GetActiveScene();
        sceneIndex = scene.buildIndex;

        SceneManager.LoadScene(sceneIndex + 1);


    }

    public void ReloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }


    public void MenuUIExit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit(); // original code to quit Unity player
#endif
    }
}
