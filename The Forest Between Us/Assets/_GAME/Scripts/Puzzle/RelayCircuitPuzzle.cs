using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class CircuitNode
{
    public Button tileButton;
    public Image tileImage;
    public float currentAngle;
    public float targetAngle;
    public bool isConnected;
}

public class RelayCircuitPuzzle : MonoBehaviour
{
    public static RelayCircuitPuzzle instance;

    [Header("Puzzle Panel UI")]
    public GameObject puzzleCanvasPanel;
    public TextMeshProUGUI statusText;
    public Button closeButton;

    [Header("Circuit Nodes Grid")]
    public List<CircuitNode> nodes = new List<CircuitNode>();

    [Header("Audio (Optional)")]
    public AudioSource audioSource;
    public AudioClip rotateSound;
    public AudioClip solvedSound;

    private bool isSolved;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (puzzleCanvasPanel != null) puzzleCanvasPanel.SetActive(false);
        if (closeButton != null) closeButton.onClick.AddListener(ClosePuzzle);

        InitializeNodes();
    }

    public void OpenPuzzle()
    {
        if (isSolved)
        {
            Debug.Log("Relay Circuit Puzzle is already solved.");
            return;
        }

        if (puzzleCanvasPanel != null) puzzleCanvasPanel.SetActive(true);
        if (statusText != null) statusText.text = "Rotate nodes to align power frequency.";
    }

    public void ClosePuzzle()
    {
        if (puzzleCanvasPanel != null) puzzleCanvasPanel.SetActive(false);
    }

    void InitializeNodes()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            int index = i;
            CircuitNode node = nodes[i];
            
            if (node.tileButton != null)
            {
                node.tileButton.onClick.AddListener(() => OnNodeClicked(index));
            }

            // Apply initial rotation UI
            UpdateNodeUI(node);
        }
    }

    void OnNodeClicked(int index)
    {
        if (isSolved || index < 0 || index >= nodes.Count) return;

        CircuitNode node = nodes[index];
        node.currentAngle = (node.currentAngle + 90f) % 360f;
        UpdateNodeUI(node);

        if (audioSource != null && rotateSound != null)
        {
            audioSource.PlayOneShot(rotateSound);
        }

        CheckPuzzleSolved();
    }

    void UpdateNodeUI(CircuitNode node)
    {
        if (node.tileImage != null)
        {
            node.tileImage.rectTransform.localRotation = Quaternion.Euler(0, 0, node.currentAngle);
        }
    }

    void CheckPuzzleSolved()
    {
        bool allCorrect = true;

        foreach (CircuitNode node in nodes)
        {
            float diff = Mathf.Abs(Mathf.DeltaAngle(node.currentAngle, node.targetAngle));
            if (diff > 5f)
            {
                allCorrect = false;
                break;
            }
        }

        if (allCorrect)
        {
            SolvePuzzle();
        }
    }

    void SolvePuzzle()
    {
        isSolved = true;
        if (statusText != null) statusText.text = "<color=#00FF00>RELAY CIRCUIT RESTORED!</color>";

        if (audioSource != null && solvedSound != null)
        {
            audioSource.PlayOneShot(solvedSound);
        }

        Debug.Log("Relay Circuit Puzzle Completed!");

        // Advance Quest
        if (QuestManager.instance != null)
        {
            QuestManager.instance.AdvanceStep(StepType.Interaction, 1);
        }

        // Open Ending Manager Decision
        Invoke(nameof(TriggerEndingPhase), 2f);
    }

    void TriggerEndingPhase()
    {
        ClosePuzzle();
        if (EndingManager.instance != null)
        {
            EndingManager.instance.ShowEndingChoiceUI();
        }
    }

    public bool IsSolved() => isSolved;
}
