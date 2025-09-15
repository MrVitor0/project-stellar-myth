using System;
using UnityEngine;

namespace EnemySystem
{
    /// <summary>
    /// Estrutura de dados para configurar uma wave de inimigos
    /// Seguindo o Single Responsibility Principle (SRP)
    /// </summary>
    [Serializable]
    public class Wave
    {
        [Header("Wave Configuration")]
        [SerializeField] private string waveName = "Wave";
        [SerializeField] private EnemySpawnData[] enemySpawnData;
        [SerializeField] private float delayBetweenSpawns = 1f;
        [SerializeField] private float waveStartDelay = 2f;

        public string WaveName => waveName;
        public EnemySpawnData[] EnemySpawnData => enemySpawnData;
        public float DelayBetweenSpawns => delayBetweenSpawns;
        public float WaveStartDelay => waveStartDelay;

        public int TotalEnemiesInWave
        {
            get
            {
                int total = 0;
                foreach (var enemyData in enemySpawnData)
                {
                    total += enemyData.Count;
                }
                return total;
            }
        }
    }

    /// <summary>
    /// Dados de spawn para cada tipo de inimigo em uma wave
    /// </summary>
    [Serializable]
    public class EnemySpawnData
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private int count = 1;
        [SerializeField] private float spawnWeight = 1f; // Peso para spawn aleatório

        public GameObject EnemyPrefab => enemyPrefab;
        public int Count => count;
        public float SpawnWeight => spawnWeight;
    }
}
