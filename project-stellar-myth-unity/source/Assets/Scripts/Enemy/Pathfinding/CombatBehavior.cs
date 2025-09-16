using UnityEngine;

namespace EnemySystem.Pathfinding
{
    /// <summary>
    /// Classe que implementa o comportamento de combate
    /// Seguindo o Single Responsibility Principle (SRP)
    /// </summary>
    public class CombatBehavior : ICombatBehavior
    {
        private readonly PathfindingConfig config;
        private float lastAttackTime;

        public CombatBehavior(PathfindingConfig config)
        {
            this.config = config;
            lastAttackTime = -config.attackCooldown; // Permite atacar imediatamente
        }

        public bool IsInAttackRange(Vector2 currentPosition, Vector2 targetPosition)
        {
            // Para inimigos ranged, considera uma distância mínima também
            if (config.combatType == EnemyCombatType.Ranged)
            {
                float minRange = config.attackRange * 0.5f;
                float distanceToTarget = Vector2.Distance(currentPosition, targetPosition);
                return distanceToTarget <= config.attackRange && distanceToTarget >= minRange;
            }

            return Vector2.Distance(currentPosition, targetPosition) <= config.attackRange;
        }

        public void PerformAttack()
        {
            if (Time.time - lastAttackTime < config.attackCooldown)
                return;

            if (config.logAttacks)
            {
                string attackType = config.combatType switch
                {
                    EnemyCombatType.Melee => "ataque corpo a corpo",
                    EnemyCombatType.Ranged => "ataque à distância",
                    EnemyCombatType.Support => "habilidade de suporte",
                    _ => "ataque"
                };

            }

            lastAttackTime = Time.time;
        }

        public void UpdateCombat()
        {
            // Aqui você pode adicionar lógica adicional de combate
            // Por exemplo, carregar ataques especiais, mudar estados, etc.
        }
    }
}
