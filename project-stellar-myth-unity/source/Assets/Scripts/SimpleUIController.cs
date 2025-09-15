using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script simplificado apenas para controlar os sliders de UI
/// Use este se você quer separar a lógica de stats do personagem da UI
/// </summary>
public class SimpleUIController : MonoBehaviour
{
    [Header("UI Sliders")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider staminaSlider;
    
    void Start()
    {
        // Configura os sliders para usar valores normalizados (0 a 1)
        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = 1f;
            healthSlider.value = 1f; // Começa com vida cheia
        }
        
        if (staminaSlider != null)
        {
            staminaSlider.minValue = 0f;
            staminaSlider.maxValue = 1f;
            staminaSlider.value = 1f; // Começa com stamina cheia
        }
    }
    
    /// <summary>
    /// Atualiza o slider de vida com um valor normalizado (0 a 1)
    /// </summary>
    /// <param name="normalizedValue">Valor entre 0 e 1</param>
    public void UpdateHealthSlider(float normalizedValue)
    {
        if (healthSlider != null)
        {
            healthSlider.value = Mathf.Clamp01(normalizedValue);
        }
    }
    
    /// <summary>
    /// Atualiza o slider de stamina com um valor normalizado (0 a 1)
    /// </summary>
    /// <param name="normalizedValue">Valor entre 0 e 1</param>
    public void UpdateStaminaSlider(float normalizedValue)
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = Mathf.Clamp01(normalizedValue);
        }
    }
    
    /// <summary>
    /// Atualiza o slider de vida baseado nos valores atuais e máximos
    /// </summary>
    /// <param name="currentHealth">Vida atual</param>
    /// <param name="maxHealth">Vida máxima</param>
    public void UpdateHealthSlider(float currentHealth, float maxHealth)
    {
        if (healthSlider != null && maxHealth > 0)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
    }
    
    /// <summary>
    /// Atualiza o slider de stamina baseado nos valores atuais e máximos
    /// </summary>
    /// <param name="currentStamina">Stamina atual</param>
    /// <param name="maxStamina">Stamina máxima</param>
    public void UpdateStaminaSlider(float currentStamina, float maxStamina)
    {
        if (staminaSlider != null && maxStamina > 0)
        {
            staminaSlider.value = currentStamina / maxStamina;
        }
    }
    
    /// <summary>
    /// Obtém a referência do slider de vida
    /// </summary>
    public Slider GetHealthSlider() => healthSlider;
    
    /// <summary>
    /// Obtém a referência do slider de stamina
    /// </summary>
    public Slider GetStaminaSlider() => staminaSlider;
}
