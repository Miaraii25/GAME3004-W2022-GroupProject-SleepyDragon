//*********************************************************************************************************************************************
// Author: Mariam Ogunlesi
//
// Last Modified: January 29, 2022
//  
// Description: Minimap Enums that Manages and also retains information regarding the loaded save files and all available save files. 
// MinimapScreen that manages all the objects that would be shown on screen
//
//***************************************************************************************************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


enum MinimapMarker
{
    NONE = 0,
    PLAYER = 1,
    ENEMY = 2,
    PICKUP = 3,
    CHECKPOINT = 4,
    HAZARD = 5,
}

public class MinimapScreen : MonoBehaviour
{
    [Header("Manual References")]                               //Serialized references
    [SerializeField] private Transform targetPlayer;             //The minimap player follow.
    [SerializeField] private Transform[] registeredEnemy;
    [SerializeField] private Transform[] registeredPickup;
    [SerializeField] private Transform[] registeredCheckpoint;

    [Header("Preset References")]    //References to objects.
    [SerializeField] private GameObject canvasContainer;         //Drag and Drop canvas gameobject here.

     //Image mask used to shape the minimap
    [SerializeField] private GameObject minimapMask;           
    [SerializeField] private GameObject minimapBorder;

    //Container object that holds the camera for the minimap view.
    [SerializeField] private Transform miniMapCamContainer;

    //The visual marker of how a player character will appear on the minimap.
    [SerializeField] private Sprite playerMinimapMarkerSprite;

    //The visual marker of how an enemy will appear on the minimap.
    [SerializeField] private Sprite enemyMinimapMarkerSprite;

    //The visual marker of how a pickup will appear on the minimap.
    [SerializeField] private Sprite pickupMinimapMarkerSprite;

    //The visual marker of how a checkpoint will appear on the minimap.
    [SerializeField] private Sprite checkpointMinimapMarkerSprite;

    [SerializeField] private GameObject miniMap;


    [Header("Minimap Settings")]
    [SerializeField] private float camFollowSpeed = 10;             //How fast the minimap camera will follow the player. 
    [SerializeField] private float miniMapSize = 256;               //How big the minimap will appear on the screen.
    [SerializeField] private float miniMapZoom = 26;                //How much ground the minimap can cover. 
    [SerializeField] private float miniMapIconSizes = 6;
    [SerializeField] private float playerIconYRotation = 0;         //This setting was added in case our player prefab forward direction differs from other objects and would need custom changes.
    [SerializeField] private float camOverheadDistance = 30;
    [SerializeField] private float iconOverheadHeight = 2;          //Note: Player scale affects this part
    [SerializeField] private bool rotateWithPlayer = false;         //Set whether or not the minimap rotates with player oriantation

    private Transform initialPlayer;                             //Used to compare with targetPlayer to check if targetPlayer has been changed.
    private Transform initialPlayerIcon;

    private void Awake()
    {
        miniMapSize = (Screen.height / 5) * 2;
        InsureCanvasExists();               //Insures that this object is within Canvas.
        ExtractCameraToRootHierarchy();     //Extract the Minimap Camera from within the prefab and unto the root of the scene hierarchy.
        ApplyMinimapLevelIcons();
    }

    private void Update()
    {
        FollowTargetPlayerWithCam();        //Allows the minimap camera to follow the player movements. 

        //If the target player character has been changed: Create new Minimap icon for the new player character.
        if (HasTargetPlayerChanged())
        {
            //Destroys previous player icon.
            if (initialPlayerIcon)
            {
                Destroy(initialPlayerIcon);
            }
            //Creates new Minimap icon for the new player character.
            AddMinimapMarker(targetPlayer, MinimapMarker.PLAYER);
        }
    }

    public void OnMapButton_Pressed()
    {
        miniMap.SetActive(!miniMap.activeInHierarchy);
    }

