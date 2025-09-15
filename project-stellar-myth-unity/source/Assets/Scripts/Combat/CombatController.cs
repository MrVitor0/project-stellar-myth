using UnityEngine;

namespace CombatSystem
{
    /// <summary>
    /// Interface base para controladores de combate
    /// </summary>
    public interface ICombatController
    {
        CombatAttributes Attributes { get; }
        Transform AttackPoint { get; }
        bool CanAttack { get; }
        void Attack();
        void TakeDamage(float damage, Vector2 attackDirection);
    }
    
    /// <summary>
    /// Controlador de combate base
    /// Responsável por gerenciar atributos de combate e coordenar ataques
    /// </summary>
    public abstract class CombatController : MonoBehaviour, ICombatController
    {
        [Header("Combat Configuration")]
        [SerializeField] protected CombatAttributes attributes;
        [SerializeField] protected Transform attackPoint;
        [SerializeField] protected LayerMask targetLayers;
        
        [Header("Attack Settings")]
        [SerializeField] protected float staminaCostPerAttack = 15f;
        
        // Estado de combate
        protected float lastAttackTime;
        protected bool isAttacking;
        protected bool isCombatLocked = false; // Bloqueia ataques quando true
        
        // Componentes
        protected Animator animator;
        
        public CombatAttributes Attributes => attributes;
        public Transform AttackPoint => attackPoint;
        public virtual bool CanAttack => !isAttacking && 
                                        !isCombatLocked && // Verifica se combate está bloqueado
                                        attributes != null &&
                                        attributes.IsAlive() && 
                                        Time.time >= lastAttackTime + (1f / attributes.AttackSpeed);
        
        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
            
            // Se não há attackPoint definido, cria um
            if (attackPoint == null)
            {
                GameObject attackPointGO = new GameObject("AttackPoint");
                attackPointGO.transform.SetParent(transform);
                attackPointGO.transform.localPosition = Vector3.forward; // 1 unidade à frente
                attackPoint = attackPointGO.transform;
            }
        }
        
        protected virtual void Start()
        {
            // Inicializa attributes se não foi feito no inspector
            if (attributes == null)
            {
                Debug.LogWarning($"CombatAttributes não foi configurado em {gameObject.name}. Criando instância padrão.");
                attributes = new CombatAttributes();
            }
            
            attributes.Initialize();
            
            // Configura eventos
            attributes.OnDeath += OnDeath;
        }
        
        protected virtual void Update()
        {
            // Para de processar se o componente está desabilitado ou sendo destruído
            if (!enabled || !gameObject.activeInHierarchy) return;
            
            // Regenera stamina apenas se attributes foi inicializado
            if (attributes != null)
            {
                attributes.RegenerateStamina(Time.deltaTime);
            }
        }
        
        public virtual void Attack()
        {
            Debug.Log($"[COMBAT] {gameObject.name} - Tentando Attack(): CanAttack={CanAttack}, isAttacking={isAttacking}, attributes={attributes != null}");

            if (!CanAttack || attributes == null) 
            {
                Debug.Log($"[COMBAT] {gameObject.name} - Attack() cancelado: CanAttack={CanAttack}, attributes={attributes != null}");
                return;
            }
            
            // Verifica stamina
            if (!attributes.ConsumeStamina(staminaCostPerAttack))
            {
                OnInsufficientStamina();
                return;
            }
            
            Debug.Log($"[COMBAT] 🚀 {gameObject.name} - Attack() executado com sucesso!");
            
            isAttacking = true;
            lastAttackTime = Time.time;
            
            PerformAttack();
        }
        
        public virtual void TakeDamage(float damage, Vector2 attackDirection)
        {
            if (attributes == null || !attributes.IsAlive()) return;
            
            attributes.TakeDamage(damage);
            OnDamageTaken(damage, attackDirection);
        }
        
        protected abstract void PerformAttack();
        
        protected virtual void OnDamageTaken(float damage, Vector2 attackDirection)
        {
            // Implementado nas classes filhas para animações específicas
        }
        
        protected virtual void OnDeath()
        {
            // Implementado nas classes filhas
        }
        
        protected virtual void OnInsufficientStamina()
        {
            Debug.Log($"[COMBAT] ⚡ {gameObject.name} não tem stamina suficiente para atacar!");
            Debug.Log($"[COMBAT] Current Stamina: {attributes?.CurrentStamina}, Required: {staminaCostPerAttack}");
        }
        
        protected virtual void OnAttackComplete()
        {
            isAttacking = false;
            Debug.Log($"[COMBAT] {gameObject.name} - Ataque completado, isAttacking = false");
        }
        
        protected virtual void OnDrawGizmosSelected()
        {
            if (attackPoint != null && attributes != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(attackPoint.position, attributes.AttackRange);
            }
        }
        
        /// <summary>
        /// Bloqueia o sistema de combate, impedindo ataques
        /// </summary>
        public virtual void LockCombat()
        {
            isCombatLocked = true;
            Debug.Log($"[COMBAT] {gameObject.name} - Sistema de combate bloqueado!");
        }
        
        /// <summary>
        /// Desbloqueia o sistema de combate, permitindo ataques
        /// </summary>
        public virtual void UnlockCombat()
        {
            isCombatLocked = false;
            Debug.Log($"[COMBAT] {gameObject.name} - Sistema de combate desbloqueado!");
        }
        
        /// <summary>
        /// Verifica se o sistema de combate está bloqueado
        /// </summary>
        public virtual bool IsCombatLocked()
        {
            return isCombatLocked;
        }
    }
}
