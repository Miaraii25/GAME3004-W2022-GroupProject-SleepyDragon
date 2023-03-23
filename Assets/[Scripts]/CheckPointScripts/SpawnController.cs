//*********************************************************************************************************
// Author: Jeongyeon Jane Hong, Mariam Ogunlesi
//
// Last Modified: April 15, 2022
//  
// Description: This script is used to implement Spawn Function.
//
//******************************************************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    //Note: Used Vector3 instead of GameObject because it is easier to save/load from savefile
    [HideInInspector] public Vector3 currentSpawnPoint;
    [HideInInspector] public Vector3 currentSpawnPointRotation;
    [HideInInspector] public Vector3 oldSpawnPoint;

    [SerializeField] Transform cameraTransform;

    [SerializeField]
    private int damage = 1;

    //Sound Manager
    [SerializeField]
    public SoundManagerScript soundManager;

    private void Awake()
    {
        currentSpawnPoint = this.transform.position;
        currentSpawnPointRotation = this.transform.localEulerAngles;
        oldSpawnPoint = currentSpawnPoint;
    }

    void Start()
    {
        soundManager = FindObjectOfType<SoundManagerScript>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hazard"))
        {
            //transform.position = currentSpawnPoint.transform.position;
            //transform.rotation = currentSpawnPoint.transform.rotation;
            transform.position = currentSpawnPoint;
            transform.eulerAngles = currentSpawnPointRotation;

            cameraTransform.transform.rotation = this.transform.rotation;

            gameObject.GetComponent<PlayerBehaviour>().TakeDamage(damage);
            soundManager.PlayPlayerDamagedSFX();

        }

        if (other.gameObject.CompareTag("Spawn"))
        {
            oldSpawnPoint = currentSpawnPoint;
            currentSpawnPoint = other.transform.position;
            currentSpawnPointRotation = other.transform.eulerAngles;
            //currentSpawnPoint = other.gameObject;
            //oldSpawnPoint = null;
        }
    }
    //[Header("Player")]
    //public Transform player;
    //public Transform currentSpawnPoint;
    //[HideInInspector] public Vector3 newSpawnPoint;
    //[HideInInspector] public Vector3 newSpawnPointRotation;

    //void Start()
    //{
    //    player.position = currentSpawnPoint.position;
    //    newSpawnPointRotation = this.transform.localEulerAngles;

    //}
    //public void SetCurrentSpawnPoint(Transform newSpawnPoint)
    //{
    //    currentSpawnPoint = newSpawnPoint;

    //}
}
