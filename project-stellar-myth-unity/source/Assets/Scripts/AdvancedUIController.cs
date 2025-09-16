using UnityEngine;
using UnityEngine.UI;
using CombatSystem;
using System.Collections;

/// <summary>
/// Controller avançado de UI com efeitos visuais e animações para os sliders
/// </summary>
public class AdvancedUIController : MonoBehaviour
{
    [Header("UI Sliders")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider staminaSlider;
    
    [Header("Visual Effects")]
    [SerializeField] private Image healthFill;
    [SerializeField] private Image staminaFill;
    [SerializeField] private Color healthColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private Color staminaColor = Color.blue;
    [SerializeField] private Color lowStaminaColor = Color.yellow;
    [SerializeField] private float lowHealthThreshold = 0.25f;
    [SerializeField] private float lowStaminaThreshold = 0.2f;
    
    [Header("Animation Settings")]
    [SerializeField] private bool animateSliders = true;
    [SerializeField] private float sliderAnimationSpeed = 5f;
    [SerializeField] private bool pulseOnLowHealth = true;
    [SerializeField] private float pulseSpeed = 2f;
    
    [Header("Text Display (Optional)")]
    [SerializeField] private Text healthText;
    [SerializeField] private Text staminaText;
    [SerializeField] private bool showValues = true;
    [SerializeField] private bool showPercentage = false;
    
    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button restartButton;
    
    [Header("Player Reference")]
    [SerializeField] private PlayerController2D playerController;
    
    // Referências do sistema de combate
    private ICombatController playerCombat;
    private CombatAttributes playerAttributes;
    
    // Valores para animação suave
    private float targetHealthValue;
    private float targetStaminaValue;
    private bool isLowHealth = false;
    private bool isLowStamina = false;
    
    void Start()
    {
        InitializeAdvancedUI();
        InitializeGameOverUI();
    }
    
    void Update()
    {
        // Auto-find references
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController2D>();
        }
        
        if (playerCombat == null && playerController != null)
        {
            playerCombat = playerController.GetComponent<ICombatController>();
            if (playerCombat != null)
            {
                playerAttributes = playerCombat.Attributes;
                SubscribeToEvents();
                InitializeSliders();
            }
        }
        
        // Update animations and effects
        UpdateSliderAnimations();
        UpdateVisualEffects();
    }
    
    private void InitializeAdvancedUI()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController2D>();
            Debug.Log("AdvancedUIController: Player encontrado automaticamente!");
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
                
