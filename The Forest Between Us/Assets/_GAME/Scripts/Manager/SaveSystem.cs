using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem instance;

    private const string KEY_CURRENT_DAY = "Save_CurrentDay";
    private const string KEY_PLAYER_KARMA = "Save_PlayerKarma";
    private const string KEY_TUTORIAL_DONE = "Save_TutorialDone";

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public static void SaveProgress(int currentDay, int karma, bool tutorialDone = true)
    {
        PlayerPrefs.SetInt(KEY_CURRENT_DAY, currentDay);
        PlayerPrefs.SetInt(KEY_PLAYER_KARMA, karma);
        PlayerPrefs.SetInt(KEY_TUTORIAL_DONE, tutorialDone ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log($"[SaveSystem] Saved progress: Day {currentDay}, Karma {karma}");
    }

    public static bool LoadProgress(out int currentDay, out int karma, out bool tutorialDone)
    {
        if (!PlayerPrefs.HasKey(KEY_CURRENT_DAY))
        {
            currentDay = 1;
            karma = 50;
            tutorialDone = false;
            return false;
        }

        currentDay = PlayerPrefs.GetInt(KEY_CURRENT_DAY, 1);
        karma = PlayerPrefs.GetInt(KEY_PLAYER_KARMA, 50);
        tutorialDone = PlayerPrefs.GetInt(KEY_TUTORIAL_DONE, 0) == 1;
        Debug.Log($"[SaveSystem] Loaded progress: Day {currentDay}, Karma {karma}");
        return true;
    }

    public static void ClearSaveData()
    {
        PlayerPrefs.DeleteKey(KEY_CURRENT_DAY);
        PlayerPrefs.DeleteKey(KEY_PLAYER_KARMA);
        PlayerPrefs.DeleteKey(KEY_TUTORIAL_DONE);
        PlayerPrefs.Save();
        Debug.Log("[SaveSystem] Cleared all saved data.");
    }
}
