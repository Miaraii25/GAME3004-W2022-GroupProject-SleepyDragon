//*********************************************************************************************************************************************
// Author: Mariam Ogunlesi
//
// Last Modified: January 28, 2022
//  
// Description: This class holds all the loaded data of a save file. Game data must be converted to this format before writing to save files. 
// And Save files must be converted to this data before being used by the game.
//
// Note: This class cannot use Unity's Vector3..
//
//***************************************************************************************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
  

    public TransformLite[] platformCoordinates;   //Note: Not set in constructor; Count must be checked before use
    public TransformLite[] enemiesCoordinates;
    public int[] playerInventory;
    public string savefileHeader;            //The save file header seen in-game view. This is different from the save file name.
    public string gameVersion;
    public TransformLite playercoordinates;
    public TransformLite checkpointcoordinates;
    public bool[] levelPickUps;              //Note: Not set in constructor; Count must be checked before use
    public bool[] enemiesExist;                 //Note: Not set in constructor; Count must be checked before use
    public int MailCollide;

    public int healthAmount;
    public int livesAmount;
    public int scoreAmount;
    public int biscuitCollected;
    public int enemiesKilled;
    public int currentLevel; //0 means not in a level
    public int checkpoint;
    public int levelsUnlocked;


    public SaveData()
    {
         //These are default values that SHOULD be replaced upon instantiation
        savefileHeader = "default header";
        gameVersion = "undefined";

        playercoordinates = new TransformLite(0, 0, 0, 0, 0, 0);
        checkpointcoordinates = new TransformLite(0, 0, 0, 0, 0, 0);

        playerInventory = new int[8];
        playerInventory[0] = 0;
        playerInventory[1] = 0;
        playerInventory[2] = 0;
        playerInventory[3] = 0;
        playerInventory[4] = 0;
        playerInventory[5] = 0;
        playerInventory[6] = 0;

        healthAmount = 100;
        livesAmount = 3;
        scoreAmount = 100;
        MailCollide = 0;
        biscuitCollected = 0;
        enemiesKilled = 0;
        currentLevel = 0; //0 means not in a level
        checkpoint = 1;
        levelsUnlocked = 1;

    }
}
