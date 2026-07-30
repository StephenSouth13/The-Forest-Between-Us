using UnityEngine;

public class RadioInteractable : MonoBehaviour, Interactable
{
    public string prompt = "Nhặt Đài Radio 01 [F]";
    public bool destroyAfterPickup = false;
    public GameObject objectToHide;

    public string GetInteractPrompt()
    {
        return prompt;
    }

    public void OnInteract()
    {
        QuestManager.instance?.AdvanceStep(StepType.Interaction, 1);
        QuestManager.instance?.UpdateObjectiveText("🎯 Tín hiệu vô tuyến đã được kết nối.");

        if (RadioDialogueUIController.instance != null)
        {
            RadioDialogueUIController.instance.StartRadioDialogueSequence();
        }

        if (objectToHide != null)
        {
            objectToHide.SetActive(false);
        }
        else if (destroyAfterPickup)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
