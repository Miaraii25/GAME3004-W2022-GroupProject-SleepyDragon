using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


class InventoryException : System.Exception
{
    public InventoryException(string message) : base(message)
    {
    }
}

public class InventorySystem : MonoBehaviour
{
    public const int m_kNumItems = 8;
    //public GameObject spriteEmpty;
    public GameObject[] itemImagesList;
    public Button[] itemButtonList;
    public PlayerHealth playerHealth;
    bool[] itemUsedList;
    bool m_bIsEnabled;

    //Sound manager
    [SerializeField]
    public SoundManagerScript soundManager;
    // Start is called before the first frame update
    void Start()
    {
        InitInventory();
        soundManager = FindObjectOfType<SoundManagerScript>();
    }

    public void InitInventory()
    {
        m_bIsEnabled = (playerHealth.currentHealth < playerHealth.maxhealth);
        //1.init panels
         itemUsedList = new bool[m_kNumItems];
        for (int i = 0; i < m_kNumItems; i++)
        {
            int idx = i;
            itemButtonList[i].onClick.AddListener(delegate {ItemButtonOnClick(idx); });
            itemButtonList[i].interactable = m_bIsEnabled;
            itemUsedList[i] = false;
           
        }
    }


    // Update is called once per frame
    void Update()
    {
        bool bIsMaxHealth = (playerHealth.currentHealth == playerHealth.maxhealth);
        if (!m_bIsEnabled && !bIsMaxHealth)
        {
            m_bIsEnabled = true;
           for (int i = 0; i < m_kNumItems; i++)
           {
                if(!itemUsedList[i])
                   itemButtonList[i].interactable = true;
              
            }
        }
        else if(m_bIsEnabled  && bIsMaxHealth)
        {
            m_bIsEnabled = false;
            for (int i = 0; i < m_kNumItems; i++)
            {
                if (!itemUsedList[i])
                    itemButtonList[i].interactable = false;

            }
        }

    }

    public void ItemButtonOnClick(int idx)
    {
        itemImagesList[idx].GetComponent<Image>().sprite = null;
        var tempColor = itemImagesList[idx].GetComponent<Image>().color;
        tempColor.a = 0f;
        itemImagesList[idx].GetComponent<Image>().color = tempColor;
        itemButtonList[idx].interactable = false;
        itemUsedList[idx] = true;
        playerHealth.AddHealth(1);

        //Play SFX for item click
        soundManager.PlayPlayerHealSFX();
    }

    
}
