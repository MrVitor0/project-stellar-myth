using UnityEngine;

namespace EnemySystem.Pathfinding
{
    /// <summary>
    /// Classe que implementa a detecção e evitação de colisões em 2D
    /// Seguindo o Single Responsibility Principle (SRP)
    /// </summary>
    public class CollisionAvoidance : ICollisionAvoidance
    {
        private readonly PathfindingConfig config;

        public CollisionAvoidance(PathfindingConfig config)
        {
            this.config = config;
        }

        public Vector2 GetAvoidanceDirection(Vector2 currentPosition, Vector2 targetPosition)
        {
            Vector2 avoidanceDirection = Vector2.zero;
            
            // Evita obstáculos usando Physics2D
            Collider2D[] obstacles = Physics2D.OverlapCircleAll(currentPosition, config.avoidanceRadius, config.obstacleLayer);
            foreach (var obstacle in obstacles)
            {
                Vector2 obstaclePos = (Vector2)obstacle.transform.position;
                Vector2 awayFromObstacle = currentPosition - obstaclePos;
                float distance = awayFromObstacle.magnitude;
                
                // Evita divisão por zero
                if (distance < 0.01f) continue;
                
                // Quanto mais perto do obstáculo, mais forte a força de evitação
                float strength = (config.avoidanceRadius - distance) / config.avoidanceRadius;
                avoidanceDirection += awayFromObstacle.normalized * strength;
            }

            // Evita outros inimigos usando Physics2D
            Collider2D[] enemies = Physics2D.OverlapCircleAll(currentPosition, config.avoidanceRadius, config.enemyLayer);
            foreach (var enemy in enemies)
            {
                Vector2 enemyPos = (Vector2)enemy.transform.position;
                
                // Ignora a si mesmo
                if ((enemyPos - currentPosition).sqrMagnitude < 0.01f)
                    continue;

                Vector2 awayFromEnemy = currentPosition - enemyPos;
                float distance = awayFromEnemy.magnitude;
                
                // Evita divisão por zero
                if (distance < 0.01f) continue;
                
                // Força de separação mais forte para inimigos muito próximos
                float separationForce = 1f;
                if (distance < config.separationRadius)
                {
                    separationForce = 2f; // Força extra para separação
                }
                
                // Quanto mais perto do inimigo, mais forte a força de evitação
                float strength = (config.avoidanceRadius - distance) / config.avoidanceRadius * separationForce;
                avoidanceDirection += awayFromEnemy.normalized * strength;
            }

            // Se há forças de evitação, normaliza e aplica a força configurada
            if (avoidanceDirection != Vector2.zero)
            {
                avoidanceDirection = avoidanceDirection.normalized * config.avoidanceForce;
                
                // Para top-down, mistura com a direção do alvo de forma mais suave
                Vector2 directionToTarget = (targetPosition - currentPosition).normalized;
                
                // Se há obstáculos próximos, prioriza mais a evitação
                float avoidanceWeight = obstacles.Length > 0 ? 0.8f : 0.6f;
                avoidanceDirection = Vector2.Lerp(directionToTarget, avoidanceDirection, avoidanceWeight);
            }

            return avoidanceDirection;
        }

        public bool NeedsAvoidance(Vector2 currentPosition)
        {
            // Verifica se há obstáculos ou inimigos próximos que precisam ser evitados
            bool hasObstacles = Physics2D.OverlapCircle(currentPosition, config.avoidanceRadius, config.obstacleLayer);
            bool hasEnemies = Physics2D.OverlapCircle(currentPosition, config.avoidanceRadius, config.enemyLayer);

            return hasObstacles || hasEnemies;
        }

        #region Debug Visualization

        public void OnDrawGizmos(Vector2 position)
        {
            if (!config.showDebugGizmos)
                return;

            // Desenha o raio de detecção de colisão
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere((Vector3)position, config.avoidanceRadius);
            
            // Desenha o raio de separação
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere((Vector3)position, config.separationRadius);

            // Desenha os objetos detectados
            Collider2D[] obstacles = Physics2D.OverlapCircleAll(position, config.avoidanceRadius, config.obstacleLayer);
            foreach (var obstacle in obstacles)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine((Vector3)position, obstacle.transform.position);
                
                // Desenha um círculo no obstáculo
                Gizmos.color = new Color(1, 0, 0, 0.3f);
                Gizmos.DrawSphere(obstacle.transform.position, 0.2f);
            }

            Collider2D[] enemies = Physics2D.OverlapCircleAll(position, config.avoidanceRadius, config.enemyLayer);
            foreach (var enemy in enemies)
            {
                Vector2 enemyPos = (Vector2)enemy.transform.position;
                if (enemyPos != position)
                {
                    // Cor diferente para inimigos muito próximos
                    float distance = Vector2.Distance(position, enemyPos);
                    if (distance < config.separationRadius)
                    {
                        Gizmos.color = Color.magenta;
                    }
                    else
                    {
                        Gizmos.color = Color.blue;
                    }
                    
                    Gizmos.DrawLine((Vector3)position, enemy.transform.position);
                    
                    // Desenha um círculo no inimigo
                    Gizmos.color = new Color(0, 0, 1, 0.3f);
                    Gizmos.DrawSphere(enemy.transform.position, 0.15f);
                }
            }
        }

        #endregion
    }
}
