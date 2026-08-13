using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum EndingType
{
    EndingA_ReturnHome,
    EndingB_MaiAnTiemEcho,
    EndingC_DarkKing,
    EndingD_SilentSacrifice,
    EndingE_TimeLoopSecret
}

public class EndingManager : MonoBehaviour
{
    public static EndingManager instance;

    [Header("Moral Karma System")]
    public int playerKarma = 50; // 0 to 100

    [Header("Player Survival Lifetime Stats")]
    public int totalDaysSurvived = 1;
    public int totalTreesChopped = 0;
    public int totalRocksMined = 0;
    public int totalMonstersKilled = 0;
    public int totalItemsCrafted = 0;

    [Header("UI Panels")]
    public GameObject endingChoicePanel;
    public Button optionAButton;
    public Button optionBButton;
    public Button optionCButton;
    public Button optionDButton;
    public Button optionEButton;

    [Header("Ending Cutscene Overlay")]
    public GameObject endingScreenPanel;
    public TextMeshProUGUI endingTitleText;
    public TextMeshProUGUI endingDescText;
    public TextMeshProUGUI statsSummaryText; // Bảng tổng kết thống kê chỉ số
    public Button returnHomeButton;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (endingChoicePanel != null) endingChoicePanel.SetActive(false);
        if (endingScreenPanel != null) endingScreenPanel.SetActive(false);

        if (optionAButton != null) optionAButton.onClick.AddListener(() => ChooseEnding(EndingType.EndingA_ReturnHome));
        if (optionBButton != null) optionBButton.onClick.AddListener(() => ChooseEnding(EndingType.EndingB_MaiAnTiemEcho));
        if (optionCButton != null) optionCButton.onClick.AddListener(() => ChooseEnding(EndingType.EndingC_DarkKing));
        if (optionDButton != null) optionDButton.onClick.AddListener(() => ChooseEnding(EndingType.EndingD_SilentSacrifice));
        if (optionEButton != null) optionEButton.onClick.AddListener(() => ChooseEnding(EndingType.EndingE_TimeLoopSecret));

        if (returnHomeButton != null) returnHomeButton.onClick.AddListener(ReturnToMainMenu);
    }

    public void ModifyKarma(int amount)
    {
        playerKarma = Mathf.Clamp(playerKarma + amount, 0, 100);
        Debug.Log($"Karma updated: {playerKarma}");
    }

    public void RecordTreeChopped() { totalTreesChopped++; }
    public void RecordRockMined() { totalRocksMined++; }
    public void RecordMonsterKilled() { totalMonstersKilled++; }
    public void RecordItemCrafted() { totalItemsCrafted++; }

    public void ShowEndingChoiceUI()
    {
        if (endingChoicePanel != null)
        {
            endingChoicePanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ChooseEnding(EndingType choice)
    {
        if (endingChoicePanel != null) endingChoicePanel.SetActive(false);
        if (endingScreenPanel != null) endingScreenPanel.SetActive(true);

        // Cập nhật số ngày sống sót từ DayManager nếu có
        if (DayManager.Instance != null)
        {
            totalDaysSurvived = DayManager.Instance.currentDay;
        }

        switch (choice)
        {
            case EndingType.EndingA_ReturnHome:
                DisplayEnding(
                    "ENDING 1: SỨ GIẢ TRỞ VỀ (TRUE ENDING)",
                    "Bạn dùng năng lượng Trạm Phát Sóng gửi toàn bộ tài liệu nghiên cứu về thế giới hiện đại và tiêu hủy tần số độc hại. Sương mù tan biến, lối ra khỏi rừng mở rộng. Bạn trở về làm một nhà khoa học anh hùng cứu sống thế giới khỏi thảm họa Vùng Đứt Gãy."
                );
                break;

            case EndingType.EndingB_MaiAnTiemEcho:
                DisplayEnding(
                    "ENDING 2: TIẾNG VỌNG KHÔNG-THỜI GIAN",
                    "Bước qua Cổng Tần Số Rift, bạn bước vào chiều không gian thứ 4. Tại đây, bạn gặp Mai An Tiêm — người đã tồn tại qua hàng trăm năm bảo vệ hạt giống. Bạn quyết định ở lại cùng ông giữ vững ranh giới bảo vệ cả hai thế giới."
                );
                break;

            case EndingType.EndingC_DarkKing:
                DisplayEnding(
                    "ENDING 3: TÂN VƯƠNG BÓNG ĐÊM (DARK KARMA ENDING)",
                    "Dùng sức mạnh Hạt Đen hấp thụ toàn bộ năng lượng Rift, bạn tiêu diệt cả bầy quái vật lẫn những kẻ can thiệp. Bạn không trở về nữa, mà biến thành Chúa Rừng tối cao trị vì Vùng Đứt Gãy vĩnh viễn."
                );
                break;

            case EndingType.EndingD_SilentSacrifice:
                DisplayEnding(
                    "ENDING 4: SỰ HY SINH THẦM LẶNG",
                    "Nhận thấy tần số không thể dập tắt bằng cách thường, bạn kích hoạt chế độ tự hủy Trạm Vô Tuyến Siêu Cấp. Cổng Void Rift bị phong ấn vĩnh viễn. Không ai biết bạn đã hy sinh, nhưng Trái Đất được an toàn."
                );
                break;

            case EndingType.EndingE_TimeLoopSecret:
                DisplayEnding(
                    "ENDING 5: VÒNG LẶP VĨNH HẰNG (SECRET ENDING)",
                    "Tần số cao phát nổ gây giãn nở thời gian. Bạn tỉnh dậy trên một bờ biển hoang sơ của hàng trăm năm trước... Chiếc đài vô tuyến trên tay bạn rơ rát. Hóa ra, chính BẠN là người đã để lại những cuộn băng vô tuyến đầu tiên cho người sau!"
                );
                break;
        }
    }

    void DisplayEnding(string title, string description)
    {
        if (endingTitleText != null) endingTitleText.text = title;
        if (endingDescText != null) endingDescText.text = description;

        if (statsSummaryText != null)
        {
            string karmaRating = playerKarma >= 75 ? "<color=#00FF88>Anh Hùng Rừng Xanh (Thiện)</color>" :
                                (playerKarma <= 25 ? "<color=#FF4444>Bị Tha Hóa Bóng Đêm (Ác)</color>" : "<color=#FFCC00>Kẻ Sống Sót Trung Lập</color>");

            statsSummaryText.text = $"<b>📊 TỔNG KẾT TỔNG THỂ HÀNH TRÌNH:</b>\n" +
                                    $"• Số Ngày Sống Sót: <b>{totalDaysSurvived} Ngày</b>\n" +
                                    $"• Số Cây Đã Chặt: <b>{totalTreesChopped} Cây</b>\n" +
                                    $"• Số Đá Khai Thác: <b>{totalRocksMined} Khối</b>\n" +
                                    $"• Quái Vật Tiêu Diệt: <b>{totalMonstersKilled} Con</b>\n" +
                                    $"• Đồ Đạc Đã Chế Tạo: <b>{totalItemsCrafted} Món</b>\n" +
                                    $"• Điểm Nhân Quả (Karma): <b>{playerKarma}/100</b> ({karmaRating})";
        }
    }

    void ReturnToMainMenu()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene("Home");
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Home");
        }
    }
}
