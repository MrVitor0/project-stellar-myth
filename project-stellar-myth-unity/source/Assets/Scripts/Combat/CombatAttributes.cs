using UnityEngine;

namespace CombatSystem
{
    /// <summary>
    /// Dados de atributos de combate que podem ser compartilhados entre player e inimigos
    /// </summary>
    [System.Serializable]
    public class CombatAttributes
    {
        [Header("Health")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;
        
        [Header("Stamina")]
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float currentStamina;
        [SerializeField] private float staminaRegenRate = 10f;
        
        [Header("Attack")]
        [SerializeField] private float attackPower = 25f;
        [SerializeField] private float attackSpeed = 1f; // Ataques por segundo
        [SerializeField] private float attackRange = 1.5f;
        
        // Properties
        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;
        public float MaxStamina => maxStamina;
        public float CurrentStamina => currentStamina;
        public float StaminaRegenRate => staminaRegenRate;
        public float AttackPower => attackPower;
        public float AttackSpeed => attackSpeed;
        public float AttackRange => attackRange;
        
        // Events
        public System.Action<float, float> OnHealthChanged; // current, max
        public System.Action<float, float> OnStaminaChanged; // current, max
        public System.Action OnDeath;
        
        public void Initialize()
        {
            currentHealth = maxHealth;
            currentStamina = maxStamina;
        }
        
        public void TakeDamage(float damage)
        {
            currentHealth = Mathf.Max(0, currentHealth - damage);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            
            if (currentHealth <= 0)
            {
                OnDeath?.Invoke();
            }
        }
        
        public void Heal(float healAmount)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
        
        public void IncreaseMaxHealth(float amount)
        {
            maxHealth += amount;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
        
        public void IncreaseMaxStamina(float amount)
        {
            maxStamina += amount;
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        }
        
        public void RestoreStamina(float amount)
        {
            currentStamina = Mathf.Min(maxStamina, currentStamina + amount);
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        }
        
        public bool ConsumeStamina(float staminaCost)
        {
            if (currentStamina >= staminaCost)
            {
                currentStamina -= staminaCost;
                OnStaminaChanged?.Invoke(currentStamina, maxStamina);
                return true;
            }
            return false;
        }
        
        public void RegenerateStamina(float deltaTime)
        {
            if (currentStamina < maxStamina)
            {
                currentStamina = Mathf.Min(maxStamina, currentStamina + staminaRegenRate * deltaTime);
                OnStaminaChanged?.Invoke(currentStamina, maxStamina);
            }
        }
        
        public bool IsAlive()
        {
            return currentHealth > 0;
        }
    }
}
