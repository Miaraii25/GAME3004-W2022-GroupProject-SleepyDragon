//*********************************************************************************************************
// Author: Mariam Ogunlesi
//
// Last Modified: April 14 2022
//  
// Description: This saves all the Game Screen.
//
//******************************************************************************************************
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SaveGame : MonoBehaviour
{


    [Header("References")]
    [SerializeField] private Transform playerCharacter;
    [SerializeField] private Transform camera;
    [SerializeField] private PlayerBehaviour MailGoal;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private InventorySystem inventoryRef;
    [SerializeField] private Text[] saveSlots = new Text[5];
    [SerializeField] private Transform[] pickupsInLevel;
    [SerializeField] private Transform[] platformsInLevel;
    [SerializeField] private Transform[] enemiesInLevel;
    [SerializeField] private SpawnController checkpoint;

    [Header("Settings")]
    [SerializeField] private string savefileName = "Pawggers";       //This is the name of the save file. An indexing number will be appended to this name. This is different from the save file header seen in-game.
    [SerializeField] private int levelNumber = 1;                       //TODO: Verify it is the same level
    private string[] saveFileDisplayHeaders;                            //This game will have a maximum 4 save slots hardcoded.
    private string gameVersion = "0.4";

    private void Awake()
    {
        if (LoadedSaveFile.loadLevelBasedOnSaveFile == true)
        {
            LoadedSaveFile.loadLevelBasedOnSaveFile = false;
            LoadGameFromSelectedSaveFile();
        }
        else
        {
            if (playerHealth)
            {
                playerHealth.SetHealth(playerHealth.maxhealth);
            }
        }
    }

    private void Start()
    {
        Time.timeScale = 1;
        SetUpSaveSlotHeaders();
    }

    private string GetSaveSlotHeader(int _saveSlotIndex)
    {
        if (_saveSlotIndex < 1 || _saveSlotIndex > 5)
        { //This game will have a maximum 4 save slots hardcoded.
            Debug.LogError("[Error] Invalid save slot index! Slot number must be between from 1 to 4.");
            return "[Error] Invalid Save slot index!";
        }

        if (saveFileDisplayHeaders == null)
        {
            saveFileDisplayHeaders = SaveFileReaderWriter.CheckAvailableSaveFiles(Application.persistentDataPath, savefileName);
        }

        if (saveFileDisplayHeaders != null)
        {
            if (saveFileDisplayHeaders.Length <= 0)
            {
                Debug.LogError("[Error] SaveFilesavailable array not initialized!");
                return "[Error] SaveFilesavailable array not initialized!";
            }
        }
        else
        {
            Debug.LogError("[Error] SaveFilesavailable array not initialized!");
            return "[Error] SaveFilesavailable array not initialized!";
        }

        return saveFileDisplayHeaders[_saveSlotIndex - 1];
    }

    private void SetUpSaveSlotHeaders()
    {
        saveSlots[0].text = GetSaveSlotHeader(1);
        saveSlots[1].text = GetSaveSlotHeader(2);
        saveSlots[2].text = GetSaveSlotHeader(3);
        saveSlots[3].text = GetSaveSlotHeader(4);
        saveSlots[4].text = GetSaveSlotHeader(4);

    }

    private void LoadGameFromSelectedSaveFile()
    {

        //Check Loaded Save File
        if (LoadedSaveFile.loadedSaveData == null)
        {
            Debug.LogError("[Error] Could not load save file.");
            return;
        }

        //Check if current scene matches save file level
        if (LoadedSaveFile.loadedSaveData.currentLevel == 2)
        {
            if (SceneManager.GetActiveScene().buildIndex != 2)
            {
                Debug.LogError("[Error] Save file level data mismatch.");
                return;
            }
        }

        //Set up level if applicable
        StartCoroutine(LoadGameStateRoutine());

    }

    //Saves game data at given save slot index
    public void SavedGame(int _saveSlotIndex)
    {
        if (_saveSlotIndex <= 0 || _saveSlotIndex > 4)
        { //This game will have a maximum 4 (1 to 4 or more) save slots. 
            Debug.LogError("[Error] Invalid save slot index! Slot number must be between from 1 to 4 or more.");
            return;
        }

        SaveData newSaveData = new SaveData();
        newSaveData.gameVersion = this.gameVersion;




        Debug.Log("Game data saved!");

        ////Save Player location
        if (playerCharacter)
        {
            newSaveData.playercoordinates = new TransformLite(playerCharacter.position.x, playerCharacter.position.y, playerCharacter.position.z,
                playerCharacter.eulerAngles.x, playerCharacter.eulerAngles.y, playerCharacter.eulerAngles.z);
        }
        else
        {
            Debug.LogError("[Error] Reference to player character is missing!");
        }

        //Save Player Health
        if (playerHealth)
        {
            newSaveData.healthAmount = playerHealth.currentHealth;
        }
        else
        {
            Debug.LogError("[Error] Reference to player health is missing!");
        }

        //////Save Collected Items amount and inventory
        if (inventoryRef)
        {
            //newSaveData.ItemsCollected = inventory.GetPlayerItemAmount();

            //    for (int inventorySlot = 0; inventorySlot < newSaveData.playerInventory.Length; inventorySlot++)
            //    {

            //        if (inventoryRef.CheckItemAtInventorySlot(inventorySlot) == null)
            //        {
            //            newSaveData.playerInventory[inventorySlot] = 0;
            //        }

            //        switch (inventoryRef.CheckItemAtInventorySlot(inventorySlot))
            //        {
            //            case "Biscuit":
            //                newSaveData.playerInventory[inventorySlot] = 1;
            //                break;
            //            case "":
            //                newSaveData.playerInventory[inventorySlot] = 0;
            //                break;
            //            default:
            //                newSaveData.playerInventory[inventorySlot] = 0;
            //                break;
            //        }
            //    }
        }
        else
        {
            Debug.LogError("[Error] Reference to inventory is missing!");
        }



        //if (MailGoal)
        //{
        //    newSaveData.MailCollected = MailGoal.goal.MailCollected;
        //}
        //else
        //{
        //    Debug.LogWarning("[Warning] Reference to Goal.cs is missing!"); //TODO
        //}

        //Save current pickups in level
        if (pickupsInLevel.Length > 0)
        {
            newSaveData.levelPickUps = new bool[pickupsInLevel.Length];
            for (int i = 0; i < pickupsInLevel.Length; i++)
            {
                if (pickupsInLevel[i])
                {
                    newSaveData.levelPickUps[i] = true;
                }
                else
                {
                    newSaveData.levelPickUps[i] = false;
                }
            }
        }
        else
        {
            Debug.LogWarning("[Warning] There are no pickups found in pickupsInLevel references.");
        }

        //Save platform positions
        if (platformsInLevel.Length > 0)
        {
            newSaveData.platformCoordinates = new TransformLite[platformsInLevel.Length];
            for (int platformIndex = 0; platformIndex < platformsInLevel.Length; platformIndex++)
            {
                if (platformsInLevel[platformIndex])
                {
                    newSaveData.platformCoordinates[platformIndex] = new TransformLite(platformsInLevel[platformIndex].position.x, platformsInLevel[platformIndex].position.y, platformsInLevel[platformIndex].position.z,
                        platformsInLevel[platformIndex].eulerAngles.x, platformsInLevel[platformIndex].eulerAngles.y, platformsInLevel[platformIndex].eulerAngles.z);
                }
                else
                {
                    Debug.LogWarning("[Warning] Platform reference in index is missing.");
                    newSaveData.platformCoordinates[platformIndex] = new TransformLite(0, 0, 0, 0, 0, 0);
                }
            }
        }
        else
        {
            Debug.LogWarning("[Warning] There are no platforms found in platformsInLevel references.");
        }

        //Save Enemies positions
        if (enemiesInLevel.Length > 0)
        {
            newSaveData.enemiesCoordinates = new TransformLite[enemiesInLevel.Length];
            newSaveData.enemiesExist = new bool[enemiesInLevel.Length];
            for (int i = 0; i < enemiesInLevel.Length; i++)
            {
                if (enemiesInLevel[i])
                {
                    newSaveData.enemiesExist[i] = true;
                    newSaveData.enemiesCoordinates[i] = new TransformLite(enemiesInLevel[i].position.x, enemiesInLevel[i].position.y, enemiesInLevel[i].position.z,
                        enemiesInLevel[i].eulerAngles.x, enemiesInLevel[i].eulerAngles.y, enemiesInLevel[i].eulerAngles.z);
                }
                else
                {
                    newSaveData.enemiesExist[i] = false;
                    newSaveData.enemiesCoordinates[i] = new TransformLite(0, 0, 0, 0, 0, 0);
                }
            }
        }
        else
        {
            Debug.LogWarning("[Warning] There are no enemies found in platformsInLevel references.");
        }

        // Save Checkpoint position
        if (checkpoint)
        {
            newSaveData.checkpointcoordinates = new TransformLite(checkpoint.currentSpawnPoint.x, checkpoint.currentSpawnPoint.y, checkpoint.currentSpawnPoint.z, checkpoint.currentSpawnPoint.x, checkpoint.currentSpawnPoint.y, checkpoint.currentSpawnPoint.z);
        }
        else
        {
            Debug.LogWarning("[Warning] Reference to RespawnLogic.cs missing!");
        }


        //TEMP settings
        newSaveData.livesAmount = 3;
        newSaveData.scoreAmount = 100;
        newSaveData.enemiesKilled = 0;
        newSaveData.currentLevel = 1; //0 means not in a level
        newSaveData.levelsUnlocked = 1;
        newSaveData.savefileHeader = "[Pawggers] Health: " + newSaveData.healthAmount + ";";

        SaveFileReaderWriter.WriteToSaveFile(Application.persistentDataPath + "/" + savefileName + _saveSlotIndex + ".pawsave", newSaveData);

        //Update save slot button header
        saveSlots[_saveSlotIndex - 1].text = newSaveData.savefileHeader;

        Debug.Log("[Notice] Game Saved.");
    }

    IEnumerator LoadGameStateRoutine()
    {
        Time.timeScale = 0;
        //Set up player character transform
        if (playerCharacter)
        {
            playerCharacter.position = new Vector3(LoadedSaveFile.loadedSaveData.playercoordinates.positionX, LoadedSaveFile.loadedSaveData.playercoordinates.positionY,
                                                   LoadedSaveFile.loadedSaveData.playercoordinates.positionZ);
            playerCharacter.eulerAngles = new Vector3(0, LoadedSaveFile.loadedSaveData.playercoordinates.orientationY, 0);
        }
        else
        {
            Debug.LogError("[Error] Reference to player character missing.");
        }

        //Set up camera
        if (camera && playerCharacter)
        {
            camera.position = playerCharacter.position;
            camera.rotation = playerCharacter.rotation;
        }

        //Set player health
        if (playerHealth)
        {
            playerHealth.SetHealth(LoadedSaveFile.loadedSaveData.healthAmount);
        }
        else
        {
            Debug.LogError("[Error] Reference to player health is missing!");
        }

        //if (MailGoal)
        //{
        //    MailGoal.SetMail(LoadedSaveFile.loadedSaveData.MailGoalcollide);
        //}
        //else
        //{
        //    Debug.LogWarning("[Warning] Reference to MailGoal.cs is missing!"); //TODO
        //}

        //Set seed collected amount and inventory
        if (inventoryRef)
        {
            //    //inventoryRef.SetPlayerSeedAmount(LoadedSaveFile.loadedSaveData.seedsCollected);

            //    for (int inventorySlot = 0; inventorySlot < 8; inventorySlot++)
            //    {
            //        switch (LoadedSaveFile.loadedSaveData.playerInventory[inventorySlot])
            //        {
            //            case 1:
            //                inventoryRef.AddItemToList("Biscuit");
            //                break;
            //            case 0:
            //                //inventoryRef.AddItemToList("");
            //                break;
            //            default:
            //                //inventoryRef.AddItemToList("");
            //                break;
            //        }
            //    }
            //}
            //else
            //{
            //    Debug.LogError("[Error] Reference to inventory is missing!");
            //}
            inventoryRef.InitInventory(); //Add seed sprites to inventory

            //Remove pickups that are already taken in save file game
            if (LoadedSaveFile.loadedSaveData.levelPickUps.Length > 0)
            {
                for (int i = 0; i < LoadedSaveFile.loadedSaveData.levelPickUps.Length; i++)
                {
                    if (LoadedSaveFile.loadedSaveData.levelPickUps[i] == false)
                    {
                        if (pickupsInLevel[i])
                        {
                            Destroy(pickupsInLevel[i].gameObject);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < pickupsInLevel.Length; i++)
                {
                    if (pickupsInLevel[i])
                    {
                        Destroy(pickupsInLevel[i].gameObject);
                    }
                }
            }

            //Load Platform positions
            if (LoadedSaveFile.loadedSaveData.platformCoordinates.Length > 0)
            {
                for (int i = 0; i < platformsInLevel.Length; i++)
                {
                    platformsInLevel[i].position = new Vector3(LoadedSaveFile.loadedSaveData.platformCoordinates[i].positionX, LoadedSaveFile.loadedSaveData.platformCoordinates[i].positionY, LoadedSaveFile.loadedSaveData.platformCoordinates[i].positionZ);
                    platformsInLevel[i].eulerAngles = new Vector3(LoadedSaveFile.loadedSaveData.platformCoordinates[i].orientationX, LoadedSaveFile.loadedSaveData.platformCoordinates[i].orientationY, LoadedSaveFile.loadedSaveData.platformCoordinates[i].orientationZ);
                }
            }
            else
            {
                Debug.LogWarning("[Warning] No platform postional data in save file.");
            }

            //Load Enemy positions
            if (LoadedSaveFile.loadedSaveData.enemiesCoordinates.Length > 0)
            {
                for (int i = 0; i < enemiesInLevel.Length; i++)
                {
                    if (LoadedSaveFile.loadedSaveData.enemiesExist[i] && enemiesInLevel[i])
                    {
                        enemiesInLevel[i].position = new Vector3(LoadedSaveFile.loadedSaveData.enemiesCoordinates[i].positionX, LoadedSaveFile.loadedSaveData.enemiesCoordinates[i].positionY, LoadedSaveFile.loadedSaveData.enemiesCoordinates[i].positionZ);
                        enemiesInLevel[i].eulerAngles = new Vector3(LoadedSaveFile.loadedSaveData.enemiesCoordinates[i].orientationX, LoadedSaveFile.loadedSaveData.enemiesCoordinates[i].orientationY, LoadedSaveFile.loadedSaveData.enemiesCoordinates[i].orientationZ);
                    }
                    else if (!LoadedSaveFile.loadedSaveData.enemiesExist[i] && enemiesInLevel[i])
                    {
                        Destroy(enemiesInLevel[i].gameObject);
                    }
                    else if (LoadedSaveFile.loadedSaveData.enemiesExist[i] && !enemiesInLevel[i])
                    {
                        Debug.LogError("[Error] enemies exists in save file, but not in level.");
                    }
                }
            }

            //Load Checkpoint position
            if (checkpoint)
            {
                checkpoint.oldSpawnPoint = checkpoint.currentSpawnPoint;
                checkpoint.currentSpawnPoint = new Vector3(LoadedSaveFile.loadedSaveData.checkpointcoordinates.positionX, LoadedSaveFile.loadedSaveData.checkpointcoordinates.positionY, LoadedSaveFile.loadedSaveData.checkpointcoordinates.positionZ);
                checkpoint.currentSpawnPointRotation = new Vector3(LoadedSaveFile.loadedSaveData.checkpointcoordinates.orientationX, LoadedSaveFile.loadedSaveData.checkpointcoordinates.orientationY, LoadedSaveFile.loadedSaveData.checkpointcoordinates.orientationZ);
            }
            else
            {
                Debug.LogWarning("[Warning] Reference to RespawnLogic.cs missing!");
            }

            yield return new WaitForSeconds(0.6f);
            Time.timeScale = 1;

        }

    }
}

