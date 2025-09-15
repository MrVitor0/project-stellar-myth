using UnityEngine;

namespace CombatSystem
{
    /// <summary>
    /// Sistema para detectar ataques e aplicar dano
    /// Pode ser usado tanto para player quanto para inimigos
    /// </summary>
    public class CombatDetector : MonoBehaviour
    {
        [Header("Detection Settings")]
        [SerializeField] private LayerMask targetLayers;
        [SerializeField] private bool debugDetection = true;
        [SerializeField] private Color gizmoColor = Color.red;
        
        private ICombatController combatController;
        
        private void Awake()
        {
            combatController = GetComponent<ICombatController>();
            if (combatController == null)
            {
                Debug.LogError($"CombatDetector em {gameObject.name} precisa de um ICombatController!");
            }
        }
        
        /// <summary>
        /// Detecta alvos em uma área específica e aplica dano
        /// </summary>
        public int DetectAndDamageTargets(Vector2 center, float radius, float damage, LayerMask layers)
        {
            Collider2D[] targets = Physics2D.OverlapCircleAll(center, radius, layers);
            int hitCount = 0;
            
            foreach (Collider2D target in targets)
            {
                // Ignora a si mesmo
                if (target.gameObject == gameObject)
                    continue;
                
                ICombatController targetCombat = target.GetComponent<ICombatController>();
                if (targetCombat != null)
                {
                    Vector2 attackDirection = (target.transform.position - transform.position).normalized;
                    targetCombat.TakeDamage(damage, attackDirection);
                    hitCount++;
                    
                    if (debugDetection)
                    {
                        Debug.Log($"{gameObject.name} atingiu {target.name} causando {damage} de dano");
                    }
                }
                else
                {
                    // Fallback para sistemas antigos
                    var enemy = target.GetComponent<EnemySystem.Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage);
                        hitCount++;
                        
                        if (debugDetection)
                        {
                            Debug.Log($"{gameObject.name} atingiu {target.name} (sistema antigo) causando {damage} de dano");
                        }
                    }
                }
            }
            
            return hitCount;
        }
        
        /// <summary>
        /// Detecta alvos usando as configurações do combatController
        /// </summary>
        public int DetectAndDamageTargets()
        {
            if (combatController?.AttackPoint == null || combatController?.Attributes == null)
                return 0;
            
            return DetectAndDamageTargets(
                combatController.AttackPoint.position,
                combatController.Attributes.AttackRange,
                combatController.Attributes.AttackPower,
                targetLayers
            );
        }
        
        /// <summary>
        /// Verifica se há alvos no alcance sem causar dano
        /// </summary>
        public bool HasTargetsInRange()
        {
            if (combatController?.AttackPoint == null || combatController?.Attributes == null)
                return false;
            
            Collider2D[] targets = Physics2D.OverlapCircleAll(
                combatController.AttackPoint.position,
                combatController.Attributes.AttackRange,
                targetLayers
            );
            
            return targets.Length > 0;
        }
        
        /// <summary>
        /// Obtém o alvo mais próximo no alcance
        /// </summary>
        public Transform GetNearestTarget()
        {
            if (combatController?.AttackPoint == null || combatController?.Attributes == null)
                return null;
            
            Collider2D[] targets = Physics2D.OverlapCircleAll(
                combatController.AttackPoint.position,
                combatController.Attributes.AttackRange,
                targetLayers
            );
            
            Transform nearestTarget = null;
            float nearestDistance = float.MaxValue;
            
            foreach (Collider2D target in targets)
            {
                if (target.gameObject == gameObject) continue;
                
                float distance = Vector2.Distance(transform.position, target.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestTarget = target.transform;
                }
            }
            
            return nearestTarget;
        }
        
        private void OnDrawGizmosSelected()
        {
            if (combatController?.AttackPoint != null && combatController?.Attributes != null)
            {
                Gizmos.color = gizmoColor;
                Gizmos.DrawWireSphere(combatController.AttackPoint.position, combatController.Attributes.AttackRange);
                
                if (debugDetection && Application.isPlaying)
                {
                    // Mostra alvos detectados
                    Collider2D[] targets = Physics2D.OverlapCircleAll(
                        combatController.AttackPoint.position,
                        combatController.Attributes.AttackRange,
                        targetLayers
                    );
                    
                    Gizmos.color = Color.yellow;
                    foreach (Collider2D target in targets)
                    {
                        if (target.gameObject != gameObject)
                        {
                            Gizmos.DrawLine(transform.position, target.transform.position);
                        }
                    }
                }
            }
        }
    }
}
