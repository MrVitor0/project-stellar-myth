using UnityEngine;

namespace EnemySystem.Pathfinding
{
    /// <summary>
    /// Enum que define os tipos de comportamento de combate dos inimigos
    /// </summary>
    public enum EnemyCombatType
    {
        Melee,      // Combate corpo a corpo
        Ranged,     // Combate à distância
        Support     // Suporte (cura, buff, etc.)
    }

    /// <summary>
    /// Enum que define o estado atual do pathfinding
    /// </summary>
    public enum PathfindingState
    {
        Idle,           // Parado
        MovingToTarget, // Movendo em direção ao alvo
        Attacking,      // Atacando o alvo
        Avoiding,       // Evitando obstáculos/outros inimigos
        Stuck,          // Preso/travado
        Dead            // Morto
    }
}
