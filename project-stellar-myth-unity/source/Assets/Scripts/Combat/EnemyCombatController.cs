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
        
        /// <summary>
        /// Indica se o inimigo está atualmente tomando dano
        /// </summary>
        public bool IsHurt => isHurt;
        
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
        }
        
        protected override void Start()
        {
            base.Start();
            
            // Força a criação de atributos se não existirem
            if (attributes == null)
            {
                attributes = new CombatAttributes();
                attributes.Initialize();
            }
  
            // Conecta eventos do CombatController com callbacks do Enemy
            if (enemy != null)
            {
                // CombatController notifica Enemy sobre morte (única fonte de verdade)
                attributes.OnDeath += () => enemy.Die();
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
            
            float distance = Vector2.Distance(transform.position, targetPosition);
            bool inRange = distance <= attributes.AttackRange;
            
            return inRange;
        }
        
        // Método público para o pathfinding executar ataque
        public void ExecuteAttack()
        {
        
            
            if (CanAttack)
            {
                Debug.Log($"[ENEMY ATTACK] ✅ {gameObject.name} executando ataque!");
                Attack();
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
