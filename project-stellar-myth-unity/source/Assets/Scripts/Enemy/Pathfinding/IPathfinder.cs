using UnityEngine;

namespace EnemySystem.Pathfinding
{
    /// <summary>
    /// Interface que define o comportamento de pathfinding
    /// Seguindo o Interface Segregation Principle (ISP)
    /// </summary>
    public interface IPathfinder
    {
        void SetTarget(Transform target);
        void UpdatePath();
        bool HasReachedTarget();
        bool IsStuck();
        Vector2 GetNextPosition();
        void OnDrawGizmos();
    }

    /// <summary>
    /// Interface que define o comportamento de combate
    /// Seguindo o Interface Segregation Principle (ISP)
    /// </summary>
    public interface ICombatBehavior
    {
        bool IsInAttackRange(Vector2 currentPosition, Vector2 targetPosition);
        void PerformAttack();
        void UpdateCombat();
    }

    /// <summary>
    /// Interface que define o comportamento de colisão
    /// Seguindo o Interface Segregation Principle (ISP)
    /// </summary>
    public interface ICollisionAvoidance
    {
        Vector2 GetAvoidanceDirection(Vector2 currentPosition, Vector2 targetPosition);
        bool NeedsAvoidance(Vector2 currentPosition);
    }
}
