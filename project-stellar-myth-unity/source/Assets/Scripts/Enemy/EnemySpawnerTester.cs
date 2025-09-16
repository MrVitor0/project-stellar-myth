using UnityEngine;

namespace EnemySystem
{
    /// <summary>
    /// Utilitário para testar o sistema de spawn de inimigos
    /// Inclui funcionalidades para matar inimigos e controlar o sistema
    /// </summary>
    public class EnemySpawnerTester : MonoBehaviour
    {
        [Header("Testing Controls")]
        [SerializeField] private EnemySpawner enemySpawner;
        [SerializeField] private KeyCode startWavesKey = KeyCode.Space;
        [SerializeField] private KeyCode stopWavesKey = KeyCode.Escape;
        [SerializeField] private KeyCode skipWaveKey = KeyCode.N;
        [SerializeField] private KeyCode restartWavesKey = KeyCode.R;
        [SerializeField] private KeyCode killAllEnemiesKey = KeyCode.K;
        [SerializeField] private KeyCode killRandomEnemyKey = KeyCode.X;
        
        [Header("Auto Kill Settings")]
        [SerializeField] private bool autoKillEnemies = false;
        [SerializeField] private float autoKillInterval = 3f;
        
        private float lastAutoKillTime;

        #region Unity Lifecycle

        private void Start()
        {
            if (enemySpawner == null)
            {
                enemySpawner = FindObjectOfType<EnemySpawner>();
            }

            lastAutoKillTime = Time.time;
        }

        private void Update()
        {
            HandleInput();
            
            if (autoKillEnemies && Time.time - lastAutoKillTime >= autoKillInterval)
            {
                KillRandomEnemy();
                lastAutoKillTime = Time.time;
            }
        }

        #endregion

        #region Input Handling

        private void HandleInput()
        {
            if (Input.GetKeyDown(startWavesKey))
            {
                StartWaves();
            }
            
            if (Input.GetKeyDown(stopWavesKey))
            {
                StopWaves();
            }
            
            if (Input.GetKeyDown(skipWaveKey))
            {
                SkipWave();
            }
            
            if (Input.GetKeyDown(restartWavesKey))
            {
                RestartWaves();
            }
            
            if (Input.GetKeyDown(killAllEnemiesKey))
            {
                KillAllEnemies();
            }
            
            if (Input.GetKeyDown(killRandomEnemyKey))
            {
                KillRandomEnemy();
            }
        }

        #endregion

        #region Testing Methods

        [ContextMenu("Start Waves")]
        public void StartWaves()
        {
            if (enemySpawner != null)
            {
                enemySpawner.StartWaveSystem();
                Debug.Log("[Tester] Started wave system");
            }
            else
            {
                Debug.LogError("[Tester] No EnemySpawner assigned!");
            }
        }

        [ContextMenu("Stop Waves")]
        public void StopWaves()
        {
            if (enemySpawner != null)
            {
                enemySpawner.StopWaveSystem();
                Debug.Log("[Tester] Stopped wave system");
            }
        }

        [ContextMenu("Skip Wave")]
        public void SkipWave()
        {
            if (enemySpawner != null)
            {
                enemySpawner.SkipToNextWave();
                Debug.Log("[Tester] Skipped to next wave");
            }
        }

        [ContextMenu("Restart Waves")]
        public void RestartWaves()
        {
            if (enemySpawner != null)
            {
                enemySpawner.RestartWaveSystem();
                Debug.Log("[Tester] Restarted wave system");
            }
        }

        [ContextMenu("Kill All Enemies")]
        public void KillAllEnemies()
        {
            Enemy[] enemies = FindObjectsOfType<Enemy>();
            int killedCount = 0;
            
            foreach (var enemy in enemies)
            {
                if (!enemy.IsDead)
                {
                    enemy.Die();
                    killedCount++;
                }
            }
            
            Debug.Log($"[Tester] Killed {killedCount} enemies");
        }

        [ContextMenu("Kill Random Enemy")]
        public void KillRandomEnemy()
        {
            Enemy[] enemies = FindObjectsOfType<Enemy>();
            var aliveEnemies = System.Array.FindAll(enemies, e => !e.IsDead);
            
            if (aliveEnemies.Length > 0)
            {
                int randomIndex = Random.Range(0, aliveEnemies.Length);
                aliveEnemies[randomIndex].Die();
                Debug.Log($"[Tester] Killed random enemy: {aliveEnemies[randomIndex].name}");
            }
            else
            {
                Debug.Log("[Tester] No alive enemies to kill");
            }
        }

        [ContextMenu("Damage All Enemies")]
        public void DamageAllEnemies()
        {
            Enemy[] enemies = FindObjectsOfType<Enemy>();
            int damagedCount = 0;
            
            foreach (var enemy in enemies)
            {
                if (!enemy.IsDead)
                {
                    enemy.TakeDamage(25f); // Dano fixo para teste
                    damagedCount++;
                }
            }
            
            Debug.Log($"[Tester] Damaged {damagedCount} enemies");
        }

        #endregion

        #region Event Handlers

        public void OnWaveStarted()
        {
        }

        public void OnWaveCompleted()
        {
        }

        public void OnAllWavesCompleted()
        {
        }

        public void OnEnemySpawned(int totalEnemies)
        {
        }

        public void OnEnemyKilled(int remainingEnemies)
        {
        }

        #endregion

        #region GUI

        private void OnGUI()
        {
            if (enemySpawner == null) return;

            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label("Enemy Spawner Tester", GUI.skin.box);
            
            GUILayout.Space(5);
            
            if (GUILayout.Button($"Start Waves ({startWavesKey})"))
                StartWaves();
                
            if (GUILayout.Button($"Stop Waves ({stopWavesKey})"))
                StopWaves();
                
            if (GUILayout.Button($"Skip Wave ({skipWaveKey})"))
                SkipWave();
                
            if (GUILayout.Button($"Restart Waves ({restartWavesKey})"))
                RestartWaves();
                
            GUILayout.Space(5);
            
            if (GUILayout.Button($"Kill All Enemies ({killAllEnemiesKey})"))
                KillAllEnemies();
                
            if (GUILayout.Button($"Kill Random Enemy ({killRandomEnemyKey})"))
                KillRandomEnemy();
                
            if (GUILayout.Button("Damage All Enemies"))
                DamageAllEnemies();

            GUILayout.Space(5);
            
            // Status
            Enemy[] enemies = FindObjectsOfType<Enemy>();
            int aliveCount = System.Array.FindAll(enemies, e => !e.IsDead).Length;
            GUILayout.Label($"Alive Enemies: {aliveCount}");
            
            GUILayout.EndArea();
        }

        #endregion
    }
}
