using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // What happens when I click the start button?
    public void StartGame()
    {
        Game.Level = 1;
        Game.settingsGenerated = false;
        SceneManager.LoadScene("Level01");
    }

    public void GoToLevelComplete()
    {
        SceneManager.LoadScene("FinishLevelScreen");
    }

    public void GoToNextLevel()
    {
        Game.Level++;                
        switch (Game.Level){
            case 2 : GoToLevel02();break;
            case 3 : GoToLevel03();break;
            default : GoToGameOver();break;
        }
    }


    public void GoToLevel02()
    {
        SceneManager.LoadScene("Level01");
    }

    public void GoToLevel03()
    {
        SceneManager.LoadScene("Level01");
    }

    public void GoToTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void GoToGameOver()
    {
        SceneManager.LoadScene("GameOverScreen");
    }

    public void MenuGame()
    {
        Game.Level = 1;
        SceneManager.LoadScene("MainScreen");
    }

    public void ControlsScene()
    {
        Game.Level = 1;
        SceneManager.LoadScene("ControlScreen");
    }

    // What happens when I click the quit button?
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}