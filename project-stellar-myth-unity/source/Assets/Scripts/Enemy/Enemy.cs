using UnityEngine;
using CombatSystem;

namespace EnemySystem
{
    /// <summary>
    /// Classe de interface para inimigos - PROXY para CombatController
    /// Responsabilidades:
    /// - Interface com EnemySpawner
    /// - Proxy para CombatController (única fonte de verdade)
    /// - Compatibilidade com sistemas legados
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        [Header("Enemy Settings")]
        [SerializeField] private bool destroyOnDeath = true;

        private EnemySpawner spawner;
        
        // CombatController é a ÚNICA fonte de verdade
        private ICombatController combatController;

        #region Properties

        // CombatController é a ÚNICA fonte de verdade - Enemy é apenas um proxy
        public float Health => combatController?.Attributes?.CurrentHealth ?? 0f;
        public float MaxHealth => combatController?.Attributes?.MaxHealth ?? 0f;
        public bool IsDead => combatController?.Attributes?.IsAlive() == false;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // CombatController é OBRIGATÓRIO - é a única fonte de verdade
            combatController = GetComponent<ICombatController>();
            
            if (combatController == null)
            {
             
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Inicializa o inimigo com referência ao spawner
        /// </summary>
        public void Initialize(EnemySpawner enemySpawner)
        {
            spawner = enemySpawner;
        }

        /// <summary>
        /// PROXY: Delega dano para CombatController (única fonte de verdade)
        /// </summary>
        public virtual void TakeDamage(float damage)
        {
            if (combatController == null)
            {
                return;
            }

            // CombatController é a ÚNICA fonte de verdade
            Vector2 attackDirection = Vector2.zero; // Direção neutra para ataques legacy
            combatController.TakeDamage(damage, attackDirection);
        }

        /// <summary>
        /// Método chamado pelo CombatController quando o inimigo morre
        /// APENAS para notificar o spawner - NÃO gerencia morte
        /// </summary>
        public virtual void Die()
        {
            // APENAS notifica o spawner - CombatController gerencia a morte
            OnDeath();
            spawner?.OnEnemyDeath();

            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
        }

        #endregion

        #region Event Callbacks (Chamados pelo CombatController)

        /// <summary>
        /// Callback para quando o inimigo recebe dano - apenas para logs/efeitos
        /// </summary>
        public virtual void OnDamageTaken(float damage)
        {
            float currentHP = Health;
            float maxHP = MaxHealth;
        }

        /// <summary>
        /// Callback para quando o inimigo morre - apenas para efeitos
        /// </summary>
        protected virtual void OnDeath()
        {
            
            // Aqui você pode adicionar efeitos de morte, drop de itens, etc.
            // Por exemplo:
            // - Tocar som de morte
            // - Criar efeito de partículas
            // - Dropar itens
            // - Dar experiência ao jogador
        }

        /// <summary>
        /// Callback para quando o inimigo é curado - apenas para logs/efeitos
        /// </summary>
        protected virtual void OnHealed(float healAmount)
        {
            float currentHP = Health;
            float maxHP = MaxHealth;
        }

    
        #endregion

        #region Editor Support

        private void OnDrawGizmosSelected()
        {
            // Visualização da vida no editor
            Gizmos.color = Color.red;
            Vector3 healthBarPos = transform.position + Vector3.up * 2f;
            Vector3 healthBarSize = new Vector3(2f, 0.2f, 0f);
            
            // Barra de vida de fundo
            Gizmos.DrawCube(healthBarPos, healthBarSize);
            
            // Barra de vida atual
            if (Application.isPlaying)
            {
                Gizmos.color = Color.green;
                float currentHP = Health;
                float maxHP = MaxHealth;
                
                if (maxHP > 0)
                {
                    float healthPercentage = currentHP / maxHP;
                    Vector3 currentHealthSize = new Vector3(healthBarSize.x * healthPercentage, healthBarSize.y, healthBarSize.z);
                    Vector3 currentHealthPos = healthBarPos - Vector3.right * (healthBarSize.x - currentHealthSize.x) * 0.5f;
                    Gizmos.DrawCube(currentHealthPos, currentHealthSize);
                }
            }
        }

        #endregion
    }
}
