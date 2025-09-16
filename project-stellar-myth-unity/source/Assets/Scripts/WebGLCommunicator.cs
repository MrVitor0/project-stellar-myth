using UnityEngine;
using System.Runtime.InteropServices;

/// <summary>
/// Gerenciador de comunicação entre Unity e JavaScript para WebGL
/// Também funciona como GameManager para receber comandos do JavaScript
/// </summary>
public class WebGLCommunicator : MonoBehaviour
{
    public static WebGLCommunicator Instance { get; private set; }
    
    [Header("Debug")]
    public bool debugMode = true;
    
    [Header("Game State")]
    public bool gameStarted = false;
    
    // Eventos para notificar quando os dados são recebidos
    public System.Action<string> OnBuffsReceived;
    public System.Action<string> OnParameterReceived;
    public System.Action OnGameStarted;
    public System.Action<object> OnPlayerDataReceived;
    public System.Action<System.Collections.Generic.List<object>> OnBlessingsReceived;
    public System.Action<string> OnShopOptionsReceived;
    
    // DLL Imports para comunicação com JavaScript
    #if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void RequestBuffsFromJS();
    
    [DllImport("__Internal")]
    private static extern string GetBuffsFromJS();
    #endif
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        // Solicita os buffs do JavaScript quando o jogo inicializa
        RequestBuffsFromBrowser();
    }
    
    /// <summary>
    /// Solicita os buffs do navegador via JavaScript
    /// </summary>
    public void RequestBuffsFromBrowser()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        if (debugMode)
        {
            Debug.Log("WebGLCommunicator: Solicitando buffs do navegador...");
        }
        
        RequestBuffsFromJS();
        #else
        if (debugMode)
        {
            Debug.Log("WebGLCommunicator: Não está rodando em WebGL, simulando dados...");
            // Simula dados para teste no editor
            string simulatedData = @"{
                ""buffs"": [
                    {
                        ""id"": ""health_boost"",
                        ""name"": ""Poção de Vida"",
                        ""description"": ""Aumenta a vida máxima"",
                        ""type"": ""HealthUpgrade"",
                        ""value"": 25.0,
                        ""icon"": ""health_icon""
                    },
                    {
                        ""id"": ""stamina_boost"",
                        ""name"": ""Elixir de Energia"",
                        ""description"": ""Aumenta a stamina máxima"",
                        ""type"": ""StaminaUpgrade"",
                        ""value"": 20.0,
                        ""icon"": ""stamina_icon""
                    }
                ]
            }";
            OnBuffsReceivedFromJS(simulatedData);
        }
        #endif
    }
    
    /// <summary>
    /// Método chamado pelo JavaScript quando os buffs são recebidos
    /// Este método deve ser chamado pelo JS: SendMessage('WebGLCommunicator', 'OnBuffsReceivedFromJS', jsonString)
    /// </summary>
    /// <param name="jsonData">String JSON contendo os dados dos buffs</param>
    public void OnBuffsReceivedFromJS(string jsonData)
    {
        if (debugMode)
        {
            Debug.Log($"WebGLCommunicator: Buffs recebidos do JavaScript:");
            Debug.Log($"JSON Data: {jsonData}");
        }
        
        try
        {
            // Processa os dados JSON aqui
            ProcessBuffsData(jsonData);
            
            // Notifica outros scripts que os buffs foram recebidos
            OnBuffsReceived?.Invoke(jsonData);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"WebGLCommunicator: Erro ao processar dados JSON: {e.Message}");
        }
    }
    
    /// <summary>
    /// Processa os dados de buffs recebidos
    /// </summary>
    /// <param name="jsonData">String JSON com os dados</param>
    private void ProcessBuffsData(string jsonData)
    {
        if (string.IsNullOrEmpty(jsonData))
        {
            Debug.LogWarning("WebGLCommunicator: Dados JSON vazios ou nulos recebidos");
            return;
        }
        
        // Por enquanto apenas faz log dos dados
        // Futuramente aqui podemos parsear o JSON e atualizar a loja
        if (debugMode)
        {
            Debug.Log($"WebGLCommunicator: Processando dados de buffs...");
            Debug.Log($"Dados brutos: {jsonData}");
            
            // Tenta extrair informações básicas do JSON para debug
            try
            {
                // Conta quantos buffs foram recebidos (método simples)
                int buffCount = jsonData.Split(new string[] { "\"id\":" }, System.StringSplitOptions.None).Length - 1;
                Debug.Log($"WebGLCommunicator: Aproximadamente {buffCount} buffs detectados no JSON");
            }
            catch
            {
                Debug.Log("WebGLCommunicator: Não foi possível contar os buffs automaticamente");
            }
        }
    }
    
    /// <summary>
    /// Método para enviar dados de volta para o JavaScript (opcional)
    /// </summary>
    /// <param name="message">Mensagem para enviar ao JS</param>
    public void SendMessageToJS(string message)
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        // Aqui podemos implementar envio de dados para o JS se necessário
        if (debugMode)
        {
            Debug.Log($"WebGLCommunicator: Enviando mensagem para JS: {message}");
        }
        #endif
    }
    
    /// <summary>
    /// Recebe um parâmetro personalizado do JavaScript
    /// Este método pode ser chamado do JS usando: 
    /// unityInstance.SendMessage('WebGLCommunicator', 'ReceiveParameter', 'valor_do_parametro');
    /// </summary>
    /// <param name="parameter">Parâmetro enviado do JavaScript</param>
    public void ReceiveParameter(string parameter)
    {
        if (debugMode)
        {
            Debug.Log($"WebGLCommunicator: Parâmetro recebido do JavaScript: {parameter}");
        }
        
        // Processa o parâmetro recebido
        ProcessReceivedParameter(parameter);
        
        // Notifica outros sistemas que um parâmetro foi recebido
        OnParameterReceived?.Invoke(parameter);
    }
    
    /// <summary>
    /// Inicia o jogo - Chamado pelo JavaScript
    /// </summary>
    public void StartGame()
    {
        if (debugMode)
        {
            Debug.Log("WebGLCommunicator: Comando StartGame recebido do JavaScript");
        }
        
        gameStarted = true;
        
        // Aqui você pode adicionar lógica específica para iniciar o jogo
        // Por exemplo: ativar menus, iniciar música, etc.
        
        // Notifica outros sistemas que o jogo foi iniciado
        OnGameStarted?.Invoke();
        
        if (debugMode)
        {
            Debug.Log("WebGLCommunicator: Jogo iniciado com sucesso");
        }
    }
    
    /// <summary>
    /// Recebe dados do player do JavaScript
    /// </summary>
    /// <param name="playerDataJson">JSON com dados do player</param>
    public void SetPlayerData(string playerDataJson)
    {
        if (debugMode)
        {
            Debug.Log($"WebGLCommunicator: Dados do player recebidos: {playerDataJson}");
        }
        
        try
        {
            // Aqui você pode deserializar o JSON se necessário
            // Por enquanto, apenas faz debug
            OnPlayerDataReceived?.Invoke(playerDataJson);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"WebGLCommunicator: Erro ao processar dados do player: {e.Message}");
        }
    }
    
    /// <summary>
    /// Recebe atualizações de blessings do JavaScript
    /// </summary>
    /// <param name="blessingsJson">JSON com dados das blessings</param>
    public void UpdateBlessings(string blessingsJson)
    {
        if (debugMode)
        {
            Debug.Log($"WebGLCommunicator: Blessings atualizadas: {blessingsJson}");
        }
        
        try
        {
            // Aqui você pode deserializar o JSON e processar as blessings
            // Por enquanto, apenas faz debug
            Debug.Log($"WebGLCommunicator: Processando {blessingsJson.Length} caracteres de dados de blessings");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"WebGLCommunicator: Erro ao processar blessings: {e.Message}");
        }
    }
    
    /// <summary>
    /// Conecta uma carteira - Chamado pelo JavaScript
    /// </summary>
    /// <param name="walletDataJson">JSON com dados da carteira</param>
    public void ConnectWallet(string walletDataJson)
    {
        if (debugMode)
        {
            Debug.Log($"WebGLCommunicator: Dados da carteira recebidos: {walletDataJson}");
        }
        
        try
        {
            // Processa dados da carteira aqui
            Debug.Log("WebGLCommunicator: Carteira conectada com sucesso");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"WebGLCommunicator: Erro ao conectar carteira: {e.Message}");
        }
    }
    
    /// <summary>
    /// Recebe opções da loja do JavaScript
    /// Este método deve ser chamado pelo JS: SendMessage('WebGLCommunicator', 'OnShopOptionsReceivedFromJS', jsonString)
    /// </summary>
    /// <param name="jsonData">String JSON contendo os dados das opções da loja</param>
    public void OnShopOptionsReceivedFromJS(string jsonData)
    {
        if (debugMode)
        {
            Debug.Log($"WebGLCommunicator: Opções da loja recebidas do JavaScript:");
            Debug.Log($"JSON Data: {jsonData}");
        }
        
        try
        {
            // Processa os dados JSON das opções da loja
            ProcessShopOptionsData(jsonData);
            
            // Notifica outros scripts que as opções da loja foram recebidas
            OnShopOptionsReceived?.Invoke(jsonData);
            
            // Envia dados para o ShopManager se disponível
            SendShopOptionsToShopManager(jsonData);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"WebGLCommunicator: Erro ao processar dados das opções da loja: {e.Message}");
        }
    }
    
    /// <summary>
    /// Processa os dados das opções da loja recebidos
    /// </summary>
    /// <param name="jsonData">String JSON com os dados das opções da loja</param>
    private void ProcessShopOptionsData(string jsonData)
    {
        if (string.IsNullOrEmpty(jsonData))
        {
            Debug.LogWarning("WebGLCommunicator: Dados JSON das opções da loja vazios ou nulos");
            return;
        }
        
        if (debugMode)
        {
            Debug.Log($"WebGLCommunicator: Processando dados das opções da loja...");
            Debug.Log($"Dados brutos: {jsonData}");
            
            // Tenta extrair informações básicas do JSON para debug
            try
            {
                // Conta quantas opções foram recebidas
                int optionsCount = jsonData.Split(new string[] { "\"optionName\":" }, System.StringSplitOptions.None).Length - 1;
                Debug.Log($"WebGLCommunicator: Aproximadamente {optionsCount} opções da loja detectadas no JSON");
            }
            catch
            {
                Debug.Log("WebGLCommunicator: Não foi possível contar as opções automaticamente");
            }
        }
    }
    
    /// <summary>
    /// Envia os dados das opções da loja para o ShopManager
    /// </summary>
    /// <param name="jsonData">Dados JSON das opções da loja</param>
    private void SendShopOptionsToShopManager(string jsonData)
    {
        try
        {
            // Converte o JSON para array de ShopOption
            ShopOption[] shopOptions = ParseJsonToShopOptions(jsonData);
            
            if (shopOptions == null || shopOptions.Length == 0)
            {
                Debug.LogWarning("WebGLCommunicator: Nenhuma opção válida encontrada para enviar ao ShopManager");
                return;
            }
            
            // Verifica se existe uma instância do ShopManager (via singleton)
            ShopManager shopManager = ShopManager.Instance;
            
            if (shopManager != null)
            {
                if (debugMode)
                {
                    Debug.Log("WebGLCommunicator: Enviando opções da loja para o ShopManager via singleton");
                }
                
                shopManager.UpdateShopOptions(shopOptions);
                
                if (debugMode)
                {
                    Debug.Log($"WebGLCommunicator: {shopOptions.Length} opções enviadas para o ShopManager");
                }
            }
            else
            {
                // Procura pelo ShopManager na cena como fallback
                shopManager = FindObjectOfType<ShopManager>();
                
                if (shopManager != null)
                {
                    if (debugMode)
                    {
                        Debug.Log("WebGLCommunicator: Enviando opções da loja para o ShopManager encontrado na cena");
                    }
                    
                    shopManager.UpdateShopOptions(shopOptions);
                    
                    if (debugMode)
                    {
                        Debug.Log($"WebGLCommunicator: {shopOptions.Length} opções enviadas para o ShopManager");
                    }
                }
                else
                {
                    if (debugMode)
                    {
                        Debug.LogWarning("WebGLCommunicator: ShopManager não encontrado na cena");
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"WebGLCommunicator: Erro ao enviar opções para o ShopManager: {e.Message}");
        }
    }
    
    /// <summary>
    /// Converte JSON em array de ShopOption
    /// </summary>
    /// <param name="jsonData">Dados JSON</param>
    /// <returns>Array de ShopOption</returns>
    private ShopOption[] ParseJsonToShopOptions(string jsonData)
    {
        try
        {
            // Parse do JSON usando a estrutura esperada
            ShopOptionsWrapper wrapper = JsonUtility.FromJson<ShopOptionsWrapper>(jsonData);
            
            if (wrapper != null && wrapper.options != null)
            {
                if (debugMode)
                {
                    Debug.Log($"WebGLCommunicator: Parsed {wrapper.options.Length} shop options successfully");
                }
                
                return wrapper.options;
            }
            
            // Fallback: tenta parse direto como array
            ShopOptionsArray directArray = JsonUtility.FromJson<ShopOptionsArray>("{\"options\":" + jsonData + "}");
            if (directArray != null && directArray.options != null)
            {
                if (debugMode)
                {
                    Debug.Log($"WebGLCommunicator: Parsed {directArray.options.Length} shop options via fallback method");
                }
                
                return directArray.options;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"WebGLCommunicator: Erro ao fazer parse do JSON: {e.Message}");
        }
        
        return null;
    }
    
    /// <summary>
    /// Wrapper class para deserializar JSON das opções da loja
    /// </summary>
    [System.Serializable]
    public class ShopOptionsWrapper
    {
        public ShopOption[] options;
        public int playerLevel;
        public PlayerStatsWrapper playerStats;
        public ShopMetadata shopMetadata;
    }

    /// <summary>
    /// Array de opções da loja para fallback parsing
    /// </summary>
    [System.Serializable]
    public class ShopOptionsArray
    {
        public ShopOption[] options;
    }

    /// <summary>
    /// Estatísticas do jogador para wrapper
    /// </summary>
    [System.Serializable]
    public class PlayerStatsWrapper
    {
        public string players;
        public string volume;
        public string nfts;
    }

    /// <summary>
    /// Metadados da loja
    /// </summary>
    [System.Serializable]
    public class ShopMetadata
    {
        public string version;
        public string timestamp;
        public string sessionId;
        public int totalAvailableOptions;
    }
    
    /// <summary>
    /// Processa o parâmetro recebido do JavaScript
    /// </summary>
    /// <param name="parameter">Parâmetro para processar</param>
    private void ProcessReceivedParameter(string parameter)
    {
        if (string.IsNullOrEmpty(parameter))
        {
            if (debugMode)
            {
                Debug.LogWarning("WebGLCommunicator: Parâmetro recebido está vazio ou nulo");
            }
            return;
        }
        
        // Aqui você pode adicionar lógica específica para processar diferentes tipos de parâmetros
        // Por exemplo, verificar se é um JSON, um comando específico, etc.
        
        if (debugMode)
        {
            Debug.Log($"WebGLCommunicator: Processando parâmetro do tipo: {parameter.GetType().Name}");
            Debug.Log($"WebGLCommunicator: Conteúdo do parâmetro: '{parameter}'");
            Debug.Log($"WebGLCommunicator: Tamanho do parâmetro: {parameter.Length} caracteres");
        }
        
        // Exemplo de processamento condicional
        if (parameter.StartsWith("{") && parameter.EndsWith("}"))
        {
            if (debugMode)
            {
                Debug.Log("WebGLCommunicator: Parâmetro parece ser um JSON");
            }
            ProcessJsonParameter(parameter);
        }
        else
        {
            if (debugMode)
            {
                Debug.Log("WebGLCommunicator: Parâmetro parece ser texto simples");
            }
            ProcessTextParameter(parameter);
        }
    }
    
    /// <summary>
    /// Processa parâmetros em formato JSON
    /// </summary>
    /// <param name="jsonParameter">Parâmetro em formato JSON</param>
    private void ProcessJsonParameter(string jsonParameter)
    {
        try
        {
            if (debugMode)
            {
                Debug.Log($"WebGLCommunicator: Processando JSON: {jsonParameter}");
            }
            
            // Aqui você pode usar JsonUtility.FromJson ou outra biblioteca para deserializar
            // Por enquanto, apenas faz log para debug
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"WebGLCommunicator: Erro ao processar JSON: {e.Message}");
        }
    }
    
    /// <summary>
    /// Processa parâmetros em formato texto
    /// </summary>
    /// <param name="textParameter">Parâmetro em formato texto</param>
    private void ProcessTextParameter(string textParameter)
    {
        if (debugMode)
        {
            Debug.Log($"WebGLCommunicator: Processando texto: '{textParameter}'");
        }
        
        // Exemplo de comandos simples
        switch (textParameter.ToLower())
        {
            case "test":
                Debug.Log("WebGLCommunicator: Comando de teste executado!");
                break;
            case "debug":
                Debug.Log("WebGLCommunicator: Modo debug ativado via parâmetro!");
                debugMode = true;
                break;
            case "hello":
                Debug.Log("WebGLCommunicator: Olá do Unity!");
                break;
            default:
                Debug.Log($"WebGLCommunicator: Parâmetro personalizado processado: '{textParameter}'");
                break;
        }
    }
    
    /// <summary>
    /// Método para testar a comunicação da loja (método público para debug)
    /// </summary>
    public void TestShopCommunication()
    {
        if (debugMode)
        {
            Debug.Log("WebGLCommunicator: Testando comunicação da loja...");
        }
        
        // Simula dados da loja para teste
        string testShopData = @"{
            ""options"": [
                {
                    ""optionName"": ""Poção de Vida Teste"",
                    ""description"": ""Uma poção de teste para aumentar vida"",
                    ""stellarTransactionId"": ""ST-TEST-001"",
                    ""title"": ""Teste de Vida"",
                    ""buff"": ""+25 Vida Máxima"",
                    ""icon"": ""test_icon"",
                    ""optionType"": ""HealthUpgrade"",
                    ""value"": 25.0,
                    ""rarity"": ""common"",
                    ""cost"": 50.0,
                    ""isSpecial"": false
                }
            ],
            ""playerLevel"": 1,
            ""shopMetadata"": {
                ""version"": ""1.0.0"",
                ""timestamp"": ""2025-09-15T12:00:00Z"",
                ""sessionId"": ""test_session_123"",
                ""totalAvailableOptions"": 1
            }
        }";
        
        OnShopOptionsReceivedFromJS(testShopData);
    }
    
    /// <summary>
    /// Método para obter informações de debug do WebGLCommunicator
    /// </summary>
    public void LogDebugInfo()
    {
        if (debugMode)
        {
            Debug.Log("=== WebGLCommunicator Debug Info ===");
            Debug.Log($"Game Started: {gameStarted}");
            Debug.Log($"Debug Mode: {debugMode}");
            Debug.Log($"Instance Active: {gameObject.activeInHierarchy}");
            
            // Verifica se o ShopManager existe
            ShopManager shopManager = FindObjectOfType<ShopManager>();
            if (shopManager != null)
            {
                Debug.Log($"ShopManager Found: {shopManager.name}");
                Debug.Log($"Shop Open: {shopManager.IsShopOpen}");
            }
            else
            {
                Debug.Log("ShopManager: NOT FOUND");
            }
            
            Debug.Log("=== End Debug Info ===");
        }
    }
    
    /// <summary>
    /// Método para forçar atualização das opções da loja via JavaScript
    /// </summary>
    public void RequestShopOptionsUpdate()
    {
        if (debugMode)
        {
            Debug.Log("WebGLCommunicator: Solicitando atualização das opções da loja do JavaScript...");
        }
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        // Em WebGL, podemos chamar uma função JavaScript se necessário
        // Por enquanto, apenas registra a solicitação
        Debug.Log("WebGLCommunicator: Solicitação de atualização enviada");
        #else
        // No editor, simula dados de teste
        if (debugMode)
        {
            Debug.Log("WebGLCommunicator: Simulando dados da loja no editor...");
            TestShopCommunication();
        }
        #endif
    }
}
