using UnityEngine;

public class RadioInteractable : MonoBehaviour, Interactable
{
    public string prompt = "Pick up Radio";
    public bool destroyAfterPickup = false;
    public GameObject objectToHide;

    public string GetInteractPrompt()
    {
        return prompt;
    }

    public void OnInteract()
    {
        QuestManager.instance?.AdvanceStep(StepType.Interaction, 1);
        QuestManager.instance?.UpdateObjectiveText("Radio signal acquired.");

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
