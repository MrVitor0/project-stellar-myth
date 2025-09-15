using UnityEngine;

namespace EnemySystem.Pathfinding
{
    /// <summary>
   
    /// Seguindo os princípios SOLID:
    /// - Single Responsibility: Coordena os comportamentos de movimento
    /// - Open/Closed: Pode ser estendido sem modificação
    /// - Liskov Substitution: Trabalha com interfaces
    /// - Interface Segregation: Usa interfaces específicas
    /// - Dependency Inversion: Depende de abstrações
    /// </summary>
    [RequireComponent(typeof(Enemy))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class EnemyPathfinder : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private PathfindingConfig config;
        [SerializeField] private string playerTag = "Player";

        // Componentes e dependências
        private Enemy enemy;
        private Rigidbody2D rb;
        private Transform target;
        private SpriteRenderer spriteRenderer; // Para flip do sprite
        
        // Comportamentos (seguindo Dependency Inversion Principle)
        private ICombatBehavior combatBehavior;
        private ICollisionAvoidance collisionAvoidance;
        
        // Estado do pathfinding
        private PathfindingState currentState = PathfindingState.Idle;
        private Vector2 lastPosition;
        private float stuckCheckTimer;
        private int stuckCounter;
        
        #region Unity Lifecycle

        private void Start()
        {
            InitializeComponents();
            InitializeBehaviors();
            FindTarget();
        }

        private void FixedUpdate()
        {
            if (enemy.IsDead)
            {
                currentState = PathfindingState.Dead;
                return;
            }

            UpdatePathfinding();
        }

        #endregion

        #region Initialization

        private void InitializeComponents()
        {
            enemy = GetComponent<Enemy>();
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }

            SetupRigidbody();
            lastPosition = (Vector2)transform.position;
        }

        private void InitializeBehaviors()
        {
            combatBehavior = new CombatBehavior(config);
            collisionAvoidance = new CollisionAvoidance(config);
        }

        private void SetupRigidbody()
        {
            rb.isKinematic = false;
            rb.gravityScale = 0f; // Sem gravidade em jogo top-down
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.linearDamping = 5f; // Adiciona drag para parar suavemente
        }

        private void FindTarget()
        {
            var player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                Debug.LogWarning($"[EnemyPathfinder] Player with tag {playerTag} not found!");
            }
        }

        #endregion

        #region Pathfinding Update

        private void UpdatePathfinding()
        {
            if (target == null)
            {
                FindTarget();
                return;
            }

            // Atualiza detecção de travamento
            UpdateStuckDetection();

            // Determina o próximo estado
            DetermineState();

            // Executa comportamento baseado no estado
            switch (currentState)
            {
                case PathfindingState.MovingToTarget:
                    MoveTowardsTarget();
                    break;

                case PathfindingState.Attacking:
                    PerformAttack();
                    break;

                case PathfindingState.Avoiding:
                    AvoidObstacles();
                    break;

                case PathfindingState.Stuck:
                    HandleStuckState();
                    break;
            }

            // Atualiza posição anterior
            lastPosition = (Vector2)transform.position;
        }

        private void DetermineState()
        {
            if (IsStuck())
            {
                currentState = PathfindingState.Stuck;
                return;
            }

            if (combatBehavior.IsInAttackRange((Vector2)transform.position, (Vector2)target.position))
            {
                currentState = PathfindingState.Attacking;
                return;
            }

            if (collisionAvoidance.NeedsAvoidance((Vector2)transform.position))
            {
                currentState = PathfindingState.Avoiding;
                return;
            }

            currentState = PathfindingState.MovingToTarget;
        }

        #endregion

        #region Movement Behaviors

        private void MoveTowardsTarget()
        {
            Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
            Vector2 movement = direction * config.moveSpeed;
            
            // Aplica o movimento em ambos os eixos (X e Y) para jogo top-down
            rb.linearVelocity = movement;
            
            // Orienta o sprite na direção do movimento
            UpdateSpriteOrientation(movement);
        }

        private void AvoidObstacles()
        {
            Vector2 avoidanceDirection = collisionAvoidance.GetAvoidanceDirection((Vector2)transform.position, (Vector2)target.position);
            Vector2 movement = avoidanceDirection * config.moveSpeed;
            
            // Aplica o movimento em ambos os eixos para jogo top-down
            rb.linearVelocity = movement;
            
            // Orienta o sprite na direção do movimento
            UpdateSpriteOrientation(movement);
        }

        private void PerformAttack()
        {
            // Para o movimento completamente durante o ataque
            rb.linearVelocity = Vector2.zero;
            
            // Mantém o inimigo olhando para o alvo
            if (target != null && spriteRenderer != null)
            {
                Vector2 directionToTarget = ((Vector2)target.position - (Vector2)transform.position).normalized;
                UpdateSpriteOrientation(directionToTarget);
            }
            
            // Executa o ataque
            combatBehavior.PerformAttack();
        }

        /// <summary>
        /// Atualiza a orientação do sprite baseado na direção de movimento
        /// </summary>
        private void UpdateSpriteOrientation(Vector2 direction)
        {
            if (direction == Vector2.zero || spriteRenderer == null)
                return;

            if (config.useRotation)
            {
                // Rotaciona o sprite na direção do movimento
                float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                float currentAngle = transform.eulerAngles.z;
                float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, config.rotationSpeed * Time.fixedDeltaTime);
                transform.rotation = Quaternion.Euler(0, 0, newAngle);
            }
            else
            {
                // Usa flip horizontal baseado na direção dominante
                if (Mathf.Abs(direction.x) > 0.1f)
                {
                    spriteRenderer.flipX = direction.x < 0;
                }
            }
        }

        #endregion

        #region Stuck Detection

        private void UpdateStuckDetection()
        {
            stuckCheckTimer += Time.fixedDeltaTime;
            
            if (stuckCheckTimer >= config.stuckCheckTime)
            {
                float distanceMoved = Vector2.Distance((Vector2)transform.position, lastPosition);
                
                if (distanceMoved < config.stuckThreshold && currentState != PathfindingState.Attacking)
                {
                    stuckCounter++;
                }
                else
                {
                    stuckCounter = 0;
                }
                
                stuckCheckTimer = 0f;
            }
        }

        private bool IsStuck()
        {
            return stuckCounter >= 3;
        }

        private void HandleStuckState()
        {
            // Tenta se mover em uma direção aleatória para escapar
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            
            rb.linearVelocity = randomDirection * config.moveSpeed;
            
            // Reseta o contador de stuck após algumas tentativas
            if (stuckCounter > 5)
            {
                stuckCounter = 0;
            }
        }

        #endregion

        #region Debug Visualization

        private void OnDrawGizmos()
        {
            if (!config.showDebugGizmos)
                return;

            // Desenha o raio de ataque
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, config.attackRange);

            // Desenha o estado atual
            Vector3 textPosition = transform.position + Vector3.up * 2f;
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(textPosition, currentState.ToString());
            #endif

            // Desenha a visualização de colisão
            if (collisionAvoidance != null)
            {
                (collisionAvoidance as CollisionAvoidance)?.OnDrawGizmos((Vector2)transform.position);
            }

            // Desenha a direção do movimento
            if (rb != null && rb.linearVelocity.magnitude > 0.1f)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position, rb.linearVelocity.normalized * 2f);
            }
        }

        #endregion
    }
}
