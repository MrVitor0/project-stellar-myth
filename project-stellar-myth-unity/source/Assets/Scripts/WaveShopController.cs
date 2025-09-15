using UnityEngine;
using EnemySystem;

/// <summary>
/// Controller que integra o sistema de waves com a loja
/// Gerencia quando mostrar a loja entre waves
/// </summary>
public class WaveShopController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private GameController gameController;
    
    [Header("Settings")]
    [SerializeField] private bool showShopBetweenWaves = true;
    [SerializeField] private bool debugMode = false;
    
    // Estado interno
    private bool isWaitingForShopSelection = false;
    
    private void Start()
    {
        InitializeWaveShopController();
    }
    
    private void InitializeWaveShopController()
    {
        // Encontra referências automaticamente se não estiverem definidas
        if (enemySpawner == null)
        {
            enemySpawner = FindObjectOfType<EnemySpawner>();
        }
        
        if (shopManager == null)
        {
            shopManager = FindObjectOfType<ShopManager>();
        }
        
        if (gameController == null)
        {
            gameController = GameController.Instance;
            if (gameController == null)
            {
                gameController = FindObjectOfType<GameController>();
            }
        }
        
        // Configura o spawner para pausar para loja
        if (enemySpawner != null)
        {
            enemySpawner.SetPauseForShop(showShopBetweenWaves);
            enemySpawner.OnWaveCompleted.AddListener(OnWaveCompleted);
        }
        else
        {
            Debug.LogError("WaveShopController: EnemySpawner não encontrado!");
        }
        
        // Subscreve aos eventos da loja
        if (shopManager != null)
        {
            shopManager.OnShopClosed.AddListener(OnShopClosed);
        }
        else
        {
            Debug.LogError("WaveShopController: ShopManager não encontrado!");
        }
        
        if (debugMode)
        {
            Debug.Log("WaveShopController: Inicializado com sucesso!");
        }
    }
    
    private void OnWaveCompleted()
    {
        if (!showShopBetweenWaves || shopManager == null)
        {
            // Se não deve mostrar loja, deixa o spawner continuar normalmente
            if (enemySpawner != null)
            {
                enemySpawner.ContinueToNextWave();
            }
            return;
        }
        
        if (debugMode)
        {
            Debug.Log("WaveShopController: Wave completada, abrindo loja");
        }
        
        // Espera um pouco antes de abrir a loja para dar feedback visual
        Invoke(nameof(OpenShop), 1f);
    }
    
    private void OpenShop()
    {
        if (shopManager != null)
        {
            isWaitingForShopSelection = true;
            shopManager.OpenShop();
        }
    }
    
    private void OnShopClosed()
    {
        if (debugMode)
        {
            Debug.Log("WaveShopController: Loja fechada, continuando para próxima wave");
        }
        
        isWaitingForShopSelection = false;
        
        // Continua para a próxima wave
        if (enemySpawner != null)
        {
            enemySpawner.ContinueToNextWave();
        }
    }
    
    /// <summary>
    /// Método público para desabilitar/habilitar a loja entre waves
    /// </summary>
    public void SetShopBetweenWaves(bool enabled)
    {
        showShopBetweenWaves = enabled;
        
        // Atualiza a configuração do spawner
        if (enemySpawner != null)
        {
            enemySpawner.SetPauseForShop(enabled);
        }
        
        if (debugMode)
        {
            Debug.Log($"WaveShopController: Loja entre waves {(enabled ? "habilitada" : "desabilitada")}");
        }
    }
    
    /// <summary>
    /// Força a abertura da loja (para debug ou uso especial)
    /// </summary>
    public void ForceOpenShop()
    {
        if (shopManager != null && !shopManager.IsShopOpen)
        {
            OpenShop();
        }
    }
    
    // Propriedades públicas
    public bool IsWaitingForShopSelection => isWaitingForShopSelection;
    public bool ShopBetweenWavesEnabled => showShopBetweenWaves;
    
    private void OnDestroy()
    {
        // Remove listeners para evitar erros
        if (enemySpawner != null)
        {
            enemySpawner.OnWaveCompleted.RemoveListener(OnWaveCompleted);
        }
        
        if (shopManager != null)
        {
            shopManager.OnShopClosed.RemoveListener(OnShopClosed);
        }
    }
}
