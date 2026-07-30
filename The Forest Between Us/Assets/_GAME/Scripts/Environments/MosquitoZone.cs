using UnityEngine;

public class MosquitoZone : MonoBehaviour
{
    [Header("Mosquito Swarm Settings")]
    public string zoneName = "Vùng Đầm Lầy Sương Ẩm (Ổ Muỗi)";
    public float biteIntervalSeconds = 4f; // Cứ 4 giây muỗi đốt 1 lần nếu không bôi thuốc
    public float biteInfectionChance = 0.6f; // 60% xác suất bị bệnh sốt rét khi bị đốt

    [Header("🎨 Custom Asset Slots (Kéo Thả VFX / SFX Của Bạn Vào Đây)")]
    public ParticleSystem mosquitoSwarmVFX;
    public AudioClip mosquitoBuzzSFX;

    private float biteTimer = 0f;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        biteTimer += Time.deltaTime;
        if (biteTimer >= biteIntervalSeconds)
        {
            biteTimer = 0f;

            if (PlayerDiseaseManager.instance != null)
            {
                if (Random.value <= biteInfectionChance)
                {
                    PlayerDiseaseManager.instance.GetMosquitoBitten();
                }
            }

            if (mosquitoBuzzSFX != null && audioSource != null)
            {
                audioSource.PlayOneShot(mosquitoBuzzSFX);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.LogWarning($"🦟 ⚠️ BẠN ĐÃ ĐI VÀO {zoneName.ToUpper()}! Vo ve đàn muỗi vây quanh. Hãy bôi Thuốc Chống Muỗi!");
            if (mosquitoSwarmVFX != null) mosquitoSwarmVFX.Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (mosquitoSwarmVFX != null) mosquitoSwarmVFX.Stop();
        }
    }
}
