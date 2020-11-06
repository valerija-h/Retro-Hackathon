using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject loadingScreen;
    public Text progressText;
    public GameObject pauseMenu;
    public GameObject instructions;
    public GameObject AIMessage;
    
    public void LoadLevel (int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously (int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);
        progressText.text = "Loading...";
        loadingScreen.SetActive(true);

        while(!operation.isDone)
        {

            yield return null;

        }
    }

    public void Pause()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    public void Instructions()
    {
        Time.timeScale = 0;
        instructions.SetActive(true);
    }

    public void closeInstructions()
    {
        if(pauseMenu.active == true) { Time.timeScale = 0; }
        else { Time.timeScale = 1; }
        instructions.SetActive(false);
    }

    public void Unpause()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void Quit()
    {
        Application.Quit();
    }

}
