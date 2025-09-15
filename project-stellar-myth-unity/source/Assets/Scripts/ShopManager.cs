using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// Gerenciador da loja que aparece entre waves
/// </summary>
public class ShopManager : MonoBehaviour
{
    [Header("Shop UI References")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Button[] optionButtons = new Button[3];
    [SerializeField] private TextMeshProUGUI[] optionTexts = new TextMeshProUGUI[3];
    [SerializeField] private Image[] optionImages = new Image[3];
    
    [Header("Text References - 3 Cards")]
    [SerializeField] private TextMeshProUGUI[] stellarTransactionIdTexts = new TextMeshProUGUI[3];
    [SerializeField] private TextMeshProUGUI[] titleTexts = new TextMeshProUGUI[3];
    [SerializeField] private TextMeshProUGUI[] descriptionTexts = new TextMeshProUGUI[3];
    [SerializeField] private TextMeshProUGUI[] buffTexts = new TextMeshProUGUI[3];
    
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
        
        // Atualiza textos específicos da loja
        UpdateShopTexts();
        
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
                var option = currentOptions[i];
                
                // Atualiza texto principal da opção
                if (optionTexts[i] != null)
                {
                    // Usa title se disponível, senão usa optionName
                    string displayTitle = !string.IsNullOrEmpty(option.title) ? option.title : option.optionName;
                    string displayDescription = option.description;
                    string buffInfo = !string.IsNullOrEmpty(option.buff) ? $"\n<color=yellow>{option.buff}</color>" : "";
                    
                    optionTexts[i].text = $"<b>{displayTitle}</b>\n{displayDescription}{buffInfo}";
                }
                
                // Atualiza imagem
                if (optionImages[i] != null && option.icon != null)
                {
                    optionImages[i].sprite = option.icon;
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
    
    private void UpdateShopTexts()
    {
        for (int i = 0; i < 3; i++)
        {
            var option = currentOptions[i];
            
            if (option != null)
            {
                // Atualiza stellar transaction ID para cada card
                if (stellarTransactionIdTexts[i] != null)
                {
                    string transactionId = !string.IsNullOrEmpty(option.stellarTransactionId) 
                        ? option.stellarTransactionId 
                        : GenerateTransactionId();
                    stellarTransactionIdTexts[i].text = $"ID: {transactionId}";
                }
                
                // Atualiza título para cada card
                if (titleTexts[i] != null)
                {
                    string cardTitle = !string.IsNullOrEmpty(option.title) 
                        ? option.title 
                        : option.optionName;
                    titleTexts[i].text = cardTitle;
                }
                
                // Atualiza descrição para cada card
                if (descriptionTexts[i] != null)
                {
                    descriptionTexts[i].text = option.description;
                }
                
                // Atualiza buff info para cada card
                if (buffTexts[i] != null)
                {
                    string buffInfo = !string.IsNullOrEmpty(option.buff) 
                        ? option.buff 
                        : GetDefaultBuffInfo(option.optionType, option.value);
                    buffTexts[i].text = buffInfo;
                }
            }
            else
            {
                // Limpa textos para cards vazios
                if (stellarTransactionIdTexts[i] != null)
                    stellarTransactionIdTexts[i].text = "";
                
                if (titleTexts[i] != null)
                    titleTexts[i].text = "";
                
                if (descriptionTexts[i] != null)
                    descriptionTexts[i].text = "";
                
                if (buffTexts[i] != null)
                    buffTexts[i].text = "";
            }
        }
    }
    
    private string GenerateTransactionId()
    {
        // Gera um ID único baseado no tempo
        int timestamp = (int)(Time.unscaledTime * 1000) % 100000;
        int randomSuffix = Random.Range(100, 999);
        return $"ST-{timestamp:D5}-{randomSuffix}";
    }
    
    private string GetCurrentBuffInfo()
    {
        var gameController = GameController.Instance;
        if (gameController?.PlayerAttributes != null)
        {
            var playerAttributes = gameController.PlayerAttributes;
            float healthPercent = (playerAttributes.CurrentHealth / playerAttributes.MaxHealth) * 100f;
            float staminaPercent = (playerAttributes.CurrentStamina / playerAttributes.MaxStamina) * 100f;
            
            return $"Vida: {healthPercent:F0}% | Stamina: {staminaPercent:F0}%";
        }
        return "Status não disponível";
    }
    
    private string GetDefaultBuffInfo(ShopOptionType optionType, float value)
    {
        switch (optionType)
        {
            case ShopOptionType.HealthUpgrade:
                return $"+{value} Vida Máxima";
            case ShopOptionType.StaminaUpgrade:
                return $"+{value} Stamina Máxima";
            case ShopOptionType.HealOnly:
                return $"Cura {value} HP";
            case ShopOptionType.StaminaRestore:
                return $"Restaura {value} Stamina";
            default:
                return "Efeito Desconhecido";
        }
    }
    
    /// <summary>
    /// Atualiza o stellar transaction ID para um card específico (método público para uso externo)
    /// </summary>
    public void UpdateStellarTransactionId(int cardIndex, string customId = null)
    {
        if (cardIndex >= 0 && cardIndex < stellarTransactionIdTexts.Length && stellarTransactionIdTexts[cardIndex] != null)
        {
            stellarTransactionIdTexts[cardIndex].text = customId ?? $"ID: {GenerateTransactionId()}";
        }
    }
    
    /// <summary>
    /// Atualiza o título para um card específico (método público para uso externo)
    /// </summary>
    public void UpdateTitle(int cardIndex, string newTitle)
    {
        if (cardIndex >= 0 && cardIndex < titleTexts.Length && titleTexts[cardIndex] != null)
        {
            titleTexts[cardIndex].text = newTitle;
        }
    }
    
    /// <summary>
    /// Atualiza a descrição para um card específico (método público para uso externo)
    /// </summary>
    public void UpdateDescription(int cardIndex, string newDescription)
    {
        if (cardIndex >= 0 && cardIndex < descriptionTexts.Length && descriptionTexts[cardIndex] != null)
        {
            descriptionTexts[cardIndex].text = newDescription;
        }
    }
    
    /// <summary>
    /// Atualiza o texto de buff para um card específico (método público para uso externo)
    /// </summary>
    public void UpdateBuffText(int cardIndex, string newBuffText)
    {
        if (cardIndex >= 0 && cardIndex < buffTexts.Length && buffTexts[cardIndex] != null)
        {
            buffTexts[cardIndex].text = newBuffText;
        }
    }
    
    /// <summary>
    /// Atualiza todos os textos de todos os cards
    /// </summary>
    public void UpdateAllCardTexts()
    {
        UpdateShopTexts();
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
    public string stellarTransactionId = "";
    public string title = "";
    [TextArea(2, 4)]
    public string buff = ""; // String descrevendo o buff
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
