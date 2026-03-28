using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    [Header("Configuration")]
    public StepType typeToAdvance; //Choose: Movement,ReachTarget,etc,..
    public int amount = 1; //Usually  1 for reaching a place
    public bool destroyOnActivation = true;

    private void OnTriggerEnter(Collider other)
    {
        //Check if object entering is the Player

        if (other.CompareTag("Player"))
        {
            QuestManager.instance.AdvanceStep(typeToAdvance,amount);
            Debug.Log($"Objective Updated: {typeToAdvance}");
            if (destroyOnActivation) Destroy(gameObject);
        }
    }
}