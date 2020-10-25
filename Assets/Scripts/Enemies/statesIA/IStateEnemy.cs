public interface IStateEnemy
{
    void UpdateState();
    void ToFiringState();
    void ToFollowState();
    void ToPatrolState();
}