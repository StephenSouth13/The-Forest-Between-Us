using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    public static GameDirector instance;

    [Header("Player Settings")]
    public float walkSpeed = 3.5f;
    public float runSpeed = 6.0f;
    public float jumpHeight = 1.2f;

    [Header("Game Balance Settings")]
    public float globalHungerDepletionRate = 0.5f;
    public float globalThirstDepletionRate = 0.8f;
    public float globalStaminaDepletionRate = 1.2f;
    public float dayNightCycleSpeed = 1.0f;

    [Header("Combat & Death Settings")]
    public float enemyDamageMultiplier = 1.0f;
    public float respawnHealthPercentage = 100f; // Bao nhiêu % máu khi hồi sinh
    public bool dropItemsOnDeath = true; // Mất đồ khi chết không?
    public int respawnKarmaPenalty = 10; // Trừ Karma khi chết

    [System.Serializable]
    public class DialogueData
    {
        public string characterName;
        [TextArea(2, 4)]
        public string textLine;
        public string option1Text;
        public string option2Text;
        public ItemData rewardItem;
    }

    [Header("Story & Dialogue Configuration")]
    public List<DialogueData> mainQuestDialogues = new List<DialogueData>();
    
    [Header("Trading Configuration")]
    public ItemData globalCurrencyItem;
    
    [System.Serializable]
    public class TradeListing
    {
        public ItemData itemToSell;
        public int priceInCurrency;
    }

    public List<TradeListing> merchantInventory = new List<TradeListing>();

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("[GameDirector] Ultimate Developer Dashboard initialized. Overriding global game settings.");
    }
}
