using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndingUIController : MonoBehaviour
{
    [Header("Main Choice Panel")]
    public GameObject choicePanel;
    public Button btnEnding1_ReturnHome;
    public Button btnEnding2_MaiAnTiem;
    public Button btnEnding3_DarkKing;
    public Button btnEnding4_Sacrifice;
    public Button btnEnding5_TimeLoop;

    [Header("Ending Story Cutscene Panel")]
    public GameObject resultCutscenePanel;
    public TextMeshProUGUI endingTitleText;
    public TextMeshProUGUI endingStoryText;
    public Button btnRestartGame;

    void Start()
    {
        if (choicePanel != null) choicePanel.SetActive(false);
        if (resultCutscenePanel != null) resultCutscenePanel.SetActive(false);

        if (btnEnding1_ReturnHome != null) btnEnding1_ReturnHome.onClick.AddListener(() => TriggerEnding(EndingType.EndingA_ReturnHome));
        if (btnEnding2_MaiAnTiem != null) btnEnding2_MaiAnTiem.onClick.AddListener(() => TriggerEnding(EndingType.EndingB_MaiAnTiemEcho));
        if (btnEnding3_DarkKing != null) btnEnding3_DarkKing.onClick.AddListener(() => TriggerEnding(EndingType.EndingC_DarkKing));
        if (btnEnding4_Sacrifice != null) btnEnding4_Sacrifice.onClick.AddListener(() => TriggerEnding(EndingType.EndingD_SilentSacrifice));
        if (btnEnding5_TimeLoop != null) btnEnding5_TimeLoop.onClick.AddListener(() => TriggerEnding(EndingType.EndingE_TimeLoopSecret));

        if (btnRestartGame != null) btnRestartGame.onClick.AddListener(OnRestartClicked);
    }

    public void OpenChoiceWindow()
    {
        if (choicePanel != null)
        {
            choicePanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void TriggerEnding(EndingType type)
    {
        if (choicePanel != null) choicePanel.SetActive(false);
        if (EndingManager.instance != null)
        {
            EndingManager.instance.ChooseEnding(type);
        }
    }

    void OnRestartClicked()
    {
        if (SceneTransitionManager.instance != null)
        {
            SceneTransitionManager.instance.TransitionToScene("Home");
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Home");
        }
    }
}
