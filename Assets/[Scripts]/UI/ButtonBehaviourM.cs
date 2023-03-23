//*********************************************************************************************************
// Author: Mingkun Yang, Phu Pham
//
// Last Modified: February 2, 2022
//  
// Description: This script is used to implement volume slider function.
//
//******************************************************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonBehaviourM : MonoBehaviour
{
    public SoundManagerScript soundManager;
    // to load scene
    //public string NewGameScene = string.Empty;

    // to start new game and load game
    public static bool newGame;
    public static bool loadGame;


    private void Start()
    {
        soundManager = FindObjectOfType<SoundManagerScript>();
        //newGame = false;
        //loadGame = false;
    }

    public void OnOptionButtonPressed()
    {
        soundManager.PlayButtonSFX();
        SceneManager.LoadScene("OptionScene");
        
    }
    public void OnNewGameButtonPressed()
    {
        soundManager.PlayButtonSFX();
        SceneManager.LoadScene("GameLevelScene 1");

        //SceneManager.LoadScene(NewGameScene);
        //newGame = true;
        //loadGame = false;
    }

    public void OnExitButtonPressed()
    {
        soundManager.PlayButtonSFX();
        Application.Quit();
    }

    public void OnBackButtonPressed()
    {
        soundManager.PlayButtonSFX();
        SceneManager.LoadScene("MainMenuScene");
    }

    public void OnRestartButtonPressed()
    {
        soundManager.PlayButtonSFX();
        SceneManager.LoadScene("GameLevelScene 1");
    }

    public void OnNextButtonPressed()
    {
        soundManager.PlayButtonSFX();
        SceneManager.LoadScene("GameOverScene");
    }

    public void OnControlButtonPressed()
    {
        soundManager.PlayButtonSFX();
    }

    public void OnKeyMappingButtonPressed()
    {
        soundManager.PlayButtonSFX();
    }

    public void OnPauseButtonPressed()
    {
        soundManager.PlayButtonSFX();
    }

    public void OnInventoryButtonPressed()
    {
        soundManager.PlayButtonSFX();
    }

    public void OnResumeButtonPressed()
    {
        soundManager.PlayButtonSFX();
    }

    public void OnSaveButtonPressed()
    {
        soundManager.PlayButtonSFX();
    }

    public void OnBackSoundButton()
    {
        soundManager.PlayButtonSFX();
    }

    public void OnLoadButtonPressed()
    {
        soundManager.PlayButtonSFX();
        //SceneManager.LoadScene(NewGameScene);
        //newGame = false;
        //loadGame = true;
    }
}