    //Insures that this object is within Canvas.
    private void InsureCanvasExists()
    {
        bool canvasFound = false;

        //Checks if this object is already in its proper place- as a child of Canvas object. 
        if (this.transform.parent)
        {
            if (this.transform.parent.GetComponent<Canvas>())
            {
                return;
            }
        }

        //Checks if Canvas exists with the name 'Canvas'.
        if (GameObject.Find("Canvas"))
        {
            if (GameObject.Find("Canvas").GetComponent<Canvas>())
            {
                canvasContainer = GameObject.Find("Canvas");
                this.transform.SetParent(canvasContainer.transform);
                this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(miniMapSize, miniMapSize);
            }
        }

        //Checks if Canvas exists with alias.
        else if (GameObject.Find("HUD"))
        {
            if (GameObject.Find("HUD").GetComponent<Canvas>())
            {
                canvasContainer = GameObject.Find("HUD");
                this.transform.SetParent(canvasContainer.transform);
                this.transform.SetSiblingIndex(0);
                canvasFound = true;
            }
        }
        else if (GameObject.Find("HUD"))
        {
            if (GameObject.Find("HUD").GetComponent<Canvas>())
            {
                canvasContainer = GameObject.Find("HUD");
                this.transform.SetParent(canvasContainer.transform);
                this.transform.SetSiblingIndex(0);
                canvasFound = true;
            }
        }
        else if (GameObject.Find("HUD"))
        {
            if (GameObject.Find("HUD").GetComponent<Canvas>())
            {
                canvasContainer = GameObject.Find("HUD");
                this.transform.SetParent(canvasContainer.transform);
                this.transform.SetSiblingIndex(0);
                canvasFound = true;
            }
        }

        //If Canvas object cannot be found: build one.
        if (!canvasFound)
        {
            canvasContainer = new GameObject();
            canvasContainer.gameObject.AddComponent<Canvas>();
            canvasContainer.gameObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvasContainer.gameObject.AddComponent<CanvasScaler>();
            canvasContainer.gameObject.AddComponent<GraphicRaycaster>();
            canvasContainer.name = "Canvas";
            this.transform.SetParent(canvasContainer.transform);
            initialPlayerIcon = canvasContainer.transform;
        }

        this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(miniMapSize, miniMapSize);
        this.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(miniMapSize * -0.5f, miniMapSize * -0.5f);
        minimapMask.GetComponent<RectTransform>().sizeDelta = new Vector2(miniMapSize - 3, miniMapSize - 3);
        minimapBorder.GetComponent<RectTransform>().sizeDelta = new Vector2(miniMapSize, miniMapSize);
    }

    //Extracts the Minimap Camera from within the prefab and unto the root of the scene hierarchy.
    private void ExtractCameraToRootHierarchy()
    {
        if (!miniMapCamContainer)
        {
            Debug.LogError("[Error] miniMapCamContainerRef missing! Aborting operation...");
            return;
        }

        miniMapCamContainer.SetParent(null);

        if (miniMapCamContainer.GetChild(0).GetComponent<Camera>())
        {
            miniMapCamContainer.GetChild(0).GetComponent<Camera>().orthographicSize = miniMapZoom;
        }
        else
        {
            Debug.LogError("[Error] Could not find Minimap Camera!");
        }

        if (targetPlayer)
        {
            miniMapCamContainer.position = new Vector3(targetPlayer.transform.position.x, targetPlayer.transform.position.y + camOverheadDistance, targetPlayer.transform.position.z);
        }
    }

