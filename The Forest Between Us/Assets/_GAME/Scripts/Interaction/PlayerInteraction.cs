using TMPro;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Input")]
    public KeyCode interactKey = KeyCode.F;

    [Header("Raycast")]
    public Camera playerCamera;
    public Transform rayOrigin;
    public float interactDistance = 3f;
    public LayerMask interactableLayers = ~0;

    [Header("UI")]
    public GameObject promptPanel;
    public TextMeshProUGUI promptText;
    public string promptFormat = "Press F - {0}";

    private Interactable currentInteractable;

    void Update()
    {
        FindInteractable();

        if (currentInteractable != null && Input.GetKeyDown(interactKey))
        {
            currentInteractable.OnInteract();
            HidePrompt();
        }
    }

    void FindInteractable()
    {
        Ray ray = BuildRay();

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayers, QueryTriggerInteraction.Collide))
        {
            currentInteractable = FindInteractable(hit.collider);

            if (currentInteractable != null)
            {
                ShowPrompt(currentInteractable.GetInteractPrompt());
                return;
            }
        }

        currentInteractable = null;
        HidePrompt();
    }

    Interactable FindInteractable(Collider hitCollider)
    {
        MonoBehaviour[] behaviours = hitCollider.GetComponentsInParent<MonoBehaviour>();

        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is Interactable interactable)
            {
                return interactable;
            }
        }

        return null;
    }

    Ray BuildRay()
    {
        if (playerCamera != null)
        {
            return new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        }

        if (rayOrigin != null)
        {
            return new Ray(rayOrigin.position, rayOrigin.forward);
        }

        return new Ray(transform.position + Vector3.up, transform.forward);
    }

    void ShowPrompt(string prompt)
    {
        if (promptPanel != null) promptPanel.SetActive(true);
        if (promptText != null) promptText.text = string.Format(promptFormat, prompt);
    }

    void HidePrompt()
    {
        if (promptPanel != null) promptPanel.SetActive(false);
    }
}