                Debug.Log("AdvancedUIController: Sistema de combate conectado com sucesso!");
            }
        }
        
        SetupVisualComponents();
    }
    
    private void SetupVisualComponents()
    {
        // Auto-find fill images if not assigned
        if (healthFill == null && healthSlider != null)
        {
            healthFill = healthSlider.fillRect.GetComponent<Image>();
        }
        
        if (staminaFill == null && staminaSlider != null)
        {
            staminaFill = staminaSlider.fillRect.GetComponent<Image>();
        }
        
        // Set initial colors
        if (healthFill != null)
        {
            healthFill.color = healthColor;
        }
        
        if (staminaFill != null)
        {
            staminaFill.color = staminaColor;
        }
    }
    
    private void SubscribeToEvents()
    {
        if (playerAttributes != null)
        {
            playerAttributes.OnHealthChanged += OnHealthChanged;
            playerAttributes.OnStaminaChanged += OnStaminaChanged;
            playerAttributes.OnDeath += OnPlayerDeath; // Adiciona evento de morte
        }
    }
    
    private void InitializeSliders()
    {
        if (playerAttributes == null) return;
        
        // Força inicialização dos atributos
        playerAttributes.Initialize();
        
        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = playerAttributes.MaxHealth;
            healthSlider.value = playerAttributes.CurrentHealth;
            targetHealthValue = playerAttributes.CurrentHealth;
            
            Debug.Log($"AdvancedUIController: Health Slider inicializado - Value: {playerAttributes.CurrentHealth}/{playerAttributes.MaxHealth}");
        }
        
        if (staminaSlider != null)
        {
            staminaSlider.minValue = 0f;
            staminaSlider.maxValue = playerAttributes.MaxStamina;
            staminaSlider.value = playerAttributes.CurrentStamina;
            targetStaminaValue = playerAttributes.CurrentStamina;
            
            Debug.Log($"AdvancedUIController: Stamina Slider inicializado - Value: {playerAttributes.CurrentStamina}/{playerAttributes.MaxStamina}");
        }
        
        UpdateTextDisplays();
    }
    
    private void OnHealthChanged(float currentHealth, float maxHealth)
    {
        targetHealthValue = currentHealth;
        
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            
            if (!animateSliders)
            {
                healthSlider.value = currentHealth;
            }
        }
        
        // Check for low health threshold
        float healthPercentage = maxHealth > 0 ? currentHealth / maxHealth : 0f;
        isLowHealth = healthPercentage <= lowHealthThreshold;
        
        UpdateTextDisplays();
        
        // Visual feedback for damage taken
        if (currentHealth < healthSlider.value)
        {
            StartCoroutine(FlashHealthBar());
        }
    }
    
    private void OnStaminaChanged(float currentStamina, float maxStamina)
    {
        targetStaminaValue = currentStamina;
        
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            
            if (!animateSliders)
            {
                staminaSlider.value = currentStamina;
            }
        }
        
        // Check for low stamina threshold
        float staminaPercentage = maxStamina > 0 ? currentStamina / maxStamina : 0f;
        isLowStamina = staminaPercentage <= lowStaminaThreshold;
        
        UpdateTextDisplays();
    }
    
    private void UpdateSliderAnimations()
    {
        if (!animateSliders) return;
        
        // Animate health slider
        if (healthSlider != null && Mathf.Abs(healthSlider.value - targetHealthValue) > 0.1f)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, targetHealthValue, 
                                           sliderAnimationSpeed * Time.deltaTime);
        }
        
        // Animate stamina slider
        if (staminaSlider != null && Mathf.Abs(staminaSlider.value - targetStaminaValue) > 0.1f)
        {
            staminaSlider.value = Mathf.Lerp(staminaSlider.value, targetStaminaValue, 
                                           sliderAnimationSpeed * Time.deltaTime);
        }
    }
    
    private void UpdateVisualEffects()
    {
        // Update health bar color based on percentage
        if (healthFill != null)
        {
            Color targetColor = isLowHealth ? lowHealthColor : healthColor;
            
            if (pulseOnLowHealth && isLowHealth)
            {
                float pulse = Mathf.Sin(Time.time * pulseSpeed) * 0.3f + 0.7f;
                targetColor = Color.Lerp(lowHealthColor, healthColor, pulse);
            }
            
            healthFill.color = Color.Lerp(healthFill.color, targetColor, Time.deltaTime * 3f);
        }
        
        // Update stamina bar color
        if (staminaFill != null)
        {
            Color targetColor = isLowStamina ? lowStaminaColor : staminaColor;
            staminaFill.color = Color.Lerp(staminaFill.color, targetColor, Time.deltaTime * 3f);
        }
    }
    
    private void UpdateTextDisplays()
    {
        if (!showValues) return;
        
        if (playerAttributes == null) return;
        
        if (healthText != null)
        {
            if (showPercentage)
            {
                float percentage = (playerAttributes.CurrentHealth / playerAttributes.MaxHealth) * 100f;
                healthText.text = $"{percentage:F0}%";
            }
            else
            {
                healthText.text = $"{playerAttributes.CurrentHealth:F0}/{playerAttributes.MaxHealth:F0}";
            }
        }
        
        if (staminaText != null)
        {
            if (showPercentage)
            {
                float percentage = (playerAttributes.CurrentStamina / playerAttributes.MaxStamina) * 100f;
                staminaText.text = $"{percentage:F0}%";
            }
            else
            {
                staminaText.text = $"{playerAttributes.CurrentStamina:F0}/{playerAttributes.MaxStamina:F0}";
            }
        }
    }
    
    private IEnumerator FlashHealthBar()
    {
        if (healthFill == null) yield break;
        
        Color originalColor = healthFill.color;
        float flashDuration = 0.2f;
        float elapsedTime = 0f;
        
        while (elapsedTime < flashDuration)
        {
            float intensity = Mathf.Sin((elapsedTime / flashDuration) * Mathf.PI);
            healthFill.color = Color.Lerp(originalColor, Color.white, intensity * 0.7f);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        healthFill.color = originalColor;
    }
    
    // Public methods for external control
    public void SetHealthThreshold(float threshold)
    {
        lowHealthThreshold = Mathf.Clamp01(threshold);
    }
    
    public void SetStaminaThreshold(float threshold)
    {
        lowStaminaThreshold = Mathf.Clamp01(threshold);
    }
    
    public void EnablePulseEffect(bool enable)
    {
        pulseOnLowHealth = enable;
    }
    
    public void SetAnimationSpeed(float speed)
    {
        sliderAnimationSpeed = Mathf.Max(0.1f, speed);
    }
    
    // Properties
    public bool IsLowHealth => isLowHealth;
    public bool IsLowStamina => isLowStamina;
    public float HealthPercentage => playerAttributes != null ? 
        playerAttributes.CurrentHealth / playerAttributes.MaxHealth : 0f;
    public float StaminaPercentage => playerAttributes != null ? 
        playerAttributes.CurrentStamina / playerAttributes.MaxStamina : 0f;
    
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
        
        Debug.Log("AdvancedUIController: Game Over UI inicializada!");
    }
    
    /// <summary>
    /// Chamado quando o player morre
    /// </summary>
    private void OnPlayerDeath()
    {
        Debug.Log("AdvancedUIController: Player morreu! Exibindo tela de Game Over...");
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
            
            Debug.Log("AdvancedUIController: Tela de Game Over exibida!");
        }
        else
        {
            Debug.LogWarning("AdvancedUIController: Game Over Panel não está configurado!");
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
            
            Debug.Log("AdvancedUIController: Tela de Game Over escondida!");
        }
    }
    
    /// <summary>
    /// Método público para reiniciar o jogo - Conecte este método ao onClick do botão
    /// </summary>
    public void RestartGame()
    {
        Debug.Log("AdvancedUIController: Reiniciando o jogo...");
        
        // Reseta o estado do ShopManager antes de recarregar a cena
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.OnGameRestart();
            Debug.Log("AdvancedUIController: Estado do ShopManager resetado");
        }
        
        // Reseta também o sistema persistente
        if (ShopPersistentData.Instance != null)
        {
            ShopPersistentData.Instance.ResetShopState();
            Debug.Log("AdvancedUIController: Sistema persistente da loja resetado");
        }
        
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
        
        Debug.Log("AdvancedUIController: Controles do player bloqueados - sem movimento, dash ou ataque!");
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
        
        Debug.Log("AdvancedUIController: Controles do player desbloqueados!");
    }
    
    private void OnDestroy()
    {
        if (playerAttributes != null)
        {
            playerAttributes.OnHealthChanged -= OnHealthChanged;
            playerAttributes.OnStaminaChanged -= OnStaminaChanged;
            playerAttributes.OnDeath -= OnPlayerDeath; // Remove evento de morte
        }
        
        // Remove listener do botão
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(RestartGame);
        }
    }
}
