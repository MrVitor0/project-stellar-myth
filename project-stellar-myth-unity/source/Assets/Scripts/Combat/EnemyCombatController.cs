using UnityEngine;
using EnemySystem;

namespace CombatSystem
{
    /// <summary>
    /// Controlador de combate específico para inimigos
    /// Integra com o sistema de pathfinding e animações de dano
    /// </summary>
    public class EnemyCombatController : CombatController
    {
        [Header("Enemy Combat Settings")]
        [SerializeField] private bool debugEnemyCombat = true;
        [SerializeField] private float hurtAnimationDuration = 0.5f;
        [SerializeField] private float deathAnimationDuration = 1.0f;
        [SerializeField] private bool destroyOnDeath = true;
        
        private Enemy enemy;
        private bool isHurt = false;
        
        protected override void Awake()
        {
            base.Awake();
            
            // Force debug para true
            debugEnemyCombat = true;
            
            // Inimigos gastam menos stamina por ataque
            staminaCostPerAttack = 5f;
            
            enemy = GetComponent<Enemy>();
            
            // Configura layer do jogador como alvo
            targetLayers = LayerMask.GetMask("Player");
            
            Debug.Log($"[ENEMY COMBAT SETUP] {gameObject.name} - EnemyCombatController inicializado!");
            Debug.Log($"[ENEMY COMBAT SETUP] Target Layers: {targetLayers.value}");
            Debug.Log($"[ENEMY COMBAT SETUP] Stamina Cost: {staminaCostPerAttack}");
        }
        
        protected override void Start()
        {
            base.Start();
            
            // Força a criação de atributos se não existirem
            if (attributes == null)
            {
                Debug.LogWarning($"[ENEMY COMBAT SETUP] {gameObject.name} - Attributes era null, criando novo!");
                attributes = new CombatAttributes();
                attributes.Initialize();
            }
            
            Debug.Log($"[ENEMY COMBAT SETUP] {gameObject.name} - Start chamado");
            Debug.Log($"[ENEMY COMBAT SETUP] Attributes existe: {attributes != null}");
            if (attributes != null)
            {
                Debug.Log($"[ENEMY COMBAT SETUP] Max Health: {attributes.MaxHealth}");
                Debug.Log($"[ENEMY COMBAT SETUP] Current Health: {attributes.CurrentHealth}");
                Debug.Log($"[ENEMY COMBAT SETUP] Max Stamina: {attributes.MaxStamina}");
                Debug.Log($"[ENEMY COMBAT SETUP] Current Stamina: {attributes.CurrentStamina}");
                Debug.Log($"[ENEMY COMBAT SETUP] Attack Range: {attributes.AttackRange}");
                Debug.Log($"[ENEMY COMBAT SETUP] Attack Power: {attributes.AttackPower}");
                Debug.Log($"[ENEMY COMBAT SETUP] Attack Speed: {attributes.AttackSpeed}");
            }
            Debug.Log($"[ENEMY COMBAT SETUP] Attack Point: {attackPoint?.position}");
            
            // Conecta eventos do CombatController com callbacks do Enemy
            if (enemy != null)
            {
                // CombatController notifica Enemy sobre morte (única fonte de verdade)
                attributes.OnDeath += () => enemy.Die();
                Debug.Log($"[ENEMY COMBAT SETUP] Enemy component encontrado e conectado");
            }
            else
            {
                Debug.LogWarning($"EnemyCombatController em '{gameObject.name}' não encontrou componente Enemy!");
            }
        }
        
        protected override void PerformAttack()
        {
            if (debugEnemyCombat)
            {
                Debug.Log($"[ENEMY ATTACK] 🔥 {gameObject.name} - PerformAttack iniciado!");
            }
            
            // Detecta o jogador no alcance e ataca
            DetectAndDamagePlayer();
            
            // Agenda fim do ataque
            Invoke(nameof(OnAttackComplete), 0.6f);
            
            if (debugEnemyCombat)
            {
                Debug.Log($"[ENEMY ATTACK] {gameObject.name} - PerformAttack concluído, ataque será finalizado em 0.6s");
            }
        }
        
