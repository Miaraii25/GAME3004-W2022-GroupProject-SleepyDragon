//*********************************************************************************************************
// Author: Jeongyeon Jane Hong & Pauleen Lam
//
// Last Modified: February 5, 2022
//  
// Description: This script is used to implement hazard responses.
//
//******************************************************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum HazardType
{
    PIT,
    DAMAGE,
    DEATHPLANE,
    AXE,
    GRINDER,
    SPIKE,
    LAVA,
    ENEMY
}

public class HazardController : MonoBehaviour
{
    // Spawnpoint
    [SerializeField]
    private SpawnController spawnController;

    // Enum for type of hazard
    [SerializeField]
    private HazardType type = HazardType.DAMAGE;

    // Value for hazard's damage on player
    [SerializeField]
    private int damage = 1;

    //Sound Manager
    [SerializeField]
    public SoundManagerScript soundManager;

    void Start()
    {
        soundManager = FindObjectOfType<SoundManagerScript>();

        SetDefaultHazardsSFX();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // Pit hazards will send player back to spawnpoint as well as deal damage
            if (type == HazardType.PIT)
            {
                Debug.Log("Fell into pit (water/lava)");
                soundManager.PlayLavaPitDamageSFX();
                //other.transform.position = spawnController.currentSpawnPoint.position;
                other.GetComponent<PlayerBehaviour>().TakeDamage(damage);
            }

            // Axe hazards will give a deal damage to player
            if (type == HazardType.AXE)
            {
                Debug.Log("Hit Axe");
                soundManager.PlayPlayerDamagedSFX();
                other.GetComponent<PlayerBehaviour>().TakeDamage(damage);
            }

            // Axe hazards will give a deal damage to player
            if (type == HazardType.GRINDER)
            {
                Debug.Log("Hit Grinder");
                soundManager.PlayPlayerDamagedSFX();
                other.GetComponent<PlayerBehaviour>().TakeDamage(damage);
            }

            // Damage hazards will deal damage only
            if (type == HazardType.DAMAGE)
            {
                Debug.Log("Recieved damage!");
                other.GetComponent<PlayerBehaviour>().TakeDamage(damage);
            }

            // Damage hazards will deal damage only
            if (type == HazardType.SPIKE)
            {
                Debug.Log("Recieved damage!");
                soundManager.PlaySpikePitDamageSFX();
                other.GetComponent<PlayerBehaviour>().TakeDamage(damage);
            }

            // Damage hazards will deal damage only
            if (type == HazardType.LAVA)
            {
                Debug.Log("Recieved damage!");
                soundManager.PlayLavaPitDamageSFX();
                other.GetComponent<PlayerBehaviour>().TakeDamage(damage);
            }

            // Enemies damage
            if (type == HazardType.ENEMY)
            {
                Debug.Log("Enemy touch!");
                other.gameObject.GetComponent<PlayerBehaviour>().TakeDamage(damage);
                soundManager.PlayPlayerDamagedSFX();

            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            // Deathplanes send player back to last spawnpoint and deals significant damage
            if (type == HazardType.DEATHPLANE)
            {
                Debug.Log("Hit Death Plane");
                //other.transform.position = spawnController.currentSpawnPoint.position;
                other.gameObject.GetComponent<PlayerBehaviour>().TakeDamage(damage);
            }

            // Enemies damage
            if (type == HazardType.ENEMY)
            {
                Debug.Log("Enemy touch!");
                other.gameObject.GetComponent<PlayerBehaviour>().TakeDamage(damage);
                soundManager.PlayPlayerDamagedSFX();
                
            }
        }
    }

    public void SetDefaultHazardsSFX()
    {
        switch (type)
        {
            case HazardType.AXE:
                //soundManager.PlaySwingAxeSFX();
                break;
            case HazardType.GRINDER:
                if (soundManager)
                {
                    soundManager.PlayRollingGrinderLoopSFX();
                }
                break;
            case HazardType.LAVA:
                if (soundManager)
                {
                    soundManager.PlayLavaPitLoopSFX();
                }
                break;
            case HazardType.PIT:
                if (soundManager)
                {
                    soundManager.PlayLavaPitLoopSFX();
                }
                break;
        }
    }
}
