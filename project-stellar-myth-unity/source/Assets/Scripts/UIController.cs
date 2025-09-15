using UnityEngine;
using UnityEngine.UI;
using CombatSystem;

public class UIController : MonoBehaviour
{
    [Header("UI Sliders")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider staminaSlider;
    
    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button restartButton;
    
    [Header("Player Reference")]
    [SerializeField] private PlayerController2D playerController;
    
    // Referências do sistema de combate
    private ICombatController playerCombat;
    private CombatAttributes playerAttributes;
    
    void Start()
    {
        InitializeUIController();
        InitializeGameOverUI();
    }
    
    void Update()
    {
        // Se não temos referência do player, tenta encontrar automaticamente
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController2D>();
        }
        
        // Se não temos referência do combat system, tenta encontrar
        if (playerCombat == null && playerController != null)
        {
            playerCombat = playerController.GetComponent<ICombatController>();
            if (playerCombat != null)
            {
                playerAttributes = playerCombat.Attributes;
                
                // Força inicialização se necessário
                if (playerAttributes != null)
                {
                    playerAttributes.Initialize();
                }
                
                SubscribeToEvents();
                InitializeSliders();
                
                Debug.Log("UIController: Conexão com sistema de combate estabelecida no Update!");
            }
        }
        
        // Força atualização dos sliders se os valores mudaram e não temos eventos
        ForceUpdateSlidersIfNeeded();
    }
    
    /// <summary>
    /// Força atualização dos sliders se os valores mudaram mas os eventos não foram chamados
    /// </summary>
    private void ForceUpdateSlidersIfNeeded()
    {
        if (playerAttributes == null) return;
        
        // Verifica se os valores dos sliders estão desatualizados
        if (healthSlider != null && Mathf.Abs(healthSlider.value - playerAttributes.CurrentHealth) > 0.1f)
        {
            healthSlider.value = playerAttributes.CurrentHealth;
            healthSlider.maxValue = playerAttributes.MaxHealth;
        }
        
        if (staminaSlider != null && Mathf.Abs(staminaSlider.value - playerAttributes.CurrentStamina) > 0.1f)
        {
            staminaSlider.value = playerAttributes.CurrentStamina;
            staminaSlider.maxValue = playerAttributes.MaxStamina;
        }
    }
    
    private void InitializeUIController()
    {
        // Tenta encontrar referências automaticamente se não estão definidas
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController2D>();
            Debug.Log("UIController: Player encontrado automaticamente!");
        }
        