        private void DetectAndDamagePlayer()
        {
            if (debugEnemyCombat)
            {
                Debug.Log($"[ENEMY DAMAGE] {gameObject.name} - Iniciando detecção de jogador");
                Debug.Log($"[ENEMY DAMAGE] Attack Point Position: {attackPoint?.position}");
                Debug.Log($"[ENEMY DAMAGE] Attack Range: {attributes?.AttackRange}");
                Debug.Log($"[ENEMY DAMAGE] Target Layers: {targetLayers.value}");
            }
            
            // Detecta todos os colliders na área de ataque
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(
                attackPoint.position, 
                attributes.AttackRange, 
                targetLayers
            );
            
            if (debugEnemyCombat)
            {
                Debug.Log($"[ENEMY DAMAGE] {gameObject.name} detectou {hitPlayers.Length} jogadores no alcance de ataque");
                if (hitPlayers.Length == 0)
                {
                    Debug.Log($"[ENEMY DAMAGE] ❌ Nenhum jogador encontrado no alcance!");
                    // Vamos verificar se há algum objeto Player próximo
                    GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
                    Debug.Log($"[ENEMY DAMAGE] Total de objetos com tag Player no jogo: {allPlayers.Length}");
                    foreach (var player in allPlayers)
                    {
                        float distance = Vector2.Distance(transform.position, player.transform.position);
                        Debug.Log($"[ENEMY DAMAGE] Player {player.name} está a {distance:F2} unidades de distância (layer: {player.layer})");
                    }
                }
            }
            
            foreach (Collider2D playerCollider in hitPlayers)
            {
                if (debugEnemyCombat)
                {
                    Debug.Log($"[ENEMY DAMAGE] 🎯 Processando jogador: {playerCollider.name}");
                }
                
                // Verifica se tem CombatController primeiro (novo sistema)
                ICombatController playerCombat = playerCollider.GetComponent<ICombatController>();
                if (playerCombat != null)
                {
                    Vector2 attackDirection = (playerCollider.transform.position - transform.position).normalized;
                    playerCombat.TakeDamage(attributes.AttackPower, attackDirection);
                    
                    if (debugEnemyCombat)
                    {
                        Debug.Log($"[ENEMY DAMAGE] ✅ {gameObject.name} ATINGIU {playerCollider.name} causando {attributes.AttackPower} de dano!");
                        Debug.Log($"[ENEMY DAMAGE] Attack Direction: {attackDirection}");
                    }
                }
                else
                {
                    if (debugEnemyCombat)
                    {
                        Debug.LogWarning($"[ENEMY DAMAGE] ❌ Jogador {playerCollider.name} não tem CombatController! Usando sistema legado.");
                    }
                }
            }
        }
        
        public override void TakeDamage(float damage, Vector2 attackDirection)
        {
            // Para de processar se já está morto, desabilitado, ou machucado
            if (!enabled || !attributes.IsAlive() || isHurt) return;

            // CombatController processa o dano (única fonte de verdade)
            base.TakeDamage(damage, attackDirection);
            
            if (attributes.IsAlive())
            {
                PlayHurtAnimation();
            }
          
            if (debugEnemyCombat)
            {
                Debug.Log($"{gameObject.name} levou {damage} de dano. Vida restante: {attributes.CurrentHealth}");
            }
        }
        
        private void PlayHurtAnimation()
        {
            if (animator != null && !isHurt && enabled && gameObject.activeInHierarchy)
            {
                isHurt = true;
                animator.SetTrigger("isHurt");
                animator.SetBool("isHurting", true);
                
                // Agenda o fim da animação de dano
                Invoke(nameof(OnHurtComplete), hurtAnimationDuration);
            }
        }
        
        private void OnHurtComplete()
        {
            // Verifica se o objeto ainda existe e está ativo
            if (this != null && enabled && gameObject.activeInHierarchy)
            {
                isHurt = false;
                
                if (animator != null && animator.enabled)
                {
                    animator.SetBool("isHurting", false);
                }
            }
        }
        
        protected override void OnDamageTaken(float damage, Vector2 attackDirection)
        {
            // Pode adicionar efeitos visuais de dano aqui
            // Por exemplo: knockback, partículas, etc.
        }
        
        protected override void OnDeath()
        {
            if (debugEnemyCombat)
            {
                Debug.Log($"{gameObject.name} morreu!");
            }
            
            PlayDeathAnimation();
            
            // Desabilita movimentação e pathfinding
            var pathfinder = GetComponent<EnemySystem.Pathfinding.EnemyPathfinder>();
            if (pathfinder != null)
            {
                pathfinder.enabled = false;
            }
            
            // Desabilita collider para que não interfira mais
            var collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }
            
