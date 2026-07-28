using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DailyQuestConfig
{
    public int dayID;
    public string questTitle;
    [TextArea(3, 8)]
    public string storyIntro;
    public List<QuestStep> steps = new List<QuestStep>();
    public int karmaImpact;
    public bool isWaveDefenseDay;
    public int waveEnemyCount;
    public EndingType defaultEndingIfTriggered;
}

public class Campaign30DaysManager : MonoBehaviour
{
    public static Campaign30DaysManager instance;

    [Header("Campaign Progression")]
    public int currentDay = 1;
    public List<DailyQuestConfig> campaignQuests = new List<DailyQuestConfig>();

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        Build30DaysCampaignData();
    }

    void Start()
    {
        LoadQuestForDay(currentDay);
    }

    public void LoadQuestForDay(int day)
    {
        currentDay = Mathf.Clamp(day, 1, 30);
        DailyQuestConfig config = GetQuestConfigForDay(currentDay);

        if (config == null)
        {
            Debug.LogWarning($"No quest config found for Day {currentDay}");
            return;
        }

        // Convert config into runtime QuestData
        QuestData runtimeQuest = ScriptableObject.CreateInstance<QuestData>();
        runtimeQuest.dayID = config.dayID;
        runtimeQuest.questTitle = config.questTitle;
        runtimeQuest.storyIntro = config.storyIntro;
        runtimeQuest.karmaImpact = config.karmaImpact;
        
        foreach (QuestStep s in config.steps)
        {
            runtimeQuest.steps.Add(new QuestStep
            {
                description = s.description,
                type = s.type,
                targetAmount = s.targetAmount,
                currentAmount = 0,
                isFinished = false
            });
        }

        if (QuestManager.instance != null)
        {
            QuestManager.instance.InitializeQuest(runtimeQuest);
        }

        // Trigger Wave Defense if applicable
        if (config.isWaveDefenseDay && CombatManager.instance != null)
        {
            CombatManager.instance.TriggerEnemyWave(config.waveEnemyCount);
        }

        // Trigger Ending on Day 30 completion
        if (currentDay == 30)
        {
            if (EndingManager.instance != null)
            {
                EndingManager.instance.ShowEndingChoiceUI();
            }
        }
    }

    public DailyQuestConfig GetQuestConfigForDay(int day)
    {
        return campaignQuests.Find(q => q.dayID == day);
    }

    void Build30DaysCampaignData()
    {
        if (campaignQuests.Count >= 30) return; // Already built

        campaignQuests.Clear();

        // ----------------------------------------------------
        // HỒI 1: KHỞI ĐẦU BÍ ẨN (Ngày 1 - 3)
        // ----------------------------------------------------
        campaignQuests.Add(new DailyQuestConfig
        {
            dayID = 1,
            questTitle = "Ngày 1: Tín Hiệu Lạc Lối",
            storyIntro = "Bạn tỉnh dậy trong sương mù. Chiếc đài Radio cũ phát ra tiếng rè nhiễu sóng của Mai An Tiêm...",
            karmaImpact = 5,
            steps = new List<QuestStep>
            {
                new QuestStep { description = "Đến vị trí đài Radio phát tín hiệu", type = StepType.ReachTarget, targetAmount = 1 },
                new QuestStep { description = "Nhặt đài Radio (Phím F)", type = StepType.Interaction, targetAmount = 1 },
                new QuestStep { description = "Thu thập 3 Dưa Hấu Hạt Đen", type = StepType.Collect, targetAmount = 3 }
            }
        });

        campaignQuests.Add(new DailyQuestConfig
        {
            dayID = 2,
            questTitle = "Ngày 2: Bản Năng Sinh Tồn",
            storyIntro = "Sương mù ngày càng dày đặc. Bạn cần chế tạo vật phẩm chiếu sáng để chống lại cái lạnh...",
            karmaImpact = 0,
            steps = new List<QuestStep>
            {
                new QuestStep { description = "Thu thập 2 Gỗ và 1 Nhựa Phát Quang", type = StepType.Collect, targetAmount = 3 },
                new QuestStep { description = "Mở giao diện Crafting (Phím K) chế tạo Đuốc Tần Số", type = StepType.Collect, targetAmount = 1 }
            }
        });

        campaignQuests.Add(new DailyQuestConfig
        {
            dayID = 3,
            questTitle = "Ngày 3: Đêm Sương Độc",
            storyIntro = "Sinh thể bóng đêm xuất hiện. Hãy dùng tư thế Nằm (Phím X) hoặc Cúi (Phím C) để né tránh...",
            karmaImpact = 5,
            steps = new List<QuestStep>
            {
                new QuestStep { description = "Nấp cúi người/nằm né tránh kẻ thù bóng đêm", type = StepType.Survival, targetAmount = 1 },
                new QuestStep { description = "Kích hoạt Trạm Tiếp Sóng 01", type = StepType.Interaction, targetAmount = 1 }
            }
        });

        // ----------------------------------------------------
        // HỒI 2: CHIẾN ĐẤU & TRƯỜNG KỲ KHÁNG CỰ (Ngày 4 - 10)
        // ----------------------------------------------------
        campaignQuests.Add(new DailyQuestConfig
        {
            dayID = 4,
            questTitle = "Ngày 4: Bóng Ma Săn Đêm",
            storyIntro = "Kẻ thù bắt đầu chủ động tấn công. Đã đến lúc chế tạo Nỏ Tần Số để đáp trả!",
            karmaImpact = 0,
            steps = new List<QuestStep>
            {
                new QuestStep { description = "Chế tạo Nỏ Tần Số và Tấn công (Click Chuột Trái)", type = StepType.Collect, targetAmount = 1 }
            }
        });

        campaignQuests.Add(new DailyQuestConfig
        {
            dayID = 5,
            questTitle = "Ngày 5: Đêm Trăng Máu Hạt Đen",
            storyIntro = "Đợt càn quét đầu tiên của Quái Vật Bóng Đêm! Hãy cố thủ căn cứ!",
            isWaveDefenseDay = true,
            waveEnemyCount = 5,
            karmaImpact = 10,
            steps = new List<QuestStep>
            {
                new QuestStep { description = "Tiêu diệt 5 Quái Vật Bóng Đêm tấn công căn cứ", type = StepType.Survival, targetAmount = 5 }
            }
        });

        campaignQuests.Add(new DailyQuestConfig
        {
            dayID = 6,
            questTitle = "Ngày 6: Trại Nghiên Cứu Bỏ Hoang",
            storyIntro = "Khám phá tàn tích trại nghiên cứu cũ để tìm linh kiện chế giáp...",
            steps = new List<QuestStep>
            {
                new QuestStep { description = "Thu thập 3 Linh Kiện Điện Tử", type = StepType.Collect, targetAmount = 3 }
            }
        });

        campaignQuests.Add(new DailyQuestConfig
        {
            dayID = 7,
            questTitle = "Ngày 7: Trái Tim Hạt Đen",
            storyIntro = "Trùm Shadow Berserker đang chặn đường đến tháp viễn thông. Tiêu diệt nó để lấy Lõi Năng Lượng!",
            isWaveDefenseDay = true,
            waveEnemyCount = 1,
            steps = new List<QuestStep>
            {
                new QuestStep { description = "Tiêu diệt Miniboss Shadow Berserker", type = StepType.Survival, targetAmount = 1 }
            }
        });

        campaignQuests.Add(new DailyQuestConfig
        {
            dayID = 8,
            questTitle = "Ngày 8: Sương Độc Xâm Chiếm",
            storyIntro = "Nồng độ độc tố tăng cao. Bạn phải chế tạo Mặt Nạ Lọc Khí để tiếp tục thám hiểm.",
            steps = new List<QuestStep>
            {
                new QuestStep { description = "Chế tạo Mặt Nạ Lọc Khí", type = StepType.Collect, targetAmount = 1 }
            }
        });

        campaignQuests.Add(new DailyQuestConfig
        {
            dayID = 9,
            questTitle = "Ngày 9: Trận Chiến Đèo Sương Mù",
            storyIntro = "Đặt bẫy bảo vệ Trạm Tiếp Sóng 02 trước khi đêm xuống...",
            steps = new List<QuestStep>
            {
                new QuestStep { description = "Đặt 3 Bẫy Tần Số quanh Trạm Tiếp Sóng 02", type = StepType.Interaction, targetAmount = 3 }
            }
        });

        campaignQuests.Add(new DailyQuestConfig
        {
            dayID = 10,
            questTitle = "Ngày 10: Cơn Bão Rift Đầu Tiên",
            storyIntro = "Cơn bão đứt gãy không gian bùng nổ! Bầy quái tràn vào dồn dập!",
            isWaveDefenseDay = true,
            waveEnemyCount = 10,
            karmaImpact = 15,
            steps = new List<QuestStep>
            {
                new QuestStep { description = "Cố thủ sống sót qua đợt bão Bầy Đàn Bóng Đêm (10 kẻ thù)", type = StepType.Survival, targetAmount = 10 }
            }
        });

        // ----------------------------------------------------
        // HỒI 3: KHÁM PHÁ BÍ ẨN CỔ ĐẠI (Ngày 11 - 20)
        // ----------------------------------------------------
        for (int i = 11; i <= 13; i++)
        {
            campaignQuests.Add(new DailyQuestConfig
            {
                dayID = i,
                questTitle = $"Ngày {i}: Phế Tích Cổ Mai An Tiêm - Phần {i - 10}",
                storyIntro = "Khảo sát bia đá cổ truyền thuyết Mai An Tiêm và giải đố mạch điện cổ đại...",
                steps = new List<QuestStep>
                {
                    new QuestStep { description = $"Giải đố mạch điện cổ đại tại Phế Tích {i - 10}", type = StepType.Interaction, targetAmount = 1 }
                }
            });
        }

        for (int i = 14; i <= 16; i++)
        {
            campaignQuests.Add(new DailyQuestConfig
            {
                dayID = i,
                questTitle = $"Ngày {i}: Vùng Đất Chết & Đầm Lầy Đen",
                storyIntro = "Đối đầu quái bay Shadow Drakes và thu thập mẫu độc tố chế thuốc giải...",
                steps = new List<QuestStep>
                {
                    new QuestStep { description = "Tiêu diệt 2 Quái Bay Shadow Drakes", type = StepType.Survival, targetAmount = 2 }
                }
            });
        }

        for (int i = 17; i <= 20; i++)
        {
            campaignQuests.Add(new DailyQuestConfig
            {
                dayID = i,
                questTitle = $"Ngày {i}: Chìa Khóa Tần Số Trụ Vũ Trụ - Mảnh {i - 16}",
                storyIntro = "Thu thập mảnh chìa khóa năng lượng cổ đại để mở cửa Trạm Phát Sóng Siêu Cấp...",
                steps = new List<QuestStep>
                {
                    new QuestStep { description = $"Thu thập Mảnh Chìa Khóa Tần Số #{i - 16}", type = StepType.Collect, targetAmount = 1 }
                }
            });
        }

        // ----------------------------------------------------
        // HỒI 4: ĐẠI CHIẾN RANH GIỚI (Ngày 21 - 29)
        // ----------------------------------------------------
        for (int i = 21; i <= 24; i++)
        {
            campaignQuests.Add(new DailyQuestConfig
            {
                dayID = i,
                questTitle = $"Ngày {i}: Pháo Đài Trạm Vô Tuyến Siêu Cấp",
                storyIntro = "Gia cố tường thành và lắp đặt Pháo Sóng Âm tự động...",
                steps = new List<QuestStep>
                {
                    new QuestStep { description = "Gia cố tường rào trạm vô tuyến", type = StepType.Interaction, targetAmount = 1 }
                }
            });
        }

        for (int i = 25; i <= 28; i++)
        {
            campaignQuests.Add(new DailyQuestConfig
            {
                dayID = i,
                questTitle = $"Ngày {i}: Trận Đại Chiến Void Leviathan",
                storyIntro = "Trùm Cổ Đại Void Leviathan xuất hiện! Hãy dốc toàn bộ hỏa lực!",
                isWaveDefenseDay = (i == 28),
                waveEnemyCount = 12,
                steps = new List<QuestStep>
                {
                    new QuestStep { description = "Tấn công làm suy yếu Void Leviathan", type = StepType.Survival, targetAmount = 1 }
                }
            });
        }

        campaignQuests.Add(new DailyQuestConfig
        {
            dayID = 29,
            questTitle = "Ngày 29: Đêm Trước Ngày Phán Quyết",
            storyIntro = "Mai An Tiêm trò chuyện lần cuối qua đài vô tuyến. Bạn sẽ chọn vận mệnh nào cho nhân loại?",
            steps = new List<QuestStep>
            {
                new QuestStep { description = "Tải dữ liệu vô tuyến cuối cùng", type = StepType.Interaction, targetAmount = 1 }
            }
        });

        // ----------------------------------------------------
        // HỒI 5: NGÀY PHÁN QUYẾT & 5 ĐA KẾT THÚC (Ngày 30)
        // ----------------------------------------------------
        campaignQuests.Add(new DailyQuestConfig
        {
            dayID = 30,
            questTitle = "Ngày 30: NGÀY PHÁN QUYẾT (JUDGMENT DAY)",
            storyIntro = "Cổng Vùng Đứt Gãy Void Rift mở ra hoàn toàn! Hãy đưa ra Lựa Chọn Vận Mệnh!",
            karmaImpact = 50,
            steps = new List<QuestStep>
            {
                new QuestStep { description = "Quyết định Vận Mệnh (Chọn 1 trong 5 Kết Thúc)", type = StepType.Interaction, targetAmount = 1 }
            }
        });

        Debug.Log($"Built complete 30-Day Campaign Data with {campaignQuests.Count} days!");
    }
}
