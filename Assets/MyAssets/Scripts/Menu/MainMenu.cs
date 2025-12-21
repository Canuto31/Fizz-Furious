using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;



public class MainMenu : MonoBehaviour
{
    public string sceneToLoad = "SelectCharacter";
    public void StartGame()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }

}