        if (playerController != null)
        {
            playerCombat = playerController.GetComponent<ICombatController>();
            if (playerCombat != null)
            {
                playerAttributes = playerCombat.Attributes;
                
                // Força a inicialização dos atributos se necessário
                if (playerAttributes != null)
                {
                    playerAttributes.Initialize();
                }
                
                SubscribeToEvents();
                InitializeSliders();
                
                Debug.Log("UIController: Sistema de combate conectado com sucesso!");
            }
            else
            {
                Debug.LogWarning("UIController: Player não tem ICombatController! UI não funcionará corretamente.");
            }
        }
        else
        {
            Debug.LogWarning("UIController: PlayerController2D não encontrado!");
        }
    }
    
    private void SubscribeToEvents()
    {
        if (playerAttributes != null)
        {
            // Se inscreve nos eventos do sistema de combate
            playerAttributes.OnHealthChanged += UpdateHealthSlider;
            playerAttributes.OnStaminaChanged += UpdateStaminaSlider;
            playerAttributes.OnDeath += OnPlayerDeath; // Novo evento para morte do player
        }
    }
    
    private void InitializeSliders()
    {
        if (playerAttributes == null) return;
        
        // Configura slider de vida
        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = playerAttributes.MaxHealth;
            healthSlider.value = playerAttributes.CurrentHealth;
            
            Debug.Log($"UIController: Health Slider inicializado - Value: {playerAttributes.CurrentHealth}/{playerAttributes.MaxHealth}");
        }
        
        // Configura slider de stamina  
        if (staminaSlider != null)
        {
            staminaSlider.minValue = 0f;
            staminaSlider.maxValue = playerAttributes.MaxStamina;
            staminaSlider.value = playerAttributes.CurrentStamina;
            
            Debug.Log($"UIController: Stamina Slider inicializado - Value: {playerAttributes.CurrentStamina}/{playerAttributes.MaxStamina}");
        }
    }
    
    /// <summary>
    /// Chamado automaticamente quando a vida do player muda
    /// </summary>
    private void UpdateHealthSlider(float currentHealth, float maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }
    
    /// <summary>
    /// Chamado automaticamente quando a stamina do player muda
    /// </summary>
    private void UpdateStaminaSlider(float currentStamina, float maxStamina)
    {
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }
    }
    
    private void OnDestroy()
    {
        // Remove os eventos para evitar memory leaks
        if (playerAttributes != null)
        {
            playerAttributes.OnHealthChanged -= UpdateHealthSlider;
            playerAttributes.OnStaminaChanged -= UpdateStaminaSlider;
            playerAttributes.OnDeath -= OnPlayerDeath; // Remove evento de morte
        }
        
        // Remove listener do botão
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(RestartGame);
        }
    }
    
    // Métodos públicos para acesso direto (opcional, para compatibilidade)
    public void ManualUpdateHealth(float currentHealth, float maxHealth)
    {
        UpdateHealthSlider(currentHealth, maxHealth);
    }
    
    public void ManualUpdateStamina(float currentStamina, float maxStamina)
    {
        UpdateStaminaSlider(currentStamina, maxStamina);
    }
    
    /// <summary>
    /// Inicializa o sistema de Game Over UI
    /// </summary>
    private void InitializeGameOverUI()
    {
        // Garante que o painel de Game Over esteja desabilitado no início
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        // Configura o botão de restart
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        
        Debug.Log("UIController: Game Over UI inicializada!");
    }
    
    /// <summary>
    /// Chamado quando o player morre
    /// </summary>
    private void OnPlayerDeath()
    {
        Debug.Log("UIController: Player morreu! Exibindo tela de Game Over...");
        ShowGameOverScreen();
    }
    
    /// <summary>
    /// Exibe a tela de Game Over
    /// </summary>
    public void ShowGameOverScreen()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            // Pausa o jogo
            Time.timeScale = 0f;
            
            // Bloqueia movimentos e ataques do player
            LockPlayerControls();
            
            Debug.Log("UIController: Tela de Game Over exibida!");
        }
        else
        {
            Debug.LogWarning("UIController: Game Over Panel não está configurado!");
        }
    }
    
    /// <summary>
    /// Esconde a tela de Game Over
    /// </summary>
    public void HideGameOverScreen()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            
            // Resume o jogo
            Time.timeScale = 1f;
            
            // Desbloqueia controles do player (caso necessário)
            UnlockPlayerControls();
            
            Debug.Log("UIController: Tela de Game Over escondida!");
        }
    }
    
    /// <summary>
    /// Método público para reiniciar o jogo - Conecte este método ao onClick do botão
    /// </summary>
    public void RestartGame()
    {
        Debug.Log("UIController: Reiniciando o jogo...");
        
        // Restaura o time scale antes de recarregar a cena
        Time.timeScale = 1f;
        
        // Recarrega a cena atual
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    
    /// <summary>
    /// Bloqueia todos os controles do player (movimento, dash, ataque)
    /// </summary>
    private void LockPlayerControls()
    {
        if (playerController != null)
        {
            // Bloqueia movimentos e dash
            playerController.LockPlayer();
        }
        
        if (playerCombat != null)
        {
            // Bloqueia sistema de combate
            if (playerCombat is CombatSystem.CombatController combatController)
            {
                combatController.LockCombat();
            }
        }
        
        Debug.Log("UIController: Controles do player bloqueados - sem movimento, dash ou ataque!");
    }
    
    /// <summary>
    /// Desbloqueia todos os controles do player
    /// </summary>
    private void UnlockPlayerControls()
    {
        if (playerController != null)
        {
            // Desbloqueia movimentos e dash
            playerController.UnlockPlayer();
        }
        
        if (playerCombat != null)
        {
            // Desbloqueia sistema de combate
            if (playerCombat is CombatSystem.CombatController combatController)
            {
                combatController.UnlockCombat();
            }
        }
        
        Debug.Log("UIController: Controles do player desbloqueados!");
    }
    
    // Propriedades para acesso externo
    public float CurrentHealth => playerAttributes?.CurrentHealth ?? 0f;
    public float MaxHealth => playerAttributes?.MaxHealth ?? 0f;
    public float CurrentStamina => playerAttributes?.CurrentStamina ?? 0f;
    public float MaxStamina => playerAttributes?.MaxStamina ?? 0f;
    
    public bool IsHealthEmpty => playerAttributes?.CurrentHealth <= 0f;
    public bool IsStaminaEmpty => playerAttributes?.CurrentStamina <= 0f;
    public bool HasEnoughStamina(float required) => playerAttributes?.CurrentStamina >= required;
}
