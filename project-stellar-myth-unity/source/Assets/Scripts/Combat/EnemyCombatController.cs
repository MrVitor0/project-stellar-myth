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
            enemy = GetComponent<Enemy>();
            
            // Configura layer do jogador como alvo
            targetLayers = LayerMask.GetMask("Player");
        }
        
        protected override void Start()
        {
            base.Start();
            
            // Sincroniza com o sistema antigo do Enemy se existir
            if (enemy != null)
            {
                // Copia valores do sistema antigo para o novo sistema
                // Se necessário, pode ajustar os valores aqui
            }
        }
        
        protected override void PerformAttack()
        {
            // Por enquanto, inimigos não atacam o player
            // Implementação futura quando for necessário
            
            if (debugEnemyCombat)
            {
                Debug.Log($"{gameObject.name} tentou atacar (não implementado ainda)");
            }
        }
        
        public override void TakeDamage(float damage, Vector2 attackDirection)
        {
            // Para de processar se já está morto, desabilitado, ou machucado
            if (!enabled || !attributes.IsAlive() || isHurt) return;
            
            base.TakeDamage(damage, attackDirection);
            
            if (attributes.IsAlive())
            {
                PlayHurtAnimation();
            }
            
            // Também aplica dano no sistema antigo para compatibilidade
            if (enemy != null && attributes.IsAlive())
            {
                // Sincroniza com o sistema antigo
                enemy.TakeDamage(damage);
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
            return distance <= attributes.AttackRange;
        }
        
        // Método público para o pathfinding executar ataque
        public void ExecuteAttack()
        {
            if (CanAttack)
            {
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
