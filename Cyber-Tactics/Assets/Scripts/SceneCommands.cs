using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCommands : MonoBehaviour
{
    [Tooltip("Enter name of the scene for the first level of the game.")]
    public string firstSceneName;
    [Tooltip("Enter name of the scene of the main menu scene.")]
    public string mainMenuSceneName;
    [Tooltip("Enter name of the scene of the end credits scene.")]
    public string creditsSceneName;

    //Loads the first level of the game when called.
    //First level scene should be placed after the main menu scene or as the second build in the build settings for this
    //function to work properly or be labelled properly in the inspector.
    public void LoadFirstLevel()
    {
        Time.timeScale = 1;
        if (firstSceneName == null)
        {
            if(SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().name == mainMenuSceneName)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }

            else
            {
                Debug.Log("Unable to load first level. No scene designated as first level.");
            }
        }
        
        else
        {
            SceneManager.LoadScene(firstSceneName);
        }
    }

    //Loads the next scene after the currently loaded scene in the build settings.
    public void LoadNextScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    //Loads the currently loaded scene, resetting any progression that has been made.
    public void ReloadScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Loads a specific scene witihin the build settings.
    public void LoadThisScene(string sceneName)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }

    //Loads the first scene listed in the build settings or a specifically designated main menu scene.
    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        if (mainMenuSceneName == null)
        {
            SceneManager.LoadScene(0);
        }

        else
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }

    //Loads the last scene listed in the build settings or a specifically designated credits scene.
    public void LoadCredits()
    {
        Time.timeScale = 1;
        if (creditsSceneName == null)
        {
            SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
        }

        else
        {
            SceneManager.LoadScene(creditsSceneName);
        }
    }

    //Closes the game client.
    public void QuitGame()
    {
        Debug.Log("Quitting Game.");
        Application.Quit();
    }
}
