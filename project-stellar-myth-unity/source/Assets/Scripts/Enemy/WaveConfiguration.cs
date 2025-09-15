using UnityEngine;

namespace EnemySystem
{
    /// <summary>
    /// ScriptableObject para configurar as waves
    /// Seguindo o Single Responsibility Principle (SRP)
    /// </summary>
    [CreateAssetMenu(fileName = "WaveConfiguration", menuName = "Enemy System/Wave Configuration", order = 1)]
    public class WaveConfiguration : ScriptableObject
    {
        [Header("Wave Settings")]
        [SerializeField] private Wave[] waves;
        [SerializeField] private float timeBetweenWaves = 5f;
        [SerializeField] private bool loopWaves = false;

        public Wave[] Waves => waves;
        public float TimeBetweenWaves => timeBetweenWaves;
        public bool LoopWaves => loopWaves;

        public Wave GetWave(int index)
        {
            if (index >= 0 && index < waves.Length)
            {
                return waves[index];
            }
            return null;
        }

        public int TotalWaves => waves?.Length ?? 0;
    }
}
