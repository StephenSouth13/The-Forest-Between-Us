using UnityEngine;

public class SavePointCheckpoint : MonoBehaviour, Interactable
{
    public string checkpointName = "Trạm Lửa Trại Nhập Môn";
    public string prompt = "Kích Hoạt Điểm Lưu Campfire [E]";
    public GameObject activeLightObject;
    public ParticleSystem fireParticle;

    private bool isActivated = false;

    public string GetInteractPrompt()
    {
        return isActivated ? $"🔥 {checkpointName} (Đã Lưu)" : prompt;
    }

    public void OnInteract()
    {
        ActivateCheckpoint();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            ActivateCheckpoint();
        }
    }

    public void ActivateCheckpoint()
    {
        isActivated = true;

        if (activeLightObject != null) activeLightObject.SetActive(true);
        if (fireParticle != null && !fireParticle.isPlaying) fireParticle.Play();

        // Save Checkpoint Position in Respawn Manager
        if (PlayerRespawnManager.instance != null)
        {
            PlayerRespawnManager.instance.SetCheckpoint(transform.position + Vector3.up * 0.5f, checkpointName);
        }

        // Heal Player Stats at Checkpoint
        if (PlayerStatsManager.instance != null)
        {
            PlayerStatsManager.instance.Heal(100f);
            PlayerStatsManager.instance.RestSleep(100f);
        }

        Debug.Log($"[Checkpoint] Activated Save Point: {checkpointName} at position {transform.position}");
    }
}
