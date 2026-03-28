using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Day_XX_Quest", menuName = "Quest System/Quest Data")]
public class QuestData : ScriptableObject
{
    [Header("Quest Identity")]
    public int dayID;               
    public string questTitle;       
    
    [TextArea(3, 10)]
    public string storyIntro;       

    [Header("Objectives List")]
    public List<QuestStep> steps = new List<QuestStep>();

    [Header("Moral Alignment")]
    public int karmaImpact;         

    [HideInInspector] public bool isCompleted;
}

[System.Serializable]
public class QuestStep
{
    public string description;      
    public StepType type;           
    public float targetAmount = 1;  // Đổi thành float để đo được cả Mét (m)
    public float currentAmount = 0;   
    public bool isFinished;         
}

public enum StepType 
{ 
    Movement,    
    Collect,     
    Interaction, 
    ReachTarget, 
    Survival     
}