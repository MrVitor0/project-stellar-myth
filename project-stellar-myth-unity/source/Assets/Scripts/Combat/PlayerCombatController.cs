using UnityEngine;

namespace CombatSystem
{
    /// <summary>
    /// Controlador de combate específico para o jogador
    /// Integra com o PlayerController2D e gerencia ataques direcionais
    /// </summary>
    public class PlayerCombatController : CombatController
    {
        [Header("Player Combat Settings")]
        [SerializeField] private bool debugAttackDetection = true;
        
        [Header("Input Settings")]
        [SerializeField] private string attackInputName = "Fire1"; // Nome do input no Input Manager
        [SerializeField] private KeyCode[] alternativeAttackKeys = { KeyCode.E, KeyCode.Return }; // Teclas alternativas
        
        private PlayerController2D playerController;
        
        protected override void Awake()
        {
            base.Awake();
            playerController = GetComponent<PlayerController2D>();
            
            // Configura layer de inimigos como alvo
            targetLayers = LayerMask.GetMask("Enemy");
        }
        
        protected override void Update()
        {
            base.Update();
            
            // Verifica input de ataque usando Input Manager e teclas alternativas
            if (GetAttackInput())
            {
                Attack();
            }
        }
        
        /// <summary>
        /// Verifica se algum input de ataque foi pressionado
        /// </summary>
        private bool GetAttackInput()
        {
            // Verifica input principal (configurável no Input Manager)
            if (Input.GetButtonDown(attackInputName))
                return true;
            
            // Verifica teclas alternativas
            foreach (KeyCode key in alternativeAttackKeys)
            {
                if (Input.GetKeyDown(key))
                    return true;
            }
            
            return false;
        }
        
        protected override void PerformAttack()
        {
            // Toca animação de ataque
            if (animator != null)
            {
                animator.SetTrigger("Attack");
                animator.SetBool("isAttacking", true);
            }
            
            // Detecta inimigos no alcance
            DetectAndDamageEnemies();
            
            // Agenda fim do ataque baseado na duração da animação
            Invoke(nameof(OnAttackComplete), 0.4f); // Mesmo tempo do punch no PlayerController2D
        }
        
        private void DetectAndDamageEnemies()
        {
            // Detecta todos os colliders na área de ataque
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
                attackPoint.position, 
                attributes.AttackRange, 
                targetLayers
            );
            
            if (debugAttackDetection)
            {
                Debug.Log($"Detectados {hitEnemies.Length} alvos no alcance de ataque");
            }
            
            foreach (Collider2D enemyCollider in hitEnemies)
            {
                // Tenta pegar o componente de combate do inimigo
                ICombatController enemyCombat = enemyCollider.GetComponent<ICombatController>();
                if (enemyCombat != null)
                {
                    Vector2 attackDirection = (enemyCollider.transform.position - transform.position).normalized;
                    enemyCombat.TakeDamage(attributes.AttackPower, attackDirection);
                    
                    if (debugAttackDetection)
                    {
                        Debug.Log($"Dano aplicado a {enemyCollider.name}: {attributes.AttackPower}");
                    }
                }
                else
                {
                    // Fallback para inimigos que ainda usam o sistema antigo
                    var enemy = enemyCollider.GetComponent<EnemySystem.Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(attributes.AttackPower);
                        
                        if (debugAttackDetection)
                        {
                            Debug.Log($"Dano aplicado (sistema antigo) a {enemyCollider.name}: {attributes.AttackPower}");
                        }
                    }
                }
            }
        }
        
        protected override void OnDamageTaken(float damage, Vector2 attackDirection)
        {
            // Por enquanto, player não toma dano conforme solicitado
            // Pode ser implementado futuramente
            if (debugAttackDetection)
            {
                Debug.Log($"Player levou {damage} de dano (não implementado ainda)");
            }
        }
        
        protected override void OnDeath()
        {
            if (debugAttackDetection)
            {
                Debug.Log("Player morreu!");
            }
            
            // Implementar lógica de morte do player no futuro
            // Por exemplo: tela de game over, respawn, etc.
        }
        
        protected override void OnAttackComplete()
        {
            base.OnAttackComplete();
            
            if (animator != null)
            {
                animator.SetBool("isAttacking", false);
            }
        }
        
        private void OnDrawGizmos()
        {
            if (debugAttackDetection && attackPoint != null)
            {
                // Mostra área de ataque durante o ataque
                if (isAttacking)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(attackPoint.position, attributes.AttackRange);
                }
                else
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(attackPoint.position, attributes.AttackRange);
                }
            }
        }
    }
}
