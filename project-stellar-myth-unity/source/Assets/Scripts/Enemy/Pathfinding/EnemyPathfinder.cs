using UnityEngine;
using CombatSystem;

namespace EnemySystem.Pathfinding
{
    /// <summary>
    /// Enum para controlar a direção do inimigo (similar ao PlayerController2D)
    /// </summary>
    public enum EnemyDirection
    {
        Up,
        Down,
        Right,
        Left
    }
    
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
        private Animator animator; // Para controlar animações
        private EnemyCombatController combatController; // Sistema de combate
        
        // Comportamentos (seguindo Dependency Inversion Principle)
        private ICombatBehavior combatBehavior;
        private ICollisionAvoidance collisionAvoidance;
        
        // Estado do pathfinding
        private PathfindingState currentState = PathfindingState.Idle;
        private Vector2 lastPosition;
        private float stuckCheckTimer;
        private int stuckCounter;
        
        // Sistema de cooldown após ser ferido
        private bool wasHurt = false;
        private float hurtRecoveryTimer = 0f;
        
        // Sistema de animação (similar ao PlayerController2D)
        private EnemyDirection currentDirection = EnemyDirection.Down;
        private EnemyDirection lastDirection = EnemyDirection.Down;
        private float directionBlendID = 2f; // Começa olhando para baixo (Down = 2)
        private bool isWalking = false;
        
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
            animator = GetComponent<Animator>();
            combatController = GetComponent<EnemyCombatController>();
            
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
            
            // Atualiza sistema de cooldown após ser ferido
            UpdateHurtRecovery();

            // Verifica se pode atacar (independente do estado)
            CheckAndPerformAttack();

            // Determina o próximo estado
            DetermineState();

            // Executa comportamento baseado no estado
            switch (currentState)
            {
                case PathfindingState.MovingToTarget:
                    MoveTowardsTarget();
                    break;

                case PathfindingState.Attacking:
                    // Para o movimento durante o ataque
                    rb.linearVelocity = Vector2.zero;
                    break;

                case PathfindingState.Avoiding:
                    AvoidObstacles();
                    break;

                case PathfindingState.Stuck:
                    //Ignore stuck state for now
                    MoveTowardsTarget();
                    break;
            }

            // Atualiza animações
            UpdateAnimations();

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

            // Verifica se está em alcance de ataque
            bool inAttackRange = IsInAttackRange();
            
            if (inAttackRange)
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

        /// <summary>
        /// Atualiza o sistema de cooldown de recuperação após ser ferido
        /// </summary>
        private void UpdateHurtRecovery()
        {
            if (combatController == null) return;

            // Detecta quando o inimigo foi ferido
            if (combatController.IsHurt && !wasHurt)
            {
                wasHurt = true;
                hurtRecoveryTimer = config.hurtRecoveryCooldown;
            }
            
            // Quando não está mais ferido, inicia o cooldown
            else if (!combatController.IsHurt && wasHurt)
            {
                // Continua decrementando o timer até acabar
                if (hurtRecoveryTimer > 0f)
                {
                    hurtRecoveryTimer -= Time.fixedDeltaTime;
                }
                else
                {
                    // Cooldown terminou, pode atacar novamente
                    wasHurt = false;
                }
            }
        }

        #endregion

        #region Attack System (Simplified)

        /// <summary>
        /// Verifica se está em range de ataque, independente do estado
        /// </summary>
        private bool IsInAttackRange()
        {
            if (target == null) return false;

            // Usa o novo sistema de combate se disponível
            if (combatController != null && combatController.Attributes != null)
            {
                return combatController.IsInAttackRange((Vector2)target.position);
            }
            else
            {
                // Fallback para o sistema antigo
                return combatBehavior.IsInAttackRange((Vector2)transform.position, (Vector2)target.position);
            }
        }

        /// <summary>
        /// Verifica se pode atacar e executa o ataque se possível
        /// </summary>
        private void CheckAndPerformAttack()
        {
            if (!IsInAttackRange()) return;

            Debug.Log($"[ISHURT] {combatController.IsHurt} está em alcance de ataque.");
            
            // Verifica se o inimigo está tomando dano (não pode atacar enquanto sendo ferido)
            if (combatController != null && combatController.IsHurt)
            {
                return; // Não ataca se estiver tomando dano
            }
            
            // Verifica se ainda está no cooldown de recuperação após ser ferido
            if (wasHurt && hurtRecoveryTimer > 0f)
            {
                return; // Ainda em cooldown de recuperação
            }

            // Usa o novo sistema de combate se disponível
            if (combatController != null)
            {
                // Verifica se pode atacar considerando o attack speed (cooldown)
                if (combatController.CanAttack)
                {
                    combatController.ExecuteAttack();
                }
            }
            else
            {
                // Fallback para o sistema antigo
                combatBehavior.PerformAttack();
            }
        }

