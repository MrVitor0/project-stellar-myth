using UnityEngine;

namespace EnemySystem.Pathfinding
{
    /// <summary>
    /// Configurações compartilhadas do pathfinding
    /// Seguindo o Single Responsibility Principle (SRP)
    /// </summary>
    [System.Serializable]
    public class PathfindingConfig
    {
        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        public float stoppingDistance = 0.1f;
        public float drag = 5f; // Drag para parar suavemente em top-down
        
        [Header("Rotation Settings (Top-Down)")]
        public bool useRotation = false; // Se true, rotaciona o sprite; se false, usa flip
        public float rotationSpeed = 180f; // Graus por segundo
        
        [Header("Combat Settings")]
        public EnemyCombatType combatType = EnemyCombatType.Melee;
        public float attackRange = 2f;
        public float attackCooldown = 1f;
        [Tooltip("Tempo de cooldown após ser ferido antes de poder atacar novamente")]
        public float hurtRecoveryCooldown = 1.5f;
        
        [Header("Avoidance Settings")]
        public float avoidanceRadius = 1f;
        public float avoidanceForce = 2f;
        public float separationRadius = 0.5f; // Raio mínimo de separação entre inimigos
        public LayerMask obstacleLayer;
        public LayerMask enemyLayer;
        
        [Header("Stuck Detection")]
        public float stuckThreshold = 0.1f;
        public float stuckCheckTime = 0.5f;
        
        [Header("Debug")]
        public bool showDebugGizmos = false;
        public bool logAttacks = false;
    }
}
