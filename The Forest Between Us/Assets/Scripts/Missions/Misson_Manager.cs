using UnityEngine;

public class Misson_Manager : MonoBehaviour
{
    public static Misson_Manager instance;

    [Header("Time Settings")]
    public int currentDay = 1;
    public float timeMultiplier = 1f; // Tốc độ trôi thời gian
    private float timer;
    public float secondsPerDay = 300f; // 5 phút một ngày

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    void Update()
    {
        // Chạy thời gian: Time.deltaTime là thời gian thực giữa các khung hình
        timer += Time.deltaTime * timeMultiplier;

        if (timer >= secondsPerDay)
        {
            NextDay();
            timer = 0;
        }
    }

    void NextDay()
    {
        currentDay++;
        Debug.Log("Chào mừng ngày thứ: " + currentDay);

        // Gọi QuestManager để cập nhật nhiệm vụ mới
        if (Quest_Manager.instance != null)
        {
            Quest_Manager.instance.ActivateQuestForDay(currentDay);
        }
    }
}