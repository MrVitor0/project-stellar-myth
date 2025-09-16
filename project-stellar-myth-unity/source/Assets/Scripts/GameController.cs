using UnityEngine;
using CombatSystem;

/// <summary>
/// Game Controller principal que gerencia a UI e coordena sistemas do jogo
/// Adicione este script em um GameObject vazio chamado "GameController" na cena
/// </summary>
public class GameController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private UIController uiController;
    [SerializeField] private AdvancedUIController advancedUIController;
    [SerializeField] private Canvas gameCanvas;
    
    [Header("Player References")]
    [SerializeField] private PlayerController2D player;
    [SerializeField] private GameObject playerGameObject;
    
    [Header("Game Settings")]
    [SerializeField] private bool useAdvancedUI = true;
    [SerializeField] private bool autoFindReferences = true;
    [SerializeField] private bool debugMode = false;
    
    // System references
    private ICombatController playerCombat;
    private CombatAttributes playerAttributes;
    
    // Game state
    private bool isGamePaused = false;
    private bool isPlayerAlive = true;
    
    void Awake()
    {
        InitializeGameController();
    }
    
    void Start()
    {
        SetupGameSystems();
    }
    
    void Update()
    {
        if (debugMode)
        {
            HandleDebugInput();
        }
        
        UpdateGameState();
    }
    
    private void InitializeGameController()
    {
        if (autoFindReferences)
        {
            FindMissingReferences();
        }
        
        // Disable the UI controller we're not using
        if (useAdvancedUI)
        {
            if (uiController != null) uiController.enabled = false;
            if (advancedUIController != null) advancedUIController.enabled = true;
        }
        else
        {
            if (advancedUIController != null) advancedUIController.enabled = false;
            if (uiController != null) uiController.enabled = true;
        }
    }
    
    private void FindMissingReferences()
    {
        // Find player references
        if (player == null)
        {
            player = FindObjectOfType<PlayerController2D>();
        }
        
        if (playerGameObject == null && player != null)
        {
            playerGameObject = player.gameObject;
        }
        
        // Find UI references
        if (gameCanvas == null)
        {
            gameCanvas = FindObjectOfType<Canvas>();
        }
        
        if (uiController == null)
        {
            uiController = FindObjectOfType<UIController>();
        }
        
        if (advancedUIController == null)
        {
            advancedUIController = FindObjectOfType<AdvancedUIController>();
        }
        
 
    }
    
    private void SetupGameSystems()
    {
        // Get player combat system reference
        if (player != null)
        {
            playerCombat = player.GetComponent<ICombatController>();
            if (playerCombat != null)
            {
                playerAttributes = playerCombat.Attributes;
                
                // Força inicialização dos atributos
                if (playerAttributes != null)
                {
                    playerAttributes.Initialize();
                }
                
                SubscribeToPlayerEvents();
            }
          
        }
        
        // Setup UI system
        SetupUISystem();
        
      
    }
    
    private void SetupUISystem()
    {
        // Both UI controllers auto-initialize, so we don't need to do much here
        // This method is here for future expansion

    }
    
    private void SubscribeToPlayerEvents()
    {
        if (playerAttributes != null)
        {
            playerAttributes.OnDeath += OnPlayerDeath;
        }
    }
    
    private void UpdateGameState()
    {
        // Update player alive status
        if (playerAttributes != null)
        {
            bool wasAlive = isPlayerAlive;
            isPlayerAlive = playerAttributes.IsAlive();
            
            // Player died
            if (wasAlive && !isPlayerAlive)
            {
                HandlePlayerDeath();
            }
        }
    }
    
    private void OnPlayerDeath()
    {
        // Notifica o UIController apropriado para mostrar a tela de Game Over
        if (useAdvancedUI && advancedUIController != null)
        {
            advancedUIController.ShowGameOverScreen();
        }
        else if (uiController != null)
        {
            uiController.ShowGameOverScreen();
        }
      
        
        HandlePlayerDeath();
    }
    
    private void HandlePlayerDeath()
    {
        // Lógica adicional de morte do player
        isPlayerAlive = false;
        
        // Aqui você pode adicionar outras lógicas como:
        // - Parar música de fundo
        // - Tocar som de morte
        // - Salvar estatísticas
        // - etc.
    }
    
    private void HandleDebugInput()
    {
        // Debug controls for testing
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ToggleUIMode();
        }
        
        if (Input.GetKeyDown(KeyCode.F2) && playerAttributes != null)
        {
            playerAttributes.TakeDamage(10f); // Debug damage
        }
        
        if (Input.GetKeyDown(KeyCode.F3) && playerAttributes != null)
        {
            playerAttributes.Heal(20f); // Debug heal
        }
        
        if (Input.GetKeyDown(KeyCode.F4) && playerAttributes != null)
        {
            playerAttributes.ConsumeStamina(15f); // Debug stamina consume
        }
        
        if (Input.GetKeyDown(KeyCode.F5))
        {
            ForceReinitialize(); // Debug reinicialização
        }
    }
    
    // Public methods for external control
    public void ToggleUIMode()
    {
        useAdvancedUI = !useAdvancedUI;
        
        if (useAdvancedUI)
        {
            if (uiController != null) uiController.enabled = false;
            if (advancedUIController != null) advancedUIController.enabled = true;
        }
        else
        {
            if (advancedUIController != null) advancedUIController.enabled = false;
            if (uiController != null) uiController.enabled = true;
        }
    }
    
    public void PauseGame()
    {
        Time.timeScale = 0f;
        isGamePaused = true;
    }
    
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isGamePaused = false;
    }
    
    public void RestartGame()
    {
        // Reseta o estado do ShopManager antes de recarregar a cena
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.OnGameRestart();
            
        
        }
        
        // Reseta também o sistema persistente
        if (ShopPersistentData.Instance != null)
        {
            ShopPersistentData.Instance.ResetShopState();
            
      
        }
        
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    
    /// <summary>
    /// Força a reinicialização da UI e sistemas
    /// Útil se algo der errado na inicialização
    /// </summary>
    public void ForceReinitialize()
    {
  
        // Reinicializa referências
        FindMissingReferences();
        SetupGameSystems();
        
        // Força reinicialização da UI ativa
        if (useAdvancedUI && advancedUIController != null)
        {
            advancedUIController.enabled = false;
            advancedUIController.enabled = true;
        }
        else if (!useAdvancedUI && uiController != null)
        {
            uiController.enabled = false;
            uiController.enabled = true;
        }
        
    }
    
    // Properties for external access
    public bool IsGamePaused => isGamePaused;
    public bool IsPlayerAlive => isPlayerAlive;
    public UIController BasicUIController => uiController;
    public AdvancedUIController AdvancedUIController => advancedUIController;
    public PlayerController2D Player => player;
    public CombatAttributes PlayerAttributes => playerAttributes;
    
    private void OnDestroy()
    {
        if (playerAttributes != null)
        {
            playerAttributes.OnDeath -= OnPlayerDeath;
        }
    }
    
    // Static instance for easy access (optional)
    public static GameController Instance { get; private set; }
    
    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    private void OnDisable()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
