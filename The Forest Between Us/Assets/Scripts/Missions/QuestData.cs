using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Day_XX_Quest", menuName = "Quest System/Quest Data")]
public class QuestData : ScriptableObject
{
    [Header("Quest Identity")]
    public int dayID;               // The day this quest appears (1-30)
    public string questTitle;       // Example: "Static Echoes"
    
    [TextArea(3, 10)]
    public string storyIntro;       // Narrative text shown at start of day

    [Header("Objectives List")]
    public List<QuestStep> steps = new List<QuestStep>();

    [Header("Moral Alignment")]
    public int karmaImpact;         // Positive for Happy Ending, Negative for Bad

    [HideInInspector] public bool isCompleted;
}

[System.Serializable]
public class QuestStep
{
    public string description;      // Example: "Find the old Radio"
    public StepType type;           // What does the player need to do?
    public int targetAmount = 1;    // How many (e.g., collect 5 wood)
    public int currentAmount = 0;   
    public bool isFinished;         // Is THIS specific step done?
}

public enum StepType 
{ 
    Movement,    // Walking/Running/Jumping
    Collect,     // Picking up items
    Interaction, // Pressing E on objects (Radio, Door, etc.)
    ReachTarget, // Going to a specific area (Trigger Zone)
    Survival     // Staying alive or using the Radio to repel threats
}