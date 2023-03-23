//**************************************************************************************************************************************************************************************************************************************
// Author: Mariam Ogunlesi
//
// Last Modified: March 12, 2022
//  
// Description: This Loads the Menu screen with the option to choose which level to reload
//  
//  
//
//*******************************************************************************************************************************************************************************************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;


public class LoadMenuScreen : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI saveSlot1;
    [SerializeField] private TextMeshProUGUI saveSlot2;
    [SerializeField] private TextMeshProUGUI saveSlot3;
    [SerializeField] private TextMeshProUGUI saveSlot4;
    [SerializeField] private TextMeshProUGUI saveSlot5;

    [Header("Name")]
    [SerializeField] private string savefileName = "Pawggers";       //Pawggers is the name of the save file. An indexing number will be appended to this name. This is different from the save file header seen in-game.
    private string[] saveFileDisplayHeaders;                            //This game will have a maximum 4 or more save slots .
    private string gameVersion = "0.4";

    private void Start()
    {
        SetUpSaveSlotHeaders();
    }

    private string GetSaveSlotHeader(int _saveSlotIndex)
    {

        if (_saveSlotIndex < 1 || _saveSlotIndex > 5)
        { 
            //This game will have a maximum 4 or more save slots .
            Debug.LogError("[Error] Invalid save slot index! Slot number must be between from 1 to 4 or 10.");
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
                Debug.LogError("[Error] availableSaveFiles array not initialized!");
                return "[Error] availableSaveFiles array not initialized!";
            }
        }
        else
        {
            Debug.LogError("[Error] availableSaveFiles array not initialized!");
            return "[Error] availableSaveFiles array not initialized!";
        }

        return saveFileDisplayHeaders[_saveSlotIndex - 1];
    }

    private void SetUpSaveSlotHeaders()
    {
        saveSlot1.text = GetSaveSlotHeader(1);
        saveSlot2.text = GetSaveSlotHeader(2);
        saveSlot3.text = GetSaveSlotHeader(3);
        saveSlot4.text = GetSaveSlotHeader(4);
        saveSlot5.text = GetSaveSlotHeader(5);

    }

    //Loads save file data at given save slot index
    private bool LoadSaveFile(int saveSlotIndex)
    {
        //This game will have a maximum 4 or more save slots .
        if (saveSlotIndex <= 0 || saveSlotIndex > 5)
        {
            Debug.LogError("[Error] Invalid save slot index! Slot number must be between from 1 to 5.");
            return false;
        }

        if (!File.Exists(Application.persistentDataPath + "/" + savefileName + saveSlotIndex + ".pawsave"))
        {
            Debug.LogError("[Error] File does not exist; Cannot load a save file that does not exist.");
            return false;
        }

        SaveData readSaveData = SaveFileReaderWriter.ReadFromSaveFile(Application.persistentDataPath + "/" + savefileName + saveSlotIndex + ".pawsave");

        if (this.gameVersion != readSaveData.gameVersion)
        {
            Debug.LogWarning("[Warning] Cannot load save file; incompatible version. ");
            return false;
        }

        LoadedSaveFile.loadedSaveData = readSaveData;

        return true;
    }

    private void LoadGameFromSave(int saveSlotIndex)
    {
        //Load Save File
        if (!LoadSaveFile(saveSlotIndex))
        {
            return;
        }

        //A Game scene will load a game state from save file on Awake() if this value is set to true
        LoadedSaveFile.loadLevelBasedOnSaveFile = true;

        //Load appropriate scene
        switch (LoadedSaveFile.loadedSaveData.currentLevel)
        {
            case 0:
                //Don't load anything, but stats are loaded anyway because they're on LoadedSaveFile.loadedSaveData
                break;
            case 1:
                SceneManager.LoadScene(0);
                break;
            default:
                //Don't load anything, but stats are loaded anyway because they're on LoadedSaveFile.loadedSaveData
                break;
        }

    }

    public void OnClickSaveSlot(int slotIndex)
    {

        if (slotIndex < 1 || slotIndex > 5)
        { //This game will have a maximum 4 save slots hardcoded.
            Debug.LogError("[Error] Invalid save slot index! Slot number must be between from 1 to 5.");
            return;
        }

        if (saveFileDisplayHeaders.Length != 5)
        {
            Debug.LogError("[Error] saveFileDisplayHeaders[_slotIndex].Length is not 5. Length is: " + saveFileDisplayHeaders[slotIndex].Length);
            return;
        }

        //Empty Save Slots do nothing
        if (saveFileDisplayHeaders[slotIndex - 1] == "Empty Save Slot")
        {
            return;
        }

        LoadSaveFile(slotIndex);
        LoadGameFromSave(slotIndex);
    }

}
