using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using CombatSystem;
using System.Collections.Generic;

/// <summary>
/// Gerenciador da loja que aparece entre waves
/// </summary>
public class ShopManager : MonoBehaviour
{
    // Singleton para acesso na cena atual (sem DontDestroyOnLoad)
    public static ShopManager Instance { get; private set; }
    
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
    [SerializeField] private bool debugMode = true; // Forçando debug temporariamente
    
    [Header("Events")]
    public UnityEvent OnShopOpened;
    public UnityEvent OnShopClosed;
    public UnityEvent<int> OnOptionSelected;
    
    // Estado interno (não persistente)
    private bool isShopOpen = false;
    private ShopOption[] currentOptions = new ShopOption[3];
    
    private void Awake()
    {
        // Singleton simples para a cena atual (sem DontDestroyOnLoad)
        if (Instance == null)
        {
            Instance = this;
            
            // Carrega opções do cache persistente se disponível
            LoadOptionsFromPersistentData();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        InitializeShop();
        
        // Se houver um WebGLCommunicator, escuta por buffs recebidos
        if (WebGLCommunicator.Instance != null)
        {
            WebGLCommunicator.Instance.OnBuffsReceived += OnWebGLBuffsReceived;
            
            // Adiciona listener para opções da loja também
            WebGLCommunicator.Instance.OnShopOptionsReceived += OnShopOptionsReceived;
        }
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
        // Se a loja já está aberta, não faz nada
        if (isShopOpen)
        {
            if (debugMode)
            {
                Debug.Log("ShopManager: Loja já está aberta");
            }
            return;
        }
        
        // Solicita novas opções aleatórias do WebGL antes de abrir a loja
        RequestNewShopOptions();
        
        // Verifica se temos opções disponíveis (pode ser do cache ou opções configuradas manualmente)
        if (availableOptions == null || availableOptions.Length == 0)
        {
            Debug.LogWarning("ShopManager: Tentou abrir loja sem opções disponíveis");
            return;
        }
        
        // Garante que o painel esteja fechado antes de abrir
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
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
        else
        {
            Debug.LogError("ShopManager: shopPanel é null, a loja não pode ser exibida");
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
    /// Solicita novas opções aleatórias da loja do WebGL
    /// </summary>
    private void RequestNewShopOptions()
    {
        try
        {
            // Envia mensagem para o WebGL solicitando novas opções aleatórias
            Application.ExternalCall("requestNewShopOptions");
            
            if (debugMode)
            {
                Debug.Log("ShopManager: Solicitadas novas opções da loja do WebGL");
            }
        }
        catch (System.Exception e)
        {
            if (debugMode)
            {
                Debug.LogWarning($"ShopManager: Não foi possível solicitar novas opções do WebGL: {e.Message}");
            }
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
        // Limpa as opções atuais primeiro
        for (int i = 0; i < currentOptions.Length; i++)
        {
            currentOptions[i] = null;
        }
        
        if (availableOptions == null || availableOptions.Length == 0)
        {
            if (debugMode)
            {
                Debug.LogWarning("ShopManager: Nenhuma opção disponível para gerar opções aleatórias");
            }
            return;
        }
        
        // Sempre seleciona opções aleatórias, mesmo se tiver 3 ou menos
        // Isso garante que seja 100% aleatório a cada abertura da loja
        var usedIndices = new System.Collections.Generic.HashSet<int>();
        int optionsToGenerate = Mathf.Min(3, availableOptions.Length);
        
        for (int i = 0; i < optionsToGenerate; i++)
        {
            int randomIndex;
            
            // Se temos menos opções que slots, permite repetição após esgotar as únicas
            if (usedIndices.Count == availableOptions.Length && availableOptions.Length < 3)
            {
                // Permite repetição se não temos opções suficientes
                randomIndex = Random.Range(0, availableOptions.Length);
            }
            else
            {
                // Garante que não há repetição enquanto há opções únicas disponíveis
                do
                {
                    randomIndex = Random.Range(0, availableOptions.Length);
                } while (usedIndices.Contains(randomIndex) && usedIndices.Count < availableOptions.Length);
            }
            
            usedIndices.Add(randomIndex);
            currentOptions[i] = availableOptions[randomIndex];
            
            if (debugMode)
            {
                Debug.Log($"ShopManager: Slot {i} - Selecionada opção '{availableOptions[randomIndex].optionName}' (índice {randomIndex})");
            }
        }
        
        if (debugMode)
        {
            Debug.Log($"ShopManager: Geradas {optionsToGenerate} opções aleatórias de {availableOptions.Length} disponíveis");
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
        if (debugMode)
        {
            Debug.Log($"ShopManager: Tentando aplicar opção {option.optionType} com valor {option.value}");
        }
        
        // Primeira tentativa: usar o GameController.Instance se disponível
        var gameController = GameController.Instance;
        CombatAttributes playerAttributes = null;
        
        if (gameController != null && gameController.PlayerAttributes != null)
        {
            playerAttributes = gameController.PlayerAttributes;
            if (debugMode)
            {
                Debug.Log("ShopManager: Usando PlayerAttributes do GameController.Instance");
            }
        }
        else
        {
            // Segunda tentativa: encontrar o player diretamente através do PlayerController2D
            var playerController = FindObjectOfType<PlayerController2D>();
            if (playerController != null)
            {
                var combatController = playerController.GetComponent<ICombatController>();
                if (combatController != null && combatController.Attributes != null)
                {
                    playerAttributes = combatController.Attributes;
                    if (debugMode)
                    {
                        Debug.Log("ShopManager: Usando PlayerAttributes encontrado diretamente via PlayerController2D");
                    }
                }
            }
        }
        
        if (playerAttributes != null)
        {
            
            switch (option.optionType)
            {
                case ShopOptionType.HealthUpgrade:
                    if (debugMode)
                    {
                        Debug.Log($"ShopManager: Aplicando upgrade de vida de {option.value}. Vida antes: {playerAttributes.CurrentHealth}/{playerAttributes.MaxHealth}");
                    }
                    playerAttributes.IncreaseMaxHealth(option.value);
                    playerAttributes.Heal(option.value); // Também cura
                    if (debugMode)
                    {
                        Debug.Log($"ShopManager: Upgrade de vida aplicado! Vida depois: {playerAttributes.CurrentHealth}/{playerAttributes.MaxHealth}");
                    }
                    break;
                    
                case ShopOptionType.StaminaUpgrade:
                    playerAttributes.IncreaseMaxStamina(option.value);
                    playerAttributes.RestoreStamina(option.value); // Também restaura
                    break;
                    
                case ShopOptionType.HealOnly:
                    if (debugMode)
                    {
                        Debug.Log($"ShopManager: Aplicando cura de {option.value} HP. Vida antes: {playerAttributes.CurrentHealth}/{playerAttributes.MaxHealth}");
                    }
                    playerAttributes.Heal(option.value);
                    if (debugMode)
                    {
                        Debug.Log($"ShopManager: Cura aplicada! Vida depois: {playerAttributes.CurrentHealth}/{playerAttributes.MaxHealth}");
                    }
                    break;
                    
                case ShopOptionType.StaminaRestore:
                    playerAttributes.RestoreStamina(option.value);
                    break;
                    
                case ShopOptionType.DamageIncrease:
                    playerAttributes.IncreaseDamage(option.value);
                    break;
            }
            
            // Aplica efeitos especiais se a opção for especial
            if (option.isSpecial)
            {
                ApplySpecialEffects(option, playerAttributes);
            }
            
            if (debugMode)
            {
                Debug.Log($"ShopManager: Aplicado {option.optionType} com valor {option.value}");
                if (option.isSpecial)
                {
                    Debug.Log($"ShopManager: Efeitos especiais aplicados para opção {option.optionName}");
                }
            }
        }
        else
        {
            Debug.LogError("ShopManager: PlayerAttributes não encontrado! Impossível aplicar a opção da loja.");
        }
    }
    
    /// <summary>
    /// Aplica efeitos especiais de uma opção da loja
    /// </summary>
    /// <param name="option">Opção com efeitos especiais</param>
    /// <param name="playerAttributes">Atributos do jogador</param>
    private void ApplySpecialEffects(ShopOption option, CombatAttributes playerAttributes)
    {
        if (option.healthBonus > 0)
        {
            playerAttributes.IncreaseMaxHealth(option.healthBonus);
            playerAttributes.Heal(option.healthBonus);
        }
        
        if (option.staminaBonus > 0)
        {
            playerAttributes.IncreaseMaxStamina(option.staminaBonus);
            playerAttributes.RestoreStamina(option.staminaBonus);
        }
        
        if (option.damageBonus > 0)
        {
            playerAttributes.IncreaseDamage(option.damageBonus);
        }
        
        if (debugMode)
        {
            Debug.Log($"ShopManager: Efeitos especiais aplicados - Vida: +{option.healthBonus}, Stamina: +{option.staminaBonus}, Dano: +{option.damageBonus}");
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
            case ShopOptionType.DamageIncrease:
                return $"+{value} Dano";
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
    
    /// <summary>
    /// Método público para reiniciar completamente o estado do jogo
    /// Deve ser chamado pelo GameController quando o jogador perde e a wave é reiniciada
    /// </summary>
    public void OnGameRestart()
    {
        // Reinicia o estado da loja
        ResetShopState();
        
        // Garante que a loja pode ser aberta novamente
        isShopOpen = false;
        
        // Reseta o estado no sistema persistente
        if (ShopPersistentData.Instance != null)
        {
            ShopPersistentData.Instance.ResetShopState();
        }
        
        if (debugMode)
        {
            Debug.Log("ShopManager: Jogo reiniciado, estado da loja redefinido");
        }
    }
    
    /// <summary>
    /// Carrega as opções da loja do sistema de dados persistentes
    /// </summary>
    private void LoadOptionsFromPersistentData()
    {
        // Cria o sistema persistente se não existir
        if (ShopPersistentData.Instance == null)
        {
            CreatePersistentDataObject();
        }
        
        // Carrega opções do cache se disponível
        if (ShopPersistentData.Instance != null && ShopPersistentData.Instance.HasCachedOptions())
        {
            ShopOption[] cachedOptions = ShopPersistentData.Instance.LoadShopOptions();
            if (cachedOptions != null)
            {
                availableOptions = cachedOptions;
                
                if (debugMode)
                {
                    Debug.Log($"ShopManager: {cachedOptions.Length} opções carregadas do sistema persistente");
                }
            }
        }
    }
    
    /// <summary>
    /// Cria o objeto de dados persistentes se necessário
    /// </summary>
    private void CreatePersistentDataObject()
    {
        GameObject persistentObject = new GameObject("ShopPersistentData");
        persistentObject.AddComponent<ShopPersistentData>();
        
        if (debugMode)
        {
            Debug.Log("ShopManager: Sistema de dados persistentes criado");
        }
    }
    
    private void OnDestroy()
    {
        // Remove o listener quando o objeto é destruído
        if (WebGLCommunicator.Instance != null)
        {
            WebGLCommunicator.Instance.OnBuffsReceived -= OnWebGLBuffsReceived;
            WebGLCommunicator.Instance.OnShopOptionsReceived -= OnShopOptionsReceived;
        }
        
        // Limpa a instância se esta for a instância principal
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
    /// <summary>
    /// Reinicia o estado da loja para permitir que ela seja aberta novamente
    /// Deve ser chamado quando o jogador perde e a wave é reiniciada
    /// </summary>
    public void ResetShopState()
    {
        // Garante que a loja esteja fechada
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
        
        // Reinicia o estado interno
        isShopOpen = false;
        
        // Restaura o tempo normal do jogo (importante para o caso da loja estar aberta durante a derrota)
        Time.timeScale = 1f;
        
        if (debugMode)
        {
            Debug.Log("ShopManager: Estado da loja reiniciado");
        }
    }
    
    /// <summary>
    /// Limpa o cache de opções da loja (útil para testes ou redefinir o jogo)
    /// </summary>
    public void ClearShopOptionsCache()
    {
        // Limpa o cache do sistema persistente
        if (ShopPersistentData.Instance != null)
        {
            ShopPersistentData.Instance.ClearCache();
        }
        
        // Limpa as opções locais também
        availableOptions = null;
        
        if (debugMode)
        {
            Debug.Log("ShopManager: Cache de opções da loja limpo");
        }
    }
    
    /// <summary>
    /// Método público para atualizar as opções da loja via código externo
    /// </summary>
    /// <param name="newOptions">Novas opções para a loja</param>
    public void UpdateShopOptions(ShopOption[] newOptions)
    {
        if (newOptions != null && newOptions.Length > 0)
        {
            availableOptions = newOptions;
            
            // Salva as opções no sistema persistente
            if (ShopPersistentData.Instance != null)
            {
                ShopPersistentData.Instance.SaveShopOptions(newOptions);
            }
            else if (debugMode)
            {
                Debug.LogWarning("ShopManager: Sistema persistente não encontrado para salvar opções");
            }
            
            if (debugMode)
            {
                Debug.Log($"ShopManager: Opções da loja atualizadas. {newOptions.Length} opções disponíveis.");
            }
        }
    }
    
    /// <summary>
    /// Callback para quando buffs são recebidos do WebGL
    /// </summary>
    private void OnWebGLBuffsReceived(string buffData)
    {
        if (debugMode)
        {
            Debug.Log($"ShopManager: Buffs recebidos do WebGL: {buffData}");
        }
        
        // Aqui você pode implementar a lógica para processar os buffs recebidos
        // Por exemplo, aplicar buffs diretamente ao jogador ou atualizar a UI
        // TODO: Implementar processamento de buffs conforme necessário
    }
    
    /// <summary>
    /// Callback para quando opções da loja são recebidas do WebGL
    /// </summary>
    private void OnShopOptionsReceived(string optionsData)
    {
        if (debugMode)
        {
            Debug.Log($"ShopManager: Opções da loja recebidas do WebGL: {optionsData}");
        }
        
        try
        {
            // Parse do JSON recebido
            var shopData = JsonUtility.FromJson<WebGLShopData>(optionsData);
            
            if (shopData != null && shopData.options != null && shopData.options.Length > 0)
            {
                // Converte as opções WebGL para ShopOption[]
                availableOptions = new ShopOption[shopData.options.Length];
                
                for (int i = 0; i < shopData.options.Length; i++)
                {
                    availableOptions[i] = ConvertWebGLOptionToShopOption(shopData.options[i]);
                }
                
                if (debugMode)
                {
                    Debug.Log($"ShopManager: {availableOptions.Length} opções atualizadas a partir do WebGL");
                }
                
                // Se a loja estiver aberta, regenera as opções para usar as novas
                if (isShopOpen)
                {
                    GenerateRandomOptions();
                    UpdateShopUI();
                    UpdateShopTexts();
                }
            }
            else
            {
                Debug.LogWarning("ShopManager: Dados da loja recebidos do WebGL são inválidos ou vazios");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"ShopManager: Erro ao processar opções da loja do WebGL: {e.Message}");
        }
    }
    
    /// <summary>
    /// Converte uma opção WebGL para ShopOption
    /// </summary>
    private ShopOption ConvertWebGLOptionToShopOption(WebGLShopOption webglOption)
    {
        var shopOption = new ShopOption();
        
        shopOption.optionName = webglOption.optionName ?? "Opção";
        shopOption.description = webglOption.description ?? "Descrição";
        shopOption.stellarTransactionId = webglOption.stellarTransactionId ?? "";
        shopOption.title = webglOption.title ?? "";
        shopOption.buff = webglOption.buff ?? "";
        shopOption.value = webglOption.value;
        shopOption.rarity = webglOption.rarity ?? "common";
        shopOption.cost = webglOption.cost;
        shopOption.isSpecial = webglOption.isSpecial;
        
        // Converte optionType string para enum
        if (System.Enum.TryParse<ShopOptionType>(webglOption.optionType, out ShopOptionType optionType))
        {
            shopOption.optionType = optionType;
        }
        else
        {
            shopOption.optionType = ShopOptionType.HealthUpgrade; // Default
        }
        
        // Se tem efeitos especiais, aplica
        if (webglOption.specialEffects != null)
        {
            shopOption.healthBonus = webglOption.specialEffects.healthBonus;
            shopOption.staminaBonus = webglOption.specialEffects.staminaBonus;
            shopOption.damageBonus = webglOption.specialEffects.damageBonus;
        }
        
        return shopOption;
    }
    
    // Propriedades públicas
    /// <summary>
    /// Indica se a loja está atualmente aberta
    /// </summary>
    public bool IsShopOpen => isShopOpen;
}

/// <summary>
/// Classe para deserializar dados da loja recebidos do WebGL
/// </summary>
[System.Serializable]
public class WebGLShopData
{
    public WebGLShopOption[] options;
    public int playerLevel;
    public WebGLShopMetadata shopMetadata;
}

/// <summary>
/// Classe para deserializar opções da loja recebidas do WebGL
/// </summary>
[System.Serializable]
public class WebGLShopOption
{
    public string optionName;
    public string description;
    public string stellarTransactionId;
    public string title;
    public string buff;
    public string icon;
    public string optionType;
    public float value;
    public string rarity;
    public float cost;
    public bool isSpecial;
    public WebGLSpecialEffects specialEffects;
}

/// <summary>
/// Classe para efeitos especiais de opções WebGL
/// </summary>
[System.Serializable]
public class WebGLSpecialEffects
{
    public float healthBonus;
    public float staminaBonus;
    public float damageBonus;
}

/// <summary>
/// Classe para metadata da loja WebGL
/// </summary>
[System.Serializable]
public class WebGLShopMetadata
{
    public string version;
    public string timestamp;
    public string sessionId;
    public int totalAvailableOptions;
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
    
    [Header("Extended Properties")]
    public string rarity = "common";
    public float cost = 0f;
    public bool isSpecial = false;
    
    [Header("Special Effects (if isSpecial = true)")]
    public float healthBonus = 0f;
    public float staminaBonus = 0f;
    public float damageBonus = 0f;
}

/// <summary>
/// Tipos de opções disponíveis na loja
/// </summary>
public enum ShopOptionType
{
    HealthUpgrade,    // Aumenta vida máxima e cura
    StaminaUpgrade,   // Aumenta stamina máxima e restaura
    HealOnly,         // Apenas cura sem aumentar máximo
    StaminaRestore,   // Apenas restaura stamina sem aumentar máximo
    DamageIncrease    // Aumenta o dano do player
}
