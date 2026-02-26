using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    public EnemyState currentState {get ; private set;}
    public void Initialize(EnemyState startingState)
    {
        currentState = startingState;
        currentState.Enter();
    }
    public void ChangeState(EnemyState newState)
    {
        if(newState == null || newState == currentState)
        {
            return;
        }
        if(currentState != null)
        {
            currentState.Exit();
        }
        currentState = newState;
        currentState.Enter();
    }
    public void UpdateState()
    {
        if(currentState != null)
        {
            currentState.Update();
        }
    }
}