        #endregion

        #region Movement Behaviors

        private void MoveTowardsTarget()
        {
            Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
            Vector2 movement = direction * config.moveSpeed;
            
            // Aplica o movimento em ambos os eixos (X e Y) para jogo top-down
            rb.linearVelocity = movement;
        }

        private void AvoidObstacles()
        {
            Vector2 avoidanceDirection = collisionAvoidance.GetAvoidanceDirection((Vector2)transform.position, (Vector2)target.position);
            Vector2 movement = avoidanceDirection * config.moveSpeed;
            
            // Aplica o movimento em ambos os eixos para jogo top-down
            rb.linearVelocity = movement;
        }

        #endregion

        #region Animation System

        /// <summary>
        /// Atualiza o sistema de animação (similar ao PlayerController2D)
        /// </summary>
        private void UpdateAnimations()
        {
            // Verifica se o inimigo está se movendo
            bool isMoving = rb.linearVelocity.magnitude > 0.1f;
            bool isTaunting = currentState == PathfindingState.Attacking;
            
            // Verifica se o inimigo está machucado (novo sistema de combate)
            bool isHurting = false;
            if (combatController != null && combatController.Attributes != null)
            {
                // Pode implementar lógica adicional aqui se necessário
            }
            
            // Atualiza direção do inimigo apenas se estiver se movendo E não estiver atacando
            if (isMoving && !isTaunting)
            {
                UpdateEnemyDirection();
            }
            
            // Define se está caminhando
            isWalking = isMoving && currentState == PathfindingState.MovingToTarget;
            
            // Atualiza o ID da direção para Blend Tree apenas se não estiver atacando
            if (!isTaunting)
            {
                UpdateDirectionBlendID();
            }
            
            // Envia parâmetros para o Animator
            if (animator != null)
            {
                animator.SetFloat("DirecaoID", directionBlendID);
                animator.SetBool("isWalking", isWalking);
                animator.SetBool("isTaunting", isTaunting);
                
                // Parâmetros adicionais que podem ser úteis
                animator.SetBool("isAttacking", currentState == PathfindingState.Attacking);
                animator.SetBool("isDead", currentState == PathfindingState.Dead);
                
                // Novos parâmetros para o sistema de combate
                // O isHurt e isHurting são controlados pelo EnemyCombatController
            }
    
        }
        
        /// <summary>
        /// Atualiza a direção atual do inimigo baseado no movimento
        /// </summary>
        private void UpdateEnemyDirection()
        {
            Vector2 movement = rb.linearVelocity.normalized;
            
            // Determina direção predominante (up, down, left, right)
            float absX = Mathf.Abs(movement.x);
            float absY = Mathf.Abs(movement.y);
            
            if (movement.magnitude > 0.1f)
            {
                // Salva direção atual como última direção antes de atualizar
                lastDirection = currentDirection;
                
                // Verifica qual componente (x ou y) é dominante para determinar direção
                if (absX > absY)
                {
                    // Movimento horizontal é dominante
                    if (movement.x > 0)
                        currentDirection = EnemyDirection.Right;
                    else
                        currentDirection = EnemyDirection.Left;
                }
                else
                {
                    // Movimento vertical é dominante
                    if (movement.y > 0)
                        currentDirection = EnemyDirection.Up;
                    else
                        currentDirection = EnemyDirection.Down;
                }
            }
        }
        
        /// <summary>
        /// Converte direção do inimigo para valor numérico para Blend Tree
        /// </summary>
        private void UpdateDirectionBlendID()
        {
            // Define valores distintos para cada direção principal
            // Norte (Up): 0, Leste (Right): 1, Sul (Down): 2, Oeste (Left): 3
            // (mesmos valores usados no PlayerController2D)
            
            // Usa direção atual quando se movendo, ou última direção conhecida quando parado
            EnemyDirection directionToUse = rb.linearVelocity.magnitude > 0.1f ? currentDirection : lastDirection;
            
            // Converte enum de direção para valor numérico específico
            switch (directionToUse)
            {
                case EnemyDirection.Up:
                    directionBlendID = 0f;
                    break;
                case EnemyDirection.Right:
                    directionBlendID = 1f;
                    break;
                case EnemyDirection.Down:
                    directionBlendID = 2f;
                    break;
                case EnemyDirection.Left:
                    directionBlendID = 3f;
                    break;
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
