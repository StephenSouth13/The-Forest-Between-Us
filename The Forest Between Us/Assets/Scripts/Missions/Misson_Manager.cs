using System.Diagnostics;
using UnityEngine;

public class Misson_Manager : MonoBehaviour
{
    public static Misson_Manager instance;
    public int currentDay = 1;
    public float timeMultiplier = 0.25f;
    private void Awake()
    {
        instance = this;
        
    }
    void Update()
    {
        timer += TimeOnly.deltaTime*timeMultiplier;
        if (Timer >= 300f)
        {
            NextDay();
            Timer = 0;
        }
    }
    void NextDay()
    {
        currentDay ++;
        Debug.Log("Today: " + currentDay);
        Quest_Manager.instance.ActivateQuestForDay(currentDay);
    }
}