//*********************************************************************************************************
// Author: Jeongyeon Jane Hong
//
// Last Modified: February 5, 2022
//  
// Description: This script is used to implement Check Point Function.
//
//******************************************************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointTriggerController : MonoBehaviour
{
    [Header("Spawn Point")]
    public Transform spawnPoint;

    private SpawnController spawnController;
    // Start is called before the first frame update
    void Start()
    {
        spawnController = GameObject.FindObjectOfType<SpawnController>();
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.CompareTag("Player"))
        //{
        //    Debug.Log("Hit CheckPoint");
        //    spawnController.SetCurrentSpawnPoint(spawnPoint);
        //}

        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Hit CheckPoint");
            // Spawn player to currentSpawnPoint
            //spawnController.SetCurrentSpawnPoint(spawnPoint);
        }
    }
}
