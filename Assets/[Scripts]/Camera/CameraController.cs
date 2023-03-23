//*********************************************************************************************************
// Author: Pauleen Lam
//
// Last Modified: March 10, 2022
//  
// Description: This script is used to move the camera based on input or scene.
//
//******************************************************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public enum CameraPosition
{
    LEFT,
    RIGHT,
    FRONT,
    BACK
}

public class CameraController : MonoBehaviour
{
    // Player reference
    [SerializeField]
    private GameObject player;
    private PlayerBehaviour playerBehaviour;

    // Camera attributes
    [SerializeField]
    public float cameraRotationSensitivity = 30;

    // Input System reference value
    Vector2 lookVector = Vector2.zero;

    // Camera animation components
    private bool autoTransitioning = false;
    private Quaternion startAngle;
    private Quaternion targetAngle;
    private float time;
    private float transitionDuration = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerBehaviour = player.GetComponent<PlayerBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        // Update look vecter from player
        lookVector = playerBehaviour.lookVector;
        // Follow the player's position
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 0.2f, player.transform.position.z);

        if (!autoTransitioning) // If not auto transitioning, let hte player adjust the camera angle
        {
            // Rotate the camera based on Vector2 values received from PlayerActionMap
            transform.rotation *= Quaternion.AngleAxis(lookVector.x * cameraRotationSensitivity * Time.deltaTime, Vector3.up);
            transform.rotation *= Quaternion.AngleAxis(lookVector.y * cameraRotationSensitivity * Time.deltaTime, Vector3.left);
            var angle = transform.localEulerAngles;
            angle.z = 0;
            transform.localEulerAngles = angle;
        }
        else
        {
            time += Time.deltaTime;
            float xRot = Mathf.Lerp(startAngle.x, targetAngle.x, time / transitionDuration); 
            float yRot = Mathf.Lerp(startAngle.y, targetAngle.y, time / transitionDuration);
            transform.rotation.Set(xRot, yRot, transform.rotation.z, transform.rotation.w);
            if (transform.rotation.x == targetAngle.x && transform.rotation.y == targetAngle.y)
            {
                autoTransitioning = false;
                time = 0.0f;
            }
        }
    }

    // function called by camera adjusting mesh bounds. will change the camera's position/rotation around player
    public void RepositionCamera(CameraPosition pos, float xRot = 12)
    {
        Quaternion newAngle = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        // Enum determines what angle the CameraController should be
        switch (pos)
        {
            case CameraPosition.LEFT:
                newAngle = Quaternion.Euler(xRot, 90.0f, 0.0f);
                break;
            case CameraPosition.RIGHT:
                newAngle = Quaternion.Euler(xRot, -90.0f, 0.0f);
                break;
            case CameraPosition.FRONT:
                newAngle = Quaternion.Euler(xRot, 180.0f, 0.0f);
                break;
            case CameraPosition.BACK:
                newAngle = Quaternion.Euler(xRot, 0.0f, 0.0f);
                break;
        }

        newAngle.z = transform.rotation.z;

        transform.rotation = newAngle;

        // For camera animation
        //targetAngle = newAngle;
        //autoTransitioning = true;
        //startAngle = transform.rotation;
    }
}
