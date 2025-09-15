using UnityEngine;

namespace CombatSystem
{
    /// <summary>
    /// Utilitário para configurar layers de combate automaticamente
    /// Ajuda na configuração inicial do projeto
    /// </summary>
    public static class CombatLayers
    {
        // Definições de layers
        public const string PLAYER_LAYER = "Player";
        public const string ENEMY_LAYER = "Enemy";
        public const string NEUTRAL_LAYER = "Neutral";
        public const string PROJECTILE_LAYER = "Projectile";
        
        // Layer masks para conveniência
        public static LayerMask PlayerMask => LayerMask.GetMask(PLAYER_LAYER);
        public static LayerMask EnemyMask => LayerMask.GetMask(ENEMY_LAYER);
        public static LayerMask AllCombatantsMask => LayerMask.GetMask(PLAYER_LAYER, ENEMY_LAYER);
        public static LayerMask ProjectileMask => LayerMask.GetMask(PROJECTILE_LAYER);
        
        /// <summary>
        /// Configura o layer de um GameObject baseado no tipo de combatente
        /// </summary>
        public static void SetCombatantLayer(GameObject go, CombatantType type)
        {
            string layerName = type switch
            {
                CombatantType.Player => PLAYER_LAYER,
                CombatantType.Enemy => ENEMY_LAYER,
                CombatantType.Neutral => NEUTRAL_LAYER,
                CombatantType.Projectile => PROJECTILE_LAYER,
                _ => NEUTRAL_LAYER
            };
            
            int layerIndex = LayerMask.NameToLayer(layerName);
            if (layerIndex == -1)
            {
                Debug.LogWarning($"Layer '{layerName}' não encontrado. Certifique-se de criar os layers necessários no projeto.");
                return;
            }
            
            go.layer = layerIndex;
        }
        
        /// <summary>
        /// Obtém o layer mask apropriado para atacar baseado no tipo do atacante
        /// </summary>
        public static LayerMask GetTargetLayerMask(CombatantType attackerType)
        {
            return attackerType switch
            {
                CombatantType.Player => EnemyMask,
                CombatantType.Enemy => PlayerMask,
                CombatantType.Projectile => AllCombatantsMask,
                _ => 0
            };
        }
        
        /// <summary>
        /// Verifica se os layers de combate estão configurados corretamente
        /// </summary>
        public static bool ValidateCombatLayers()
        {
            bool valid = true;
            
            if (LayerMask.NameToLayer(PLAYER_LAYER) == -1)
            {
                Debug.LogError($"Layer '{PLAYER_LAYER}' não encontrado. Adicione nos Project Settings > Tags and Layers.");
                valid = false;
            }
            
            if (LayerMask.NameToLayer(ENEMY_LAYER) == -1)
            {
                Debug.LogError($"Layer '{ENEMY_LAYER}' não encontrado. Adicione nos Project Settings > Tags and Layers.");
                valid = false;
            }
            
            if (LayerMask.NameToLayer(NEUTRAL_LAYER) == -1)
            {
                Debug.LogWarning($"Layer '{NEUTRAL_LAYER}' não encontrado. Considere adicionar nos Project Settings > Tags and Layers.");
            }
            
            if (LayerMask.NameToLayer(PROJECTILE_LAYER) == -1)
            {
                Debug.LogWarning($"Layer '{PROJECTILE_LAYER}' não encontrado. Considere adicionar nos Project Settings > Tags and Layers.");
            }
            
            return valid;
        }
    }
    
    /// <summary>
    /// Tipos de combatentes para facilitar a configuração
    /// </summary>
    public enum CombatantType
    {
        Player,
        Enemy,
        Neutral,
        Projectile
    }
}
