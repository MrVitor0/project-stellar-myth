using UnityEngine;
using System.Collections;

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
        
        [Header("Visual Effects")]
        [SerializeField] private float damageBlinksCount = 3f; // Número de piscadas quando recebe dano
        [SerializeField] private float damageBlinkDuration = 0.1f; // Duração de cada piscada
        [SerializeField] private Color damageBlinkColor = Color.red; // Cor da piscada de dano
        
        private PlayerController2D playerController;
        private SpriteRenderer playerSpriteRenderer;
        private Color originalSpriteColor;
        private bool isBlinking = false;
        
        protected override void Awake()
        {
            base.Awake();
            playerController = GetComponent<PlayerController2D>();
            playerSpriteRenderer = GetComponent<SpriteRenderer>();
            
            // Se não encontrou SpriteRenderer no mesmo GameObject, procura nos filhos
            if (playerSpriteRenderer == null)
            {
                playerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
            
            if (playerSpriteRenderer != null)
            {
                originalSpriteColor = playerSpriteRenderer.color;
            }
            else
            {
                Debug.LogWarning($"PlayerCombatController em {gameObject.name} não encontrou SpriteRenderer. Efeitos visuais de dano não funcionarão.");
            }
            
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
                // CombatController é OBRIGATÓRIO - única fonte de verdade
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
                    Debug.LogWarning($"Inimigo {enemyCollider.name} não tem CombatController! Sistema legado removido.");
                }
            }
        }
        
        protected override void OnDamageTaken(float damage, Vector2 attackDirection)
        {
            // Implementa efeito visual de dano (piscada vermelha)
            if (playerSpriteRenderer != null && !isBlinking)
            {
                StartCoroutine(DamageBlinkEffect());
            }
            
            // Pode adicionar outros efeitos como knockback, shake da câmera, etc.
            
            if (debugAttackDetection)
            {
                Debug.Log($"Player levou {damage} de dano. Vida restante: {attributes.CurrentHealth}/{attributes.MaxHealth}");
            }
        }
        
        /// <summary>
        /// Corrotina para efeito de piscada vermelha quando recebe dano
        /// </summary>
        private IEnumerator DamageBlinkEffect()
        {
            if (playerSpriteRenderer == null || isBlinking) yield break;
            
            isBlinking = true;
            
            for (int i = 0; i < damageBlinksCount; i++)
            {
                // Muda para cor de dano
                playerSpriteRenderer.color = damageBlinkColor;
                yield return new WaitForSeconds(damageBlinkDuration);
                
                // Volta para cor original
                playerSpriteRenderer.color = originalSpriteColor;
                yield return new WaitForSeconds(damageBlinkDuration);
            }
            
            // Garante que volta para a cor original
            playerSpriteRenderer.color = originalSpriteColor;
            isBlinking = false;
        }
        
        protected override void OnDeath()
        {
            if (debugAttackDetection)
            {
                Debug.Log("Player morreu!");
            }
            
            // Para o efeito de piscada se estiver ativo
            StopAllCoroutines();
            isBlinking = false;
            
            // Pode adicionar efeitos de morte aqui
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
