using UnityEngine;
using CombatSystem;

/// <summary>
/// Script de exemplo para testar o sistema de Game Over
/// Adicione este script a qualquer GameObject na cena para testar
/// </summary>
public class GameOverTester : MonoBehaviour
{
    [Header("Test Controls")]
    [SerializeField] private KeyCode testDamageKey = KeyCode.K; // Tecla para causar dano
    [SerializeField] private KeyCode testKillKey = KeyCode.L;   // Tecla para matar player
    [SerializeField] private float testDamageAmount = 25f;
    
    void Update()
    {
        // Teste de dano
        if (Input.GetKeyDown(testDamageKey))
        {
            TestDamage();
        }
        
        // Teste de morte instantânea
        if (Input.GetKeyDown(testKillKey))
        {
            TestInstantDeath();
        }
    }
    
    /// <summary>
    /// Causa dano ao player para teste
    /// </summary>
    private void TestDamage()
    {
        PlayerController2D player = FindObjectOfType<PlayerController2D>();
        if (player != null)
        {
            ICombatController combat = player.GetComponent<ICombatController>();
            if (combat != null)
            {
                combat.TakeDamage(testDamageAmount, Vector2.zero);
            }
        }
     
    }
    
    /// <summary>
    /// Mata o player instantaneamente para teste
    /// </summary>
    private void TestInstantDeath()
    {
        PlayerController2D player = FindObjectOfType<PlayerController2D>();
        if (player != null)
        {
            ICombatController combat = player.GetComponent<ICombatController>();
            if (combat != null)
            {
                // Causa dano igual à vida máxima para garantir morte
                float maxHealth = combat.Attributes.MaxHealth;
                combat.TakeDamage(maxHealth + 10f, Vector2.zero);
            }
        }
    
    }
    
    void OnGUI()
    {
        // Interface simples para teste
        GUI.Box(new Rect(10, 10, 250, 80), "Game Over Tester");
        
        if (GUI.Button(new Rect(20, 35, 100, 25), $"Damage ({testDamageKey})"))
        {
            TestDamage();
        }
        
        if (GUI.Button(new Rect(130, 35, 100, 25), $"Kill ({testKillKey})"))
        {
            TestInstantDeath();
        }
        
        // Mostra informações do player
        PlayerController2D player = FindObjectOfType<PlayerController2D>();
        if (player != null)
        {
            ICombatController combat = player.GetComponent<ICombatController>();
            if (combat != null)
            {
                float currentHealth = combat.Attributes.CurrentHealth;
                float maxHealth = combat.Attributes.MaxHealth;
                GUI.Label(new Rect(20, 65, 200, 20), $"Health: {currentHealth:F0}/{maxHealth:F0}");
            }
        }
    }
}
