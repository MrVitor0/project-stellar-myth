using UnityEngine;

namespace CombatSystem
{
    /// <summary>
    /// ScriptableObject para configurar presets de combate
    /// Facilita a criação de diferentes tipos de personagens e inimigos
    /// </summary>
    [CreateAssetMenu(fileName = "NewCombatPreset", menuName = "Combat System/Combat Preset")]
    public class CombatPreset : ScriptableObject
    {
        [Header("Character Info")]
        public string characterName = "New Character";
        [TextArea(3, 5)]
        public string description = "Descrição do personagem";
        
        [Header("Health Settings")]
        public float maxHealth = 100f;
        
        [Header("Stamina Settings")]
        public float maxStamina = 100f;
        public float staminaRegenRate = 10f;
        
        [Header("Attack Settings")]
        public float attackPower = 25f;
        public float attackSpeed = 1f;
        public float attackRange = 1.5f;
        public float staminaCostPerAttack = 15f;
        
        [Header("Visual Settings")]
        public Color healthBarColor = Color.green;
        public Color staminaBarColor = Color.blue;
        
        /// <summary>
        /// Aplica este preset a um CombatAttributes
        /// </summary>
        public void ApplyToAttributes(CombatAttributes attributes)
        {
            // Usa reflexão para aplicar os valores
            var attributesType = typeof(CombatAttributes);
            
            // Health
            var maxHealthField = attributesType.GetField("maxHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            maxHealthField?.SetValue(attributes, maxHealth);
            
            // Stamina
            var maxStaminaField = attributesType.GetField("maxStamina", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            maxStaminaField?.SetValue(attributes, maxStamina);
            
            var staminaRegenField = attributesType.GetField("staminaRegenRate", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            staminaRegenField?.SetValue(attributes, staminaRegenRate);
            
            // Attack
            var attackPowerField = attributesType.GetField("attackPower", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            attackPowerField?.SetValue(attributes, attackPower);
            
            var attackSpeedField = attributesType.GetField("attackSpeed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            attackSpeedField?.SetValue(attributes, attackSpeed);
            
            var attackRangeField = attributesType.GetField("attackRange", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            attackRangeField?.SetValue(attributes, attackRange);
            
            // Reinicializa os atributos para aplicar os novos valores
            attributes.Initialize();
        }
        
        /// <summary>
        /// Aplica configurações específicas a um CombatController
        /// </summary>
        public void ApplyToCombatController(CombatController controller)
        {
            ApplyToAttributes(controller.Attributes);
            
            // Aplica custo de stamina por ataque
            var controllerType = typeof(CombatController);
            var staminaCostField = controllerType.GetField("staminaCostPerAttack", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            staminaCostField?.SetValue(controller, staminaCostPerAttack);
        }
    }
}
