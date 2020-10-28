using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadPlayer()
    {
        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }

    public void LoadAI()
    {
        //TODO - ADD LOADSCENE FOR AI
    }

    public void LoadMainMenu()
    {
        //TODO - ADD LOADSCENE FOR MAIN MENU
    }
}
