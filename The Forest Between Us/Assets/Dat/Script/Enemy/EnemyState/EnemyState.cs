using UnityEngine;

public abstract class EnemyState
{
    protected EnemyAIController controller;
    public EnemyState(EnemyAIController controller) // dùng để khởi tạo enemyState với controller
    {
        this.controller = controller;
    }
    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}
