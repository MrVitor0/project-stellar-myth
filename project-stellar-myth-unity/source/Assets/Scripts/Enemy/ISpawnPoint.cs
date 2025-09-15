using UnityEngine;

namespace EnemySystem
{
    /// <summary>
    /// Interface que define um ponto de spawn
    /// Seguindo o Interface Segregation Principle (ISP)
    /// </summary>
    public interface ISpawnPoint
    {
        Vector3 Position { get; }
        bool IsAvailable { get; }
        void SetAvailability(bool available);
    }
}