            // Desabilita rigidbody para parar movimento
            var rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.simulated = false;
            }
        }
        
        private void PlayDeathAnimation()
        {
            // Cancela qualquer invoke pendente (como OnHurtComplete)
            CancelInvoke();
            
            if (animator != null && animator.enabled)
            {
                animator.SetTrigger("isDead");
                animator.SetBool("isDead", true);
            }
            
            if (destroyOnDeath)
            {
                // Desabilita o Animator antes de destruir para evitar erros
                if (animator != null)
                {
                    animator.enabled = false;
                }
                
                // Desabilita este componente de combate
                this.enabled = false;
                
                Destroy(gameObject, deathAnimationDuration);
            }
        }
        
        // Método público para ser chamado pelo pathfinding quando em range de ataque
        public bool IsInAttackRange(Vector2 targetPosition)
        {
            if (attributes == null)
            {
                Debug.LogError($"[RANGE CHECK] ❌ {gameObject.name} - Attributes é NULL no IsInAttackRange!");
                return false;
            }
            
            float distance = Vector2.Distance(transform.position, targetPosition);
            bool inRange = distance <= attributes.AttackRange;
            
            Debug.Log($"[RANGE CHECK] {gameObject.name} - Distance: {distance:F2}, Attack Range: {attributes.AttackRange}, In Range: {inRange}");
            
            return inRange;
        }
        
        // Método público para o pathfinding executar ataque
        public void ExecuteAttack()
        {
            Debug.Log($"[ENEMY ATTACK] ⚔️  {gameObject.name} - ExecuteAttack CHAMADO!");
            Debug.Log($"[ENEMY ATTACK] debugEnemyCombat: {debugEnemyCombat}");
            
            if (debugEnemyCombat)
            {
                Debug.Log($"[ENEMY ATTACK] {gameObject.name} - ExecuteAttack chamado");
                Debug.Log($"[ENEMY ATTACK] CanAttack: {CanAttack}");
                Debug.Log($"[ENEMY ATTACK] IsAlive: {(attributes?.IsAlive()).ToString()}");
                Debug.Log($"[ENEMY ATTACK] Current Health: {attributes?.CurrentHealth}");
                Debug.Log($"[ENEMY ATTACK] Current Stamina: {attributes?.CurrentStamina}");
                Debug.Log($"[ENEMY ATTACK] Attack Power: {attributes?.AttackPower}");
                Debug.Log($"[ENEMY ATTACK] Attack Range: {attributes?.AttackRange}");
                Debug.Log($"[ENEMY ATTACK] IsAttacking: {isAttacking}");
                Debug.Log($"[ENEMY ATTACK] Time since last attack: {Time.time - lastAttackTime}");
                Debug.Log($"[ENEMY ATTACK] Attack cooldown: {(attributes != null ? 1f / attributes.AttackSpeed : 0f)}");
            }
            
            if (CanAttack)
            {
                Debug.Log($"[ENEMY ATTACK] ✅ {gameObject.name} executando ataque!");
                Attack();
            }
            else
            {
                Debug.Log($"[ENEMY ATTACK] ❌ {gameObject.name} NÃO PODE atacar!");
                
                // Vamos debuggar por que não pode atacar
                Debug.Log($"[ENEMY ATTACK DEBUG] isAttacking: {isAttacking}");
                Debug.Log($"[ENEMY ATTACK DEBUG] attributes != null: {attributes != null}");
                Debug.Log($"[ENEMY ATTACK DEBUG] IsAlive: {(attributes?.IsAlive()).ToString()}");
                Debug.Log($"[ENEMY ATTACK DEBUG] Time check: {Time.time} >= {lastAttackTime + (attributes != null ? 1f / attributes.AttackSpeed : 1f)}");
            }
        }
        
        
        private void OnDrawGizmos()
        {
            if (debugEnemyCombat && attackPoint != null)
            {
                // Mostra alcance de ataque
                Gizmos.color = isAttacking ? Color.red : new Color(1f, 0.5f, 0f); // Laranja personalizado
                Gizmos.DrawWireSphere(attackPoint.position, attributes.AttackRange);
                
                // Mostra status de vida
                if (attributes != null && attributes.IsAlive())
                {
                    Gizmos.color = Color.green;
                    Vector3 healthBarPos = transform.position + Vector3.up * 2f;
                    float healthPercentage = attributes.CurrentHealth / attributes.MaxHealth;
                    
                    // Barra de vida visual
                    Gizmos.DrawCube(healthBarPos, new Vector3(2f * healthPercentage, 0.2f, 0f));
                }
            }
        }
    }
}