    //Allows the minimap camera to follow the player movements. 
    //Note: This is elected over simply having the camera as a child of the target player character to avoid potential issues of this object being destroyed along with the player character.
    private void FollowTargetPlayerWithCam()
    {
        if (!targetPlayer)
        {
            return;
        }

        if (!miniMapCamContainer)
        {
            Debug.LogError("[Error] miniMapCamContainerRef missing! Aborting operation...");
            return;
        }

        //If Minimap Camera is too far from the player: teleport the camera to the player.
        if (Vector2.Distance(new Vector2(miniMapCamContainer.position.x, miniMapCamContainer.position.z), new Vector2(targetPlayer.position.x, targetPlayer.position.z)) > 10 ||
            miniMapCamContainer.position.y < targetPlayer.position.y ||
            Vector3.Distance(miniMapCamContainer.position, targetPlayer.position) > (10 + camOverheadDistance))
        {
            miniMapCamContainer.position = new Vector3(targetPlayer.position.x, targetPlayer.position.y + camOverheadDistance, targetPlayer.position.z);
            return;
        }

        //Set minimap Camera to follow the player
        miniMapCamContainer.position = Vector3.MoveTowards(miniMapCamContainer.position, new Vector3(targetPlayer.position.x, targetPlayer.position.y + camOverheadDistance, targetPlayer.position.z), camFollowSpeed * Time.deltaTime);

        if (rotateWithPlayer)
        {
            miniMapCamContainer.eulerAngles = new Vector3(0, targetPlayer.localEulerAngles.y + playerIconYRotation, 0);
        }

    }

    //Checks if the targetPlayerRef has changed since last checked.
    private bool HasTargetPlayerChanged()
    {
        if (targetPlayer == initialPlayer)
        {
            return false;
        }

        initialPlayer = targetPlayer;
        return true;
    }

    //Creates an icon that will represent the given target object on the minimap
    //Note: This marker object should only be seen by the minimap camera; turn off the Minimap Marker layer on other cameras.
    private void AddMinimapMarker(Transform _targetObj, MinimapMarker _markerType)
    {
        GameObject minimapMarker;

        //Temp marker type check. Will need to be modified as more marker types are accomodated.
        if (!(_markerType == MinimapMarker.PLAYER || _markerType == MinimapMarker.ENEMY ||
              _markerType == MinimapMarker.PICKUP || _markerType == MinimapMarker.CHECKPOINT))
        {
            Debug.LogError("[Error] Invalid minimap marker type; Aborting operation...");
            return;
        }

        minimapMarker = new GameObject();
        minimapMarker.name = "Minimap Icon";
        minimapMarker.layer = LayerMask.NameToLayer("Minimap Marker");
        minimapMarker.AddComponent<SpriteRenderer>();

        switch (_markerType)
        {
            case MinimapMarker.NONE:
                return;
            case MinimapMarker.PLAYER:
                if (playerMinimapMarkerSprite)
                {
                    minimapMarker.transform.localScale = new Vector3(miniMapIconSizes, miniMapIconSizes, 1);
                    minimapMarker.transform.position = new Vector3(_targetObj.position.x, _targetObj.position.y + iconOverheadHeight, _targetObj.position.z);
                    minimapMarker.transform.localEulerAngles = new Vector3(90, _targetObj.localEulerAngles.y, _targetObj.localEulerAngles.z);
                    minimapMarker.transform.SetParent(_targetObj);
                    minimapMarker.GetComponent<SpriteRenderer>().sprite = playerMinimapMarkerSprite;

                    //Custom settings
                    //minimapMarker.transform.localScale = new Vector3(playerIconSize, playerIconSize, 1);
                    if (playerIconYRotation != 0)
                    {
                        minimapMarker.transform.localEulerAngles = new Vector3(90, playerIconYRotation, 0);
                    }
                }
                else
                {
                    Debug.LogError("[Error] Player minimap marker material reference missing!");
                }
                break;
            case MinimapMarker.ENEMY:
                if (enemyMinimapMarkerSprite)
                {
                    minimapMarker.transform.localScale = new Vector3(miniMapIconSizes, miniMapIconSizes, 1);
                    minimapMarker.transform.position = new Vector3(_targetObj.position.x, _targetObj.position.y + iconOverheadHeight - 1, _targetObj.position.z);
                    minimapMarker.transform.localEulerAngles = new Vector3(90, _targetObj.localEulerAngles.y, _targetObj.localEulerAngles.z);
                    minimapMarker.transform.SetParent(_targetObj);
                    minimapMarker.GetComponent<SpriteRenderer>().sprite = enemyMinimapMarkerSprite;
                }
                else
                {
                    Debug.LogError("[Error] Enemy minimap marker material reference missing!");
                }
                break;
            case MinimapMarker.PICKUP:
                if (pickupMinimapMarkerSprite)
                {
                    minimapMarker.transform.localScale = new Vector3(miniMapIconSizes * 0.8f, miniMapIconSizes * 0.8f, 1);
                    minimapMarker.transform.position = new Vector3(_targetObj.position.x, _targetObj.position.y + iconOverheadHeight - 2, _targetObj.position.z);
                    minimapMarker.transform.localEulerAngles = new Vector3(90, 0, 0);
                    minimapMarker.transform.SetParent(_targetObj);
                    minimapMarker.GetComponent<SpriteRenderer>().sprite = pickupMinimapMarkerSprite;
                }
                else
                {
                    Debug.LogError("[Error] Pickup minimap marker material reference missing!");
                }
                break;
            case MinimapMarker.CHECKPOINT:
                if (checkpointMinimapMarkerSprite)
                {
                    minimapMarker.transform.localScale = new Vector3(miniMapIconSizes, miniMapIconSizes, 1);
                    minimapMarker.transform.position = new Vector3(_targetObj.position.x, _targetObj.position.y + iconOverheadHeight - 2, _targetObj.position.z);
                    minimapMarker.transform.localEulerAngles = new Vector3(90, 0, 0);
                    minimapMarker.transform.SetParent(_targetObj);
                    minimapMarker.GetComponent<SpriteRenderer>().sprite = checkpointMinimapMarkerSprite;
                }
                else
                {
                    Debug.LogError("[Error] Checkpoint minimap marker material reference missing!");
                }
                break;
            default:
                Debug.LogError("[Error] Invalid minimap marker type; Aborting operation...");
                return;
        }

    }

