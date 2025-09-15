using UnityEngine;

namespace EnemySystem
{
    /// <summary>
    /// Classe base para inimigos
    /// Seguindo o Single Responsibility Principle (SRP)
    /// Esta classe é responsável apenas por gerenciar o estado básico do inimigo
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        [Header("Enemy Settings")]
        [SerializeField] private float health = 100f;
        [SerializeField] private bool destroyOnDeath = true;

        private EnemySpawner spawner;
        private float currentHealth;
        private bool isDead = false;

        #region Properties

        public float Health => currentHealth;
        public float MaxHealth => health;
        public bool IsDead => isDead;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            currentHealth = health;
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
        /// Aplica dano ao inimigo
        /// </summary>
        public virtual void TakeDamage(float damage)
        {
            if (isDead) return;

            currentHealth -= damage;
            currentHealth = Mathf.Max(0, currentHealth);

            OnDamageTaken(damage);

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Mata o inimigo instantaneamente
        /// </summary>
        public virtual void Die()
        {
            if (isDead) return;

            isDead = true;
            OnDeath();

            // Notifica o spawner sobre a morte
            spawner?.OnEnemyDeath();

            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Cura o inimigo
        /// </summary>
        public virtual void Heal(float healAmount)
        {
            if (isDead) return;

            currentHealth += healAmount;
            currentHealth = Mathf.Min(health, currentHealth);
            OnHealed(healAmount);
        }

        #endregion

        #region Protected Virtual Methods

        /// <summary>
        /// Chamado quando o inimigo recebe dano
        /// </summary>
        protected virtual void OnDamageTaken(float damage)
        {
            Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}/{health}");
        }

        /// <summary>
        /// Chamado quando o inimigo morre
        /// </summary>
        protected virtual void OnDeath()
        {
            Debug.Log($"{gameObject.name} died!");
            
            // Aqui você pode adicionar efeitos de morte, drop de itens, etc.
            // Por exemplo:
            // - Tocar som de morte
            // - Criar efeito de partículas
            // - Dropar itens
            // - Dar experiência ao jogador
        }

        /// <summary>
        /// Chamado quando o inimigo é curado
        /// </summary>
        protected virtual void OnHealed(float healAmount)
        {
            Debug.Log($"{gameObject.name} healed {healAmount}. Health: {currentHealth}/{health}");
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
                float healthPercentage = currentHealth / health;
                Vector3 currentHealthSize = new Vector3(healthBarSize.x * healthPercentage, healthBarSize.y, healthBarSize.z);
                Vector3 currentHealthPos = healthBarPos - Vector3.right * (healthBarSize.x - currentHealthSize.x) * 0.5f;
                Gizmos.DrawCube(currentHealthPos, currentHealthSize);
            }
        }

        #endregion
    }
}
