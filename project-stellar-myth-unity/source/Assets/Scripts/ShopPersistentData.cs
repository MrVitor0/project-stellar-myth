using UnityEngine;

/// <summary>
/// Classe responsável por persistir os dados da loja entre cenas
/// Este GameObject usa DontDestroyOnLoad para manter os dados da loja
/// </summary>
public class ShopPersistentData : MonoBehaviour
{
    // Singleton para acesso global
    public static ShopPersistentData Instance { get; private set; }
    
    [Header("Shop Persistent Settings")]
    [SerializeField] private bool debugMode = false;
    
    // Dados persistentes da loja
    private ShopOption[] cachedShopOptions = null;
    private bool shopWasOpenedBefore = false;
    
    private void Awake()
    {
        // Implementa o padrão Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Este objeto persiste entre cenas
            
            if (debugMode)
            {
                Debug.Log("ShopPersistentData: Instância criada e persistente");
            }
        }
        else
        {
            if (debugMode)
            {
                Debug.Log("ShopPersistentData: Instância já existe, destruindo duplicata");
            }
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Salva as opções da loja no cache persistente
    /// </summary>
    /// <param name="options">Opções da loja para salvar</param>
    public void SaveShopOptions(ShopOption[] options)
    {
        if (options != null && options.Length > 0)
        {
            cachedShopOptions = new ShopOption[options.Length];
            System.Array.Copy(options, cachedShopOptions, options.Length);
            
            if (debugMode)
            {
                Debug.Log($"ShopPersistentData: {options.Length} opções da loja salvas no cache");
            }
        }
    }
    
    /// <summary>
    /// Carrega as opções da loja do cache persistente
    /// </summary>
    /// <returns>Array de opções da loja ou null se não houver cache</returns>
    public ShopOption[] LoadShopOptions()
    {
        if (cachedShopOptions != null && cachedShopOptions.Length > 0)
        {
            if (debugMode)
            {
                Debug.Log($"ShopPersistentData: Carregando {cachedShopOptions.Length} opções do cache");
            }
            
            // Retorna uma cópia para evitar modificações acidentais
            ShopOption[] copy = new ShopOption[cachedShopOptions.Length];
            System.Array.Copy(cachedShopOptions, copy, cachedShopOptions.Length);
            return copy;
        }
        
        return null;
    }
    
    /// <summary>
    /// Verifica se existem opções salvas no cache
    /// </summary>
    /// <returns>True se há opções no cache</returns>
    public bool HasCachedOptions()
    {
        return cachedShopOptions != null && cachedShopOptions.Length > 0;
    }
    
    /// <summary>
    /// Limpa o cache de opções da loja
    /// </summary>
    public void ClearCache()
    {
        cachedShopOptions = null;
        shopWasOpenedBefore = false;
        
        if (debugMode)
        {
            Debug.Log("ShopPersistentData: Cache limpo");
        }
    }
    
    /// <summary>
    /// Marca que a loja foi aberta pelo menos uma vez
    /// </summary>
    public void MarkShopAsOpened()
    {
        shopWasOpenedBefore = true;
        
        if (debugMode)
        {
            Debug.Log("ShopPersistentData: Loja marcada como aberta anteriormente");
        }
    }
    
    /// <summary>
    /// Verifica se a loja já foi aberta antes
    /// </summary>
    /// <returns>True se a loja já foi aberta</returns>
    public bool WasShopOpenedBefore()
    {
        return shopWasOpenedBefore;
    }
    
    /// <summary>
    /// Reseta o estado para permitir que a loja seja aberta novamente
    /// Chamado quando o jogo é reiniciado
    /// </summary>
    public void ResetShopState()
    {
        shopWasOpenedBefore = false;
        
        if (debugMode)
        {
            Debug.Log("ShopPersistentData: Estado da loja resetado");
        }
    }
    
    private void OnDestroy()
    {
        // Limpa a instância se esta for a instância principal
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