    //Apply minimap icons on player, enemy, pickups, and checkpoints
    private void ApplyMinimapLevelIcons()
    {

        //Checks if player exists, but doesn't have a minimap icon. If so: add icon.
        if (targetPlayer && !initialPlayerIcon)
        {
            HasTargetPlayerChanged();
            AddMinimapMarker(targetPlayer, MinimapMarker.PLAYER);
        }

        //Checks if there are any enemies registered to be tracked on the minimap. If so: add icon(s).
        if (registeredEnemy.Length > 0)
        {
            for (int enemyIndex = 0; enemyIndex < registeredEnemy.Length; enemyIndex++)
            {
                if (registeredEnemy[enemyIndex])
                {
                    AddMinimapMarker(registeredEnemy[enemyIndex], MinimapMarker.ENEMY);
                }
            }
        }

        //Checks if there are any pickups registered to be tracked on the minimap. If so: add icon(s).
        if (registeredPickup.Length > 0)
        {
            for (int pickupIndex = 0; pickupIndex < registeredPickup.Length; pickupIndex++)
            {
                if (registeredPickup[pickupIndex])
                {
                    AddMinimapMarker(registeredPickup[pickupIndex], MinimapMarker.PICKUP);
                }
            }
        }

        //Checks if there are any checkpoints registered to be tracked on the minimap. If so: add icon(s).
        if (registeredCheckpoint.Length > 0)
        {
            for (int checkpointIndex = 0; checkpointIndex < registeredCheckpoint.Length; checkpointIndex++)
            {
                if (registeredCheckpoint[checkpointIndex])
                {
                    AddMinimapMarker(registeredCheckpoint[checkpointIndex], MinimapMarker.CHECKPOINT);
                }
            }
        }

       
    }

