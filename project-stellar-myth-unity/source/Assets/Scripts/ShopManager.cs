using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Gerenciador da loja que aparece entre waves
/// </summary>
public class ShopManager : MonoBehaviour
{
    [Header("Shop UI References")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Button[] optionButtons = new Button[3];
    [SerializeField] private Text[] optionTexts = new Text[3];
    [SerializeField] private Image[] optionImages = new Image[3];
    
    [Header("Shop Settings")]
    [SerializeField] private ShopOption[] availableOptions;
    [SerializeField] private bool debugMode = false;
    
    [Header("Events")]
    public UnityEvent OnShopOpened;
    public UnityEvent OnShopClosed;
    public UnityEvent<int> OnOptionSelected;
    
    // Estado interno
    private bool isShopOpen = false;
    private ShopOption[] currentOptions = new ShopOption[3];
    
    private void Start()
    {
        InitializeShop();
    }
    
    private void InitializeShop()
    {
        // Esconde o painel da loja inicialmente
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
        
        // Configura os botões
        for (int i = 0; i < optionButtons.Length; i++)
        {
            int optionIndex = i; // Captura para closure
            if (optionButtons[i] != null)
            {
                optionButtons[i].onClick.AddListener(() => SelectOption(optionIndex));
            }
        }
        
        if (debugMode)
        {
            Debug.Log("ShopManager: Inicializado com sucesso!");
        }
    }
    
    /// <summary>
    /// Abre a loja com 3 opções aleatórias
    /// </summary>
    public void OpenShop()
    {
        if (isShopOpen || availableOptions == null || availableOptions.Length == 0)
        {
            return;
        }
        
        // Pausa o jogo
        Time.timeScale = 0f;
        isShopOpen = true;
        
        // Gera 3 opções aleatórias
        GenerateRandomOptions();
        
        // Atualiza a UI
        UpdateShopUI();
        
        // Mostra o painel
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
        }
        
        OnShopOpened?.Invoke();
        
        if (debugMode)
        {
            Debug.Log("ShopManager: Loja aberta!");
        }
    }
    
    /// <summary>
    /// Método público para ser chamado pelos botões da UI
    /// </summary>
    public void SelectOption(int optionIndex)
    {
        if (!isShopOpen || optionIndex < 0 || optionIndex >= currentOptions.Length)
        {
            return;
        }
        
        var selectedOption = currentOptions[optionIndex];
        if (selectedOption != null)
        {
            // Aplica o efeito da opção escolhida
            ApplyShopOption(selectedOption);
            
            OnOptionSelected?.Invoke(optionIndex);
            
            if (debugMode)
            {
                Debug.Log($"ShopManager: Opção selecionada: {selectedOption.optionName}");
            }
        }
        
        CloseShop();
    }
    
    /// <summary>
    /// Fecha a loja e retorna o jogo ao normal
    /// </summary>
    public void CloseShop()
    {
        if (!isShopOpen)
        {
            return;
        }
        
        // Esconde o painel
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
        
        isShopOpen = false;
        
        // Retorna o jogo ao tempo normal
        Time.timeScale = 1f;
        
        OnShopClosed?.Invoke();
        
        if (debugMode)
        {
            Debug.Log("ShopManager: Loja fechada!");
        }
    }
    
    private void GenerateRandomOptions()
    {
        if (availableOptions.Length <= 3)
        {
            // Se temos 3 ou menos opções, usa todas
            for (int i = 0; i < availableOptions.Length && i < 3; i++)
            {
                currentOptions[i] = availableOptions[i];
            }
        }
        else
        {
            // Seleciona 3 opções aleatórias diferentes
            var usedIndices = new System.Collections.Generic.HashSet<int>();
            for (int i = 0; i < 3; i++)
            {
                int randomIndex;
                do
                {
                    randomIndex = Random.Range(0, availableOptions.Length);
                } while (usedIndices.Contains(randomIndex));
                
                usedIndices.Add(randomIndex);
                currentOptions[i] = availableOptions[randomIndex];
            }
        }
    }
    
    private void UpdateShopUI()
    {
        for (int i = 0; i < 3; i++)
        {
            if (currentOptions[i] != null)
            {
                // Atualiza texto
                if (optionTexts[i] != null)
                {
                    optionTexts[i].text = $"{currentOptions[i].optionName}\n{currentOptions[i].description}";
                }
                
                // Atualiza imagem
                if (optionImages[i] != null && currentOptions[i].icon != null)
                {
                    optionImages[i].sprite = currentOptions[i].icon;
                }
                
                // Habilita o botão
                if (optionButtons[i] != null)
                {
                    optionButtons[i].interactable = true;
                }
            }
            else
            {
                // Desabilita opções vazias
                if (optionButtons[i] != null)
                {
                    optionButtons[i].interactable = false;
                }
                
                if (optionTexts[i] != null)
                {
                    optionTexts[i].text = "Opção não disponível";
                }
            }
        }
    }
    
    private void ApplyShopOption(ShopOption option)
    {
        // Encontra o player
        var gameController = GameController.Instance;
        if (gameController != null && gameController.PlayerAttributes != null)
        {
            var playerAttributes = gameController.PlayerAttributes;
            
            switch (option.optionType)
            {
                case ShopOptionType.HealthUpgrade:
                    playerAttributes.IncreaseMaxHealth(option.value);
                    playerAttributes.Heal(option.value); // Também cura
                    break;
                    
                case ShopOptionType.StaminaUpgrade:
                    playerAttributes.IncreaseMaxStamina(option.value);
                    playerAttributes.RestoreStamina(option.value); // Também restaura
                    break;
                    
                case ShopOptionType.HealOnly:
                    playerAttributes.Heal(option.value);
                    break;
                    
                case ShopOptionType.StaminaRestore:
                    playerAttributes.RestoreStamina(option.value);
                    break;
            }
            
            if (debugMode)
            {
                Debug.Log($"ShopManager: Aplicado {option.optionType} com valor {option.value}");
            }
        }
        else
        {
            Debug.LogWarning("ShopManager: Não foi possível encontrar o player para aplicar a opção!");
        }
    }
    
    // Propriedades públicas
    public bool IsShopOpen => isShopOpen;
}

/// <summary>
/// Estrutura de dados para uma opção da loja
/// </summary>
[System.Serializable]
public class ShopOption
{
    [Header("Option Info")]
    public string optionName = "Opção";
    [TextArea(2, 4)]
    public string description = "Descrição da opção";
    public Sprite icon;
    
    [Header("Effect")]
    public ShopOptionType optionType = ShopOptionType.HealthUpgrade;
    public float value = 10f;
}

/// <summary>
/// Tipos de opções disponíveis na loja
/// </summary>
public enum ShopOptionType
{
    HealthUpgrade,    // Aumenta vida máxima e cura
    StaminaUpgrade,   // Aumenta stamina máxima e restaura
    HealOnly,         // Apenas cura sem aumentar máximo
    StaminaRestore    // Apenas restaura stamina sem aumentar máximo
}
