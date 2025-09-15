using UnityEngine;

namespace CombatSystem
{
    /// <summary>
    /// Componente helper para configurar automaticamente o sistema de combate
    /// Adicione este componente para configuração automática
    /// </summary>
    public class CombatSystemSetup : MonoBehaviour
    {
        [Header("Setup Options")]
        [SerializeField] private bool autoSetupOnStart = true;
        [SerializeField] private CombatantType combatantType = CombatantType.Enemy;
        [SerializeField] private CombatPreset combatPreset;
        
        [Header("Auto-Create Components")]
        [SerializeField] private bool createCombatController = true;
        [SerializeField] private bool createCombatDetector = true;
        [SerializeField] private bool createAttackPoint = true;
        
        [Header("Attack Point Settings")]
        [SerializeField] private Vector2 attackPointOffset = new Vector2(0, 1);
        
        private void Start()
        {
            if (autoSetupOnStart)
            {
                SetupCombatSystem();
            }
        }
        
        [ContextMenu("Setup Combat System")]
        public void SetupCombatSystem()
        {
            // Valida layers
            if (!CombatLayers.ValidateCombatLayers())
            {
                Debug.LogError("Combat layers não estão configurados corretamente! Configure nos Project Settings primeiro.");
                return;
            }
            
            // Configura layer do objeto
            CombatLayers.SetCombatantLayer(gameObject, combatantType);
            
            // Cria ou configura CombatController
            SetupCombatController();
            
            // Cria ou configura CombatDetector
            if (createCombatDetector)
            {
                SetupCombatDetector();
            }
            
            // Cria attack point se necessário
            if (createAttackPoint)
            {
                SetupAttackPoint();
            }
            
            Debug.Log($"Combat system configurado para {gameObject.name} como {combatantType}");
        }
        
        private void SetupCombatController()
        {
            if (!createCombatController) return;
            
            ICombatController existingController = GetComponent<ICombatController>();
            if (existingController != null)
            {
                Debug.Log($"CombatController já existe em {gameObject.name}");
                return;
            }
            
            // Adiciona o controller apropriado baseado no tipo
            CombatController controller = null;
            
            switch (combatantType)
            {
                case CombatantType.Player:
                    controller = gameObject.AddComponent<PlayerCombatController>();
                    break;
                    
                case CombatantType.Enemy:
                    controller = gameObject.AddComponent<EnemyCombatController>();
                    break;
                    
                default:
                    Debug.LogWarning($"Tipo {combatantType} não suporta CombatController automático");
                    return;
            }
            
            // Aplica preset se disponível
            if (combatPreset != null && controller != null)
            {
                // Aguarda o próximo frame para garantir que o controller foi inicializado
                StartCoroutine(ApplyPresetNextFrame(controller));
            }
        }
        
        private System.Collections.IEnumerator ApplyPresetNextFrame(CombatController controller)
        {
            yield return null; // Aguarda um frame
            combatPreset.ApplyToCombatController(controller);
            Debug.Log($"Preset {combatPreset.name} aplicado a {gameObject.name}");
        }
        
        private void SetupCombatDetector()
        {
            CombatDetector detector = GetComponent<CombatDetector>();
            if (detector != null)
            {
                Debug.Log($"CombatDetector já existe em {gameObject.name}");
                return;
            }
            
            detector = gameObject.AddComponent<CombatDetector>();
            
            // Configura target layers baseado no tipo
            LayerMask targetMask = CombatLayers.GetTargetLayerMask(combatantType);
            
            // Usa reflexão para configurar o LayerMask (já que é SerializeField private)
            var detectorType = typeof(CombatDetector);
            var targetLayersField = detectorType.GetField("targetLayers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            targetLayersField?.SetValue(detector, targetMask);
        }
        
        private void SetupAttackPoint()
        {
            // Verifica se já existe um attack point
            Transform existingAttackPoint = transform.Find("AttackPoint");
            if (existingAttackPoint != null)
            {
                Debug.Log($"AttackPoint já existe em {gameObject.name}");
                return;
            }
            
            // Cria o attack point
            GameObject attackPointGO = new GameObject("AttackPoint");
            attackPointGO.transform.SetParent(transform);
            attackPointGO.transform.localPosition = attackPointOffset;
            
            // Adiciona um gizmo visual
            var gizmo = attackPointGO.AddComponent<AttackPointGizmo>();
            
            Debug.Log($"AttackPoint criado para {gameObject.name}");
        }
        
        private void OnValidate()
        {
            // Validação no editor
            if (combatPreset != null && createCombatController)
            {
                // Mostra informações do preset no inspetor
                name = $"{combatPreset.characterName} ({combatantType})";
            }
        }
    }
    
    /// <summary>
    /// Componente simples para visualizar o attack point no editor
    /// </summary>
    public class AttackPointGizmo : MonoBehaviour
    {
        [SerializeField] private Color gizmoColor = Color.red;
        [SerializeField] private float gizmoSize = 0.2f;
        
        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position, gizmoSize);
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(transform.position, gizmoSize);
        }
    }
}