    //Sets new player character that the minimap cam will follow.
    public void SetTargetPlayer(Transform _insertPlayer)
    {
        targetPlayer = _insertPlayer;
        miniMapCamContainer.position = new Vector3(targetPlayer.transform.position.x, targetPlayer.transform.position.y + 10, targetPlayer.transform.position.z);

        //If the target player character has been changed: Create new Minimap icon for the new player character.
        if (HasTargetPlayerChanged())
        {
            AddMinimapMarker(targetPlayer, MinimapMarker.PLAYER);
        }

        miniMapCamContainer.position = new Vector3(targetPlayer.transform.position.x, targetPlayer.transform.position.y + 30, targetPlayer.transform.position.z);
    }

    //Toggle whether or not the minimap is visible
    public void ToggleMiniMapVisibility()
    {
        //If minimapMask cannot be found, try to find it. If it still cannot be found return with error log.
        if (!minimapMask)
        {
            if (this.transform.GetChild(0))
            {
                if (this.transform.GetChild(0).gameObject.GetComponent<Mask>())
                {
                    minimapMask = this.transform.GetChild(0).gameObject;
                }
            }
            if (!minimapMask)
            {
                Debug.LogError("[Error] Minimap mask reference missing! Aborting operation...");
                return;
            }
        }

        if (minimapMask.activeSelf)
        {
            minimapMask.SetActive(true); //Turn off Minimap visual 
        }
        else
        {
            minimapMask.SetActive(false);  //Turn on Minimap visual
        }
    }

    //Set whether or not the minimap is visible.
    public void SetMiniMapVisibility(bool _set)
    {
        //If minimapMask cannot be found, try to find it. If it still cannot be found return with error log.
        if (!minimapMask)
        {
            if (this.transform.GetChild(0))
            {
                if (this.transform.GetChild(0).gameObject.GetComponent<Mask>())
                {
                    minimapMask = this.transform.GetChild(0).gameObject;
                }
            }
            if (!minimapMask)
            {
                Debug.LogError("[Error] Minimap mask reference missing! Aborting operation...");
                return;
            }
        }

        //Set Minimap visual
        minimapMask.SetActive(_set);
    }

    //Adjusts how big the minimap will appear in the screen.
    public void SetMinimapSize(float _newMiniMapSize)
    {
        if (!minimapMask)
        {
            Debug.LogError("[Error] minimapMaskRef missing! Aborting operation...");
            return;
        }

        if (_newMiniMapSize <= 0)
        {
            Debug.LogError("[Error] Invalid minimap size! Aborting operation...");
            return;
        }

        miniMapSize = _newMiniMapSize;
        this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(miniMapSize, miniMapSize);
        this.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(miniMapSize * -0.5f, miniMapSize * -0.5f);
        minimapMask.GetComponent<RectTransform>().sizeDelta = new Vector2(miniMapSize, miniMapSize);
    }

    //Adjusts how much ground the minimap can cover. Like an aerial view zoom effect.
    public void SetMinimapZoom(float _newZoomAmount)
    {
        if (!miniMapCamContainer)
        {
            Debug.LogError("[Error] miniMapCamContainerRef missing! Aborting operation...");
            return;
        }

        if (!miniMapCamContainer.GetChild(0).GetComponent<Camera>())
        {
            Debug.LogError("[Error] Could not find Minimap Camera! Aborting operation...");
            return;
        }

        if (_newZoomAmount <= 0)
        {
            Debug.LogError("[Error] Invalid camera zoom amount! Aborting operation...");
            return;
        }

        miniMapZoom = _newZoomAmount;
        miniMapCamContainer.GetChild(0).GetComponent<Camera>().orthographicSize = miniMapZoom;
    }

    //Adjusts the size of icons in the minimap
    public void SetIconSize(float _newMiniMapIconSize)
    {
        if (!initialPlayerIcon)
        {
            return;
        }

        if (_newMiniMapIconSize <= 0)
        {
            Debug.LogError("[Error] Invalid icon size! Aborting operation...");
            return;
        }

        miniMapIconSizes = _newMiniMapIconSize;
        initialPlayerIcon.localScale = new Vector3(miniMapIconSizes, miniMapIconSizes, 1);

        //TODO adjust other icons here
    }

   
}
